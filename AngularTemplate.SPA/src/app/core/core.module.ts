// angular
import { NgModule, Optional, SkipSelf, CUSTOM_ELEMENTS_SCHEMA, APP_INITIALIZER } from '@angular/core';
import { CommonModule } from '@angular/common';

// libs
import { OAuthModule, OAuthModuleConfig, OAuthStorage } from 'angular-oauth2-oidc';
import { AuthConfig } from 'angular-oauth2-oidc';
import { AuthModule } from '@auth0/auth0-angular';
import { SocialLoginModule } from '@abacritt/angularx-social-login';

// app
import { AuthGuard } from './guard/auth.guard';
import { RightSidebarService } from './service/rightsidebar.service';
import { AuthService, authAppInitializerFactory } from './service/auth.service';
import { DirectionService } from './service/direction.service';
import { throwIfAlreadyLoaded } from './guard/module-import.guard';
import { OidcHelperService } from './service/oidc-helper.service';
import { AuthCodeFlowConfig, AuthModuleConfig } from '../config.oidc';
import { SubjectExtensions } from './extensions/general.extensions';
import { ExternalLoginConfig } from '../config.external-login';

export function storageFactory(): OAuthStorage {
	return localStorage;
}

@NgModule({
	declarations: [],
	imports: [
		CommonModule,
		SocialLoginModule,
		OAuthModule.forRoot(),
		AuthModule.forRoot(ExternalLoginConfig.getAuth0Config()),
	],
	providers: [
		{ provide: APP_INITIALIZER, useFactory: authAppInitializerFactory, deps: [AuthService], multi: true },
		{ provide: AuthConfig, useValue: AuthCodeFlowConfig },
		{ provide: OAuthModuleConfig, useValue: AuthModuleConfig },
		{ provide: OAuthStorage, useFactory: storageFactory },
		{ provide: 'SocialAuthServiceConfig', useValue: ExternalLoginConfig.getSocialConfig() },
		RightSidebarService,
		AuthGuard,
		AuthService,
		DirectionService,
		OidcHelperService,
		SubjectExtensions,
	],
	schemas: [
		CUSTOM_ELEMENTS_SCHEMA
	]
})
export class CoreModule {
	constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
		throwIfAlreadyLoaded(parentModule, 'CoreModule');
	}
}
