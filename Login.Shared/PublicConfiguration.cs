using JetBrains.Annotations;

namespace NFive.Login.Shared
{
	[PublicAPI]
	public class PublicConfiguration
	{
		public int MinPasswordLength { get; set; }
		public bool ForceSymbols { get; set; }
		public bool ForceDigits { get; set; }
		public bool ForceMixedCase { get; set; }

		public PublicConfiguration() { }

		public PublicConfiguration(Configuration config)
		{
			this.MinPasswordLength = config.MinPasswordLength;
			this.ForceSymbols = config.ForceSymbols;
			this.ForceDigits = config.ForceDigits;
			this.ForceMixedCase = config.ForceMixedCase;
		}
	}
}
