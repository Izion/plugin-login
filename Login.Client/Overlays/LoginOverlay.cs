using System;
using NFive.SDK.Client.Interface;
using NFive.Login.Shared;

namespace NFive.Login.Client.Overlays
{
	public class LoginOverlay : Overlay
	{
		public event EventHandler<CredentialsOverlayEventArgs> Login;
		public event EventHandler<CredentialsOverlayEventArgs> Register;

		public LoginOverlay(OverlayManager manager) : base("PluginLoginOverlay.html", manager)
		{
			Attach<Credentials>("login", (credentials, callback) => this.Login?.Invoke(this, new CredentialsOverlayEventArgs(this, credentials)));
			Attach<Credentials>("register", (credentials, callback) => this.Register?.Invoke(this, new CredentialsOverlayEventArgs(this, credentials)));
		}

		public void Configure(PublicConfiguration config)
		{
			Send("config", new ConfigOverlayEventArgs(config, this));
		}

		public void SwitchToForm(Forms form)
		{
			Send("switchForm", form);
		}

		public void ShowError(string error)
		{
			Send("showError", error);
		}

		public void HideError()
		{
			Send("hideError");
		}

		public void ShowInfo(string info)
		{
			Send("showInfo", info);
		}

		public void HideInfo()
		{
			Send("hideInfo");
		}
	}

	public class CredentialsOverlayEventArgs : OverlayEventArgs
	{
		public string Email;
		public string Password;

		public CredentialsOverlayEventArgs(Overlay overlay, Credentials credentials) : base(overlay)
		{
			this.Email = credentials.Email;
			this.Password = credentials.Password;
		}
	}

	public class ConfigOverlayEventArgs : OverlayEventArgs
	{
		public int MinPasswordLength { get; }
		public bool ForceSymbols { get; }
		public bool ForceDigits { get; }
		public bool ForceMixCase { get; }

		public ConfigOverlayEventArgs(PublicConfiguration config, Overlay overlay) : base(overlay)
		{
			this.MinPasswordLength = config.MinPasswordLength;
			this.ForceSymbols = config.ForceSymbols;
			this.ForceDigits = config.ForceDigits;
			this.ForceMixCase = config.ForceMixCase;
		}
	}

	public enum Forms
	{
		Login = 1,
		Register
	}
}
