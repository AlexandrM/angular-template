// angular
import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UntypedFormBuilder } from '@angular/forms';
import { UntypedFormGroup } from '@angular/forms';
import { Validators } from '@angular/forms';
import { FormControl } from '@angular/forms';

// libs
import Swal from 'sweetalert2';

// app
import { ApiRoutes } from '../../api-routes';
import { ExternalLoginConfig } from '../../config.external-login';
import { AuthService } from '../../core/service/auth.service';

@Component({
	selector: 'app-blank',
	templateUrl: './user-profile.component.html',
	styleUrls: ['./user-profile.component.scss'],
})
export class UserProfileComponent {
	form: UntypedFormGroup;
	providers?: ProviderModel[];

	constructor(
		private fb: UntypedFormBuilder,
		private httpClient: HttpClient,
		private authService: AuthService,
	) {
		this.form = this.fb.group({
			firstName: ['', [Validators.pattern('[a-zA-Z0-9]+')]],
			lastName: ['', [Validators.pattern('[a-zA-Z0-9]+')]],
			fullName: ['', [Validators.pattern('[a-zA-Z0-9 ]+')]],
			email: [{ value: '', disabled: true }, []],
		});

		this.providers = ExternalLoginConfig
			.getConfiguredProviders()
			.map(x => <ProviderModel>{
				provider: x.provider,
				name: x.name,
				isConnected: false,
				isEmailConfirmed: false
			});
	}

	ngOnInit(): void {
		this.load();
	}

	private updateForm(data: ProfileModel) {
		this.form.patchValue(data);
	}

	private load() {
		this.httpClient.get<ProfileModel>(ApiRoutes.UserProfile).subscribe(data => {
			this.updateForm(data);

			this.providers?.map((provider) => {
				var attached = data.providers?.find(x => x.provider === provider.provider);
				provider.isConnected = false;
				provider.isEmailConfirmed = false;
				if (attached) {
					provider.isConnected = true;
					provider.isEmailConfirmed = attached.isEmailConfirmed;
				}
			});
		});
	}

	onSubmit() {
		if (this.form.invalid) {
			return;
		}

		this.httpClient.post<ProfileModel>(ApiRoutes.UserProfile, this.form.value).subscribe(data => {
			this.updateForm(data);
			console.log(this.form);
		});
	}

	connect(provider: ProviderModel) {
		provider.error = undefined;
		provider.isLoading = true;

		if (provider.provider === ExternalLoginConfig.GOOGLE) {
			this.authService.attachGoogle().subscribe(r => {
				this.load();
				provider.isLoading = false;
			});
		}
		if (provider.provider === ExternalLoginConfig.AUTH0) {
			this.authService.attachAuth0().subscribe(resp => {
				provider.error = '';
				if (!resp.success) {
					provider.error = resp.data || 'Connect failed';
				}
				this.load();
				provider.isLoading = false;
			});
		}
	}

	disconnect(provider: string) {
		Swal.fire({
			title: 'Are you sure to disconnect ' + provider + ' ?',
			text: 'Some providers can be connected only with login!',
			icon: 'warning',
			showCancelButton: true,
			confirmButtonColor: '#3085d6',
			cancelButtonColor: '#d33',
			confirmButtonText: 'Yes, disconnect it!',
		}).then((result) => {
			if (result.value) {
				this.authService.deattach(provider).subscribe(x => {
					this.load();
				});
			}
		});
	}
}

interface ProviderModel {
	provider: string;
	name: string;
	isConnected: boolean;
	isEmailConfirmed: boolean;
	error?: string;
	isLoading: boolean;
}

interface ProfileModel {
	firstName?: string,
	lastName?: string,
	fullName?: string,
	isEmailConfirmed?: boolean,
	providers?: ProviderModel[],
}
