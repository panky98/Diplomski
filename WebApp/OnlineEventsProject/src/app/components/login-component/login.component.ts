import { HttpClient, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import {catchError} from 'rxjs/operators'


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  @ViewChild('forma') forma:NgForm | undefined;

  constructor(private router:Router,private httpClient:HttpClient) {

   }

  ngOnInit(): void {
  }

  onSubmit(){
      this.httpClient.get<{"token":string}>("http://localhost:52800/api/User/LogIn/"+this.forma?.value.usernameInput+"/"+this.forma?.value.passwordInput).pipe(catchError((error=>{
        return throwError(error);
      })))
      .subscribe((response)=>{
          localStorage.setItem("eventsToken",response.token)
          this.router.navigate(["events"]);
      },(error:HttpErrorResponse)=>{
        console.log(error);
      });
  }
}
