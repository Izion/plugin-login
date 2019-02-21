using NFive.Login.Shared;
using NFive.SDK.Client.Interface;
using System;

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

		public void ShowForm(Forms form) => Send("showForm", form);

		public void ShowError(string message) => Send("showError", message);

		public void ShowInfo(string message) => Send("showInfo", message);
	}

	public class CredentialsOverlayEventArgs : OverlayEventArgs
	{
		public Credentials Credentials { get; }

		public CredentialsOverlayEventArgs(Overlay overlay, Credentials credentials) : base(overlay) => this.Credentials = credentials;
	}

	public enum Forms
	{
		Login,
		Register
	}
}
