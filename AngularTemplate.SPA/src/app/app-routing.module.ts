import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { Page404Component } from './authentication/page404/page404.component';
import { AuthGuard } from './core/guard/auth.guard';
import { AuthLayoutComponent } from './layout/app-layout/auth-layout/auth-layout.component';
import { MainLayoutComponent } from './layout/app-layout/main-layout/main-layout.component';

const routes: Routes = [
	{
		path: '',
		component: MainLayoutComponent,
		canActivate: [AuthGuard],
		children: [
			{ path: '', redirectTo: '/authentication/signin', pathMatch: 'full' },
			{
				path: 'dashboard',
				loadChildren: () =>
					import('./dashboard/dashboard.module').then((m) => m.DashboardModule),
			},
			{
				path: 'extra-pages',
				loadChildren: () =>
					import('./extra-pages/extra-pages.module').then(
						(m) => m.ExtraPagesModule
					),
			},
			{
				path: 'multilevel',
				loadChildren: () =>
					import('./multilevel/multilevel.module').then(
						(m) => m.MultilevelModule
					),
			},
		],
	},
	{
		path: 'authentication',
		component: AuthLayoutComponent,
		loadChildren: () =>
			import('./authentication/authentication.module').then(
				(m) => m.AuthenticationModule
			),
	},
	{
		path: '',
		component: MainLayoutComponent,
		canActivate: [AuthGuard],
		loadChildren: () =>
			import('./pages/pages.module').then(
				(m) => m.PagesModule
			),
	},
	{ path: '**', component: Page404Component },
];

@NgModule({
	imports: [RouterModule.forRoot(routes, {})],
	exports: [RouterModule],
})
export class AppRoutingModule {
}
