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
using JetBrains.Annotations;

namespace NFive.Login.Server
{
	/// <summary>
	/// Wrapper library for accessing login and account events from external plugins.
	/// </summary>
	[PublicAPI]
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
		/// Occurs when a new account has been registered for a user.
		/// </summary>
		public event EventHandler<ClientAccountEventArgs> AccountRegistered;

		/// <summary>
		/// Occurs when a client logs in to an existing account.
		/// </summary>
		public event EventHandler<ClientAccountEventArgs> ClientLoggedIn;

		/// <summary>
		/// Gets the list of currently logged in accounts.
		/// </summary>
		/// <value>
		/// List of the current logged in accounts.
		/// </value>
		public List<Account> LoggedInAccounts => this.Events.Request<List<Account>>(LoginEvents.GetCurrentAccounts);

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
		/// Determines whether a user is logged in.
		/// </summary>
		/// <param name="user">The user to check.</param>
		/// <returns><c>true</c> if the user is logged in; otherwise, <c>false</c>.</returns>
		public bool IsLoggedIn(User user) => this.LoggedInAccounts.Any(a => a.UserId == user.Id);

		/// <summary>
		/// Gets the current account for a user, if any.
		/// </summary>
		/// <param name="user">The user to get the current account for.</param>
		/// <returns>The currently logged in user's account; otherwise, <c>null</c> if not currently logged in to an account.</returns>
		public Account GetCurrentAccount(User user) => this.LoggedInAccounts.FirstOrDefault(a => a.UserId == user.Id);
	}
}
