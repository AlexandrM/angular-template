// angular
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { OAuthErrorEvent } from 'angular-oauth2-oidc';

// libs
import { filter, map } from 'rxjs/operators';
import { Subject, Observable, BehaviorSubject, from } from 'rxjs';

// app
import { GeneralResultModel } from '../models/general-result.model';

@Injectable()
export class OidcHelperService {
	private isAuthenticatedSubject$ = new BehaviorSubject<IAuthenticatedState>({
		isAuthenticated: false,
		action: AuthenticatedActionEnum.init
	});
	isAuthenticated$ = this.isAuthenticatedSubject$.asObservable();

	private isDoneLoadingFailed = false;
	private isDoneLoadingSubject$ = new BehaviorSubject<boolean>(false);
	isDoneLoading$ = this.isDoneLoadingSubject$.asObservable();

	constructor(
		private oauthService: OAuthService,
		private router: Router,
	) {
		this.oauthService.oidc = false;

		// all events handler
		this.oauthService.events.subscribe(event => {
			if (event instanceof OAuthErrorEvent) {
				console.error('OAuthErrorEvent Object:', event);
				if (event.type === 'discovery_document_validation_error') {
					this.isDoneLoadingFailed = true;
					this.setIsAuthenticatedSubject$(false, AuthenticatedActionEnum.update);
				}
			} else {
				var hasValidAccessToken = this.hasValidAccessToken();
				var isAuthenticated = this.isAuthenticatedSubject$.getValue().isAuthenticated;
				if (isAuthenticated !== hasValidAccessToken) {
					this.setIsAuthenticatedSubject$(hasValidAccessToken, AuthenticatedActionEnum.update);
				}
			}
		});

		// This is tricky, as it might cause race conditions (where access_token is set in another
		// tab before everything is said and done there.
		// TODO: Improve this setup. See: https://github.com/jeroenheijmans/sample-angular-oauth2-oidc-with-auth-guards/issues/2
		window.addEventListener('storage',
			(event) => {
				// The `key` is `null` if the event was caused by `.clear()`
				if (event.key !== 'access_token' && event.key !== null) {
					return;
				}

				console.warn(
					'Noticed changes to access_token (most likely from another tab), updating isAuthenticated');
				this.setIsAuthenticatedSubject$(this.hasValidAccessToken(), AuthenticatedActionEnum.update);

				if (!this.hasValidAccessToken()) {
					console.log('storage access_token updated !this.hasValidAccessToken()');
					this.logout();
					this.navigateToLoginPage();
				}
			});

		// init login stata
		this.setIsAuthenticatedSubject$(this.hasValidAccessToken(), AuthenticatedActionEnum.update);

		// TODO: some time receive 'message' with data 'error' == 'session_error''
		/*window.addEventListener('message', e => {
			console.log('message', e);;
			if (e.origin === 'https://dev-w08xm0pi.us.auth0.com') {
				console.log('message stopPropagation');;
				e.stopPropagation();
			}
		});
		this.oauthService.events.subscribe(x => {
			console.log('events', x);
		});*/
		this.oauthService.events
			.pipe(filter(e => ['session_terminated', 'session_error'].includes(e.type)))
			.subscribe(e => {
				console.log('events session_terminated', e, this.hasValidAccessToken());
				this.navigateToLoginPage();
			});

		this.oauthService.setupAutomaticSilentRefresh();
	}

	private setIsAuthenticatedSubject$(
		isAuthenticated: boolean,
		action: AuthenticatedActionEnum
	) {
		var state = this.isAuthenticatedSubject$.getValue();
		if (state.isAuthenticated !== isAuthenticated || state.action !== action
		) {
			this.isAuthenticatedSubject$.next({ isAuthenticated, action });
		}
	}

	private navigateToLoginPage() {
		this.router.navigateByUrl('/authentication/signin');
	}

	logout() {
		this.oauthService.logOut(true);
	}

	refresh() {
		this.oauthService.silentRefresh();
	}

	hasValidAccessToken() {
		return this.oauthService.hasValidAccessToken();
	}

	getAccessToken() {
		return this.oauthService.getAccessToken();
	}

