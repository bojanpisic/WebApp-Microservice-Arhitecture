import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, NavigationExtras } from '@angular/router';
import { TripParameter } from 'src/app/entities/trip-parameter';
import { Location } from '@angular/common';
import { Flight } from 'src/app/entities/flight';
import { AirlineService } from 'src/services/airline.service';
import { TripReservation } from 'src/app/entities/trip-reservation';
import { Seat } from 'src/app/entities/seat';
import { SeatsForFlight } from 'src/app/entities/seats-for-flight';
import { PassengersForFlight } from 'src/app/entities/passengers-for-flight';
import { Passenger } from 'src/app/entities/passenger';
import { ToastrService } from 'ngx-toastr';
import { FlightPlusCarService } from 'src/services/flight-plus-car.service';

@Component({
  selector: 'app-flight-reservation',
  templateUrl: './flight-reservation.component.html',
  styleUrls: ['./flight-reservation.component.scss']
})
export class FlightReservationComponent implements OnInit {

  index: number;
  lastStep = false;
  pickSeats = false;
  fillInfo = false;
  pickedSeat: any;
  passenger: any;
  pickedSeats: Array<{seats: Array<{seatId: any, passenger: any}>}>;
  invitedFriend = false;
  confirmData;

  exitReservation = 'exitReservation';
  errorReservation = 'errorReservation';
  exit = false;
  error = false;
  blur = false;

  flights: Array<any>;
  mySeats: Array<{seatId: number}>;
  myPassport;
  friends: Array<{id: any, seatId: number}>;
  unregisteredFriends: Array<{firstName: string, lastName: string, passport: string, seatId: number}>;

  arrayOfValues: Array<any>;

  showTripDetails = false;
  showOfferCar = false;
  showConfirmReservation = false;

  pickSeatForMe = true;
  withBonus = false;
  withCar = false;
  toDateWithCar;

  userId;
  errorToDate = false;

  constructor(private activatedRoute: ActivatedRoute, private location: Location, private router: Router,
              private airlineService: AirlineService,
              private toastr: ToastrService,
              private flightPlusCar: FlightPlusCarService) {

    const trip = this.activatedRoute.snapshot.queryParamMap.get('trip');
    activatedRoute.params.subscribe(param => {
      this.userId = param.id;
    });
    if (trip === null) {
      this.arrayOfValues = new Array<TripParameter>();
    } else {
      this.arrayOfValues = JSON.parse(trip);
    }

    this.flights = [];
    this.mySeats = [];
    this.friends = [];
    this.unregisteredFriends = [];
    this.pickedSeats = [];

    this.index = -1;
  }

  ngOnInit(): void {
    this.initialize();
  }

  initialize() {
    const ids = [];
    for (const item of this.arrayOfValues) {
      ids.push(item.f);
      this.mySeats.push({seatId: null});
      this.pickedSeats.push({seats: []});
    }
    const seats = this.airlineService.getFlightSeats(ids).subscribe(
      (res: any[]) => {
        this.flights = res;
        this.nextStep();
      },
      err => {
        this.toastr.error(err.statusText, 'Error!');
      }
    );
  }

  // STEPS

  goBack() {
    if (this.index === -1) {
      this.location.back();
    }
    if (this.index === 0) {
      this.exit = true;
      this.blur = true;
    } else {
      this.index--;
      this.updateVariables();
    }
  }

  nextStep() {
    if (this.index === this.flights.length) {
      this.index++;
      this.updateVariables();
    } else {
      if (this.validateStep()) {
        this.index++;
        this.updateVariables();
      } else {
        this.error = true;
        this.blur = true;
      }
    }
  }

  finish() {
    if (this.validateStep()) {
      this.index++;
      this.updateVariables();
    } else {
      this.error = true;
      this.blur = true;
    }
  }

  // ACTIONS

  onPickSeat(seat: any) {
    const seatIndex = this.flights[this.index].seats.indexOf(seat);
    if (this.flights[this.index].seats[seatIndex].available) {
      this.fillInfo = true;
      this.blur = true;
      this.pickedSeat = seat;

      let isTaken = false;
      this.pickedSeats[this.index].seats.forEach(element => {
        if (element.seatId === seat.seatId) {
          isTaken = true;
        }
      });
      if (isTaken) {
        this.pickedSeats[this.index].seats.forEach(element => {
          if (element.seatId === seat.seatId) {
            this.passenger = element.passenger;
          }
        });
        if (this.passenger.firstName === undefined && this.passenger.lastName === undefined && this.passenger.friendsId === undefined) {
          this.pickSeatForMe = true;
        } else {
          this.pickSeatForMe = false;
        }
        if (this.passenger.friendsId) {
          this.invitedFriend = true;
        } else {
          this.invitedFriend = false;
        }
        // TREBA DA PROMENIS DA THIS.PICKEDSEATS IMAJU SEM ID-EVA SEDISTA ZA SVAKI LET UZ TO DA IMAJU
        // I PASSPORT I FIRST NAME I LAST NAME PASSENGERA
      } else {
        if (this.mySeats[this.index].seatId === null) {
          this.pickSeatForMe = true;
        } else {
          this.pickSeatForMe = false;
        }
        this.passenger = undefined;
      }
    }
  }

