import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FlightPlusCarService {

  flightReservation;

  constructor() { }

  addFlightReservation(data: any) {
    this.flightReservation = data;
  }

  getFlightReservation() {
    return this.flightReservation;
  }
}
