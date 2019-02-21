using CitizenFX.Core.Native;
using NFive.Login.Server.Helpers;
using NFive.Login.Server.Models;
using NFive.Login.Server.Storage;
using NFive.Login.Shared;
using NFive.Login.Shared.Responses;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Server.Controllers;
using NFive.SDK.Server.Events;
using NFive.SDK.Server.Rcon;
using NFive.SDK.Server.Rpc;
using NFive.SessionManager.Server.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NFive.Login.Server
{
	public class LoginController : ConfigurableController<Configuration>
	{
		private BCryptHelper BCryptHelper { get; }
		private Dictionary<int, int> LoginAttempts { get; }
		private List<Account> LoggedInAccounts { get; }

		public LoginController(ILogger logger, IEventManager events, IRpcHandler rpc, IRconManager rcon, Configuration configuration) : base(logger, events, rpc, rcon, configuration)
		{
			this.BCryptHelper = new BCryptHelper(this.Configuration.GlobalSalt, this.Configuration.BCryptCost);

			this.LoginAttempts = new Dictionary<int, int>();
			this.LoggedInAccounts = new List<Account>();

			// Send configuration when requested
			this.Rpc.Event(LoginEvents.Configuration).On(e => e.Reply(new PublicConfiguration(this.Configuration)));

			this.Rpc.Event(LoginEvents.Register).On<string, string>(OnRegistrationRequested);
			this.Rpc.Event(LoginEvents.Login).On<string, string>(OnLoginRequested);
			this.Rpc.Event(LoginEvents.AuthenticationStarted).On(OnAuthenticationStarted);

			this.Events.OnRequest(LoginEvents.GetCurrentAccountsCount, () => this.LoggedInAccounts.Count);
			this.Events.OnRequest(LoginEvents.GetCurrentAccounts, () => this.LoggedInAccounts);

			// Listen for NFive SessionManager plugin events
			var sessions = new SessionManager.Server.SessionManager(this.Events, this.Rpc);
			sessions.ClientDisconnected += OnClientDisconnected;
		}

		private void OnClientDisconnected(object sender, ClientSessionEventArgs e)
		{
			if (this.LoginAttempts.ContainsKey(e.Client.Handle)) this.LoginAttempts.Remove(e.Client.Handle);

			this.LoggedInAccounts.RemoveAll(a => a.UserId == e.Session.UserId);
		}

		private void OnAuthenticationStarted(IRpcEvent rpc)
		{
			this.Logger.Debug($"{rpc.User.Name} ({rpc.Client.Handle}) has started the authentication process!");
			this.LoginAttempts.Add(rpc.Client.Handle, 0);
		}

		private async void OnLoginRequested(IRpcEvent rpc, string email, string password)
		{
			using (var context = new StorageContext())
			using (var transaction = context.Database.BeginTransaction())
			{
				try
				{
					var account = context.Accounts.FirstOrDefault(a => a.Email == email);
					if (account == null || !this.BCryptHelper.ValidatePassword(password, account.Password))
					{
						rpc.Reply(LoginResponse.WrongCombination);
						this.LoginAttempts[rpc.Client.Handle]++;
						if (this.Configuration.LoginAttempts <= 0 || this.LoginAttempts[rpc.Client.Handle] < this.Configuration.LoginAttempts) return;
						this.Logger.Debug($"Kicking {rpc.User.Name} for exceeding the maximum allowed login attempts.");
						API.DropPlayer(rpc.Client.Handle.ToString(), "You have exceeded the maximum allowed login attempts!");
					}

					else
					{
						account.LastLogin = DateTime.UtcNow;
						account.Password = this.BCryptHelper.UpdateHash(password, account.Password);
						await context.SaveChangesAsync();
						transaction.Commit();
						this.LoggedInAccounts.Add(account);
						this.Events.Raise(LoginEvents.LoggedIn, rpc.Client, account);
						this.Logger.Debug($"{rpc.User.Name} has just logged in ({email})");
						rpc.Reply(LoginResponse.Ok);
					}
				}
				catch (Exception e)
				{
					this.Logger.Error(e);
					transaction.Rollback();
					rpc.Reply(LoginResponse.UnexpectedError);
				}
			}
		}

		private async void OnRegistrationRequested(IRpcEvent rpc, string email, string password)
		{
			using (var context = new StorageContext())
			using (var transaction = context.Database.BeginTransaction())
			{
				try
				{
					if (this.Configuration.MaxAccountsPerUser != 0 && context.Accounts.Select(a => a.UserId == rpc.User.Id).ToList().Count >= this.Configuration.MaxAccountsPerUser)
					{
						rpc.Reply(RegisterResponse.AccountLimitReached);
						return;
					}

					if (context.Accounts.Any(e => e.Email == email))
					{
						rpc.Reply(RegisterResponse.EmailExists);
					}
					else
					{
						var account = new Account
						{
							Email = email,
							Password = this.BCryptHelper.HashPassword(password),
							UserId = rpc.User.Id
						};

						context.Accounts.Add(account);
						await context.SaveChangesAsync();
						transaction.Commit();

						this.Events.Raise(LoginEvents.Registered, rpc.Client, account);
						this.Logger.Debug($"{rpc.User.Name} has registered a new account ({email})");
						rpc.Reply(RegisterResponse.Ok);
					}
				}
				catch (Exception e)
				{
					this.Logger.Error(e);
					transaction.Rollback();
					rpc.Reply(RegisterResponse.UnexpectedError);
				}
			}
		}

		public override void Reload(Configuration configuration)
		{
			// Update local configuration
			base.Reload(configuration);

			// Send out new configuration
			this.Rpc.Event(LoginEvents.Configuration).Trigger(new PublicConfiguration(this.Configuration));
		}
	}
}
