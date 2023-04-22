import { SocialAuthServiceConfig } from '@abacritt/angularx-social-login';
import { GoogleLoginProvider } from '@abacritt/angularx-social-login';
import { FacebookLoginProvider } from '@abacritt/angularx-social-login';
import { MicrosoftLoginProvider } from '@abacritt/angularx-social-login';

import { environment } from '../environments/environment';

export class ExternalLoginConfig {

	static readonly GOOGLE = 'google';
	static readonly AUTH0 = 'auth0';
	static readonly FACEBOOK = 'facebook';
	static readonly MICROSOFT = 'microsoft';

	static getConfiguredProviders() {
		return [
			{
				provider: ExternalLoginConfig.GOOGLE,
				name: 'Google',
			},
			{
				provider: ExternalLoginConfig.AUTH0,
				name: 'Auth0',
			},
			/*{
				provider: ExternalLoginConfig.FACEBOOK,
				name: "FaceBook",
			},
			{
				provider: ExternalLoginConfig.MICROSOFT,
				name: "Microsoft",
			},*/
		];
	}

	static getSocialConfig(): SocialAuthServiceConfig {
		var providers = [];

		if (environment.externalLogins &&
			environment.externalLogins.google &&
			environment.externalLogins.google.clientId
		) {
			providers.push({
				id: GoogleLoginProvider.PROVIDER_ID,
				provider: new GoogleLoginProvider(
					environment.externalLogins.google.clientId,
					{ scopes: 'email', }
				)
			});
		}

		return {
			autoLogin: false,
			providers: providers,
			onError: (err) => {
				console.error(err);
			}
		};
	}

	static getAuth0Config() {
		return {
			clientId: environment.externalLogins.auth0.clientId,
			domain: environment.externalLogins.auth0.domain,
		};
	}
}
