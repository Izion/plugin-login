using System;
using JetBrains.Annotations;
using NFive.SDK.Core.Models;

namespace NFive.Login.Shared.Models
{
	[PublicAPI]
	public interface IAccount : IIdentityModel
	{
		string Email { get; set; }

		string Password { get; set; }

		DateTime DateOfRegistration { get; set; }

		DateTime? LastLogin { get; set; }
	}
}
