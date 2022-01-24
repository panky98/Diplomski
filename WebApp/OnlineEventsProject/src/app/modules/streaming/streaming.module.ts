import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlayEventComponent } from './play-event/play-event.component';
import {VgCoreModule} from '@videogular/ngx-videogular/core';
import {VgControlsModule} from '@videogular/ngx-videogular/controls';
import {VgOverlayPlayModule} from '@videogular/ngx-videogular/overlay-play';
import {VgBufferingModule} from '@videogular/ngx-videogular/buffering';
import { StreamingRoutingModule } from './streaming-routing.module';

@NgModule({
  declarations: [
    PlayEventComponent,
  ],
  imports: [
    CommonModule,
    VgCoreModule,
    VgControlsModule,
    VgOverlayPlayModule,
    VgBufferingModule,
    StreamingRoutingModule
  ],
  exports:[
    VgCoreModule,
    VgControlsModule,
    VgOverlayPlayModule,
    VgBufferingModule
  ]
})
export class StreamingModule { }
