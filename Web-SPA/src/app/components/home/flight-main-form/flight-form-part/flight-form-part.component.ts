import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Address } from 'src/app/entities/address';

@Component({
  selector: 'app-flight-form-part',
  templateUrl: './flight-form-part.component.html',
  styleUrls: ['./flight-form-part.component.scss', '../flight-form-part/flight-form-part.component.scss']
})
export class FlightFormPartComponent implements OnInit {
  @Input() flightType: any;

  public fromLocation: Address;
  public toLocation: Address;

  public lastGoodFromLocation: string;
  public lastFromLocation: string;
  public lastGoodToLocation: string;
  public lastToLocation: string;

  errorFromLocation = false;
  errorToLocation = false;
  errorPickUpDate = false;
  errorDropOffDate = false;

  okFromLocation = false;
  okToLocation = false;
  okDepartDate = false;

  inputDepart;
  inputReturn;

  constructor() { }

  ngOnInit(): void {
  }

  onFromLocation(value: any) {
    const obj = JSON.parse(value);
    this.fromLocation = new Address(obj.city, obj.state, obj.longitude, obj.latitude);
    this.lastGoodFromLocation = this.lastFromLocation;
    this.okFromLocation = true;
  }

  onFromLocationInputChange(value: any) {
    this.lastFromLocation = value;
    if (this.lastFromLocation !== this.lastGoodFromLocation) {
      this.okFromLocation = false;
    } else {
      this.okFromLocation = true;
    }
  }

  removeErrorClassFrom() {
    this.errorFromLocation = false;
  }

  onToLocation(value: any) {
    const obj = JSON.parse(value);
    this.toLocation = new Address(obj.city, obj.state, obj.longitude, obj.latitude);
    this.lastGoodToLocation = this.lastToLocation;
    this.okToLocation = true;
  }

  onToLocationInputChange(value: any) {
    this.lastToLocation = value;
    if (this.lastToLocation !== this.lastGoodToLocation) {
      this.okToLocation = false;
    } else {
      this.okToLocation = true;
    }
  }

  removeErrorClassTo() {
    this.errorToLocation = false;
  }
}
