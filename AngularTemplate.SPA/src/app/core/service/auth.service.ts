// angular
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';

// libs
import { BehaviorSubject } from 'rxjs';
import { Observable } from 'rxjs';
import { Subject } from 'rxjs';
import { of } from 'rxjs';
import { throwError } from 'rxjs';
import { from } from 'rxjs';
import { combineLatest } from 'rxjs';
import { map } from 'rxjs/operators';
import { catchError } from 'rxjs/operators';
import { AuthService as Auth0Service } from '@auth0/auth0-angular';

import { SocialAuthService } from '@abacritt/angularx-social-login';
import { GoogleLoginProvider } from '@abacritt/angularx-social-login';

// app
import { OidcHelperService } from './oidc-helper.service';
import { UserModel } from '../models/user.model';
import { ApiRoutes } from '../../api-routes';
import { RegisterModel } from '../models/register.model';
import { LoginModel } from '../models/login.model';
import { UserProfileModel } from '../models/user-profile.model';
import { GeneralResultModel } from '../models/general-result.model';
import { SubjectExtensions } from '../extensions/general.extensions';

export function authAppInitializerFactory(authService: AuthService): () => Promise<void> {
	if (window.location.href.indexOf('/silent-refresh.html') !== -1) {
		return () => Promise.resolve();
	}
	return () => authService.runInitialLoginSequence();
}

@Injectable({
	providedIn: 'root',
})
export class AuthService {
	private currentUserSubject: BehaviorSubject<UserModel>;
	currentUser$: Observable<UserModel>;

	private isAuthenticatedSubject$ = new BehaviorSubject<boolean>(false);
	isAuthenticated$ = this.isAuthenticatedSubject$.asObservable();

	constructor(
		private http: HttpClient,
		private router: Router,
		private route: ActivatedRoute,
		private oidcHelperService: OidcHelperService,
		private auth0Service: Auth0Service,
		private externalAuthService: SocialAuthService,
	) {
		this.currentUserSubject = new BehaviorSubject<UserModel>({} as UserModel);
		this.currentUser$ = this.currentUserSubject.asObservable();

		// on external social login
		this.externalAuthService.authState.subscribe((user) => {
			oidcHelperService.loginByExternalLogin('google', user.idToken);
		});

		this.oidcHelperService.isDoneLoading$.subscribe(isDone => {
			if (!isDone) {
				return;
			}

			this.oidcHelperService.isAuthenticated$.subscribe(authState => {
				let redirectUrl = this.getRedirect();
				if (authState.isAuthenticated) {
					this.oidcHelperService.loadUserProfile().then(e => {
						let resp = (e as any).info;
						let userProfile = {
							id: resp.id,
							email: resp.email,
							phone: resp.phone,
							firstName: resp.firstName,
							lastName: resp.lastName,
							fullName: resp.fullName,
						};

						this.setCurrentUserValue(userProfile.id, userProfile);

						if (authState.action === AuthenticatedActionEnum.loggedIn) {
							this.router.navigate([redirectUrl || '/dashboard/dashboard1']);
						}
					});
				}
			});
		});
	}

	setCurrentUserValue(id: string, profile?: UserProfileModel) {
		this.currentUserSubject.next({
			id: id,
			img: 'assets/images/user/admin.jpg',
			username: profile?.email || '',
			firstName: profile?.firstName || '',
			lastName: profile?.lastName || '',
		});
	}

	get currentUserValue(): UserModel {
		return this.currentUserSubject.value;
	}

	login(loginModel: LoginModel): Subject<GeneralResultModel> {
		return SubjectExtensions.start<GeneralResultModel>(subject => {
			this.oidcHelperService
				.login(loginModel.username, loginModel.password)
				.subscribe(resp => {
						if (resp.access_token) {
							this.isAuthenticatedSubject$.next(true);
						}
						subject.next({ success: !!(resp.access_token) });
					},
					err => {
						subject.next({ success: false, error: err.error });
					});
		});
	}