  addPassenger(passenger: any) {
    if (passenger === 'CLOSE') {
      this.blur = false;
      this.fillInfo = false;
      this.pickedSeat = null;
      return;
    }
    console.log('STIGAO PASENGER!!!!!!!' + passenger);
    const seatIndex = this.flights[this.index].seats.indexOf(this.pickedSeat);
    if (passenger !== null) {
      // TREBA U BAZU ZAPISATI DA JE ZAUZETO
      this.pickedSeat.passenger = passenger;
      if (this.mySeats[this.index].seatId === null) {
        this.mySeats[this.index].seatId = this.pickedSeat.seatId;
        this.pickSeatForMe = false;
        if (this.myPassport === undefined) {
          this.myPassport = passenger.passport;
        }
      } else {
        if (passenger.passport === undefined) {
          // zovi frienda preko passenger.id
          this.friends.push({id: passenger, seatId: this.pickedSeat.seatId});
        } else {
          this.unregisteredFriends.push({
            firstName: passenger.firstName,
            lastName: passenger.lastName,
            passport: passenger.passport,
            seatId: this.pickedSeat.seatId
          });
        }
      }
      this.pickedSeats[this.index].seats.push({
        seatId: this.pickedSeat.seatId,
        // tslint:disable-next-line:object-literal-shorthand
        passenger: passenger
      });
    } else {
      let isTaken = false;
      this.pickedSeats[this.index].seats.forEach(element => {
        if (this.pickedSeat.seatId == element.seatId) {
          isTaken = true;
        }
      });
      if (isTaken) {
        console.log('SEDISTE JE ZAUZETO ' + this.pickedSeat.seatId);
        let ind;
        console.log('SVA SEDISTA:');
        // tslint:disable-next-line:prefer-for-of
        for (let i = 0; i < this.pickedSeats[this.index].seats.length; i++) {
          console.log(this.pickedSeats[this.index].seats[i].seatId);
          if (this.pickedSeats[this.index].seats[i].seatId == this.pickedSeat.seatId) {
            ind = i;
          }
        }
        console.log('ind:' + ind);
        this.pickedSeats[this.index].seats.splice(ind, 1);
        const isMySeat = this.mySeats[this.index].seatId == this.pickedSeat.seatId;
        if (isMySeat) {
          console.log('OVO JE MOJE SEDISTE');
          this.mySeats[this.index].seatId = null;
          let isEmpty = true;
          this.mySeats.forEach(element => {
            if (element.seatId !== null) {
              isEmpty = false;
            }
          });
          if (isEmpty) {
            this.myPassport = undefined;
            this.pickSeatForMe = true;
          }
        }
        let isFriendsSeat = false;
        let indexOfFriendsSeat;
        for (let i = 0; i < this.friends.length; i++) {
          console.log(this.friends[i].seatId, this.pickedSeat.seatId);
          if (this.friends[i].seatId == this.pickedSeat.seatId) {
            isFriendsSeat = true;
            indexOfFriendsSeat = i;
          }
        }
        if (isFriendsSeat) {
          console.log('OVO JE FRENDOVO SEDISTE');
          console.log(indexOfFriendsSeat);
          this.friends.splice(indexOfFriendsSeat, 1);
        }
        let isUnregisteredUsersSeat = false;
        let indexOfUnregisteredUsersSeat;
        for (let i = 0; i < this.unregisteredFriends.length; i++) {
          if (this.unregisteredFriends[i].seatId == this.pickedSeat.seatId) {
            isUnregisteredUsersSeat = true;
            indexOfUnregisteredUsersSeat = i;
          }
        }
        if (isUnregisteredUsersSeat) {
          this.unregisteredFriends.splice(indexOfUnregisteredUsersSeat, 1);
        }
      }
      // OVDE JE NESTO SA BRISANJEM
    }

    console.log(this.mySeats);
    console.log(this.myPassport);
    console.log(this.friends);
    console.log(this.unregisteredFriends);
    console.log(this.pickedSeats);
    this.blur = false;
    this.fillInfo = false;
    this.pickedSeat = null;
  }

