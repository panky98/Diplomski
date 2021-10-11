import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private readonly router:Router,
              private readonly authService:AuthService) {

  }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request.clone({headers:request.headers.append("Authorization","Bearer "+localStorage.getItem("eventsToken"))})).pipe(catchError((error:HttpErrorResponse)=>{
      if(error.status===401)
      {
        this.authService.LogOut();
      }
      return throwError(error);
    }));
  }
}
