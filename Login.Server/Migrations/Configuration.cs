using JetBrains.Annotations;
using NFive.SDK.Server.Migrations;
using NFive.Login.Server.Storage;

namespace NFive.Login.Server.Migrations
{
	[UsedImplicitly]
	public sealed class Configuration : MigrationConfiguration<StorageContext> { }
}
