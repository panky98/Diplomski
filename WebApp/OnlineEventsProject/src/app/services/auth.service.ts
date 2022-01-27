import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { throwError } from 'rxjs';
import { Subject } from 'rxjs/internal/Subject';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  public loggedIn:Subject<boolean>;

  constructor(private readonly router:Router,
            private httpClient:HttpClient) {
    this.loggedIn=new Subject<boolean>();
  }

  LogIn(usernameInput:string,passwordInput:string){
    this.httpClient.get<{"token":string}>("http://localhost:52803/User/LogIn/"+usernameInput+"/"+passwordInput).pipe(catchError((error=>{
      return throwError(error);
    })))
    .subscribe((response)=>{
        localStorage.setItem("eventsToken",response.token)
        this.router.navigate(["events"]);
        this.loggedIn.next(true);
    },(error:HttpErrorResponse)=>{
      console.log(error);
      alert("Wrong entries");
    });
  }

  LogOut(){
    localStorage.clear();
    this.router.navigate(["login"]);
    this.loggedIn.next(false);
  }
}
