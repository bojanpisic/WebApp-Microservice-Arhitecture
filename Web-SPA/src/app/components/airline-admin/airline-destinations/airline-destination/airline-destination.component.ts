import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Destination } from 'src/app/entities/destination';

@Component({
  selector: 'app-airline-destination',
  templateUrl: './airline-destination.component.html',
  styleUrls: ['./airline-destination.component.scss']
})
export class AirlineDestinationComponent implements OnInit {

  @Input() destination: {imageUrl: any, city: string, state: string};
  @Input() editable;
  @Output() delete = new EventEmitter<boolean>();
  constructor() { }

  ngOnInit(): void {
  }

  toDataURL(url, callback) {
    const xhr = new XMLHttpRequest();
    // tslint:disable-next-line:only-arrow-functions
    xhr.onload = function() {
        const reader = new FileReader();
        // tslint:disable-next-line:only-arrow-functions
        reader.onloadend = function() {
            callback(reader.result);
        };
        reader.readAsDataURL(xhr.response);
    };
    xhr.open('GET', url);
    xhr.responseType = 'blob';
    xhr.send();
  }

  removeDestination() {
    this.delete.emit(true);
  }

}
