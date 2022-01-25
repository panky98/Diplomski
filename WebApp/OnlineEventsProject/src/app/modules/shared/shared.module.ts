import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { MultiSelectModule } from 'primeng/multiselect';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { RadioButtonModule } from 'primeng/radiobutton';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { NgxSpinnerModule } from "ngx-spinner";
import {CalendarModule} from 'primeng/calendar';
import { AuthInterceptor } from 'src/app/interceptors/auth.interceptor';



@NgModule({
  declarations: [

  ],
  imports: [
    CommonModule,
    FormsModule,
    HttpClientModule,
    MultiSelectModule,
    ButtonModule,
    TooltipModule,
    RadioButtonModule,
    AutoCompleteModule,
    NgxSpinnerModule,
    CalendarModule
  ],
  exports:[
    FormsModule,
    HttpClientModule,
    MultiSelectModule,
    ButtonModule,
    TooltipModule,
    RadioButtonModule,
    AutoCompleteModule,
    NgxSpinnerModule,
    CalendarModule
  ],
  providers: [
    {provide: HTTP_INTERCEPTORS,useClass: AuthInterceptor,multi:true}
  ],
})
export class SharedModule { }
