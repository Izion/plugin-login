function ShowError(message) {
	HideInfo();
	$("#error-alert").html(message).removeClass("d-none");
}

function HideError() {
	$("#error-alert").text("").addClass("d-none");
}

function ShowInfo(message) {
	HideError();
	$("#info-alert").html(message).removeClass("d-none");
}

function HideInfo() {
	$("#info-alert").text("").addClass("d-none");
}

function ShowForm(form) {
	HideError();
	HideInfo();

	$("form").removeClass("was-validated");

	switch (form) {
		case 0:
			$("h1").text("Login");
			$("#register").addClass("d-none")[0].reset();
			$("#login").removeClass("d-none")[0].reset();
			break;

		case 1:
			$("h1").text("Register");
			$("#login").addClass("d-none")[0].reset();
			$("#register").removeClass("d-none")[0].reset();
			break;
	}
}

$(() => {
	nfive.on("showForm", ShowForm);
	nfive.on("showError", ShowError);
	nfive.on("showInfo", ShowInfo);

	nfive.on("config", (config) => {
		let pattern = "^";
		let requirement = `Your password must be at least ${config.MinPasswordLength} characters long`;

		if (config.ForceMixedCase || config.ForceDigits || config.ForceSymbols) requirement += " and contain";

		if (config.ForceMixedCase) {
			pattern += "(?=.*[a-z])(?=.*[A-Z])";
			requirement += " upper and lower case";

			if (config.ForceDigits || config.ForceSymbols) requirement += " and";
		}

		if (config.ForceDigits || config.ForceSymbols) requirement += " at least one";

		if (config.ForceDigits) {
			pattern += "(?=.*\\d)";
			requirement += " number";

			if (config.ForceSymbols) requirement += " and";
		}

		if (config.ForceSymbols) {
			pattern += "(?=.*([^a-zA-Z\\d\\s]))";
			requirement += " special character";
		}

		pattern += `.{${config.MinPasswordLength},}$`;
		requirement += ".";

		$("#register #password").attr("pattern", pattern);
		$("#password-requirements").text(requirement);

		nfive.show();
	});

	$("#switch-login").click(() => ShowForm(0));
	$("#switch-register").click(() => ShowForm(1));

	$("#register").on("input", function() {
		$("#password-repeat", this)[0].setCustomValidity($("#password-repeat", this).val() !== $("#password", this).val() ? "Error" : "");
	});

	$("form.needs-validation").on("submit", function(e) {
		e.preventDefault();

		if ($(this)[0].checkValidity() === true) {
			nfive.send($(this).attr("id"), {
				Email: $("#email", this).val(),
				Password: $("#password", this).val()
			});
		}

		$(this).addClass("was-validated");
	});

	nfive.send("load");
});
