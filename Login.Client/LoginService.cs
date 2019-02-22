using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using JetBrains.Annotations;
using NFive.Login.Client.Overlays;
using NFive.Login.Shared;
using NFive.Login.Shared.Responses;
using NFive.SDK.Client.Commands;
using NFive.SDK.Client.Events;
using NFive.SDK.Client.Extensions;
using NFive.SDK.Client.Interface;
using NFive.SDK.Client.Rpc;
using NFive.SDK.Client.Services;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.Models.Player;
using System.Threading.Tasks;

namespace NFive.Login.Client
{
	[PublicAPI]
	public class LoginService : Service
	{
		private PublicConfiguration config;
		private LoginOverlay overlay;
		private bool loggedIn;

		public LoginService(ILogger logger, ITickManager ticks, IEventManager events, IRpcHandler rpc, ICommandManager commands, OverlayManager overlay, User user) : base(logger, ticks, events, rpc, commands, overlay, user) { }

		public override async Task HoldFocus()
		{
			// Request server configuration
			this.config = await this.Rpc.Event(LoginEvents.Configuration).Request<PublicConfiguration>();

			// Update local configuration on server configuration change
			this.Rpc.Event(LoginEvents.Configuration).On<PublicConfiguration>((e, c) => this.config = c);

			// Hide HUD
			Screen.Hud.IsVisible = false;

			// Disable the loading screen from automatically being dismissed
			API.SetManualShutdownLoadingScreenNui(true);

			// Position character, required for switching
			Game.Player.Character.Position = Vector3.Zero;

			// Freeze
			Game.Player.Freeze();

			// Switch out the player if it isn't already in a switch state
			if (!API.IsPlayerSwitchInProgress()) API.SwitchOutPlayer(API.PlayerPedId(), 0, 1);

			// Remove most clouds
			API.SetCloudHatOpacity(0.01f);

			// Wait for switch
			while (API.GetPlayerSwitchState() != 5) await Delay(50);

			// Hide loading screen
			API.ShutdownLoadingScreen();

			// Fade out
			Screen.Fading.FadeOut(0);
			while (Screen.Fading.IsFadingOut) await Delay(50);

			// Create overlay
			this.overlay = new LoginOverlay(this.OverlayManager, this.config); // TODO: Handle reload
			this.overlay.Login += OnLogin;
			this.overlay.Register += OnRegister;

			// Let server know we started authentication process
			this.Rpc.Event(LoginEvents.AuthenticationStarted).Trigger();

			// Focus overlay
			API.SetNuiFocus(true, true);

			// Shut down the NUI loading screen
			API.ShutdownLoadingScreenNui();

			// Fade in
			Screen.Fading.FadeIn(500);
			while (Screen.Fading.IsFadingIn) await Delay(50);

			// Wait for user before releasing focus
			while (!this.loggedIn) await Delay(50);
		}

		private async void OnRegister(object sender, CredentialsOverlayEventArgs e)
		{
			switch (await this.Rpc.Event(LoginEvents.Register).Request<RegisterResponse>(e.Credentials))
			{
				case RegisterResponse.AccountLimitReached:
					this.overlay.ShowError("You have reached the maximum number of accounts for this GTAV license!");
					break;

				case RegisterResponse.EmailExists:
					this.overlay.ShowError("The email address you entered already has an account registered!");
					break;

				case RegisterResponse.Created:
					this.overlay.ShowForm(Forms.Login);
					this.overlay.ShowInfo("Your account has been registered! Please login.");
					break;

				// ReSharper disable once RedundantCaseLabel
				case RegisterResponse.Error:
				default:
					this.overlay.ShowError("An unexpected error has occured. Please notify a server administrator.");
					break;
			}
		}

		private async void OnLogin(object sender, CredentialsOverlayEventArgs e)
		{
			switch (await this.Rpc.Event(LoginEvents.Login).Request<LoginResponse>(e.Credentials))
			{
				case LoginResponse.Invalid:
					this.overlay.ShowError("You have entered an incorrect email/password combination!<br>Forgotten your password? Contact a server administrator.");
					break;

				case LoginResponse.Valid:
					this.overlay.Dispose();

					// Release focus hold
					this.loggedIn = true;

					break;

				// ReSharper disable once RedundantCaseLabel
				case LoginResponse.Error:
				default:
					this.overlay.ShowError("An unexpected error has occured. Please notify a server administrator.");
					break;
			}
		}
	}
}
