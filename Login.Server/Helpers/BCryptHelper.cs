namespace NFive.Login.Server.Helpers
{
	public class BCryptHelper
	{
		public string Salt { get; }
		public int WorkFactor { get; }

		public BCryptHelper(string salt, int workFactor)
		{
			this.Salt = salt;
			this.WorkFactor = workFactor;
		}

		public string HashPassword(string password) => BCrypt.Net.BCrypt.EnhancedHashPassword(password + this.Salt, this.WorkFactor);

		public bool ValidatePassword(string password, string hash) => BCrypt.Net.BCrypt.EnhancedVerify(password + this.Salt, hash);

		public string UpdateHash(string password, string hash) => BCrypt.Net.BCrypt.PasswordNeedsRehash(hash, this.WorkFactor) ? HashPassword(password) : hash;
	}
}
