// DEBUG SHIM
if (typeof window.nfive === "undefined") {
	window.nfive = {
		on: () => {},
		send: async (event, data) => console.log(event, data),
		log: (...args) => console.log(...args)
	};
}
// DEBUG SHIM


function ShowForm(form) {
	// Clear error
	$("#error-alert").text("").addClass("d-none");

	// Reset form validation
	$("form").removeClass("was-validated");

	// Show form and title
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
	nfive.on("config", (config) => {
		// Build password requirements
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

		// Apply requirements
		$("#register #password").attr("pattern", pattern);
		$("#password-requirements").text(requirement);

		// Show overlay
		nfive.show();
	});

	$("#switch-login").click(() => ShowForm(0));
	$("#switch-register").click(() => ShowForm(1));

	$("#register").on("input", function() {
		$("#password-repeat", this)[0].setCustomValidity($("#password-repeat", this).val() !== $("#password", this).val() ? "Error" : "");
	});

	$("form.needs-validation").on("submit", async function(e) {
		e.preventDefault();

		if ($(this)[0].checkValidity() === true) {
			// Hide validation
			$(this).removeClass("was-validated");

			// Disable form
			$(":input", this).prop("disabled", true);

			// Show button spinner
			$("button[type=submit] span", this).removeClass("d-none");

			// Send data
			const response = await nfive.send($(this).attr("id"), {
				Email: $("#email", this).val(),
				Password: $("#password", this).val()
			});

			const data = await response.json();

			if (data.Success) return; // Overlay will be destroyed

			// Show error
			$("#error-alert").html(data.Message).removeClass("d-none");

			// Hide button spinner
			$("button[type=submit] span", this).addClass("d-none");

			// Enable form
			$(":input", this).prop("disabled", false);

			// Clear password inputs
			$("#password, #password-repeat", this).val("");

			// Focus password input
			$("#password", this).focus();
		} else {
			$(this).addClass("was-validated");
		}
	});

	nfive.send("load");
});
