import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Flight } from 'src/app/entities/flight';
import { AirlineService } from 'src/services/airline.service';
import { DomSanitizer } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-airline-flights',
  templateUrl: './airline-flights.component.html',
  styleUrls: ['./airline-flights.component.scss']
})
export class AirlineFlightsComponent implements OnInit {

  searchText = '';
  adminId: number;
  airlineId: number;
  flights: Array<{
    flightId: number,
    flightNumber: string,
    airlineLogo: any,
    airlineName: string,
    from: string,
    takeOffDate: Date,
    takeOffTime: string,
    to: string,
    landingDate: Date,
    landingTime: string,
    flightLength: string,
    flightTime: string,
    stops: Array<any>
  }>;
  flightId;
  noFlights = false;

  constructor(private router: Router, private routes: ActivatedRoute, private airlineService: AirlineService,
              private san: DomSanitizer,
              private toastr: ToastrService) {
    routes.params.subscribe(param => {
      this.adminId = param.id;
    });
    this.flights = [];
  }

  ngOnInit(): void {
    const air1 = this.airlineService.getAdminsFlights().subscribe(
      (res: any[]) => {
        console.log(res);
        if (res.length === 0) {
          this.noFlights = true;
        }
        if (res.length) {
          res.forEach(element => {
            const new1 = {
              flightId: element.flightId,
              flightNumber: element.flightNumber,
              // tslint:disable-next-line:max-line-length
              airlineLogo: (element.airlineLogo === null) ? null : this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${element.airlineLogo}`),
              airlineName: element.airlineName,
              from: element.from,
              takeOffDate: element.takeOffDate,
              takeOffTime: element.takeOffTime,
              to: element.to,
              landingDate: element.landingDate,
              landingTime: element.landingTime,
              flightLength: element.flightLength,
              flightTime: element.flightTime,
              stops: element.stops
            };
            this.flights.push(new1);
          });
        }
      },
      err => {
        this.toastr.error(err.statusText, 'Error.');
      }
    );
  }

  goBack() {
    this.router.navigate(['/admin/' + this.adminId]);
  }

  focusInput() {
    document.getElementById('searchInput').focus();
  }

}
