using JetBrains.Annotations;
using NFive.SDK.Client.Commands;
using NFive.SDK.Client.Events;
using NFive.SDK.Client.Interface;
using NFive.SDK.Client.Rpc;
using NFive.SDK.Client.Services;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.Models.Player;
using System.Threading.Tasks;
using NFive.Login.Client.Overlays;
using NFive.Login.Shared;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using NFive.Login.Shared.Responses;
using NFive.SDK.Client.Extensions;

namespace NFive.Login.Client
{
	[PublicAPI]
	public class LoginService : Service
	{
		private PublicConfiguration config;
		private LoginOverlay overlay;

		public LoginService(ILogger logger, ITickManager ticks, IEventManager events, IRpcHandler rpc, ICommandManager commands, OverlayManager overlay, User user) : base(logger, ticks, events, rpc, commands, overlay, user) { }

		public override async Task Started()
		{
			// Request server configuration
			this.config = await this.Rpc.Event(LoginEvents.Configuration).Request<PublicConfiguration>();

			// Update local configuration on server configuration change
			this.Rpc.Event(LoginEvents.Configuration).On<PublicConfiguration>((e, c) => this.config = c);

			// Create overlay
			this.overlay = new LoginOverlay(this.OverlayManager);

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
			while (API.GetPlayerSwitchState() != 5) await Delay(10);

			// Hide loading screen
			API.ShutdownLoadingScreen();

			// Fade out
			Screen.Fading.FadeOut(0);
			while (Screen.Fading.IsFadingOut) await Delay(10);

			// Show the overlay
			this.overlay.Configure(this.config);
			this.overlay.SwitchToForm(Forms.Login);

			this.overlay.Login += OnLogin;
			this.overlay.Register += OnRegister;

			this.overlay.Show();

			// Let server know we started authentication process
			this.Rpc.Event(LoginEvents.AuthenticationStarted).Trigger();

			// Focus overlay
			API.SetNuiFocus(true, true);

			// Shut down the NUI loading screen
			API.ShutdownLoadingScreenNui();

			// Fade in
			Screen.Fading.FadeIn(500);
			while (Screen.Fading.IsFadingIn) await Delay(10);
		}

		private async void OnRegister(object sender, CredentialsOverlayEventArgs credentials)
		{
			RegisterResponse response = await this.Rpc.Event(LoginEvents.Register).Request<RegisterResponse>(credentials.Email.ToLower(), credentials.Password);

			switch (response)
			{
				case RegisterResponse.AccountLimitReached:
					this.overlay.ShowError("You have reached the maximum number of accounts per license!");
					break;
				case RegisterResponse.EmailExists:
					this.overlay.ShowError("The email you entered already exists!");
					break;
				case RegisterResponse.UnexpectedError:
					this.overlay.ShowError("An unexpected error has occured. Please notify an admin.");
					break;
				case RegisterResponse.Ok:
					this.overlay.HideError();
					this.overlay.ShowInfo("Your account has been registered! Please login.");
					this.overlay.SwitchToForm(Forms.Login);
					break;
			}
		}

		private async void OnLogin(object sender, CredentialsOverlayEventArgs credentials)
		{
			LoginResponse response = await this.Rpc.Event(LoginEvents.Login).Request<LoginResponse>(credentials.Email.ToLower(), credentials.Password);

			switch (response)
			{
				case LoginResponse.WrongCombination:
					this.overlay.ShowError("You have entered the wrong email/password combination!");
					break;
				case LoginResponse.UnexpectedError:
					this.overlay.ShowError("An unexpected error has occured. Please notify an admin.");
					break;
				case LoginResponse.Ok:
					this.overlay.HideError();
					this.overlay.Hide();
					this.Logger.Debug("You have been logged in!");
					break;
			}
		}
	}
}
