import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SignalrService } from 'src/app/services/signalr.service';
import {Event} from "src/app/models/Event"

@Component({
  selector: 'app-play-event',
  templateUrl: './play-event.component.html',
  styleUrls: ['./play-event.component.scss']
})
export class PlayEventComponent implements OnInit, OnDestroy {
  
  eventCode:string;
  event: Event | undefined;
  waitingForStreaming:boolean=true;
  eventAvailable:boolean=false;
  intervalId:any;

  @ViewChild("videoPlayer", { static: false }) videoplayer: ElementRef | undefined;
  videoUrl:string;

  constructor(private _Activatedroute:ActivatedRoute,
              private signalRService: SignalrService,
              private readonly httpClient:HttpClient) { 

    this.eventCode=<string>this._Activatedroute.snapshot.paramMap.get("code");
    this.videoUrl='http://localhost:52802/api/Stream/'+this.eventCode;
    this.intervalId=0;
  }
  ngOnDestroy(): void {
    clearInterval(this.intervalId);
  }

  ngOnInit(): void {
    this.signalRService.connect(localStorage.getItem("eventsToken"));

    //collect info about event as well
    this.httpClient.get<Event>("http://localhost:52801/api/Events/"+this.eventCode).subscribe((response)=>{
      this.event=response;
    },
    (error)=>{
      console.log(error)
    });

    this.intervalId=setInterval(this.checkIfEventIsAvailable.bind(this),1000);
  }

  toggleVideo(event: any) {
    (<ElementRef>this.videoplayer).nativeElement.play();
  }

  checkIfEventIsAvailable(){
    this.httpClient.get('http://localhost:52802/api/Stream/'+this.eventCode+'/Check').subscribe((response)=>{
      this.eventAvailable=true;
      this.waitingForStreaming=false;
      clearInterval(this.intervalId);
    },
    (error:HttpErrorResponse)=>{
      if(error.status===404){//not available yet
        
      }
      else if(error.status===204){//expired
        this.waitingForStreaming=false;
        this.eventAvailable=false;
      }
    });
  }
}
