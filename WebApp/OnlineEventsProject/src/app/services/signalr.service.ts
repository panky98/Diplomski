import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr"

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private connection:signalR.HubConnection | undefined;

  constructor() {
  }

  connect(token:string | null)
  {
    this.connection=new signalR.HubConnectionBuilder()
                                .withUrl("http://localhost:52800/notifications",{accessTokenFactory:()=>token!=null?<string>token:" "})
                                .build();
    this.connection.start().then(()=>
    {
      console.log("Connected to the notifications signalR hub!");
      this.connection?.on("EventCreatedNotification",(eventArg)=>{
          alert("Event of interest created: "+eventArg);
      });
    })
                           .catch((err)=> console.log("Error while starting connection to the notifications signalR hub: "+err));
  }

  disconnect()
  {
    this.connection?.stop().then(()=>console.log("Disconnected from the notifications signalR hub!"))
                            .catch((err)=> console.log("Error while disconnecting from the notifications signalR hub: "+err));
  }
}
