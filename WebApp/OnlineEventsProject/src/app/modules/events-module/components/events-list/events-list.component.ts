import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { SignalrService } from 'src/app/services/signalr.service';
import { Event } from 'src/app/models/Event';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-events-list',
  templateUrl: './events-list.component.html',
  styleUrls: ['./events-list.component.css']
})
export class EventsListComponent implements OnInit,OnDestroy {

  events:Event[];
  myEvents:boolean | undefined;
  dataSubscription:Subscription;
  constructor(private signalRService:SignalrService,
              private readonly httpClient:HttpClient,
              private readonly route:ActivatedRoute) {
    this.events=new Array<Event>();
    this.dataSubscription= route.data.subscribe((data)=>{this.myEvents=data["myEvents"]});
  }
  ngOnDestroy(): void {
    //this.signalRService.disconnect();
    this.dataSubscription.unsubscribe();
  }

  ngOnInit(): void {
    this.signalRService.connect(localStorage.getItem("eventsToken"));
    if(!this.myEvents){
      this.httpClient.get<Event[]>("http://localhost:52801/api/Events").subscribe((response)=>{
        this.events=response;
      });
    }
    else{
      this.httpClient.get<Event[]>("http://localhost:52801/api/Events/GetMyEvents").subscribe((response)=>{
        this.events=response;
      });
    }
  }
}
