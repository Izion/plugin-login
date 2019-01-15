namespace NFive.Login.Server.Helpers
{
	public class BCryptHelper
	{
		private string GlobalSalt { get; }
		private int WorkFactor { get; }

		public BCryptHelper(string salt, int workFactor)
		{
			this.GlobalSalt = salt;
			this.WorkFactor = workFactor;
		}

		public string HashPassword(string password)
		{
			password += GlobalSalt;
			return BCrypt.Net.BCrypt.EnhancedHashPassword(password, this.WorkFactor);
		}

		public bool ValidatePassword(string password, string hash)
		{
			password += GlobalSalt;
			return BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
		}

		public string UpdateHash(string password, string hash)
		{
			if (BCrypt.Net.BCrypt.PasswordNeedsRehash(hash, this.WorkFactor))
				return HashPassword(password);
			return hash;
		}
	}
}
