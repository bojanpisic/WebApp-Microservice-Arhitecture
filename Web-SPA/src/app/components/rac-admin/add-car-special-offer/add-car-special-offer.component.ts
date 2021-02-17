import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CarRentService } from 'src/services/car-rent.service';
import { DomSanitizer } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-add-car-special-offer',
  templateUrl: './add-car-special-offer.component.html',
  styleUrls: ['./add-car-special-offer.component.scss']
})
export class AddCarSpecialOfferComponent implements OnInit {

  adminId: number;
  racId: number;

  specialOffer: {car: any, dates: Array<any>, newPrice: number, oldPrice: number};

  searchText = '';

  cars: Array<any>;

  configureOffer = false;
  choosenCar: any;

  constructor(private router: Router, private routes: ActivatedRoute,
              private carService: CarRentService, private san: DomSanitizer,
              private toastr: ToastrService) {
    routes.params.subscribe(param => {
      this.adminId = param.id;
    });

    this.specialOffer = {
      car: null,
      dates: [],
      newPrice: 0,
      oldPrice: 0
    };
  }

  ngOnInit(): void {
    this.cars = [];
    const air1 = this.carService.getAdminsCars().subscribe(
      (res: any[]) => {
        if (res.length) {
          res.forEach(element => {
            const car = {
              brand: element.brand,
              carId: element.carId,
              city: element.city,
              model: element.model,
              name: element.name,
              pricePerDay: element.pricePerDay,
              seatsNumber: element.seatsNumber,
              state: element.state,
              type: element.type,
              year: element.year
            };
            this.cars.push(car);
          });
        }
      },
      err => {
        this.toastr.error(err.error, 'Error!');
      }
    );
  }

  goBack() {
    this.router.navigate(['/rac-admin/' + this.adminId + '/special-car-offers']);
  }

  toggleConfigureOffer(car?: any) {
    if (car !== undefined) {
      this.choosenCar = car;
    }
    this.configureOffer = !this.configureOffer;
  }

  onAddInfo(value: {dates: Array<string>, newPrice: number, oldPrice: number}) {
    value.dates.forEach(v => {
      this.specialOffer.dates.push(v);
    });
    this.specialOffer.car = this.choosenCar;
    this.specialOffer.newPrice += value.newPrice;
    this.specialOffer.oldPrice += value.oldPrice;
    this.choosenCar = !this.choosenCar;
  }

  onAddSpecialOffer(value: any) {
    this.onFinish(value);
  }

  onFinish(value: any) {
    // dodaj special offer
    const data = {
      id: this.choosenCar.carId,
      FromDate: value.fromDate,
      ToDate: value.toDate,
      NewPrice: value.newPrice,
    };
    this.carService.addSpecialOffer(data).subscribe(
      (res: any) => {
        this.exit();
      },
      err => {
        this.toastr.error(err.error, 'Error!');
      }
    );
  }

  exit() {
    this.router.navigate(['/rac-admin/' + this.adminId + '/special-car-offers']);
  }

}
