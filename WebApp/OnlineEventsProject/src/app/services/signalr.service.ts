import { HttpClient } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import * as signalR from "@microsoft/signalr"
import { ToastrService } from 'ngx-toastr';
import { EventCreated } from '../models/EventCreated';
import { Interest } from '../models/Interest';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class SignalrService implements OnInit {
  private connection:signalR.HubConnection | undefined;
  private interests:Array<Interest>;

  constructor(private readonly router:Router,
              private readonly authService:AuthService,
              private toastr:ToastrService,
              private readonly httpClient:HttpClient) {

      this.interests=[];
  }

  ngOnInit(): void {
  }

  connect(token:string | null)
  {
    this.httpClient.get<Interest[]>("http://localhost:52803/Interest").subscribe((data)=>{
      this.interests=data;
    })
    if(!this.connection || this.connection.state!='Connected'){
      this.connection=new signalR.HubConnectionBuilder()
                                  .withUrl("http://localhost:52800/notifications",{accessTokenFactory:()=>token!=null?<string>token:" "})
                                  .build();
      this.connection.start().then(()=>
      {
        console.log("Connected to the notifications signalR hub!");
        this.connection?.on("EventCreatedNotification",(eventArg:EventCreated)=>{
            let message:string=eventArg.name +"\n" + eventArg.dateTimeOfEvent+"\n";
            eventArg.interestIds.forEach((idInterest)=>{
              message+=this.interests.find(x=>x.id==idInterest)?.name+"\n";
            })
            this.toastr.info(message,"Event of interest created");
        });
        this.connection?.on("EventStartedNotification",(arg)=>{
          this.toastr.info(arg,"Event is starting");
        });
      })
                              .catch((err)=>{
                                console.log("Error while starting connection to the notifications signalR hub: "+err);
                                this.authService.LogOut();
                              });
    }
  }

  disconnect()
  {
    if(this.connection && this.connection.state=='Connected'){
      this.connection?.stop().then(()=>console.log("Disconnected from the notifications signalR hub!"))
        .catch((err)=> console.log("Error while disconnecting from the notifications signalR hub: "+err));
    }
  }
}
