import { Component, OnInit } from '@angular/core';
import { Car } from 'src/app/entities/car';
import { Router, ActivatedRoute } from '@angular/router';
import { CarRentService } from 'src/services/car-rent.service';

@Component({
  selector: 'app-rac-cars',
  templateUrl: './rac-cars.component.html',
  styleUrls: ['./rac-cars.component.scss']
})
export class RacCarsComponent implements OnInit {

  searchText = '';
  adminId: number;
  racId: number;
  cars: Array<{
    brand: string,
    carId: number,
    city: string,
    model: string,
    name: string,
    pricePerDay: number,
    seatsNumber: number,
    state: string,
    type: string,
    year: number
  }>;

  branchId: number;
  isOk = false;

  carIsReserved = false;
  blur = false;

  constructor(private router: Router, private routes: ActivatedRoute,
              private carService: CarRentService) {
    routes.params.subscribe(param => {
      this.adminId = param.id;
      this.branchId = param.branch;
    });
    this.cars = [];
  }

  ngOnInit(): void {
    // this.racId = this.racService.getAdminsRACId(this.adminId);
    // this.cars = this.carService.getCarsOfSpecificRAC(this.racId);
    if (this.branchId !== undefined) {
      const air1 = this.carService.getBranchesCars(this.branchId).subscribe(
        (res: any[]) => {
          console.log(res);
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
                year: element.year,
                rate: element.rate
              };
              this.cars.push(car);
            });
          }
          this.isOk = true;
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
    } else {
      const air1 = this.carService.getAdminsCars().subscribe(
        (res: any[]) => {
          console.log(res);
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
                year: element.year,
                rate: element.rate
              };
              this.cars.push(car);
            });
          }
          this.isOk = true;
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

    console.log(this.cars);
  }

  onEdit(value: any) {
    if (this.isReserved()) {
      this.carIsReserved = true;
      this.blur = true;
    } else {
      this.router.navigate(['/rac-admin/' + this.adminId + '/cars/' + value + '/edit-car']);
    }
  }

  onCarIsReserved(value: any) {
    this.carIsReserved = false;
    this.blur = false;
  }

  isReserved() {
    // proveriti da li je auto rezervirano
    return false;
  }

  goBack() {
    if (this.branchId !== undefined) {
      this.router.navigate(['/rac-admin/' + this.adminId + '/branches']);
    } else {
      this.router.navigate(['/rac-admin/' + this.adminId]);
    }
  }

  focusInput() {
    document.getElementById('searchInput').focus();
  }

}
