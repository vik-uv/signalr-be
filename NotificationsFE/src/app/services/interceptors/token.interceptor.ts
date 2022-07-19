import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';

import { Observable } from 'rxjs';

export const LocalStorageAuthToken = 'NotificationAuthToken';

@Injectable()
export class TokenInterceptor {

private token: string | null;

constructor() {
    this.token = localStorage.getItem(LocalStorageAuthToken);
}

intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    if (this.token) {
        request = request.clone({
            setHeaders: {
                Authorization: `Basic ${this.token}`
            }
        });
    }

    return next.handle(request);
  }
}