import { Component, OnDestroy, OnInit } from '@angular/core';
import { SignalrService } from 'src/app/services/signalr.service';

@Component({
  selector: 'app-events-list',
  templateUrl: './events-list.component.html',
  styleUrls: ['./events-list.component.css']
})
export class EventsListComponent implements OnInit,OnDestroy {

  constructor(private signalRService:SignalrService) {

  }
  ngOnDestroy(): void {
    console.log("ngOnDestroy called!");
    this.signalRService.disconnect();
  }

  ngOnInit(): void {
    this.signalRService.connect(localStorage.getItem("eventsToken"));
  }
}
