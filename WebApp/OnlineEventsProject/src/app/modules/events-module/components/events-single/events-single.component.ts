import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Event } from 'src/app/models/Event';
import { Interest } from 'src/app/models/Interest';

@Component({
  selector: 'app-events-single',
  templateUrl: './events-single.component.html',
})
export class EventsSingleComponent implements OnInit {

  currentDate:Date=new Date();
  private interests:Array<Interest>;

  @Input("event") event!:Event;
  @Input("myEvent") myEvent:boolean|undefined;

  constructor(private readonly httpClient:HttpClient,
              private readonly router:Router) {
      this.interests=[];
   }

  ngOnInit(): void {
    this.httpClient.get<Interest[]>("http://localhost:52800/api/Interest").subscribe((data)=>{
      this.interests=data;
      this.event.interests=new Array<string>();
      for(let interestId of this.event.interestIds){
        this.event.interests.push(<string>(this.interests.find(x=>x.id==interestId)?.name));
      }
    })

    this.event.dateTimeOfEvent=new Date(this.event.dateTimeOfEvent);
    this.event.dateTimeOfEvent=new Date(this.event.dateTimeOfEvent.getFullYear(),this.event.dateTimeOfEvent.getMonth(),this.event.dateTimeOfEvent.getDate(),
                                        this.event.dateTimeOfEvent.getHours()-1,this.event.dateTimeOfEvent.getMinutes(),this.event.dateTimeOfEvent.getSeconds());

  }

  onSubscribe():void{
    this.httpClient.post("http://localhost:52801/api/Events/SubscribeToEvent?code="+this.event.code,null).subscribe((response)=>{
      this.router.navigate(["my-events"]);
    },(error)=>{console.log(error)});
  }

  onUnsubscribe():void{
      this.httpClient.post("http://localhost:52801/api/Events/UnsubscribeToEvent?code="+this.event.code,null).subscribe((response)=>{
        this.router.navigate(["events"]);
      },(error)=>{console.log(error)});
  }
}
