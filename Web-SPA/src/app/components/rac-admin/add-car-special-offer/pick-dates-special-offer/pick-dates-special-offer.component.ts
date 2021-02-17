import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-pick-dates-special-offer',
  templateUrl: './pick-dates-special-offer.component.html',
  styleUrls: ['./pick-dates-special-offer.component.scss']
})
export class PickDatesSpecialOfferComponent implements OnInit {

  @Input() carPricePerDay: number;
  @Output() addSpecialOffer = new EventEmitter<{fromDate: string, toDate: string, newPrice: number}>();

  form: FormGroup;
  formOffer: FormGroup;
  oldPrice = 0;
  submitOffer = false;
  errorFromDate = false;
  errorToDate = false;
  errorNewPrice = false;
  errorForm = false;

  constructor(private toastr: ToastrService) { }

  ngOnInit(): void {
    this.initForm();
    setTimeout(() => {
      this.setDateTimeInputs();
    }, 1000);
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

  onSubmit() {
    if (this.validateForm()) {
      // this.addSpecialOffer.emit({
      //   fromDate: this.form.controls.fromDate.value,
      //   toDate: this.form.controls.toDate.value,
      //   newPrice: this.form.controls.newPrice.value
      // });
      const fromDate = new Date(this.form.controls.fromDate.value);
      const toDate = new Date(this.form.controls.toDate.value);
      const times = toDate.getTime() - fromDate.getTime();
      const days = times / (1000 * 3600 * 24);
      this.oldPrice = this.carPricePerDay * days;
      this.initFormOffer();
      this.submitOffer = true;
    }
  }

  onSubmitOffer() {
    if (this.validateFormOffer()) {
      this.addSpecialOffer.emit({
        fromDate: this.form.controls.fromDate.value,
        toDate: this.form.controls.toDate.value,
        newPrice: this.formOffer.controls.newPrice.value
      });
    }
  }

  initForm() {
    this.form = new FormGroup({
      fromDate: new FormControl('', Validators.required),
      toDate: new FormControl('', Validators.required),
    });
  }
  initFormOffer() {
    this.formOffer = new FormGroup({
      newPrice: new FormControl('', [Validators.required, Validators.max(this.oldPrice)]),
    });
  }

  validateForm() {
    let retVal = true;
    if (this.form.controls.fromDate.invalid) {
      this.errorFromDate = true;
      retVal = false;
    }
    if (this.form.controls.toDate.invalid) {
      this.errorToDate = true;
      retVal = false;
    }
    const fromDate = new Date(this.form.controls.fromDate.value);
    const toDate = new Date(this.form.controls.toDate.value);
    if (fromDate >= toDate) {
      this.errorForm = true;
      retVal = false;
    }
    return retVal;
  }

  validateFormOffer() {
    let retVal = true;
    if (this.formOffer.controls.newPrice.invalid) {
      this.errorNewPrice = true;
      retVal = false;
    }
    return retVal;
  }

}
