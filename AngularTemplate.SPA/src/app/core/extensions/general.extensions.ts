// angular
import { ActivatedRoute } from '@angular/router';

// libs
import { BehaviorSubject, Observable, Subject, of, throwError, from, combineLatest } from 'rxjs';

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

interface Action<T> {
	(item: T): void;
}

interface Func<T, TResult> {
	(item: T): TResult;
}

export class SubjectExtensions {

	static start<T>(start: Action<Subject<T>>): Subject<T> {
		var result = new Subject<T>();

		start(result);

		return result;
	}
}
