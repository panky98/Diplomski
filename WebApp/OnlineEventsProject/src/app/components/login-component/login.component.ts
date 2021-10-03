import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

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
      this.httpClient.get("http://localhost:52800/api/User/LogIn/"+this.forma?.value.usernameInput+"/"+this.forma?.value.passwordInput)
      .subscribe((response)=>{
          console.log(response);
      },(error:HttpErrorResponse)=>{
        console.log(error);
        if(error.status==200)
          this.router.navigate(["events"]);
      });
  }
}
