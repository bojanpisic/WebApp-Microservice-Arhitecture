import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AirlineService } from 'src/services/airline.service';
import { Flight } from 'src/app/entities/flight';
import { Seat } from 'src/app/entities/seat';
import { SpecialOffer } from 'src/app/entities/special-offer';
import { DomSanitizer } from '@angular/platform-browser';
import { AddSeatsSpecialOfferComponent } from './add-seats-special-offer/add-seats-special-offer.component';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-add-special-offer',
  templateUrl: './add-special-offer.component.html',
  styleUrls: ['./add-special-offer.component.scss']
})
export class AddSpecialOfferComponent implements OnInit {

  adminId: number;
  airlineId: number;

  searchText = '';

  specialOffer: {flights: Array<any>, seats: Array<any>, newPrice: number, oldPrice: number};

  flights: Array<any>;
  selectedSeats: Array<any>;

  chooseSeat = false;
  choosenFlight: any;

  @ViewChild(AddSeatsSpecialOfferComponent) child: AddSeatsSpecialOfferComponent;

  constructor(private router: Router, private routes: ActivatedRoute, private airlineService: AirlineService,
              private san: DomSanitizer,
              private toastr: ToastrService) {
    routes.params.subscribe(param => {
      this.adminId = param.id;
    });

    this.specialOffer = {
      flights: [],
      seats: [],
      newPrice: 0,
      oldPrice: 0
    };

    this.selectedSeats = [];
  }

  ngOnInit(): void {
    this.flights = [];
    const air1 = this.airlineService.getAdminsFlights().subscribe(
      (res: any[]) => {
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
          console.log(res);
        }
        console.log('ok');
        // this.airlineId = res[0].airlineId;
        // this.flights = res[0].flights;
      },
      err => {
        console.log('dada' + err.status);
        // tslint:disable-next-line: triple-equals
        if (err.status == 400) {
          console.log(err);
        // tslint:disable-next-line: triple-equals
        } else if (err.status == 401) {
          console.log(err);
        } else {
          console.log(err);
        }
      }
    );
  }

  goBack() {
    this.router.navigate(['/admin/' + this.adminId + '/special-offers']);
  }

  toggleChooseSeat(flight?: Flight) {
    console.log(flight);
    if (flight !== undefined) {
      this.choosenFlight = flight;
      console.log(this.choosenFlight);
    }
    this.chooseSeat = !this.chooseSeat;
  }

  onAddSeat(value: any) {
    console.log(value);
    this.selectedSeats.push(value);
    this.specialOffer.seats.push(value.seatId);
    this.specialOffer.flights.push(this.choosenFlight);
    this.child.reload();
    this.specialOffer.newPrice += value.price;
    this.specialOffer.oldPrice += value.oldPrice;
    this.chooseSeat = !this.chooseSeat;
  }

  onFinish() {
    // dodaj special offer
    const data = {
      NewPrice: this.specialOffer.newPrice,
      SeatsIds: this.specialOffer.seats
    };
    this.airlineService.addSpecialOffer(data).subscribe(
      (res: any) => {
        this.exit();
      },
      err => {
        this.toastr.error(err.error, 'Error!');
      }
    );
  }

  exit() {
    this.router.navigate(['/admin/' + this.adminId + '/special-offers']);
  }

}
