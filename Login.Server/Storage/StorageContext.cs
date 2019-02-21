using NFive.Login.Server.Models;
using NFive.SDK.Server.Storage;
using System.Data.Entity;

namespace NFive.Login.Server.Storage
{
	public class StorageContext : EFContext<StorageContext>
	{
		public DbSet<Account> Accounts { get; set; }
	}
}
