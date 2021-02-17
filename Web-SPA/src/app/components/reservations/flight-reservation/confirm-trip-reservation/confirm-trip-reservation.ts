import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-confirm-trip-reservation',
  templateUrl: './confirm-trip-reservation.html',
  styleUrls: ['./confirm-trip-reservation.scss']
})
export class ConfirmTripReservationComponent implements OnInit {

  @Input() data: any;
  @Input() errorToDate: any;
  @Output() bonusEmitter = new EventEmitter<any>();
  @Output() carEmitter = new EventEmitter<any>();
  @Output() dateEmitter = new EventEmitter<any>();
  useBonus = false;
  withCar = false;
  toDate;

  constructor() { }

  ngOnInit(): void {
    console.log(this.data);
    console.log(this.data.flights[0].landingDate);
    this.toDate = this.data.flights[0].landingDate + 1;
    console.log(this.toDate);
  }

  onPaymentMethod(event: any) {
    if (event.target.value === 'useBonus') {
      this.bonusEmitter.emit(true);
      this.useBonus = true;
    } else {
      this.bonusEmitter.emit(false);
      this.useBonus = false;
    }
  }

  onPickCar(event: any) {
    if (event.target.value === 'withCar') {
      this.carEmitter.emit(true);
      this.withCar = true;
    } else {
      this.carEmitter.emit(false);
      this.withCar = false;
    }
  }

  onInputToDateCar(event: any) {
    this.dateEmitter.emit(event.target.value);
  }

}
