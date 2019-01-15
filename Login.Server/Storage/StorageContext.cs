using System.Data.Entity;
using NFive.Login.Server.Models;
using NFive.SDK.Server.Storage;

namespace NFive.Login.Server.Storage
{
	public class StorageContext : EFContext<StorageContext>
	{
		public DbSet<Account> Accounts { get; set; }
	}
}
