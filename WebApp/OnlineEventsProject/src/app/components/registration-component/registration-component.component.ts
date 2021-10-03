import { Component, OnInit } from '@angular/core';
import { SelectItem } from 'primeng/api/public_api';


@Component({
  selector: 'app-registration-component',
  templateUrl: './registration-component.component.html',
  styleUrls: ['./registration-component.component.css']
})
export class RegistrationComponent implements OnInit {
  selectOptions: SelectItem[];
  selectedOptions:any;
  constructor() {
    this.selectOptions=[{label:"Interest1",value:0},{label:"Interest2",value:1}];
   }

  ngOnInit(): void {
  }

}
