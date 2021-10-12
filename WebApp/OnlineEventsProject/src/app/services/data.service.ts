import { HttpClient } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { Interest } from '../models/Interest';

@Injectable({
  providedIn: 'root'
})
export class DataService{
  public interests:Interest[];

  constructor(private readonly httpClient:HttpClient) {
    this.interests=new Array<Interest>();
    this.initialize();
  }

  initialize(): void {
    this.httpClient.get<Interest[]>("http://localhost:52800/api/Interest").subscribe((data)=>{
      this.interests=data;
    })
  }

  getInterests():Interest[]{
    return this.interests;
  }
}
