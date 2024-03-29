import { Component, OnInit, ViewChild } from '@angular/core';
import { SelectItem } from 'primeng/api/public_api';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { HttpErrorResponse,HttpClient } from '@angular/common/http';
import { Interest } from 'src/app/models/Interest';

@Component({
  selector: 'app-registration-component',
  templateUrl: './registration-component.component.html',
  styleUrls: ['./registration-component.component.css']
})
export class RegistrationComponent implements OnInit {
  selectOptions: SelectItem[];
  selectedOptions:Array<number>;

  @ViewChild('forma') forma:NgForm | undefined;

  constructor(private readonly router:Router,
              private readonly httpClient:HttpClient) {
    this.selectOptions=[];
    this.selectedOptions=[];
   }

  ngOnInit(): void {
    this.httpClient.get<Interest[]>("http://localhost:52803/Interest").subscribe((data)=>{
      data.forEach((interest)=>{
        this.selectOptions.push({label:interest.name,value:interest.id});
      });
    })
  }

  onSubmit(){
    let newUser:{name:string,surname:string,phoneNumber:string,username:string,password:string,interestsByUser:Array<number>}={
      name:this.forma?.value["name"],
      surname:this.forma?.value["surname"],
      username:this.forma?.value["username"],
      password:this.forma?.value["password"],
      phoneNumber:this.forma?.value["phoneNumber"],
      interestsByUser:this.selectedOptions
    }

    this.httpClient.post("http://localhost:52803/User",newUser).pipe(catchError((error=>{
      return throwError(error);
    })))
    .subscribe(()=>{
      this.router.navigate(["login"]);
    },(error:HttpErrorResponse)=>{
      console.log(error);
      alert("Wrong entries");
    });  }

}
