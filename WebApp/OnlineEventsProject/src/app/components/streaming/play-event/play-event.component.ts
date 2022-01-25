import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SignalrService } from 'src/app/services/signalr.service';

@Component({
  selector: 'app-play-event',
  templateUrl: './play-event.component.html',
  styleUrls: ['./play-event.component.scss']
})
export class PlayEventComponent implements OnInit {
  
  eventCode:string;
  @ViewChild("videoPlayer", { static: false }) videoplayer: ElementRef | undefined;
  videoUrl:string;

  constructor(private _Activatedroute:ActivatedRoute,
              private signalRService: SignalrService) { 
    this.eventCode=<string>this._Activatedroute.snapshot.paramMap.get("code");
    this.videoUrl='http://localhost:52802/api/Stream/'+this.eventCode;
  }

  ngOnInit(): void {
    this.signalRService.connect(localStorage.getItem("eventsToken"));
  }

  toggleVideo(event: any) {
    (<ElementRef>this.videoplayer).nativeElement.play();
  }
}
