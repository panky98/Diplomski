import { Component, OnDestroy } from '@angular/core';
import { SignalrService } from './services/signalr.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnDestroy {
  title = 'OnlineEventsProject';

  constructor(private signalRService:SignalrService){

  }
  ngOnDestroy(): void {
    this.signalRService.disconnect();
  }
}
