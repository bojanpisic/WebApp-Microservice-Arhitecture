import { Component, OnInit, Input, Output, EventEmitter, HostListener, ElementRef } from '@angular/core';
import { Seat } from 'src/app/entities/seat';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-configure-seats-modal',
  templateUrl: './configure-seats-modal.component.html',
  styleUrls: ['./configure-seats-modal.component.scss']
})
export class ConfigureSeatsModalComponent implements OnInit {

  @Input() special = false;
  @Input() seat: Seat;
  @Output() changePrice = new EventEmitter<number>();
  @Output() delete = new EventEmitter<boolean>();

  form: FormGroup;
  closeIt = 0;

  constructor(private eRef: ElementRef) {}

  ngOnInit(): void {
    this.initForm();
  }

  initForm() {
    if (this.special) {
      this.form = new FormGroup({
        price: new FormControl(this.seat.price, [Validators.required, Validators.max(this.seat.price)])
      });
    } else {
      this.form = new FormGroup({
        price: new FormControl(this.seat.price, Validators.required)
      });
    }
  }

  onChangePrice() {
    if (!this.form.controls.price.invalid) {
      this.changePrice.emit(this.form.controls.price.value);
    }
  }

  back() {
    this.delete.emit(false);
  }

  deleteSeat() {
    this.delete.emit(true);
  }

  @HostListener('document:click', ['$event'])
  clickout(event) {
    if (this.closeIt < 1) {
      this.closeIt = 1;
    } else {
      this.closeIt = 2;
    }
    if (!this.eRef.nativeElement.contains(event.target)) {
      if (this.closeIt === 2) {
        this.back();
      }
    }
  }

}
