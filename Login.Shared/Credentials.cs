namespace NFive.Login.Shared
{
	/// <summary>
	/// Represents login details.
	/// </summary>
	public class Credentials
	{
		/// <summary>
		/// Gets or sets the login email address.
		/// </summary>
		/// <value>
		/// The login email address.
		/// </value>
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the login password.
		/// </summary>
		/// <value>
		/// The login password.
		/// </value>
		public string Password { get; set; }
	}
}
