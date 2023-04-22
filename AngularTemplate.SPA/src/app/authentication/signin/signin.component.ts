// angular
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';

// libs
import { SocialAuthService } from '@abacritt/angularx-social-login';

// app
import { AuthService } from 'src/app/core/service/auth.service';
import { UnsubscribeOnDestroyAdapter } from 'src/app/shared/UnsubscribeOnDestroyAdapter';

@Component({
	selector: 'app-signin',
	templateUrl: './signin.component.html',
	styleUrls: ['./signin.component.scss'],
})
export class SigninComponent extends UnsubscribeOnDestroyAdapter
implements OnInit {
	authForm!: UntypedFormGroup;
	submitted = false;
	loading = false;
	error?= '';
	hide = true;

	constructor(
		private formBuilder: UntypedFormBuilder,
		private router: Router,
		private route: ActivatedRoute,
		private authService: AuthService,
		//private externalAuthService: SocialAuthService,
	) {
		super();
	}

	ngOnInit() {
		this.authForm = this.formBuilder.group({
			username: ['', Validators.required],
			password: ['', Validators.required],
		});

		this.authService.isAuthenticated$.subscribe(isAuthenticated => {
			if (isAuthenticated) {
				this.router.navigate([this.getRedirect() || '/dashboard/dashboard1']);
			}
		});
	}

	onSubmit() {
		if (this.authForm.invalid) {
			this.error = 'Username and Password not valid!';
			return;
		}

		this.submitted = true;
		this.loading = true;
		this.error = '';

		var user = {
			username: this.authForm.controls['username'].value,
			password: this.authForm.controls['password'].value
		};

		this.subs.sink = this.authService
			.login(user)
			.subscribe({
				next: (resp) => {
					if (!resp.success) {
						this.error = 'Invalid Login';
						if (!resp.success && resp.error?.code === 'invalid_grant') {
							this.error = 'Invalid username or password';
						}
						this.submitted = false;
						this.loading = false;
					}
				},
				error: (error) => {
					this.error = 'Invalid username or password';
					this.submitted = false;
					this.loading = false;
				},
			});
	}

	loginAuth0() {
		this.authService.loginAuth0().subscribe(resp => {
			if (!resp.success) {
				this.error = 'Invalid login';
			}
		});
	}

	private getRedirect(): string | undefined {
		var redirectUrl = this.route.snapshot.queryParams['r'];
		redirectUrl = redirectUrl ? decodeURIComponent(redirectUrl) : undefined;
		return redirectUrl;
	}
}
