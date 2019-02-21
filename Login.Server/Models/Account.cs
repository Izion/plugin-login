using NFive.Login.Shared.Models;
using NFive.SDK.Core.Models;
using NFive.SDK.Core.Models.Player;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NFive.Login.Server.Models
{
	public class Account : IdentityModel, IAccount
	{
		[Required]
		[StringLength(254, MinimumLength = 3)]
		public string Email { get; set; }

		[Required]
		[StringLength(60)]
		public string Password { get; set; }

		public DateTime? LastLogin { get; set; }

		[Required]
		[ForeignKey("User")]
		public Guid UserId { get; set; }

		public virtual User User { get; set; }
	}
}
