import { Component, OnInit } from '@angular/core';
import { AirlineService } from 'src/services/airline.service';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-my-flights',
  templateUrl: './my-flights.component.html',
  styleUrls: ['./my-flights.component.scss']
})
export class MyFlightsComponent implements OnInit {

  myFlights;
  myTrips: Array<any>;
  myTrip: {flights: Array<any>, totalPrice: any, reservationId: any};
  userId;
  itsOk = false;

  constructor(private airlineService: AirlineService,
              private router: Router,
              private routes: ActivatedRoute,
              private toastr: ToastrService,
              private san: DomSanitizer) {
      routes.params.subscribe(param => {
        this.userId = param.id;
      });

      this.myFlights = [];
      this.myTrips = [];
      this.myTrip = {
        flights: [],
        totalPrice: 0,
        reservationId: null
      };
  }

  ngOnInit(): void {
    this.loadAll();
  }

  loadAll() {
    this.myFlights = [];
    this.myTrips = [];
    this.myTrip = {
      flights: [],
      totalPrice: 0,
      reservationId: null
    };
    const c = this.airlineService.getPreviousFlights().subscribe(
      (res: any[]) => {
        if (res.length) {
          res.forEach(element => {
            const a = {
              airlineId: element.airlineId,
              // tslint:disable-next-line:max-line-length
              airlineLogo: (element.airlineLogo === null) ? null : this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${element.airlineLogo}`),
              airlineName: element.airlineName,
              class: element.clas,
              column: element.column,
              flightId: element.flightId,
              flightLength: element.flightLength,
              flightNumber: element.flightNumber,
              flightTime: element.flightTime,
              from: element.from,
              landingDate: element.landingDate,
              landingTime: element.landingTime,
              row: element.row,
              seatId: element.seatId,
              seatPrice: element.seatPrice,
              takeOffDate: element.takeOffDate,
              takeOffTime: element.takeOffTime,
              to: element.to,
              stops: element.stops,
              seat: {
                class: element.clas,
                row: element.row,
                column: element.column
              },
              reservationId: element.reservationId,
              isFlightRated: element.isFlightRated,
              isAirlineRated: element.isAirlineRated
            };

            this.myFlights.push(a);
          });
          this.itsOk = true;
        }
        console.log(res);
      },
      err => {
        this.toastr.error(err.error, 'ERROR');
      }
    );
    const b = this.airlineService.getUpcomingTrips().subscribe(
      (res: any[]) => {
        if (res.length) {
          console.log(res);
          res.forEach(el => {
            this.myTrip.flights = [];
            el.flights.forEach(element => {
              console.log(element);
              const new1 = {
                flightId: element.flightId,
                flightNumber: element.flightNumber,
                // tslint:disable-next-line:max-line-length
                airlineLogo: (element.airlineLogo === null) ? null : this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${element.airlineLogo}`),
                airlineName: element.airlineName,
                airlineId: element.airlineId,
                from: element.from,
                takeOffDate: element.takeOffDate,
                takeOffTime: element.takeOffTime,
                to: element.to,
                landingDate: element.landingDate,
                landingTime: element.landingTime,
                flightLength: element.flightLength,
                flightTime: element.flightTime,
                stops: element.stops,
                minPrice: element.minPrice,
                seat: {
                  class: element.clas,
                  column: element.column,
                  row: element.row
                }
              };
              this.myTrip.flights.push(new1);
            });
            this.myTrip.totalPrice = el.totalPrice;
            this.myTrip.reservationId = el.reservationId;
            this.myTrips.push(this.myTrip);
          });
        }
        this.itsOk = true;
        console.log(res);
      },
      err => {
        this.toastr.error(err.error, 'ERROR');
      }
    );
  }

  goBack() {
    this.router.navigate(['/' + this.userId + '/profile']);
  }

  quitReservation(value: any) {
    const data = {
      reservationId: value
    };
    this.airlineService.quitReservation(data).subscribe(
      (res: any) => {
        this.toastr.success('Success!');
        this.loadAll();
      },
      err => {
        this.toastr.error(err.error, 'Error!');
      }
    );
  }

  onRateAirline(value: any) {
    const data = {
      id: value.id,
      rate: value.rate,
    };
    this.airlineService.rateAirline(data).subscribe(
      (res: any) => {
        this.toastr.success('Success!');
        this.loadAll();
      },
      err => {
        // tslint:disable-next-line: triple-equals
        if (err.status == 400) {
          console.log(err);
          // this.toastr.error('Incorrect username or password.', 'Authentication failed.');
          this.toastr.error(err.statusText, 'Error!');
        } else {
          this.toastr.error(err.error.statusText, 'Error!');
        }
      }
    );
  }

  onRateFlight(value: any) {
    const data = {
      id: value.id,
      rate: value,
    };
    this.airlineService.rateFlight(data).subscribe(
      (res: any) => {
        this.toastr.success('Success!');
        this.loadAll();
      },
      err => {
        // tslint:disable-next-line: triple-equals
        if (err.status == 400) {
          console.log(err);
          // this.toastr.error('Incorrect username or password.', 'Authentication failed.');
          this.toastr.error(err.statusText, 'Error!');
        } else {
          this.toastr.error(err.error.statusText, 'Error!');
        }
      }
    );
  }

}
