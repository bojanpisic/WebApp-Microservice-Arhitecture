import { Component, OnInit } from '@angular/core';
import { Car } from 'src/app/entities/car';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CarRentService } from 'src/services/car-rent.service';
import { ToastrService } from 'ngx-toastr';

class ImageSnippet {
  pending = false;
  status = 'init';

  constructor(public src: string, public file: File) {}
}

@Component({
  selector: 'app-add-car',
  templateUrl: './add-car.component.html',
  styleUrls: ['./add-car.component.scss']
})
export class AddCarComponent implements OnInit {

  adminId: number;
  carId: number;
  car: Car;

  seatsNumber = 10;
  pricePerDay = 200;
  minusDisabled = false;
  plusDisabled = false;

  dropdown = false;
  dropdownBranch = false;
  pickedCarType = 'Standard';

  form: FormGroup;
  selectedFile: ImageSnippet;
  imageToShow: any;

  img;

  imagePicked = false;
  numberOfSeats = 4;

  errorImage = false;
  errorBrand = false;
  errorModel = false;
  errorYear = false;
  errorSeats = false;
  errorPrice = false;

  formOk = false;
  serviceLocation: {city: string, state: string};
  carLocation: {city: string, state: string};
  branches = [];
  imageToSend: any;
  addToMain = false;

  constructor(private router: Router, private routes: ActivatedRoute,
              private racService: CarRentService,
              private toastr: ToastrService) {
    routes.params.subscribe(param => {
      this.adminId = param.id;
    });

    this.car = new Car();
  }

  ngOnInit(): void {
    const air1 = this.racService.getRACCityState().subscribe(
      (data: any) => {
        const a = {
          city: data.city,
          state: data.state,
        };
        this.carLocation = {city: a.city, state: a.state};
        this.serviceLocation = {city: a.city, state: a.state};
        this.formOk = true;
      },
      err => {
        this.toastr.error(err.error, 'Error!');
      }
    );
    const air2 = this.racService.getAdminsBranches().subscribe(
      (res: any[]) => {
        if (res.length) {
          res.forEach(element => {
            const new1 = {
              branchId: element.branchId,
              city: element.city,
              state: element.state
            };
            this.branches.push(new1);
          });
        }
      },
      err => {
        this.toastr.error(err.error, 'Error!');
      }
    );
    this.initForm();
  }

  setCarLocation(branch: any) {
    this.carLocation = {city: branch.city, state: branch.state};
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

  }

  setCarType(value: string) {
    this.pickedCarType = value;
  }

  onFileChanged(imageInput: any) {
    const file: File = imageInput.files[0];
    const reader = new FileReader();

    reader.addEventListener('load', (event: any) => {

      this.selectedFile = new ImageSnippet(event.target.result, file);

      this.selectedFile.pending = true;
      this.imageToSend = this.selectedFile.file;
    });

    reader.readAsDataURL(file);
  }

  toggleDropDown() {
    this.dropdown = !this.dropdown;
    console.log(this.pickedCarType);
  }

  toggleDropDownBranch() {
    this.dropdownBranch = !this.dropdownBranch;
  }

  goBack() {
    this.router.navigate(['/rac-admin/' + this.adminId + '/cars']);
  }

  onSubmit() {
    if (this.validateForm()) {
      this.addToMain = false;
      let branchId = -1;
      if (this.carLocation.city === this.serviceLocation.city && this.carLocation.state === this.serviceLocation.state) {
        const data = {
          Brand: this.form.controls.brand.value,
          Model: this.form.controls.model.value,
          Year: this.form.controls.year.value,
          Type: this.pickedCarType,
          SeatsNumber: this.numberOfSeats,
          PricePerDay: this.form.controls.price.value,
        };
        this.racService.addCar(data).subscribe(
          (res: any) => {
            setTimeout(() => {
              this.router.navigate(['/rac-admin/' + this.adminId + '/cars']);
            }, 100);
          },
          err => {
            this.toastr.error(err.error, 'Error!');
          }
        );
      } else {
        const branch = this.branches.find(b => b.city === this.carLocation.city && b.state === this.carLocation.state);
        branchId = branch.branchId;
        const data = {
          BranchId: branchId,
          Brand: this.form.controls.brand.value,
          Model: this.form.controls.model.value,
          Year: this.form.controls.year.value,
          Type: this.pickedCarType,
          SeatsNumber: this.numberOfSeats,
          PricePerDay: this.form.controls.price.value,
        };
        this.racService.addCarToBranch(data).subscribe(
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
    if (this.form.controls.price.value === '') {
      this.errorPrice = true;
      retVal = false;
    }
    return retVal;
  }

  initForm() {
    this.form = new FormGroup({
      brand: new FormControl('', Validators.required),
      model: new FormControl('', Validators.required),
      year: new FormControl('', [Validators.required, Validators.pattern('^[0-9]*$')]),
      seats: new FormControl('5', Validators.required),
      price: new FormControl('50', [Validators.required, Validators.pattern('^[0-9]*$')]),
   });
  }

}
