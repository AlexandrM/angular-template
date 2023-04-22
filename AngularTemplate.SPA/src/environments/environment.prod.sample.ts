export const environment = {
	production: true,
	identityServer: window.location.origin,
	allowedUrls: [window.location.origin],
	externalLogins: {
		google: {
			clientId: '',
		},
		auth0: {
			clientId: '',
			domain: '',
		}
	}
};
