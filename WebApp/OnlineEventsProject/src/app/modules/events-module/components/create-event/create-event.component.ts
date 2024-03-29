import { HttpClient, HttpErrorResponse, HttpEventType } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { SelectItem } from 'primeng/api/public_api';
import { Event } from 'src/app/models/Event';
import { EventCreated } from 'src/app/models/EventCreated';
import { Interest } from 'src/app/models/Interest';
import { SignalrService } from 'src/app/services/signalr.service';

@Component({
  selector: 'app-create-event',
  templateUrl: './create-event.component.html',
  styleUrls: ['./create-event.component.css']
})
export class CreateEventComponent implements OnInit {

  @ViewChild('forma') forma:NgForm | undefined;

  selectOptions: SelectItem[];
  selectedOptions:Array<number>;
  isBusy:boolean=false;
  file:File | undefined;

  dateTime:Date | undefined;

  constructor(private readonly httpClient:HttpClient,
              private signalRService:SignalrService,
              private toastr:ToastrService,
              private router: Router) { 
    this.selectOptions=[];
    this.selectedOptions=[];
  }

  ngOnInit(): void {
    this.signalRService.connect(localStorage.getItem("eventsToken"));
    this.httpClient.get<Interest[]>("http://localhost:52803/Interest").subscribe((data)=>{
      data.forEach((interest)=>{
        this.selectOptions.push({label:interest.name,value:interest.id});
      });
    })
  }

  onSubmit(){
    let newEvent:{name:string,interestIds:Array<number>,dateTimeOfEvent:Date}={
      name:this.forma?.value["name"],
      interestIds:this.selectedOptions,
      dateTimeOfEvent: this.forma?.value['dateTime']
    }

    const file= <File>this.file;

    if(file.type!='video/mp4'){
      this.toastr.error("Only mp4 files are allowed!");
      return;
    }
    else if(file.size>15*1024*1024){
      this.toastr.error("Maximum allowed file size is 15MB!");
      return;
    }

    this.isBusy=true;
    this.httpClient.post<EventCreated>("http://localhost:52803/Events",newEvent).subscribe(
      (response:EventCreated)=>{
        //upload file
        const formData=new FormData();
        formData.append('file',<File>this.file,(<File>this.file).name);

        this.httpClient.post(`http://localhost:52803/Events/${response.code}/UploadFile`, formData, {reportProgress: true, observe: 'events'})
        .subscribe(event => {
          if (event.type === HttpEventType.UploadProgress)
            this.toastr.info("Uploaded: "+Math.round(100 * event.loaded / <number>event.total));
          else if (event.type === HttpEventType.Response) {
            this.isBusy=false;
            this.toastr.info("Upload successfull");
            this.router.navigate(['my-events']);
          }          
        },(error:HttpErrorResponse)=>{
          this.isBusy=false;
          this.toastr.error(error.message);  
        });
    },
    (error: HttpErrorResponse)=>{
      this.isBusy=false;
      this.toastr.error(error.message);      
    });
  }

  // On file Select
  onChange(event:any) {
    this.file = event.target.files[0];
  }
}
