import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CarRentService } from 'src/services/car-rent.service';
import { DomSanitizer } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-rac-special-offers',
  templateUrl: './rac-special-offers.component.html',
  styleUrls: ['./rac-special-offers.component.scss']
})
export class RacSpecialOffersComponent implements OnInit {

  adminId: number;
  airlineId: number;

  searchText = '';
  specialOffers: Array<{
    name: string,
    brand: string,
    model: string,
    year: number,
    type: string,
    newPrice: number,
    oldPrice: number,
    fromDate: string,
    toDate: string,
    seatsNumber: number
  }>;
  itsOk = false;
  monthNames = ['January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December'];

  constructor(private router: Router,
              private routes: ActivatedRoute,
              private carService: CarRentService,
              private san: DomSanitizer,
              private toastr: ToastrService) {
    routes.params.subscribe(param => {
      this.adminId = param.id;
    });
    this.specialOffers = [];
  }

  ngOnInit(): void {
    this.carService.getAdminsSpecialOffers().subscribe(
      (res: any[]) => {
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
              rate: element.rate === undefined ? null : element.rate
            };
            this.specialOffers.push(new1);
            const month = new1.fromDate.split('-')[1];
            console.log(month);
          });
        }
        this.itsOk = true;
        console.log(res);
      },
      err => {
        this.toastr.error(err.error, 'Error!');
      }
    );
  }

  goBack() {
    this.router.navigate(['/rac-admin/' + this.adminId]);
  }

  focusInput() {
    document.getElementById('searchInput').focus();
  }

}
