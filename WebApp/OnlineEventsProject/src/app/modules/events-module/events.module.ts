import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EventsRoutingModule } from './events-routing.module';
import { EventsListComponent } from './components/events-list/events-list.component';
import { EventsSingleComponent } from './components/events-single/events-single.component';
import { CreateEventComponent } from './components/create-event/create-event.component';
import { SharedModule } from '../shared/shared.module';



@NgModule({
  declarations: [
    EventsListComponent,
    EventsSingleComponent,
    CreateEventComponent,
  ],
  imports: [
    CommonModule,
    EventsRoutingModule,
    SharedModule
  ],
  exports:[
  ]
})
export class EventsModule { }