	login(username: string, password: string) {
		//return this.oauthService.fetchTokenUsingPasswordFlow(username, password);
		return from(this.oauthService.fetchTokenUsingPasswordFlow(username, password));
	}

	loginByExternalLogin(
		provider: string,
		token: string,
		redirectUrl?: string
	): Observable<GeneralResultModel> {
		var result = new Subject<GeneralResultModel>();

		let params = {
			token: token,
			provider: provider,
		};
		this.oauthService
			.fetchTokenUsingGrant('external', params)
			.then(x => {
				this.setIsAuthenticatedSubject$(this.hasValidAccessToken(), AuthenticatedActionEnum.loggedIn);
				result.next({ success: true });
			})
			.catch(err => {
				result.next({ success: false, error: { code: err, description: err } });
			});

		return result;
	}

	loadUserProfile(): Promise<object> {
		return this.oauthService.loadUserProfile();
	}

	runInitialLoginSequence(): Promise<void> {
		if (location.hash) {
			//console.log('Encountered hash fragment, plotting as table...');
			//console.table(location.hash.substr(1).split('&').map(kvp => kvp.split('=')));
		}
		if (this.isDoneLoadingFailed) {
			return new Promise<void>(resolve => setTimeout(() => resolve(), 1500));
		}
		// 0. LOAD CONFIG:
		// First we have to check to see how the IdServer is
		// currently configured:
		return this.oauthService.loadDiscoveryDocument()

			// For demo purposes, we pretend the previous call was very slow
			.then(() => {
				new Promise<void>(resolve => setTimeout(() => resolve(), 1500));
			})

			// 1. HASH LOGIN:
			// Try to log in via hash fragment after redirect back
			// from IdServer from initImplicitFlow:
			.then(() => {
				this.oauthService.tryLogin();
			})
			.then(() => {
				if (this.hasValidAccessToken()) {
					return Promise.resolve();
				}

				// 2. SILENT LOGIN:
				// Try to log in via a refresh because then we can prevent
				// needing to redirect the user:
				return this.oauthService.silentRefresh()
					.then(() => Promise.resolve())
					.catch(result => {
						// Subset of situations from https://openid.net/specs/openid-connect-core-1_0.html#AuthError
						// Only the ones where it's reasonably sure that sending the
						// user to the IdServer will help.
						const errorResponsesRequiringUserInteraction = [
							'interaction_required',
							'login_required',
							'account_selection_required',
							'consent_required',
						];

						if (result &&
							result.reason &&
							errorResponsesRequiringUserInteraction.indexOf(result.reason.error) >= 0) {

							// 3. ASK FOR LOGIN:
							// At this point we know for sure that we have to ask the
							// user to log in, so we redirect them to the IdServer to
							// enter credentials.
							//
							// Enable this to ALWAYS force a user to login.
							// this.login();
							//
							// Instead, we'll now do this:
							console.warn(
								'User interaction is needed to log in, we will wait for the user to manually log in.');
							return Promise.resolve();
						}

						// We can't handle the truth, just pass on the problem to the
						// next handler.
						return Promise.reject(result);
					});
			})
			.then(() => {
				this.isDoneLoadingSubject$.next(true);

				// Check for the strings 'undefined' and 'null' just to be sure. Our current
				// login(...) should never have this, but in case someone ever calls
				// initImplicitFlow(undefined | null) this could happen.
				if (this.oauthService.state &&
					this.oauthService.state !== 'undefined' &&
					this.oauthService.state !== 'null') {
					let stateUrl = this.oauthService.state;
					if (stateUrl.startsWith('/') === false) {
						stateUrl = decodeURIComponent(stateUrl);
					}
					console.log(`There was state of ${this.oauthService.state}, so we are sending you to: ${stateUrl}`);
					this.router.navigateByUrl(stateUrl);
				}
			})
			.catch(() => this.isDoneLoadingSubject$.next(true));
	}
}

export enum AuthenticatedActionEnum {
	init,
	update,
	loggedIn,
	loggedOff,
}

export interface IAuthenticatedState {
	isAuthenticated: boolean,
	action: AuthenticatedActionEnum,
	redirectUrl?: string,
}
