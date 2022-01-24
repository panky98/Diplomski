import { Component, OnInit } from '@angular/core';
import { SignalrService } from 'src/app/services/signalr.service';

@Component({
  selector: 'app-home-component',
  templateUrl: './home-component.component.html',
  styleUrls: ['./home-component.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private signalRService:SignalrService) { }

  ngOnInit(): void {
    if(localStorage.getItem("eventsToken")){
      this.signalRService.connect(localStorage.getItem("eventsToken"));
    }
  }

}
