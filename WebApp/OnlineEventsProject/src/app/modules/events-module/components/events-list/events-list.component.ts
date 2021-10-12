import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { SignalrService } from 'src/app/services/signalr.service';
import { Event } from 'src/app/models/Event';

@Component({
  selector: 'app-events-list',
  templateUrl: './events-list.component.html',
  styleUrls: ['./events-list.component.css']
})
export class EventsListComponent implements OnInit,OnDestroy {

  events:Event[];
  constructor(private signalRService:SignalrService,
              private readonly httpClient:HttpClient) {
    this.events=new Array<Event>();
  }
  ngOnDestroy(): void {
    this.signalRService.disconnect();
  }

  ngOnInit(): void {
    this.signalRService.connect(localStorage.getItem("eventsToken"));
    this.httpClient.get<Event[]>("http://localhost:52801/api/Events").subscribe((response)=>{
      this.events=response;
    });
  }
}
