import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CreateEventComponent } from './components/create-event/create-event.component';
import { EventsListComponent } from './components/events-list/events-list.component';


const routes:Routes=[
  {path: "", component: EventsListComponent},
  {path: "create-event", component: CreateEventComponent, pathMatch:"full"}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class EventsRoutingModule { }
