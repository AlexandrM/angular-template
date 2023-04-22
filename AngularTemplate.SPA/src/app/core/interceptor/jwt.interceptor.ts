import { Injectable } from '@angular/core';
import {
	HttpRequest,
	HttpHandler,
	HttpEvent,
	HttpInterceptor,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../service/auth.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
	constructor(private authenticationService: AuthService) {}

	intercept(
		request: HttpRequest<any>,
		next: HttpHandler
	): Observable<HttpEvent<any>> {
		if (this.authenticationService.isAuthenticated) {
			request = request.clone({
				setHeaders: {
					Authorization: `Bearer ${this.authenticationService.getAccessToken()}`,
				},
			});
		}

		return next.handle(request);
	}
}