	logout() {
		this.currentUserSubject.next({} as UserModel);
		this.isAuthenticatedSubject$.next(false);
		this.oidcHelperService.logout();

		return of({ success: false });
	}

	register(registerModel: RegisterModel): Observable<any> {
		return this.http
			.post<RegisterModel>(ApiRoutes.UserRegister, registerModel)
			.pipe(
				map(resp => resp),
				catchError(error => {
					return throwError(error);
				})
			);
	}

	runInitialLoginSequence() {
		return this.oidcHelperService.runInitialLoginSequence();
	}

	get isAuthenticated(): boolean {
		return this.oidcHelperService.hasValidAccessToken();
	}

	getAccessToken() {
		return this.oidcHelperService.getAccessToken();
	}

	loginGoogle(redirectUrl?: string): Observable<any> {
		return from(this.externalAuthService.signIn(GoogleLoginProvider.PROVIDER_ID));
	}

	attachGoogle(): Observable<any> {
		var result = new Subject();

		/*this.externalAuthService.signIn(GoogleLoginProvider.PROVIDER_ID).then(function (resp) {
			//console.log('attachGoogle', resp);
			//return result.next(resp);
			//this.attach(idToken.__raw, result);
		});*/

		return result;
	}

	loginAuth0(): Observable<GeneralResultModel> {
		return SubjectExtensions.start<GeneralResultModel>((subject) => {
			var subs = this.auth0Service.idTokenClaims$.subscribe(token => {
				if (token && token?.__raw) {
					subs.unsubscribe();

					this.oidcHelperService.loginByExternalLogin('auth0', token.__raw).subscribe(login => {
						if (login.success) {
							this.isAuthenticatedSubject$.next(true);
						}
						subject.next({ success: login.success, error: login.error });
					});
				} else {
					//subject.next({ success: false, error: { description: 'Login failed' } });
				}
			});

			this.auth0Service.getAccessTokenWithPopup().subscribe(popupToken => {
				if (!popupToken) {
					this.auth0Service.getAccessTokenSilently().subscribe(silently => {
					});
				}
			});
		});
	}

	attachAuth0(): Observable<any> {
		return SubjectExtensions.start<GeneralResultModel>((subject) => {
			var subs = this.auth0Service.idTokenClaims$.subscribe(token => {
				if (token && token?.__raw) {
					subs.unsubscribe();

					this.attach(token?.__raw).subscribe(
						(resp) => subject.next({ success: true, data: token?.__raw }),
						(err) => subject.next({ success: false, data: err })
					);
				} else {
					subject.next({ success: false, data: 'Failed' });
				}
			});

			this.auth0Service.getAccessTokenWithPopup().subscribe(popupToken => {
				if (!popupToken) {
					this.auth0Service.getAccessTokenSilently().subscribe(silently => {
					});
				}
			});
		});
	}

	deattach(provider: string): Observable<any> {
		var result = new Subject();

		var headers = new HttpHeaders({
			'Content-Type': 'application/json',
			Accept: 'application/json, text/plain, */*',
			'Authorization': 'Bearer ' + this.getAccessToken()
		});
		this.http
			.post<any>(ApiRoutes.UserDeattach, { provider: provider }, { headers })
			.pipe(
				map(resp => resp),
				catchError(error => {
					return throwError(error);
				}))
			.subscribe((resp) => result.next(resp));

		return result;
	}

	private attach(token: string): Observable<any> {
		var data = { provider: 'auth0', token: token };
		var headers = new HttpHeaders({
			'Content-Type': 'application/json',
			Accept: 'application/json, text/plain, */*',
			'Authorization': 'Bearer ' + this.getAccessToken()
		});
		return this.http
			.post<any>(ApiRoutes.UserAttach, data, { headers });
	}

	private getRedirect(): string | undefined {
		var redirectUrl = this.route.snapshot.queryParams['r'];
		redirectUrl = redirectUrl ? decodeURIComponent(redirectUrl) : undefined;
		return redirectUrl;
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

export enum StateEnum {
	undefined,
	inited,
	completed,
	failed,
}
