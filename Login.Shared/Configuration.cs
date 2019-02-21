using System;
using NFive.SDK.Core.Controllers;

namespace NFive.Login.Shared
{
	public class Configuration : ControllerConfiguration
	{
		public ushort LoginAttempts { get; set; } = 5; // Maximum login attempts allowed

		// ReSharper disable once IdentifierTypo, serialize as 'bcrypt_cost'
		public ushort BcryptCost { get; set; } = 12; // BCrypt cost

		public short MaxAccountsPerUser { get; set; } = 3; // Maximum numbers of accounts the same user can create

		public ushort MinPasswordLength { get; set; } = 8; // Minimum password length

		public bool ForceSymbols { get; set; } = true; // Force passwords to contain at least one symbol

		public bool ForceDigits { get; set; } = true; // Force passwords to contain at least one digit

		public bool ForceMixedCase { get; set; } = true; // Force passwords to have both uppercase and lowercase

		public string GlobalSalt { get; set; } = Guid.NewGuid().ToString(); // Global salt that will be appended to all inputs for added security
	}
}
