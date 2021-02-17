import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CarRentService } from 'src/services/car-rent.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-my-cars',
  templateUrl: './my-cars.component.html',
  styleUrls: ['./my-cars.component.scss']
})
export class MyCarsComponent implements OnInit {

  userId: number;
  cars: Array<any>;
  previousReservations: Array<any>;
  upcomingReservations: Array<any>;

  constructor(private routes: ActivatedRoute,
              private carService: CarRentService,
              private toastr: ToastrService,
              private router: Router) {
    routes.params.subscribe(param => {
      this.userId = param.id;
    });
    this.cars = new Array<any>();
    this.previousReservations = [];
    this.upcomingReservations = [];
  }

  ngOnInit(): void {
    // ovde podesi nazive parametara koje primam ukoliko se razlikuju
    const c = this.carService.getUsersCarReservations().subscribe(
      (res: any[]) => {
        if (res.length > 0) {
          res.forEach(el => {
            const r = {
              brand: el.brand,
              carId: el.carId,
              carServiceId: el.carServiceId,
              city: el.city,
              model: el.model,
              name: el.name,
              seatsNumber: el.seatsNumber,
              pricePerDay: el.pricePerDay, // VRATI I OVO ZA SAD
              state: el.state,
              type: el.type,
              year: el.year,
              rate: el.rate,
              from: el.from,
              to: el.to,
              dep: el.dep,
              ret: el.ret,
              totalPrice: el.totalPrice,
              reservationId: el.reservationId,
              isCarRated: el.isCarRated,
              isRACSRated: el.isRACSRated,
              canCancel: el.canCancel
            };
            console.log(r, el.isUpcoming, el.canCancel);
            if (el.isUpcoming) {
              this.upcomingReservations.push(r);
            } else {
              this.previousReservations.push(r);
            }
          });
        }
      },
      err => {
        console.log(err);
      }
    );
  }

  loadAll() {
    this.upcomingReservations = [];
    this.previousReservations = [];
    const c = this.carService.getUsersCarReservations().subscribe(
      (res: any[]) => {
        if (res.length > 0) {
          res.forEach(el => {
            const r = {
              brand: el.brand,
              carId: el.carId,
              carServiceId: el.carServiceId,
              city: el.city,
              model: el.model,
              name: el.name,
              seatsNumber: el.seatsNumber,
              pricePerDay: el.pricePerDay, // VRATI I OVO ZA SAD
              state: el.state,
              type: el.type,
              year: el.year,
              rate: el.rate,
              from: el.from,
              to: el.to,
              dep: el.dep,
              ret: el.ret,
              totalPrice: el.totalPrice,
              reservationId: el.reservationId,
              isCarRated: el.isCarRated,
              isRACSRated: el.isRACSRated,
              canCancel: el.canCancel
            };
            console.log(r, el.isUpcoming, el.canCancel);
            if (el.isUpcoming) {
              this.upcomingReservations.push(r);
            } else {
              this.previousReservations.push(r);
            }
          });
        }
      },
      err => {
        console.log(err);
      }
    );
  }

  goBack() {
    this.router.navigate(['/' + this.userId + '/profile']);
  }

  onRateCar(value: any, id: any) {
    const data = {
      userId: this.userId,
      rate: value,
      carId: id,
    };
    this.carService.rateCar(data).subscribe(
      (res: any) => {
        this.toastr.success('Success!');
        this.loadAll();
      },
      err => {
        // tslint:disable-next-line: triple-equals
        if (err.status == 400) {
          console.log(err);
          // this.toastr.error('Incorrect username or password.', 'Authentication failed.');
          this.toastr.error(err.statusText, 'Error!');
        } else {
          this.toastr.error(err.error.statusText, 'Error!');
        }
      }
    );
  }

  onRateService(value: any, id: any){
    const data = {
      userId: this.userId,
      rate: value,
      carServiceId: id,
    };
    this.carService.rateCarService(data).subscribe(
      (res: any) => {
        this.toastr.success('Success!');
        this.loadAll();
      },
      err => {
        this.toastr.error(err.error, 'Error!');
      }
    );
  }

  onQuitReservation(value: any) {
    const data = {
      reservationId: value
    };
    this.carService.quitReservation(data).subscribe(
      (res: any) => {
        this.toastr.success('Success!');
        this.cars = [];
        this.loadAll();
      },
      err => {
        this.toastr.error(err.error, 'Error!');
      }
    );
  }

}
