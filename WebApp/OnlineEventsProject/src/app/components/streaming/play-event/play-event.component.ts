import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { VgPlayerComponent } from '@videogular/ngx-videogular/core';
import { SignalrService } from 'src/app/services/signalr.service';

@Component({
  selector: 'app-play-event',
  templateUrl: './play-event.component.html',
  styleUrls: ['./play-event.component.scss']
})
export class PlayEventComponent implements OnInit {
  
  eventCode:string;

  constructor(private _Activatedroute:ActivatedRoute,
              private signalRService: SignalrService) { 
    this.eventCode=<string>this._Activatedroute.snapshot.paramMap.get("code");
  }

  ngOnInit(): void {
    this.signalRService.connect(localStorage.getItem("eventsToken"));
  }
}
