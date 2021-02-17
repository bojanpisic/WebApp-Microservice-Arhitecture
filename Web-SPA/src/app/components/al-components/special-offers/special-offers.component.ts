import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SpecialOffer } from 'src/app/entities/special-offer';
import { AirlineService } from 'src/services/airline.service';
import { Airline } from 'src/app/entities/airline';
import { DomSanitizer } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';
import { FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-special-offers',
  templateUrl: './special-offers.component.html',
  styleUrls: ['./special-offers.component.scss']
})
export class SpecialOffersComponent implements OnInit {

  airlineId: number;
  userId: number;
  airline: Airline;

  specialOffers: Array<{newPrice: number, oldPrice: number, flights: Array<any>, id: any}>;
  itsOk = false;
  showModal = false;

  formPassport;
  bookIt: Array<any>;
  errorPassport = false;
  reservationId;

  noOffers = false;

  constructor(private router: Router, private routes: ActivatedRoute, private airlineService: AirlineService,
              private san: DomSanitizer, private toastr: ToastrService,
              private formBuilder: FormBuilder) {
    routes.params.subscribe(param => {
      this.airlineId = param.airline;
      this.userId = param.id;
    });
    this.specialOffers = [];
    this.bookIt = [];
  }

  ngOnInit(): void {
    window.scroll(0, 0);
    if (this.airlineId === undefined) {
      this.airlineService.getAllSpecialOffers().subscribe(
        (res: any[]) => {
          if (res.length === 0) {
            this.noOffers = true;
            this.itsOk = true;
          }
          if (res.length) {
            console.log(res);
            res.forEach(element => {
              const new1 = {
                newPrice: element.newPrice,
                oldPrice: element.oldPrice,
                id: element.specialOfferId
              };
              const fli = [];
              element.flights.forEach(flight => {
                const st = [];
                flight.stops.forEach(s => {
                  st.push({city: s.city});
                });
                fli.push({
                  // tslint:disable-next-line:max-line-length
                  airlineLogo: (element.logoUrl === null) ? null : this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${element.logoUrl}`),
                  airlineName: element.name,
                  flightId: flight.flightId,
                  from: flight.from,
                  to: flight.to,
                  flightNumber: flight.flightNumber,
                  takeOffDate: flight.takeOffDate,
                  takeOffTime: flight.takeOffTime,
                  landingDate: flight.landingDate,
                  landingTime: flight.landingTime,
                  flightTime: flight.tripTime,
                  flightLength: flight.tripLength,
                  stops: st,
                  seatNum: {column: flight.column, row: flight.row, class: flight.class}
                });
              });
              this.specialOffers.push({newPrice: new1.newPrice, oldPrice: new1.oldPrice, flights: fli, id: new1.id});
              this.bookIt.push(true);
            });
            this.itsOk = true;
          }
        },
        err => {
          console.log('dada' + err.status);
          // tslint:disable-next-line: triple-equals
          if (err.status == 400) {
            console.log('400' + err);
            // this.toastr.error('Incorrect username or password.', 'Authentication failed.');
          } else if (err.status === 401) {
            console.log(err);
          } else {
            console.log(err);
          }
        }
      );
    } else {
      this.airlineService.getAirlineSpecialOffers(this.airlineId).subscribe(
        (res: any[]) => {
          if (res.length === 0) {
            this.noOffers = true;
            this.itsOk = true;
          }
          if (res.length) {
            res.forEach(element => {
              const new1 = {
                newPrice: element.newPrice,
                oldPrice: element.oldPrice,
                id: element.specialOfferId
              };
              const fli = [];
              element.flights.forEach(flight => {
                const st = [];
                flight.stops.forEach(s => {
                  st.push({city: s.city});
                });
                fli.push({
                  // tslint:disable-next-line:max-line-length
                  airlineLogo: (element.logoUrl === null) ? null : this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${element.logoUrl}`),
                  airlineName: element.name,
                  flightId: flight.flightId,
                  from: flight.from,
                  to: flight.to,
                  flightNumber: flight.flightNumber,
                  takeOffDate: flight.takeOffDate,
                  takeOffTime: flight.takeOffTime,
                  landingDate: flight.landingDate,
                  landingTime: flight.landingTime,
                  flightTime: flight.tripTime,
                  flightLength: flight.tripLength,
                  stops: st,
                  seatNum: {column: flight.column, row: flight.row, class: flight.class}
                });
              });
              this.specialOffers.push({newPrice: new1.newPrice, oldPrice: new1.oldPrice, flights: fli, id: new1.id});
              this.bookIt.push(true);
            });
            this.itsOk = true;
          }
          console.log(res);
        },
        err => {
          console.log('dada' + err.status);
          // tslint:disable-next-line: triple-equals
          if (err.status == 400) {
            console.log('400' + err);
            // this.toastr.error('Incorrect username or password.', 'Authentication failed.');
          } else if (err.status === 401) {
            console.log(err);
          } else {
            console.log(err);
          }
        }
      );
    }
    this.initForm();
  }

  goBack() {
    if (this.userId !== undefined) {
      if (this.airlineId !== undefined) {
        this.router.navigate(['/' + this.userId + '/airlines/' + this.airlineId + '/airline-info']);
      } else {
        this.router.navigate(['/' + this.userId + '/home']);
      }
    } else {
      if (this.airlineId !== undefined) {
        this.router.navigate(['/airlines/' + this.airlineId + '/airline-info']);
      } else {
        this.router.navigate(['/']);
      }
    }
  }

  viewDeal(index: any, specialOfferId: any) {
    // this.bookIt[index] = !this.bookIt[index];
    this.bookIt.forEach((element, i, theArray) => {
      // tslint:disable-next-line:triple-equals
      if (i == index) {
        theArray[i] = !this.bookIt[i];
      } else {
        theArray[i] = true;
      }
      console.log(theArray);
      // element = i === index ? !element : true;
    });
    console.log(specialOfferId);
    this.errorPassport = false;
    this.reservationId = this.bookIt[index] ? undefined : specialOfferId;
  }

  onModal(value: any) {
    if (value) {
      this.router.navigate(['/signin']);
    }
    this.showModal = false;
  }

  onSubmitPassport() {
    if (this.validateForm()) {
      if (this.userId !== undefined) {
        const data = {
          id: this.reservationId,
          passport: this.formPassport.controls.passport.value
        };
        this.airlineService.reserveSpecialOffer(data).subscribe(
          (res: any) => {
            this.toastr.success('Success!');
            this.router.navigate(['/' + this.userId + '/home']);
            this.showModal = false;
          },
          err => {
            // tslint:disable-next-line: triple-equals
            if (err.status == 400) {
              console.log(err);
              // this.toastr.error('Incorrect username or password.', 'Authentication failed.');
              this.toastr.error(err.error, 'Error!');
            } else {
              this.toastr.error(err.error, 'Error!');
            }
            this.showModal = false;
          }
        );
      } else {
        this.showModal = true;
      }
    }
  }

  validateForm() {
    let retVal = true;
    if (this.formPassport.controls.passport.invalid) {
      this.errorPassport = true;
      retVal = false;
    }
    return retVal;
  }

  initForm() {
    this.formPassport = this.formBuilder.group({
      passport: ['', Validators.required],
   });
  }

}
