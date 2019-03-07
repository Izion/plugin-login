using CitizenFX.Core;
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
			Attach<Credentials>("login", (credentials, callback) => this.Login?.Invoke(this, new CredentialsOverlayEventArgs(this, credentials, callback)));
			Attach<Credentials>("register", (credentials, callback) => this.Register?.Invoke(this, new CredentialsOverlayEventArgs(this, credentials, callback)));
		}
	}

	public class CredentialsOverlayEventArgs : OverlayEventArgs
	{
		public Credentials Credentials { get; }

		public CallbackDelegate Reply { get; }

		public CredentialsOverlayEventArgs(Overlay overlay, Credentials credentials, CallbackDelegate callback) : base(overlay)
		{
			this.Credentials = credentials;
			this.Reply = callback;
		}
	}
}
