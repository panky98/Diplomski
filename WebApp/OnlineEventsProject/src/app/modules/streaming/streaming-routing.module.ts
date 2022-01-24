import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PlayEventComponent } from './play-event/play-event.component';


const routes:Routes=[
  {
    path: ":code", component: PlayEventComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StreamingRoutingModule { }
