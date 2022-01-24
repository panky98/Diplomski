import { Component, OnDestroy, OnInit } from '@angular/core';
import {Router} from '@angular/router'
import { Subscription } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';
import { SignalrService } from 'src/app/services/signalr.service';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit,OnDestroy {

  token:string|null;
  authSubscription:Subscription;

  constructor(private authService:AuthService,
              private signalRService:SignalrService) {
    this.token=localStorage.getItem("eventsToken");
    this.authSubscription=this.authService.loggedIn.subscribe((val:boolean)=>{
        if(val)
        {
          this.token=localStorage.getItem("eventsToken");
        }
        else{
          localStorage.clear();
          this.token=null;
        }
    })
  }
  ngOnDestroy(): void {
    this.authSubscription.unsubscribe();
  }

  ngOnInit(): void {

  }

  logOut(){
    this.signalRService.disconnect();
    this.authService.LogOut();    
  }
}
