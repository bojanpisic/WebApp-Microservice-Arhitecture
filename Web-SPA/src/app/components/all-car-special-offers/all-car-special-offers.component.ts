import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CarRentService } from 'src/services/car-rent.service';
import { DomSanitizer } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-all-car-special-offers',
  templateUrl: './all-car-special-offers.component.html',
  styleUrls: ['./all-car-special-offers.component.scss']
})
export class AllCarSpecialOffersComponent implements OnInit {

  racId: number;
  userId: number;
  showModal = false;

  specialOffers: Array<any>;
  itsOk = false;
  noOffers = false;
  monthNames = ['January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December'];


  constructor(private router: Router, private routes: ActivatedRoute, private carService: CarRentService
            , private san: DomSanitizer, private toastr: ToastrService) {
    routes.params.subscribe(param => {
      this.racId = param.rac;
      this.userId = param.id;
    });
    this.specialOffers = [];
  }

  ngOnInit(): void {
    window.scroll(0, 0);
    if (this.racId === undefined) {
      this.carService.getAllSpecialOffers().subscribe(

        (res: any[]) => {
          if (res.length === 0) {
            this.noOffers = true;
            this.itsOk = true;
          }
          if (res.length) {
            res.forEach(element => {
              const yearFrom = element.fromDate.split('T')[0].split('-')[0];
              const monthFrom = this.monthNames[+element.fromDate.split('T')[0].split('-')[1] - 1];
              const dayFrom = +element.fromDate.split('T')[0].split('-')[2];
              const fromDate1 = monthFrom + ' ' + dayFrom + ', ' + yearFrom;
              const yearTo = element.toDate.split('T')[0].split('-')[0];
              const monthTo = this.monthNames[+element.toDate.split('T')[0].split('-')[1] - 1];
              const dayTo = +element.toDate.split('T')[0].split('-')[2];
              const toDate1 = monthTo + ' ' + dayTo + ', ' + yearTo;
              const new1 = {
                name: element.name,
                brand: element.brand,
                model: element.model,
                year: element.year,
                type: element.type,
                newPrice: element.newPrice,
                oldPrice: element.oldPrice,
                fromDate: fromDate1,
                toDate: toDate1,
                seatsNumber: element.seatsNumber,
                rate: element.rate,
                id: element.id,
                from: element.from,
                to: element.to
              };
              this.specialOffers.push(new1);
            });
            this.itsOk = true;
          }
          console.log(res);
        },
        err => {
          this.toastr.error(err.statusText, 'Error!');
        }
      );
    } else {
      this.carService.getRACSpecialOffers(this.racId).subscribe(
        (res: any[]) => {
          if (res.length === 0) {
            this.noOffers = true;
            this.itsOk = true;
          }
          if (res.length) {
            res.forEach(element => {
              const yearFrom = element.fromDate.split('T')[0].split('-')[0];
              const monthFrom = this.monthNames[+element.fromDate.split('T')[0].split('-')[1] - 1];
              const dayFrom = +element.fromDate.split('T')[0].split('-')[2];
              const fromDate1 = monthFrom + ' ' + dayFrom + ', ' + yearFrom;
              const yearTo = element.toDate.split('T')[0].split('-')[0];
              const monthTo = this.monthNames[+element.toDate.split('T')[0].split('-')[1] - 1];
              const dayTo = +element.toDate.split('T')[0].split('-')[2];
              const toDate1 = monthTo + ' ' + dayTo + ', ' + yearTo;
              const new1 = {
                name: element.name,
                brand: element.brand,
                model: element.model,
                year: element.year,
                type: element.type,
                newPrice: element.newPrice,
                oldPrice: element.oldPrice,
                fromDate: fromDate1,
                toDate: toDate1,
                seatsNumber: element.seatsNumber,
                rate: element.rate,
                id: element.id,
                from: element.from,
                to: element.to
              };
              this.specialOffers.push(new1);
              const year = new1.fromDate.split('-')[0];
              console.log(year);
            });
            this.itsOk = true;
          }
          console.log(res);
        },
        err => {
          this.toastr.error(err.statusText, 'Error!');
        }
      );
    }
  }

  goBack() {
    if (this.userId !== undefined) {
      if (this.racId !== undefined) {
        this.router.navigate(['/' + this.userId + '/rent-a-car-services/' + this.racId + '/rent-a-car-service-info']);
      } else {
        this.router.navigate(['/' + this.userId + '/home']);
      }
    } else {
      if (this.racId !== undefined) {
        this.router.navigate(['/rent-a-car-services/' + this.racId + '/rent-a-car-service-info']);
      } else {
        this.router.navigate(['/']);
      }
    }
  }

  viewDeal(value: any) {
    if (this.userId !== undefined) {
      const data = {
        id: value
      };
      this.carService.reserveCarSpecialOffer(data).subscribe(
        (res: any) => {
          this.toastr.success('Success!');
          this.router.navigate(['/' + this.userId + '/home']);
          this.showModal = false;
        },
        err => {
          // tslint:disable-next-line: triple-equals
          if (err.status == 400) {
            console.log(err);
            // this.toastr.error('Incorrect username or password.', 'Authentication failed.');
            this.toastr.error(err.error, 'Error!');
          } else {
            this.toastr.error(err.error, 'Error!');
          }
          this.showModal = false;
        }
      );
    } else {
      this.showModal = true;
    }
  }

  onModal(value: any) {
    if (value) {
      this.router.navigate(['/signin']);
    }
    this.showModal = false;
  }

}
