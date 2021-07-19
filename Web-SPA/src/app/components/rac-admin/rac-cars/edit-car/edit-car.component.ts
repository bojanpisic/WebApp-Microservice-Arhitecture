import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Car } from 'src/app/entities/car';
import { CarRentService } from 'src/services/car-rent.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-edit-car',
  templateUrl: './edit-car.component.html',
  styleUrls: ['./edit-car.component.scss']
})
export class EditCarComponent implements OnInit {

  adminId: number;
  carId: number;
  car: any;

  seatsNumber = 10;
  numberOfSeats = 4;
  pricePerDay = 200;
  minusDisabled = false;
  plusDisabled = false;

  dropdown = false;
  pickedCarType = 'Luxury';

  form: FormGroup;
  selectedFile: File;
  errorBrand = false;
  errorModel = false;
  errorYear = false;
  errorPrice = false;

  isOk = false;

  constructor(private router: Router,
              private routes: ActivatedRoute,
              private carService: CarRentService,
              private san: DomSanitizer,
              private toastr: ToastrService) {
    routes.params.subscribe(param => {
      this.adminId = param.id;
    });
    routes.params.subscribe(param => {
      this.carId = param.car;
    });
  }

  ngOnInit(): void {
    const air1 = this.carService.getCar(this.carId).subscribe(
      (res: any) => {
        const car = {
          brand: res.brand,
          carId: res.carId,
          city: res.city,
          model: res.model,
          name: res.name,
          pricePerDay: res.pricePerDay,
          seatsNumber: res.seatsNumber,
          state: res.state,
          type: res.type,
          year: res.year,
          rate: res.rate,
          imageUrl: (res.imageUrl === null) ? null : this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${res.imageUrl}`)
        };
        this.car = car;
        this.isOk = true;
        this.initForm();
        this.pickedCarType = this.car.type;
        this.numberOfSeats = this.car.seatsNumber;
        this.pricePerDay = this.car.pricePerDay;
      },
      err => {
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

  onPlus() {
    this.numberOfSeats++;
    this.minusDisabled = false;
    if (this.numberOfSeats === 10) {
      this.plusDisabled = true;
    }
  }

  onMinus() {
    this.plusDisabled = false;
    this.numberOfSeats--;
    if (this.numberOfSeats === 2) {
      this.minusDisabled = true;
    }
  }

  onDelete() {
    const data = {
      id: this.carId
    };
    this.carService.deleteCar(data).subscribe(
      (res: any) => {
        setTimeout(() => {
          this.router.navigate(['/rac-admin/' + this.adminId + '/cars']);
        }, 100);
      },
      err => {
        this.toastr.error(err.error, 'Error!');
      }
    );
  }

  setCarType(value: string) {
    this.pickedCarType = value;
  }

  onFileChanged(event) {
    this.selectedFile = event.target.files[0];
  }

  toggleDropDown() {
    this.dropdown = !this.dropdown;
    console.log(this.pickedCarType);
  }

  goBack() {
    this.router.navigate(['/rac-admin/' + this.adminId + '/cars']);
  }

  onSubmit() {
    if (this.validateForm()) {
      const data = {
        id: this.carId,
        Brand: this.form.controls.brand.value,
        Model: this.form.controls.model.value,
        Year: this.form.controls.year.value,
        Type: this.pickedCarType,
        SeatsNumber: this.numberOfSeats,
        PricePerDay: this.form.controls.price.value,
      };
      this.carService.editCar(data).subscribe(
        (res: any) => {
          setTimeout(() => {
            this.router.navigate(['/rac-admin/' + this.adminId + '/cars']);
          }, 100);
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
  }

  validateForm() {
    let retVal = true;
    if (this.form.controls.brand.value === '') {
      this.errorBrand = true;
      retVal = false;
    }
    if (this.form.controls.model.value === '') {
      this.errorModel = true;
      retVal = false;
    }
    if (this.form.controls.year.value === '' || isNaN(+this.form.controls.year.value)) {
      this.errorYear = true;
      retVal = false;
    }
    if (this.form.controls.price.value === '' || isNaN(+this.form.controls.price.value)) {
      this.errorPrice = true;
      retVal = false;
    }
    return retVal;
  }

  initForm() {
    this.form = new FormGroup({
      brand: new FormControl(this.car.brand, Validators.required),
      model: new FormControl(this.car.model, Validators.required),
      year: new FormControl(this.car.year, [Validators.required, Validators.pattern('^[0-9]*$')]),
      price: new FormControl(this.car.pricePerDay, [Validators.required, Validators.pattern('^[0-9]*$')]),
   });
  }

}
