import { Component, OnInit, Input } from '@angular/core';
import { Router, NavigationExtras } from '@angular/router';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Address } from 'src/app/entities/address';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-drive-main-form',
  templateUrl: './drive-main-form.component.html',
  styleUrls: ['./drive-main-form.component.scss', '../flight-main-form/flight-main-form.component.scss']
})
export class DriveMainFormComponent implements OnInit {

  sameLocationChoosed = true;
  @Input() userId;

  date: string;
  pickUpLocation: Address;
  dropOffLocation: Address;

  lastGoodPickUpLocation: string;
  lastPickUpLocation: string;
  lastGoodDropOffLocation: string;
  lastDropOffLocation: string;

  errorPickUpLocation = false;
  errorDropOffLocation = false;
  errorPickUpDate = false;
  errorDropOffDate = false;

  errorForm = false;

  form: FormGroup;

  constructor(private router: Router, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.initForm();
    this.setDateTimeInputs();
  }

  setDateTimeInputs() {
    const dtToday = new Date();

    let month = (dtToday.getMonth() + 1).toString();
    let day = (dtToday.getDate()).toString();
    const year = dtToday.getFullYear();
    if (+month < 10) {
        month = '0' + month.toString();
    }
    if (+day < 10) { 
        day = '0' + day.toString();
    }
    const maxDate = year + '-' + month + '-' + day;
    const inputDate1 = document.getElementById('fromDate');
    inputDate1.setAttribute('min', maxDate);
    const inputDate2 = document.getElementById('toDate');
    inputDate2.setAttribute('min', maxDate);
  }

  sameLocation() {
    this.sameLocationChoosed = true;
  }

  differentLocation() {
    this.sameLocationChoosed = false;
  }

  onPickUpLocation(value: any) {
    const obj = JSON.parse(value);
    this.pickUpLocation = new Address(obj.city, obj.state, obj.longitude, obj.latitude);
    this.lastGoodPickUpLocation = this.lastPickUpLocation;
  }

  onPickUpLocationInputChange(value: any) {
    this.lastPickUpLocation = value;
  }

  onDropOffLocation(value: any) {
    const obj = JSON.parse(value);
    this.dropOffLocation = new Address(obj.city, obj.state, obj.longitude, obj.latitude);
    this.lastGoodDropOffLocation = this.lastDropOffLocation;
  }

  onDropOffLocationInputChange(value: any) {
    this.lastDropOffLocation = value;
  }

  validateSameLocationForm() {
    let retVal = true;
    if (this.pickUpLocation === undefined || this.lastGoodPickUpLocation !== this.lastPickUpLocation) {
      this.errorPickUpLocation = true;
      retVal = false;
    }
    if (!this.sameLocationChoosed) {
      if (!this.validateDifferentLocationForm()) {
        this.errorDropOffLocation = true;
        retVal = false;
      }
    }
    if (this.form.controls.pickupDate.value === '') {
      this.errorPickUpDate = true;
      retVal = false;
    }
    if (this.form.controls.dropoffDate.value === '') {
      this.errorDropOffDate = true;
      retVal = false;
    }
    // if (this.form.controls.pickupDate.value.toDate() < Date.now()) {
    //   this.toastr.error('', 'Error');
    // }
    return retVal;
  }

  validateDifferentLocationForm() {
    if (this.dropOffLocation === undefined || this.lastGoodDropOffLocation !== this.lastDropOffLocation) {
      return false;
    }
    return true;
  }

  removeErrorClassPickUp() {
    this.errorPickUpLocation = false;
  }

  removeErrorClassDropOff() {
    this.errorDropOffLocation = false;
  }

  onSubmit() {
    if (this.validateSameLocationForm()) {
      this.errorForm = false;
      const queryParams: any = {};
      const array = [];
      array.push({
        type: '',
        from: this.pickUpLocation.city,
        to: (this.sameLocationChoosed) ? this.pickUpLocation.city : this.dropOffLocation.city,
        dep: this.form.controls.pickupDate.value,
        ret: this.form.controls.dropoffDate.value,
        minPrice: 0,
        maxPrice: 3000,
        racs: '',
        seatFrom: 2,
        seatTo: 10
      });
      queryParams.array = JSON.stringify(array);

      const navigationExtras: NavigationExtras = {
        queryParams
      };
      if (this.userId !== undefined) {
        this.router.navigate(['/' + this.userId + '/cars'], navigationExtras);
      } else {
        this.router.navigate(['/cars'], navigationExtras);
      }
    } else {
      this.errorForm = true;
    }
  }

  initForm() {
    this.form = new FormGroup({
      pickupDate: new FormControl('', Validators.required),
      dropoffDate: new FormControl('', Validators.required),
   });
  }
}
