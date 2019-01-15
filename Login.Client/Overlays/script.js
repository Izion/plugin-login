
var MinPasswordLength = 0;
var ForceSymbols = false;
var ForceDigits = false;
var ForceMixCase = false;

function ShowError(error) {
	HideInfo();
	$(".ErrorMessage").text("ERROR: " + error);
}

function HideError() {
	$(".ErrorMessage").text("");
}

function ShowInfo(info) {
	HideError();
	$(".InfoMessage").text("INFO: " + info);
}

function HideInfo() {
	$(".InfoMessage").text("");
}

function IsValidPassword(pwBox) {
	var password = pwBox.val();
	if (password.length < MinPasswordLength) {
		ShowError("Your password is too short!");
		pwBox.addClass("invalid");
		return false;
	}
	else if (ForceMixCase && (!password.match(/[a-z]/) || !password.match(/[A-Z]/))) {
		ShowError("Your password needs to have both lowercase and uppercase letters!");
		pwBox.addClass("invalid");
		return false;
	}
	else if (ForceDigits && !password.match(/\d/)) {
		ShowError("Your password needs to have at least one digit!");
		pwBox.addClass("invalid");
		return false;
	}
	else if (ForceSymbols && !password.match(/[$-/:-?{-~!"^_`\[\]]/)) {
		ShowError("Your password needs to have at least one symbol!");
		pwBox.addClass("invalid");
		return false;
	}
	pwBox.removeClass("invalid");
	return true;
}

function isValidEmailAddress(emailBox) {
	var emailAddress = emailBox.val();
	var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
	if (!pattern.test(emailAddress)) {
		ShowError("Invalid email given!");
		emailBox.addClass("invalid");
		return false;
	}
	emailBox.removeClass("invalid");
	return true;
}

function SwitchToForm(form) {
	switch (form) {
		case 1:
			$(".RegisterForm").hide();
			$(".LoginForm").show();
			$("#RegisterEmail").removeClass("invalid");
			$("#RegisterEmail").val("");
			$("#RegisterPassword").removeClass("invalid");
			$("#RegisterPassword").val("");
			$("#RegisterPasswordRepeat").removeClass("invalid");
			$("#RegisterPasswordRepeat").val("");
			break;
		case 2:
			$(".LoginForm").hide();
			$(".RegisterForm").show();
			$("#LoginEmail").removeClass("invalid");
			$("#LoginEmail").val("");
			$("#LoginPassword").removeClass("invalid");
			$("#LoginPassword").val("");
			break;
	}
}

$(() => {
	nfive.on("config", (config) => {
		MinPasswordLength = config.MinPasswordLength;
		ForceSymbols = config.ForceSymbols;
		ForceDigits = config.ForceDigits;
		ForceMixCase = config.ForceMixCase;
	});

	nfive.on("switchForm", (form) => SwitchToForm(form));

	nfive.on("showError", (error) => ShowError(error));

	nfive.on("hideError", () => HideError());

	nfive.on("showInfo", (info) => ShowInfo(info));

	nfive.on("hideInfo", () => HideInfo());


	$("#LoginButton").click(function (e) {
		e.preventDefault();
		var email = $("#LoginEmail").val();
		var password = $("#LoginPassword").val();
		if (email.length === 0) {
			ShowError("You must enter a valid email!");
			$("#LoginEmail").addClass("invalid");
		}
		else if (password.length === 0) {
			$("#LoginEmail").removeClass("invalid");
			ShowError("You must enter a valid password!");
			$("#LoginPassword").addClass("invalid");
		} else {
			HideError();
			$("#LoginPassword").removeClass("invalid");
			$("#LoginEmail").removeClass("invalid");

			const credentials =
			{
				Email: $("#LoginEmail").val(),
				Password:  $("#LoginPassword").val()
			};

			nfive.send("login", credentials);
		}
	});

	$("#RegisterButton").click(function (e) {
		e.preventDefault();
		if (isValidEmailAddress($("#RegisterEmail")) && IsValidPassword($("#RegisterPassword"))) {
			HideError();
			if ($("#RegisterPassword").val() != $("#RegisterPasswordRepeat").val()) {
				ShowError("The confirmation password doesn't match!");
				$("#RegisterPasswordRepeat").addClass("invalid");
			} else {
				$("#RegisterPasswordRepeat").removeClass("invalid");

				const credentials =
				{
					Email: $("#RegisterEmail").val(),
					Password: $("#RegisterPassword").val()
				};

				nfive.send("register", credentials);
			}
		}
	});

	$("#SwitchToLogin").click(function(e) {
		SwitchToForm(1);
	});

	$("#SwitchToRegister").click(function (e) {
		SwitchToForm(2);
	});
});
