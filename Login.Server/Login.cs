using NFive.Login.Server.Events;
using NFive.Login.Server.Models;
using NFive.Login.Shared;
using NFive.SDK.Core.Models.Player;
using NFive.SDK.Core.Plugins;
using NFive.SDK.Server.Events;
using NFive.SDK.Server.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NFive.Login.Server
{
	/// <summary>
	/// Wrapper library for accessing events from external plugins.
	/// </summary>
	public class Login
	{
		/// <summary>
		/// The controller event manager.
		/// </summary>
		protected readonly IEventManager Events;

		/// <summary>
		/// The controller RPC handler.
		/// </summary>
		protected readonly IRpcHandler Rpc;

		/// <summary>
		/// Occurs when a new account has been registered.
		/// </summary>
		public event EventHandler<ClientAccountEventArgs> AccountRegistered;

		/// <summary>
		/// Occurs when a client logs in to an existing account.
		/// </summary>
		public event EventHandler<ClientAccountEventArgs> ClientLoggedIn;

		/// <summary>
		/// Gets the currently logged in accounts count.
		/// </summary>
		/// <value>
		/// The current logged in accounts count.
		/// </value>
		public int CurrentLoggedInAccountsCount => this.Events.Request<int>(LoginEvents.GetCurrentAccountsCount);

		/// <summary>
		/// Gets the currently logged in accounts count.
		/// </summary>
		/// <value>
		/// The current logged in accounts count.
		/// </value>
		public List<Account> CurrentLoggedInAccounts => this.Events.Request<List<Account>>(LoginEvents.GetCurrentAccounts);

		/// <summary>
		/// Initializes a new instance of the <see cref="Login"/> wrapper.
		/// </summary>
		/// <param name="events">The controller event manager.</param>
		/// <param name="rpc">The controller RPC handler.</param>
		public Login(IEventManager events, IRpcHandler rpc)
		{
			this.Events = events;
			this.Rpc = rpc;

			this.Events.On<Client, Account>(LoginEvents.Registered, (c, a) => this.AccountRegistered?.Invoke(this, new ClientAccountEventArgs(c, a)));
			this.Events.On<Client, Account>(LoginEvents.LoggedIn, (c, a) => this.ClientLoggedIn?.Invoke(this, new ClientAccountEventArgs(c, a)));
		}

		/// <summary>
		/// Returns whether a user is currently logged in or not.
		/// </summary>
		/// <param name="user">The user to check if logged in or not.</param>
		public bool IsUserLoggedIn(User user)
		{
			return this.CurrentLoggedInAccounts.Any(a => a.UserId == user.Id);
		}
	}
}
