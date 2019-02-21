using NFive.Login.Shared;
using NFive.SDK.Client.Interface;
using System;
using NFive.SDK.Core.Diagnostics;

namespace NFive.Login.Client.Overlays
{
	public class LoginOverlay : Overlay
	{
		public event EventHandler<CredentialsOverlayEventArgs> Login;
		public event EventHandler<CredentialsOverlayEventArgs> Register;

		public LoginOverlay(OverlayManager manager, PublicConfiguration config) : base("LoginOverlay.html", manager)
		{
			Attach("load", (_, callback) => Send("config", config));
			Attach<Credentials>("login", (credentials, callback) => this.Login?.Invoke(this, new CredentialsOverlayEventArgs(this, credentials)));
			Attach<Credentials>("register", (credentials, callback) => this.Register?.Invoke(this, new CredentialsOverlayEventArgs(this, credentials)));
		}

		public void ShowForm(Forms form) => Send("switchForm", form);

		public void ShowError(string message) => Send("showError", message);

		public void HideError() => Send("hideError");

		public void ShowInfo(string message) => Send("showInfo", message);

		public void HideInfo() => Send("hideInfo");
	}

	public class CredentialsOverlayEventArgs : OverlayEventArgs
	{
		public Credentials Credentials { get; }

		public CredentialsOverlayEventArgs(Overlay overlay, Credentials credentials) : base(overlay)
		{
			this.Credentials = credentials;
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
			this.ForceMixCase = config.ForceMixedCase;
		}
	}

	public enum Forms
	{
		Login = 1,
		Register
	}
}
