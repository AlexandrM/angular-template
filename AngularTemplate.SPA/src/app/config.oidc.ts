import { AuthConfig } from 'angular-oauth2-oidc';
import { OAuthModuleConfig } from 'angular-oauth2-oidc';

import { environment } from '../environments/environment';

export const AuthCodeFlowConfig: AuthConfig = {
	// Url of the Identity Provider
	issuer: environment.identityServer,
	requireHttps: false,
	strictDiscoveryDocumentValidation: false,

	// URL of the SPA to redirect the user to after login
	redirectUri: window.location.origin + '/',
	silentRefreshRedirectUri: window.location.origin + '/silent-refresh.html',
	useSilentRefresh: true,
	silentRefreshTimeout: 5000, // For faster testing
	timeoutFactor: 0.25, // For faster testing
	sessionChecksEnabled: true,

	// The SPA's id. The SPA is registerd with this id at the auth-server
	// clientId: 'server.code',
	clientId: 'angulartemplate_spa',

	// Just needed if your auth server demands a secret. In general, this
	// is a sign that the auth server is not configured with SPAs in mind
	// and it might not enforce further best practices vital for security
	// such applications.
	// dummyClientSecret: 'secret',

	responseType: 'code',

	// set the scope for the permissions the client should request
	// The first four are defined by OIDC.
	// Important: Request offline_access to get a refresh token
	// The api scope is a usecase specific one
	scope: 'openid profile email api',

	showDebugInformation: true,
};

export const AuthModuleConfig: OAuthModuleConfig = {
	resourceServer: {
		allowedUrls: environment.allowedUrls,
		sendAccessToken: true,
	}
};
