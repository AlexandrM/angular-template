import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { GeneralErrorModel } from '../../core/models/general-error.model';
import { AuthService } from '../../core/service/auth.service';

@Component({
	selector: 'app-signup',
	templateUrl: './signup.component.html',
	styleUrls: ['./signup.component.scss'],
})
export class SignupComponent implements OnInit {
	authForm!: UntypedFormGroup;
	submitted = false;
	returnUrl!: string;
	hide = true;
	chide = true;
	generalError?: GeneralErrorModel;

	constructor(
		private formBuilder: UntypedFormBuilder,
		private route: ActivatedRoute,
		private router: Router,
		private authService: AuthService
	) {

	}

	ngOnInit() {
		this.authForm = this.formBuilder.group({
			email: ['', [Validators.required, Validators.email, Validators.minLength(5)]],
			password: ['', Validators.required],
			cpassword: ['', Validators.required],
		});
		// get return url from route parameters or default to '/'
		this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
	}

	get f() {
		return this.authForm.controls;
	}

	onSubmit() {
		this.submitted = true;
		// stop here if form is invalid
		if (this.authForm.invalid) {
			return;
		}
		//this.router.navigate(['/admin/dashboard/main']);
		this.authService.register({
			username: this.authForm.get('email')!.value!,
			password: this.authForm.get('password')!.value!,
			confirmPassword: this.authForm.get('cpassword')!.value!,
		}).subscribe(
			resp => {
				if (!resp.succeeded) {
					this.generalError = resp as GeneralErrorModel;
				} else {
					this.router.navigate(['authentication/signin']);
				}
			},
			err => {
				this.generalError = err.error as GeneralErrorModel;
			}
		);
	}

	loginAuth0() {
		this.authService.loginAuth0().subscribe(x => {
		});
	}
}
