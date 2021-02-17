import { Injectable } from '@angular/core';
import { FlightService } from './flight.service';
import { Trip } from 'src/app/entities/trip';
import { TripId } from 'src/app/entities/trip-id';

@Injectable({
  providedIn: 'root'
})
export class TripService {

  trips: Array<Trip>;

  constructor(private flightService: FlightService) {
    this.trips = new Array<Trip>();
    this.mockedTrips();
  }

  getAllTrips() {
    return this.trips;
  }

  mockedTrips() {

    // const flights = this.flightService.getAllFlights();

    // const t1 = new Trip([flights[0]], 'oneWay', flights[0].ticketPrice);
    // const t2 = new Trip([flights[2], flights[3]], 'roundTrip', flights[2].ticketPrice + flights[3].ticketPrice);
    // const t3 = new Trip([flights[4], flights[0], flights[1]],
    //                     'multiCity', flights[4].ticketPrice + flights[0].ticketPrice + flights[1].ticketPrice);
    // const t4 = new Trip([flights[4]], 'oneWay', flights[4].ticketPrice);
    // const t5 = new Trip([flights[5]], 'oneWay', flights[5].ticketPrice);
    // const t6 = new Trip([flights[1]], 'oneWay', flights[1].ticketPrice);

    // this.trips.push(t1);
    // this.trips.push(t2);
    // this.trips.push(t3);
    // this.trips.push(t4);
    // this.trips.push(t5);
    // this.trips.push(t6);
  }

}
