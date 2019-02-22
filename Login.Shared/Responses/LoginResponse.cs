namespace NFive.Login.Shared.Responses
{
	/// <summary>
	/// Server login response codes.
	/// </summary>
	public enum LoginResponse
	{
		/// <summary>
		/// Server error.
		/// </summary>
		Error,

		/// <summary>
		/// Login failed, invalid details.
		/// </summary>
		Invalid,

		/// <summary>
		/// Login complete, valid details.
		/// </summary>
		Valid
	}
}