  // MODALS

  onExitReservation(value: any) {
    if (value) {
      this.location.back();
      this.index--;
      this.emptyReserved();
      this.updateVariables();
    }
    this.exit = false;
    this.blur = false;
  }

  onErrorReservation(value: any) {
    this.error = false;
    this.blur = false;
  }

  updateVariables() {
    // tslint:disable-next-line:max-line-length
    this.lastStep = (this.index === this.flights.length - 1) ? true : false;
    this.pickSeats = (this.index >= 0 && this.index < this.flights.length) ? true : false;
    this.showConfirmReservation = false;
    if (this.index === this.flights.length) {
      // console.log('INDEX JE' + this.index);
      // console.log(this.mySeats);
      // console.log(this.myPassport);
      // console.log(this.friends);
      // console.log(this.unregisteredFriends);
      // console.log(this.pickedSeats);
      const data = {
        mySeatsIds: this.mySeats.map(res => res.seatId),
        friends: this.friends.map(res => {
          return {id: res.id.friendsId, seatId: res.seatId};
        }),
        unregisteredFriends: this.unregisteredFriends
      };
      console.log('SALJEM' + data);
      this.airlineService.getTripInfo(data).subscribe(
        (res: any) => {
          this.confirmData = {
            friends: res.friends,
            bonus: res.myBonus,
            mySeats: res.mySeats,
            priceWithBonus: res.priceWithBonus,
            totalPrice: res.totalPrice,
            unregisteredFriends: res.unregisteredFriends,
            flights: this.flights
          };
          this.showConfirmReservation = true;
        },
        err => {
          // tslint:disable-next-line: triple-equals
          if (err.status == 400) {
            // this.toastr.error('Incorrect username or password.', 'Authentication failed.');
            this.toastr.error(err.error, 'Error!');
          } else {
            this.toastr.error(err.error, 'Error!');
          }
        }
      );
    }
    if (this.index === this.flights.length + 1) {
      if (this.withCar) {
        const queryParams: any = {};
        const array = [];
        if (this.toDateWithCar === undefined) {
          this.errorToDate = true;
          return;
        }
        array.push({
          type: '',
          from: this.flights[0].to,
          to: this.flights[0].to,
          dep: this.flights[0].landingDate,
          ret: this.toDateWithCar,
          minPrice: 0,
          maxPrice: 3000,
          racs: '',
          seatFrom: 0,
          seatTo: 10
        });
        queryParams.array = JSON.stringify(array);

        const navigationExtras: NavigationExtras = {
          queryParams
        };
        this.flightPlusCar.addFlightReservation({
          mySeatsIds: this.mySeats.map(res => res.seatId),
          myPassport: this.myPassport,
          friends: this.friends,
          unregisteredFriends: this.unregisteredFriends,
          withBonus: this.withBonus,
          toDate: this.toDateWithCar
        });
        if (this.userId !== undefined) {
          this.router.navigate(['/' + this.userId + '/cars'], navigationExtras);
        } else {
          this.router.navigate(['/cars'], navigationExtras);
        }
      } else {
        const data = {
          mySeatsIds: this.mySeats.map(res => res.seatId),
          myPassport: this.myPassport,
          friends: this.friends,
          unregisteredFriends: this.unregisteredFriends,
          withBonus: this.withBonus,
          carReservation: null
        };
        this.airlineService.reserveTrip(data).subscribe(
          (res: any) => {
            this.toastr.success('Success!');
            this.router.navigate(['/' + this.userId + '/home']);
          },
          err => {
            // tslint:disable-next-line: triple-equals
            if (err.status == 400) {
              // this.toastr.error('Incorrect username or password.', 'Authentication failed.');
              this.toastr.error(err.error, 'Error!');
            } else {
              this.toastr.error(err.error, 'Error!');
            }
          }
        );
      }
    }
  }

  // HELPERS

  emptyReserved() {
    // this.flights.forEach(flight => {
    //   flight.seats.forEach(seat => {
    //     seat.reserved = false;
    //   });
    // });
    this.mySeats = [];
  }

  validateStep() {
    if (this.index >= 0) {
      if (this.pickedSeats[this.index].seats.length === 0) {
        return false;
      }
    }
    return true;
  }

  onBonusEmitter(value: any) {
    this.withBonus = value;
  }

  onCarEmitter(value: any) {
    this.withCar = value;
  }

  onDateEmitter(value: any) {
    this.errorToDate = true;
    this.toDateWithCar = value;
  }

}
