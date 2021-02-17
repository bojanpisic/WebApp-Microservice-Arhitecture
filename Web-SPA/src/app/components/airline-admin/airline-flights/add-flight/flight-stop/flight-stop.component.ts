import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Address } from 'src/app/entities/address';

@Component({
  selector: 'app-flight-stop',
  templateUrl: './flight-stop.component.html',
  styleUrls: ['./flight-stop.component.scss']
})
export class FlightStopComponent implements OnInit {

  dropdown = false;
  location: Address;
  pickedDestination: any;

  @Input() destinations: Array<{destinationId: number, city: string, state: string}>;
  @Output() stop = new EventEmitter<{destinationId: number, city: string, state: string}>();

  constructor() {}

  ngOnInit(): void {}

  setDestination(value: any) {
    this.pickedDestination = value;
    this.stop.emit(this.pickedDestination);
  }

  toggleDropDown() {
    this.dropdown = !this.dropdown;
  }

}
