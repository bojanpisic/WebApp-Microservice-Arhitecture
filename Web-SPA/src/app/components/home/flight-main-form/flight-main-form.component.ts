import {ComponentRef, ComponentFactoryResolver, ViewContainerRef, ViewChild, Component, OnInit, Renderer2, Input  } from '@angular/core';
import { FlightFormPartComponent } from './flight-form-part/flight-form-part.component';
import { Router, NavigationExtras } from '@angular/router';
import { MapTypeControlStyle } from '@agm/core/services/google-maps-types';
import { FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-flight-main-form',
  templateUrl: './flight-main-form.component.html',
  styleUrls: ['./flight-main-form.component.scss'],
})
export class FlightMainFormComponent implements OnInit {

  @ViewChild('viewContainerRef', { read: ViewContainerRef }) VCR: ViewContainerRef;
  @Input() userId: any;
  @ViewChild('child') child;

  numOfTravellers = 1;
  errorForm = false;
  flightType: string;
  @Input() filterForm;

  componentsReferences = [];
  components = [];

  // tslint:disable-next-line:max-line-length
  constructor(private renderer: Renderer2, private CFR: ComponentFactoryResolver, private router: Router) {
   }

  ngOnInit(): void {
    this.flightType = 'oneWayFlight';
  }

  createComponent() {

    const componentFactory = this.CFR.resolveComponentFactory(FlightFormPartComponent);
    const componentRef: ComponentRef<FlightFormPartComponent> = this.VCR.createComponent(componentFactory);
    const currentComponent = componentRef.instance;
    this.components.push(currentComponent);

    this.componentsReferences.push(componentRef);
  }

  addFlight() {
    this.createComponent();
  }

  removeFlight() {

    if (this.VCR.length < 1) {
            return;
    }

    const componentRef = this.componentsReferences[this.componentsReferences.length - 1];

    this.VCR.remove(this.VCR.length - 1);
    this.componentsReferences.pop();
    this.components.pop();
  }

  oneWay() {
    this.flightType = 'oneWayFlight';
  }

  roundTrip() {
    this.flightType = 'roundTripFlight';
  }

  multiCity() {
    this.flightType = 'multiCityFlight';
  }

  onSubmit() {
    if (this.flightType === 'oneWayFlight') {
      if (this.validateOneWay()) {
        this.onRoute();
      } else {
        this.errorForm = true;
      }
    } else if (this.flightType === 'roundTripFlight') {
      if (this.validateTwoWay()) {
        this.onRoute();
      } else {
        this.errorForm = true;
      }
    } else {
      if (this.components === []) {
        if (this.validateOneWay()) {
          this.onRoute();
        } else {
          this.errorForm = true;
        }
      } else {
        if (this.validateOneWay()) {
          let isValid = true;
          this.components.forEach(component => {
            console.log('usao');
            isValid = (component.okFromLocation && component.okToLocation && component.inputDepart !== undefined) && isValid;
            // if (component.okFromLocation && component.okToLocation && component.inputDepart !== undefined) {
            //   this.onRoute();
            // } else {
            //   this.errorForm = true;
            // }
          });
          if (isValid) {
            this.onRoute();
          } else {
            this.errorForm = true;
          }
        } else {
          this.errorForm = true;
        }
      }
      console.log(this.components);
    }
  }

  onRoute() {
    const queryParams: any = {};
    const array = [];
    if (this.flightType === 'oneWayFlight') {
      array.push({type: 'one'});
      array.push({from: this.child.fromLocation.city, to: this.child.toLocation.city, dep: this.child.inputDepart});
    }
    if (this.flightType === 'roundTripFlight') {
      array.push({type: 'two'});
      array.push({from: this.child.fromLocation.city, to: this.child.toLocation.city,
                  dep: this.child.inputDepart, ret: this.child.inputReturn});
    }
    if (this.flightType === 'multiCityFlight') {
      array.push({type: 'multi'});
      array.push({from: this.child.fromLocation.city, to: this.child.toLocation.city, dep: this.child.inputDepart});
      this.components.forEach(component => {
        array.push({from: component.fromLocation.city, to: component.toLocation.city, dep: component.inputDepart});
      });
    }

    array.push({minPrice: 0, maxPrice: 3000, air: '', mind: '0h 0min', maxd: '23h 0min'});
    console.log(array);
    queryParams.array = JSON.stringify(array);

    const navigationExtras: NavigationExtras = {
      queryParams
    };
    if (this.userId !== undefined) {
      this.router.navigate(['/' + this.userId + '/trips'], navigationExtras);
    } else {
      this.router.navigate(['/trips'], navigationExtras);
    }
  }

  validateOneWay() {
    let retVal = true;
    if (this.child.fromLocation === undefined || this.child.lastGoodFromLocation !== this.child.lastFromLocation) {
      retVal = false;
    }
    if (this.child.toLocation === undefined || this.child.lastGoodToLocation !== this.child.lastToLocation) {
      retVal = false;
    }
    if (this.child.inputDepart === undefined) {
      retVal = false;
    }
    return retVal;
  }

  validateTwoWay() {
    let retVal = true;
    if (this.child.fromLocation === undefined || this.child.lastGoodFromLocation !== this.child.lastFromLocation) {
      retVal = false;
    }
    if (this.child.toLocation === undefined || this.child.lastGoodToLocation !== this.child.lastToLocation) {
      retVal = false;
    }
    if (this.child.inputDepart === undefined) {
      retVal = false;
    }
    if (this.child.inputReturn === undefined) {
      retVal = false;
    }
    return retVal;
  }

  onUpdateTravellers(value: any) {
    this.numOfTravellers = value;
  }

}
