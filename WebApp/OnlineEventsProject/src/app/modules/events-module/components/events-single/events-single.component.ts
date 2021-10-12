import { Component, Input, OnInit } from '@angular/core';
import { Event } from 'src/app/models/Event';
import { DataService } from 'src/app/services/data.service';

@Component({
  selector: 'app-events-single',
  templateUrl: './events-single.component.html',
})
export class EventsSingleComponent implements OnInit {

  @Input("event") event!:Event;

  constructor(private readonly dataService:DataService) {

   }

  ngOnInit(): void {
    this.event.interests=new Array<string>();
    for(let interestId of this.event.interestIds){
      this.event.interests.push(<string>(this.dataService.interests.find(x=>x.id==interestId)?.name));
    }
  }
}
