import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MenuComponent } from './components/menu-component/menu.component';
import { LoginComponent } from './components/login-component/login.component';
import { HomeComponent } from './components/home-component/home-component.component';
import { RegistrationComponent } from './components/registration-component/registration-component.component';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import {MultiSelectModule} from 'primeng/multiselect';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { RadioButtonModule } from 'primeng/radiobutton';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { InputSwitchModule } from 'primeng/inputswitch';
import {TabViewModule} from 'primeng/tabview';
import {CommonModule} from '@angular/common';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations'
import {AccordionModule} from 'primeng/accordion'
import { ToastrModule } from 'ngx-toastr';
import { VimeModule } from '@vime/angular';

@NgModule({
  declarations: [
    AppComponent,
    MenuComponent,
    LoginComponent,
    HomeComponent,
    RegistrationComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    MultiSelectModule,
    ButtonModule,
    TooltipModule,
    RadioButtonModule,
    AutoCompleteModule,
    InputSwitchModule,
    TabViewModule,
    CommonModule,
    BrowserAnimationsModule,
    AccordionModule,
    VimeModule,
    ToastrModule.forRoot()
  ],
  exports:[

  ],
  providers: [
    {provide: HTTP_INTERCEPTORS,useClass: AuthInterceptor,multi:true}
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
