function ShowError(message) {
	HideInfo();
	$('#error-alert').text(message).removeClass('d-none');
}

function HideError() {
	$('#error-alert').text('').addClass('d-none');
}

function ShowInfo(message) {
	HideError();
	$('#info-alert').text(message).removeClass('d-none');
}

function HideInfo() {
	$('#info-alert').text('').addClass('d-none');
}

function SwitchForm(form) {
	HideError();
	HideInfo();

	switch (form) {
		case 1:
			$('h1').text('Login');
			$('#register').addClass('d-none')[0].reset();
			$('#login').removeClass('d-none')[0].reset();
			break;

		case 2:
			$('h1').text('Register');
			$('#login').addClass('d-none')[0].reset();
			$('#register').removeClass('d-none')[0].reset();
			break;
	}
}


// const nfive = {
// 	on: () => {},
// 	send: (event, data) => console.log(event, data),
// 	log: (...args) => console.log(...args)
// };


$(() => {

	nfive.log('START');

	nfive.on('switchForm', SwitchForm);
	nfive.on('showError', ShowError);
	nfive.on('hideError', HideError);
	nfive.on('showInfo', ShowInfo);
	nfive.on('hideInfo', HideInfo);

	nfive.on('config', (config) => {
		nfive.log('config');
		nfive.log(config);

		let pattern = '^';
		let requirement = 'Your password must be at least ' + config.MinPasswordLength + ' characters long';

		if (config.ForceMixedCase || config.ForceDigits || config.ForceSymbols) requirement += ' and contain';

		if (config.ForceMixedCase) {
			pattern += '(?=.*[a-z])(?=.*[A-Z])';

			requirement += ' upper and lower case';

			if (config.ForceDigits || config.ForceSymbols) requirement += ' and';
		}

		if (config.ForceDigits || config.ForceSymbols) requirement += ' at least one';

		if (config.ForceDigits) {
			pattern += '(?=.*\\d)';

			requirement += ' number';

			if (config.ForceSymbols) requirement += ' and';
		}

		if (config.ForceSymbols) {
			pattern += '(?=.*([^a-zA-Z\\d\\s]))';

			requirement += ' special character';
		}

		pattern += '.{' + config.MinPasswordLength + ',}$';
		requirement += '.';

		$('#register #password').attr('pattern', pattern);
		$('#password-requirements').text(requirement);

		nfive.log(pattern);
		nfive.log(requirement);

		nfive.log('config');

		nfive.show();
	});

	nfive.log('END');

	$('#switch-login').click(() => SwitchForm(1));
	$('#switch-register').click(() => SwitchForm(2));

	$('#register').on('input', function() {
		$('#password-repeat', this)[0].setCustomValidity($('#password-repeat', this).val() != $('#password', this).val() ? 'Error' : '');
	});

	$('form.needs-validation').on('submit', function(e) {
		e.preventDefault();

		if ($(this)[0].checkValidity() === true) {
			nfive.send($(this).attr('id'), {
				Email: $('#email', this).val(),
				Password: $('#password', this).val()
			});
		}

		$(this).addClass('was-validated');
	});

	nfive.send('load');
});
