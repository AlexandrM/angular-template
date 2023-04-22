import { ActivatedRoute } from '@angular/router';

export { }

declare global {
	interface RouterExtensions {
		addDays(days: number): Date;
	}
	}

export class ExActivatedRoute extends ActivatedRoute {

	getCurrentRoute(): string {
		var redirectUrl = this.snapshot.queryParams['r'];
		redirectUrl = redirectUrl ? decodeURIComponent(redirectUrl) : undefined;
		return redirectUrl;
	}
}
