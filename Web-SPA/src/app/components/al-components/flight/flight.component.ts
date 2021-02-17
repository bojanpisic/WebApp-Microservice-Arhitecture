import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Flight } from 'src/app/entities/flight';

@Component({
  selector: 'app-flight',
  templateUrl: './flight.component.html',
  styleUrls: ['./flight.component.scss']
})
export class FlightComponent implements OnInit {

  @Input() flight: any;
  @Input() markFlightName = false;
  @Input() seat = false;
  @Input() seatName: any;
  @Input() rate: any;
  @Input() airlineRated: any;
  @Input() flightRated: any;
  @Output() emitRateAirline = new EventEmitter<any>();
  @Output() emitRateFlight = new EventEmitter<any>();

  rateExperience = false;

  minusRateAirlineDisabled = true;
  plusRateAirlineDisabled = false;
  rateAirline = 1;
  minusRateFlightDisabled = true;
  plusRateFlightDisabled = false;
  rateFlight = 1;

  showRateAirline = false;
  showRateFlight = false;

  constructor() { }

  ngOnInit(): void {
    console.log(this.flight);
  }

  onConfirmAirlineRate() {
    if (this.rateAirline > 0 && this.rateAirline < 6) {
      this.emitRateAirline.emit({rate: this.rateAirline, id: this.flight.airlineId});
    }
  }

  onConfirmFlightRate() {
    if (this.rateFlight > 0 && this.rateFlight < 6) {
      this.emitRateFlight.emit({rate: this.rateFlight, id: this.flight.flightId});
    }
  }

  onRate() {
    this.rateExperience = !this.rateExperience;
  }

  onRateAirline() {
    this.showRateAirline = !this.showRateAirline;
  }

  onRateFlight() {
    this.showRateFlight = !this.showRateFlight;
  }

  onPlusRateAirline() {
    if (this.rateAirline > 4) {
      this.plusRateAirlineDisabled = true;
    } else {
      this.rateAirline++;
      this.minusRateAirlineDisabled = false;
    }
  }

  onMinusRateAirline() {
    if (this.rateAirline > 1) {
      this.rateAirline--;
      this.plusRateAirlineDisabled = false;
    }
    if (this.rateAirline === 1) {
      this.minusRateAirlineDisabled = true;
    }
  }

  onPlusRateFlight() {
    if (this.rateFlight > 4) {
      this.plusRateFlightDisabled = true;
    } else {
      this.rateFlight++;
      this.minusRateFlightDisabled = false;
    }
  }

  onMinusRateFlight() {
    if (this.rateFlight > 1) {
      this.rateFlight--;
      this.plusRateFlightDisabled = false;
    }
    if (this.rateFlight === 1) {
      this.minusRateFlightDisabled = true;
    }
  }

}
