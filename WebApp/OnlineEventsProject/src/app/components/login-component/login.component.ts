import { HttpClient, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable, Subscription, throwError } from 'rxjs';
import {catchError} from 'rxjs/operators'
import { AuthService } from 'src/app/services/auth.service';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  @ViewChild('forma') forma:NgForm | undefined;

  constructor(private router:Router,
              private httpClient:HttpClient,
              private authService:AuthService) {

   }

  ngOnInit(): void {

  }

  onSubmit(){
      this.authService.LogIn(this.forma?.value.usernameInput,this.forma?.value.passwordInput);
  }
}
