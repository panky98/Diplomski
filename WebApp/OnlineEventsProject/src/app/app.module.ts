import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MenuComponent } from './components/menu-component/menu.component';
import { LoginComponent } from './components/login-component/login.component';
import { HomeComponent } from './components/home-component/home-component.component';
import { RegistrationComponent } from './components/registration-component/registration-component.component';
import { InputSwitchModule } from 'primeng/inputswitch';
import {TabViewModule} from 'primeng/tabview';
import {CommonModule} from '@angular/common';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations'
import {AccordionModule} from 'primeng/accordion'
import { ToastrModule } from 'ngx-toastr';
import { PlayEventComponent } from './components/streaming/play-event/play-event.component';
import { SharedModule } from './modules/shared/shared.module';

@NgModule({
  declarations: [
    AppComponent,
    MenuComponent,
    LoginComponent,
    HomeComponent,
    RegistrationComponent,
    PlayEventComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    SharedModule,
    InputSwitchModule,
    TabViewModule,
    CommonModule,
    BrowserAnimationsModule,
    AccordionModule,
    ToastrModule.forRoot()
  ],
  providers: [
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
