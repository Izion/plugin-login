using System;
using NFive.Login.Server.Models;
using NFive.SDK.Core.Plugins;

namespace NFive.Login.Server.Events
{
	public class ClientAccountEventArgs : EventArgs
	{
		public Client Client;
		public Account Account;

		public ClientAccountEventArgs(Client client, Account account)
		{
			this.Client = client;
			this.Account = account;
		}
	}
}
