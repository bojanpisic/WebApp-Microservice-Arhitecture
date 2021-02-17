// tslint:disable-next-line:max-line-length
import { Component, OnInit, ViewEncapsulation, ComponentFactoryResolver, ViewContainerRef, ViewChild, ComponentRef, ChangeDetectorRef, TemplateRef, AfterViewInit, AfterContentChecked, AfterContentInit } from '@angular/core';
import { AirlineService } from 'src/services/airline.service';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from 'src/app/entities/user';
import { UserService } from 'src/services/user.service';
import { Airline } from 'src/app/entities/airline';
import { Flight } from 'src/app/entities/flight';
import { Address } from 'src/app/entities/address';
import { FlightStopComponent } from './flight-stop/flight-stop.component';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Seat } from 'src/app/entities/seat';
import { Location } from '@angular/common';
import { DomSanitizer } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-add-flight',
  templateUrl: './add-flight.component.html',
  styleUrls: ['./add-flight.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AddFlightComponent implements OnInit {

  flight: Flight;

  step = 1;
  stepOneConfirmed = false;
  stepTwoConfirmed = false;
  stepThreeConfirmed = false;
  stepFourConfirmed = false;

  adminId: number;
  admin: User;

  flightType = 'direct flight';

  formFrom: FormGroup;
  fromLocation: Address;
  lastGoodLocationString: string;
  lastLocationString: string;
  errorFromLocation = false;
  departureDateInvalid = false;
  flightNumberInvalid = false;
  departureTimeInvalid = false;

  currentStop: any;
  isCurrentStopValid: boolean;

  formTo: FormGroup;
  toLocation: Address;
  errorToLocation = false;
  arrivalTimeInvalid = false;
  arrivalDateInvalid = false;
  flightLengthInvalid = false;

  firstClassSeatsNumber = 10;
  firstClassSeatPrice = 200;
  businessSeatsNumber = 10;
  businessSeatPrice = 150;
  economySeatsNumber = 10;
  economySeatPrice = 100;
  basicEconomySeatsNumber = 10;
  basicEconomySeatPrice = 50;

  minusFirstClassDisabled = false;
  minusBusinessDisabled = false;
  minusEconomyDisabled = false;
  minusBasicEconomyDisabled = false;

  destinations: Array<{destinationId: number, city: string, state: string}>;
  pickedDestinations: Array<{destinationId: number, city: string, state: string}>;
  availableDestinations: Array<{destinationId: number, city: string, state: string}>;
  stops: Array<{destinationId: number, city: string, state: string}>;
  dropdown = false;

  pickedFromDestination: any;
  pickedToDestination: any;
  pickedTakeOffDateTime: string;
  pickedLandingDateTime: string;
  pickedFlightLength: number;
  pickedSeats: Array<{Column: string, Row: string, Class: string, Price: number}>;

  componentsReferences = [];
  @ViewChild('viewContainerRef', { read: ViewContainerRef }) VCR: ViewContainerRef;
  @ViewChild('viewContainerRef1', { read: ViewContainerRef }) VCR1: ViewContainerRef;
  @ViewChild('tpl', {read: TemplateRef}) tpl: TemplateRef<any>;

  constructor(private route: ActivatedRoute, private router: Router,
              private userService: UserService, private airlineService: AirlineService,
              private CFR: ComponentFactoryResolver, private location: Location,
              private san: DomSanitizer, private toastr: ToastrService) {
    route.params.subscribe(params => {
      this.adminId = params.id;
    });
    this.stops = [];
    this.flight = new Flight();
    this.destinations = [];
    this.pickedDestinations = [];
    this.pickedSeats = [];
    this.availableDestinations = [];
   }

  ngOnInit(): void {
    this.airlineService.getAdminsDestinations().subscribe(
      (res: any[]) => {
        if (res.length) {
          console.log(res);
          res.forEach(element => {
            const new1 = {
              destinationId: element.destinationId,
              city: element.city,
              state: element.state
            };
            this.destinations.push(new1);
            this.availableDestinations.push(new1);
            this.pickedFromDestination = this.destinations[0];
          });
        }
      },
      err => {
        this.toastr.error(err.statusText, 'Error.');
      }
    );
    this.flight.airlineId = this.airlineService.getAirlineId(this.adminId);
    this.initFormFrom();
    this.initFormTo();
  }

  getStopsIds() {
    const retVal = [];
    this.stops.forEach(element => {
      retVal.push(element.destinationId);
    });
    return retVal;
  }

  addFlight() {
    const stopsids = this.getStopsIds();
    this.flight.flightNumber = 'TA872';
    this.flight.tripTime = '10';
    this.flight.ticketPrice = Math.min(this.firstClassSeatPrice, this.businessSeatPrice, this.economySeatPrice, this.basicEconomySeatPrice);
    console.log('saljem:');
    console.log(this.pickedTakeOffDateTime, this.pickedLandingDateTime, stopsids,
      this.pickedFromDestination.destinationId, this.pickedToDestination.destinationId, this.pickedSeats, this.pickedFlightLength);
    const data = {
      FlightNumber: this.formFrom.controls.flightNumber.value,
      TakeOffDateTime: this.pickedTakeOffDateTime,
      LandingDateTime: this.pickedLandingDateTime,
      StopIds: stopsids, // lista ideja destinacija
      FromId: this.pickedFromDestination.destinationId,
      ToId: this.pickedToDestination.destinationId,
      Seats: this.pickedSeats, // Column, Row, Class, Price
      TripLength: this.pickedFlightLength
    };
    this.airlineService.addFlight(data).subscribe(
      (res: any) => {
        this.toastr.success('Success!');
        setTimeout(() => {
          this.exit();
        }, 100);
      },
      err => {
        this.toastr.error(err.statusText, 'Error.');
      }
    );
  }

  setFromDestination(value: any) {
    this.pickedFromDestination = value;
  }

  setToDestination(value: any) {
    this.pickedToDestination = value;
  }

  toggleDropDown() {
    this.dropdown = !this.dropdown;
  }

  onSubmitFrom() {
    if (this.validateFromForm()) {
      this.pickedTakeOffDateTime = this.formFrom.controls.dateTime.value;
      const index = this.availableDestinations.indexOf(this.pickedFromDestination);
      this.availableDestinations.splice(index, 1);
      this.nextStep();
    }
  }

  onSubmitTo() {
    if (this.validateToForm()) {
      this.pickedLandingDateTime = this.formTo.controls.dateTime.value;
      this.pickedFlightLength = this.formTo.controls.flightLength.value;
      const index = this.availableDestinations.indexOf(this.pickedToDestination);
      this.availableDestinations.splice(index, 1);
      this.nextStep();
    }
  }

  validateFromForm() {
    let retVal = true;
    if (this.formFrom.controls.dateTime.value === '') {
      this.departureDateInvalid = true;
      retVal = false;
    }
    if (this.formFrom.controls.flightNumber.value === '') {
      this.flightNumberInvalid = true;
      retVal = false;
    }
    return retVal;
  }

  validateToForm() {
    let retVal = true;
    if (this.formTo.controls.dateTime.value === '') {
      this.arrivalDateInvalid = true;
      retVal = false;
    }
    if (this.formTo.controls.flightLength.value === '') {
      this.flightLengthInvalid = true;
      retVal = false;
    }
    return retVal;
  }

  initFormFrom() {
    this.formFrom = new FormGroup({
      dateTime: new FormControl('', Validators.required),
      flightNumber: new FormControl('', Validators.required),
   });
  }

  initFormTo() {
    this.formTo = new FormGroup({
      dateTime: new FormControl('', Validators.required),
      flightLength: new FormControl('', Validators.required),
   });
  }



  // FROM

  // TO


  // STOPS


  onFlightType(event: any) {
    this.flightType = event.target.value;
    if (this.flightType === 'with stops') {
      setTimeout(() => {
        this.clearView();
        this.createComponent();
      }, 100);
    }
  }

  addToTheView(value: any, counter: number) {
    if (this.VCR1 !== undefined) {
      this.VCR1.createEmbeddedView(this.tpl, {city: value.city, state: value.state, number: counter});
    }
  }

  clearView() {
    this.stops.forEach(element => {
      this.availableDestinations.push(element);
    });
    this.stops = [];
    this.VCR1.clear();
  }

  createComponent() {
    if (this.VCR !== undefined) {
      this.VCR.clear();
    }
    const componentFactory = this.CFR.resolveComponentFactory(FlightStopComponent);
    const componentRef: ComponentRef<FlightStopComponent> = this.VCR.createComponent(componentFactory);
    // const currentComponent = componentRef.instance;
    componentRef.instance.stop.subscribe($event => {
      this.addStop($event);
    });
    componentRef.instance.destinations = this.availableDestinations;
  }

  addStop(value: any) {
    this.currentStop = value;
  }

  onAddStop() {
    // tslint:disable-next-line:max-line-length
    this.stops.push(this.currentStop);
    this.addToTheView(this.currentStop, this.stops.length);
    const index = this.availableDestinations.indexOf(this.currentStop);
    this.availableDestinations.splice(index, 1);
    this.createComponent();
  }

  onRemoveStop() {
    if (this.VCR1.length < 1) {
            return;
    }
    const stop = this.stops.pop();
    this.availableDestinations.push(stop);
    this.VCR1.remove(this.VCR1.length - 1);
  }


  loadPickedStops() {
    setTimeout(() => {
      if (this.stops.length === 0) {
        this.clearView();
      } else {
        let counter = 1;
        this.stops.forEach(stop => {
          this.addToTheView(stop, counter);
          counter++;
        });
      }
      this.createComponent();
    }, 100);
  }

  // SEATS

  onPlusFirstClass() {
    this.firstClassSeatsNumber++;
    this.minusFirstClassDisabled = false;
  }

  onMinusFirstClass() {
    if (this.firstClassSeatsNumber > 1) {
      this.firstClassSeatsNumber--;
    }
    if (this.firstClassSeatsNumber === 1) {
      this.minusFirstClassDisabled = true;
    }
  }

  onPlusBusiness() {
    this.businessSeatsNumber++;
    this.minusBusinessDisabled = false;
  }

  onMinusBusiness() {
    if (this.businessSeatsNumber > 1) {
      this.businessSeatsNumber--;
    }
    if (this.businessSeatsNumber === 1) {
      this.minusBusinessDisabled = true;
    }
  }

  onPlusEconomy() {
    this.economySeatsNumber++;
    this.minusEconomyDisabled = false;
  }

  onMinusEconomy() {
    if (this.economySeatsNumber > 1) {
      this.economySeatsNumber--;
    }
    if (this.economySeatsNumber === 1) {
      this.minusEconomyDisabled = true;
    }
  }

  onPlusBasicEconomy() {
    this.basicEconomySeatsNumber++;
    this.minusBasicEconomyDisabled = false;
  }

  onMinusBasicEconomy() {
    if (this.basicEconomySeatsNumber > 1) {
      this.basicEconomySeatsNumber--;
    }
    if (this.basicEconomySeatsNumber === 1) {
      this.minusBasicEconomyDisabled = true;
    }
  }

  addSeats() {
    if (this.validateSeats()) {
      this.addFirstClass();
      this.addBusiness();
      this.addEconomy();
      this.addBasicEconomy();
      this.addFlight();
    }
  }

  validateSeats() {
    let retVal = true;
    console.log(this.firstClassSeatPrice);
    if (this.firstClassSeatsNumber === null || this.firstClassSeatsNumber < 1) {
      retVal = false;
    }
    if (this.businessSeatsNumber === null || this.businessSeatsNumber < 1) {
      retVal = false;
    }
    if (this.economySeatsNumber === null || this.economySeatsNumber < 1) {
      retVal = false;
    }
    if (this.basicEconomySeatsNumber === null || this.basicEconomySeatsNumber < 1) {
      retVal = false;
    }
    if (this.firstClassSeatPrice === null || this.firstClassSeatPrice < 1) {
      retVal = false;
    }
    if (this.businessSeatPrice === null || this.businessSeatPrice < 1) {
      retVal = false;
    }
    if (this.economySeatPrice === null || this.economySeatPrice < 1) {
      retVal = false;
    }
    if (this.basicEconomySeatPrice === null || this.basicEconomySeatPrice < 1) {
      retVal = false;
    }

    return retVal;
  }

  addFirstClass() {
    const rows = (this.firstClassSeatsNumber % 6 === 0) ? (this.firstClassSeatsNumber / 6) : (this.firstClassSeatsNumber / 6) + 1;
    for (let r = 0; r < rows; r++) {
      for (let c = 0; c < 6; c++) {
        const column = (c === 0) ? 'A' : (c === 1) ? 'B' : (c === 2) ? 'C' : (c === 3) ? 'D' : (c === 4) ? 'E' : 'F';
        this.pickedSeats.push({Column: column, Row: (r + 1).toString(), Class: 'F', Price: this.firstClassSeatPrice});
        this.firstClassSeatsNumber--;
        if (this.firstClassSeatsNumber === 0) {
          return;
        }
      }
    }

  }

  addBusiness() {
    const rows = (this.businessSeatsNumber % 6 === 0) ? (this.businessSeatsNumber / 6) : (this.businessSeatsNumber / 6) + 1;
    for (let r = 0; r < rows; r++) {
      for (let c = 0; c < 6; c++) {
        const column = (c === 0) ? 'A' : (c === 1) ? 'B' : (c === 2) ? 'C' : (c === 3) ? 'D' : (c === 4) ? 'E' : 'F';
        this.pickedSeats.push({Column: column, Row: (r + 1).toString(), Class: 'B', Price: this.businessSeatPrice});
        this.businessSeatsNumber--;
        if (this.businessSeatsNumber === 0) {
          return;
        }
      }
    }

  }

  addEconomy() {
    const rows = (this.economySeatsNumber % 6 === 0) ? (this.economySeatsNumber / 6) : (this.economySeatsNumber / 6) + 1;
    for (let r = 0; r < rows; r++) {
      for (let c = 0; c < 6; c++) {
        const column = (c === 0) ? 'A' : (c === 1) ? 'B' : (c === 2) ? 'C' : (c === 3) ? 'D' : (c === 4) ? 'E' : 'F';
        this.pickedSeats.push({Column: column, Row: (r + 1).toString(), Class: 'E', Price: this.economySeatPrice});
        this.economySeatsNumber--;
        if (this.economySeatsNumber === 0) {
          return;
        }
      }
    }

  }

  addBasicEconomy() {
    const rows = (this.basicEconomySeatsNumber % 6 === 0) ? (this.basicEconomySeatsNumber / 6) : (this.basicEconomySeatsNumber / 6) + 1;
    for (let r = 0; r < rows; r++) {
      for (let c = 0; c < 6; c++) {
        const column = (c === 0) ? 'A' : (c === 1) ? 'B' : (c === 2) ? 'C' : (c === 3) ? 'D' : (c === 4) ? 'E' : 'F';
        this.pickedSeats.push({Column: column, Row: (r + 1).toString(), Class: 'BE', Price: this.basicEconomySeatPrice});
        this.basicEconomySeatsNumber--;
        if (this.basicEconomySeatsNumber === 0) {
          return;
        }
      }
    }

  }

  // OTHER

  nextStep() {
    this.step++;
    this.stepSetup();
    console.log(this.flight);
  }

  previousStep() {
    this.step--;
    this.stepSetup();
  }

  exit() {
    this.router.navigate(['/admin/' + this.adminId + '/flights']);
  }

  stepSetup() {
    if (this.step === 0) {
      // show modal
    }
    if (this.step === 2 && this.flightType === 'with stops') {
      this.loadPickedStops();
    }
    if (this.step === 3) {
      if (this.pickedToDestination === undefined) {
        this.pickedToDestination = this.availableDestinations[0];
      }
    }
    if (this.step === 4) {
      // setTimeout( () => {
      //   this.router.navigate(['/']);
      // }, 300);
    }
  }

  onFrom(value: any) {
    const obj = JSON.parse(value);
    this.fromLocation = new Address(obj.city, obj.state, obj.longitude, obj.latitude);
    this.lastGoodLocationString = this.lastLocationString;
  }

  onFromInputChange(value: any) {
    this.lastLocationString = value;
  }

  onTo(value: any) {
    const obj = JSON.parse(value);
    this.toLocation = new Address(obj.city, obj.state, obj.longitude, obj.latitude);
    this.lastGoodLocationString = this.lastLocationString;
  }

  onToInputChange(value: any) {
    this.lastLocationString = value;
  }



  goBack() {
    this.router.navigate(['/admin/' + this.adminId + '/flights']);
  }

}
