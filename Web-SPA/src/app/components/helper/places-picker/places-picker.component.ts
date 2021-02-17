import { Component, OnInit } from '@angular/core';
import { Destination } from 'src/app/entities/destination';
import { Address } from 'src/app/entities/address';

@Component({
  selector: 'app-places-picker',
  templateUrl: './places-picker.component.html',
  styleUrls: ['./places-picker.component.scss']
})
export class PlacesPickerComponent implements OnInit {

  constructor() { }

  choosenChangeOvers: Array<Destination>;
  jsonObj: any;

  inputStyle = 'margin-top: 10px; \
                margin-right: 3px; \
                width: 20%; \
                height: 30%; \
                display: inline; \
                background-color: rgb(62, 62, 255); \
                color: white; \
                border-radius: 5px; \
                outline: none; \
                border: none; \
                font-size: 12px; \
                text-align: center; \
                padding: 1px;';

  ngOnInit(): void {
    this.choosenChangeOvers = new Array<Destination>();
  }

  getCityName($event) { 
    this.jsonObj = JSON.parse($event);
    this.choosenChangeOvers.push(
      new Destination(this.jsonObj.photoUrl,
         new Address(this.jsonObj.city, this.jsonObj.state, this.jsonObj.longitude, this.jsonObj.latitude)));
    this.createDivChild();
  }

  createDivChild() {
    let child = document.createElement('input');
    const elementName = this.jsonObj.city;
    child.setAttribute('value', elementName);
    child.setAttribute('id', this.choosenChangeOvers.length.toString());
    child.setAttribute('type', 'button');
    child.setAttribute('style', this.inputStyle);
    child.addEventListener('click', () => {
        document.getElementById(child.getAttribute('id')).remove();
        this.choosenChangeOvers.splice(Number(child.getAttribute('id')), 1);
    });
    document.getElementById('changeover-list').appendChild(child);
  }

}
