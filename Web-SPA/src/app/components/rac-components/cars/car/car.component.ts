import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { Car } from 'src/app/entities/car';

@Component({
  selector: 'app-car',
  templateUrl: './car.component.html',
  styleUrls: ['./car.component.scss']
})
export class CarComponent implements OnInit {

  @Input() car: any;
  @Input() fromDate: any;
  @Input() toDate: any;
  @Input() fromPlace: any;
  @Input() toPlace: any;
  @Input() spec: boolean;
  @Input() customerView: boolean;
  @Input() adminView: boolean;
  @Input() quitReservation: boolean;
  @Input() carRate: boolean;
  @Input() carRated: boolean;
  @Input() racsRated: boolean;
  @Input() canQuit: boolean;
  @Output() book = new EventEmitter<number>();
  @Output() editButtonClicked = new EventEmitter<number>();
  @Output() quitButtonClicked = new EventEmitter<number>();
  @Output() emitRateService = new EventEmitter<number>();
  @Output() emitRateCar = new EventEmitter<number>();

  rateExperience = false;

  minusRateServiceDisabled = true;
  plusRateServiceDisabled = false;
  rateService = 1;
  minusRateCarDisabled = true;
  plusRateCarDisabled = false;
  rateCar = 1;

  showRateService = false;
  showRateCar = false;

  showModal = false;
  blur = false;

  constructor() { }

  ngOnInit(): void {
    console.log(this.car);
    if (this.fromDate !== undefined) {
      this.fromDate = this.fromDate.split('T')[0];
    }
    if (this.toDate !== undefined) {
      this.toDate = this.toDate.split('T')[0];
    }
  }

  onEdit() {
    this.editButtonClicked.emit(this.car.carId);
  }

  onBook() {
    this.book.emit(this.car.carId);
  }

  onQuit() {
    this.quitButtonClicked.emit(this.car.reservationId);
  }

  onConfirmServiceRate() {
    if (this.rateService > 0 && this.rateService < 6) {
      this.emitRateService.emit(this.rateService);
    }
  }

  onConfirmCarRate() {
    if (this.rateCar > 0 && this.rateCar < 6) {
      this.emitRateCar.emit(this.rateCar);
    }
  }

  onRate() {
    this.rateExperience = !this.rateExperience;
  }

  onRateService() {
    this.showRateService = !this.showRateService;
  }

  onRateCar() {
    this.showRateCar = !this.showRateCar;
  }

  onPlusRateService() {
    if (this.rateService === 4) {
      this.plusRateServiceDisabled = true;
    } else {
      this.rateService++;
      this.minusRateServiceDisabled = false;
    }
  }

  onMinusRateService() {
    if (this.rateService > 1) {
      this.rateService--;
      this.plusRateServiceDisabled = false;
    }
    if (this.rateService === 1) {
      this.minusRateServiceDisabled = true;
    }
  }

  onPlusRateCar() {
    if (this.rateCar > 4) {
      this.plusRateCarDisabled = true;
    } else {
      this.rateCar++;
      this.minusRateCarDisabled = false;
    }
  }

  onMinusRateCar() {
    if (this.rateCar > 1) {
      this.rateCar--;
      this.plusRateCarDisabled = false;
    }
    if (this.rateCar === 1) {
      this.minusRateCarDisabled = true;
    }
  }

}
