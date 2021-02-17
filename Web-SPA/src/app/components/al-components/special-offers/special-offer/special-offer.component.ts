import { Component, OnInit, Input } from '@angular/core';
import { AirlineService } from 'src/services/airline.service';
import { Airline } from 'src/app/entities/airline';

@Component({
  selector: 'app-special-offer',
  templateUrl: './special-offer.component.html',
  styleUrls: ['./special-offer.component.scss']
})
export class SpecialOfferComponent implements OnInit {

  @Input() offer;
  airline: Airline;
  showInfo: Array<boolean>;
  i: number;


  constructor(private airlineService: AirlineService) {
    this.showInfo = new Array<boolean>();
  }

  ngOnInit(): void {
    this.i = this.showInfo.length;
    this.showInfo.push(false);
  }
  getAirlineName(flightId: number) {
    // this.airline = this.airlineService.getAirline(flightId);
    return this.airline.name;
  }

  showStopsInfo(i: number) {
    this.showInfo[i] = !this.showInfo[i];
  }

  calculateFlightLength(departureTime: string, arrivalTime: string) {
    const departureTimeInMinutes = Number(departureTime.split(':')[0]) * 60 + Number(departureTime.split(':')[1]);
    const arrivalTimeInMinutes = Number(arrivalTime.split(':')[0]) * 60 + Number(arrivalTime.split(':')[1]);

    return Math.floor((arrivalTimeInMinutes - departureTimeInMinutes) / 60) + 'h'
         + Math.floor((arrivalTimeInMinutes - departureTimeInMinutes) % 60) + 'min';
  }

  calculatePrice() { // kasnije cemo to vaditi iz Trip-a, pogledaj klasu special-offer i trip
    let returnValue = 0;
    this.offer.flights.forEach(flight => {
      returnValue += flight.ticketPrice;
    });
    return returnValue;
  }

}
