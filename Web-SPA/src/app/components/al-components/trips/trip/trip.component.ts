import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AirlineService } from 'src/services/airline.service';
import { Router, ActivatedRoute, NavigationExtras } from '@angular/router';
import { Flight } from 'src/app/entities/flight';
import { Trip } from 'src/app/entities/trip';
import { TripParameter } from 'src/app/entities/trip-parameter';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-trip',
  templateUrl: './trip.component.html',
  styleUrls: ['./trip.component.scss']
})

export class TripComponent implements OnInit {

  @Input() indexOfTrip;
  @Input() trip: any;
  @Input() userId;
  @Output() showModal = new EventEmitter<any>();
  showInfo: Array<boolean>;
  i: number;
  arrayOfValues: Array<TripParameter>;


  constructor(private airlineService: AirlineService, private router: Router, private route: ActivatedRoute) {
    this.showInfo = new Array<boolean>();
    this.arrayOfValues = new Array<TripParameter>();
  }

  ngOnInit(): void {
    console.log(this.trip);
    this.i = this.showInfo.length;
    this.showInfo.push(false);
    this.trip.flightsObject.forEach(flight => {
      this.arrayOfValues.push(new TripParameter(flight.airlineId, flight.flightId));
    });
  }
  getAirlineName(airlineId: number) {
    // const airline = this.airlineService.getAirline(airlineId);
    // return airline.name;
  }


  calculateFlightLength(departureTime: string, arrivalTime: string) {
    const departureTimeInMinutes = Number(departureTime.split(':')[0]) * 60 + Number(departureTime.split(':')[1]);
    const arrivalTimeInMinutes = Number(arrivalTime.split(':')[0]) * 60 + Number(arrivalTime.split(':')[1]);

    return Math.floor((arrivalTimeInMinutes - departureTimeInMinutes) / 60) + 'h'
         + Math.floor((arrivalTimeInMinutes - departureTimeInMinutes) % 60) + 'min';
  }

  viewDeal() {
    const queryParams: any = {};
    // Create our array of values we want to pass as a query parameter
    // const arrayOfValues = [new TripParameter(2, 'bla')];

    // Add the array of values to the query parameter as a JSON string
    queryParams.trip = JSON.stringify(this.arrayOfValues);

    // Create our 'NaviationExtras' object which is expected by the Angular Router
    const navigationExtras: NavigationExtras = {
      queryParams
    };
    if (this.userId !== undefined) {
      this.router.navigate([this.userId + '/trips/trip-reservation'], navigationExtras);
    } else {
      this.showModal.emit(true);
    }
  }

}
