import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Event } from 'src/app/models/Event';
import { DataService } from 'src/app/services/data.service';

@Component({
  selector: 'app-events-single',
  templateUrl: './events-single.component.html',
})
export class EventsSingleComponent implements OnInit {

  @Input("event") event!:Event;
  @Input("myEvent") myEvent:boolean|undefined;

  constructor(private readonly dataService:DataService,
              private readonly httpClient:HttpClient,
              private readonly router:Router) {

   }

  ngOnInit(): void {
    this.event.interests=new Array<string>();
    for(let interestId of this.event.interestIds){
      this.event.interests.push(<string>(this.dataService.interests.find(x=>x.id==interestId)?.name));
    }
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
