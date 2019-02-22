namespace NFive.Login.Shared.Responses
{
	/// <summary>
	/// Server registration response codes.
	/// </summary>
	public enum RegisterResponse
	{
		/// <summary>
		/// Server error.
		/// </summary>
		Error,

		/// <summary>
		/// Registration email address is already used.
		/// </summary>
		EmailExists,

		/// <summary>
		/// License account limit reached.
		/// </summary>
		AccountLimitReached,

		/// <summary>
		/// Registration complete, account created.
		/// </summary>
		Created
	}
}
