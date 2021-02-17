import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AirlineService } from 'src/services/airline.service';
import { SpecialOffer } from 'src/app/entities/special-offer';
import { DomSanitizer } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-airline-special-offers',
  templateUrl: './airline-special-offers.component.html',
  styleUrls: ['./airline-special-offers.component.scss']
})
export class AirlineSpecialOffersComponent implements OnInit {

  adminId: number;
  airlineId: number;

  searchText = '';
  specialOffers: Array<{newPrice: number, oldPrice: number, flights: Array<any>}>;
  itsOk = false;

  constructor(private router: Router, private routes: ActivatedRoute, private airlineService: AirlineService,
              private san: DomSanitizer,
              private toastr: ToastrService) {
    routes.params.subscribe(param => {
      this.adminId = param.id;
    });
    this.specialOffers = [];
  }

  ngOnInit(): void {
    this.airlineService.getAdminsSpecialOffers().subscribe(
      (res: any[]) => {
        if (res.length) {
          res.forEach(element => {
            const new1 = {
              newPrice: element.newPrice,
              oldPrice: element.oldPrice,
            };
            const fli = [];
            element.flights.forEach(flight => {
              const st = [];
              flight.stops.forEach(s => {
                st.push({city: s.city});
              });
              fli.push({
                // tslint:disable-next-line:max-line-length
                airlineLogo: (element.logoUrl === null) ? null : this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${element.logoUrl}`),
                airlineName: element.name,
                flightId: flight.flightId,
                from: flight.from,
                to: flight.to,
                flightNumber: flight.flightNumber,
                takeOffDate: flight.takeOffDate,
                takeOffTime: flight.takeOffTime,
                landingDate: flight.landingDate,
                landingTime: flight.landingTime,
                flightTime: flight.tripTime,
                flightLength: flight.tripLength,
                stops: st,
                seatNum: {column: flight.column, row: flight.row, class: flight.class}
              });
            });
            this.specialOffers.push({newPrice: new1.newPrice, oldPrice: new1.oldPrice, flights: fli});
          });
          this.itsOk = true;
        }
        console.log(res);
      },
      err => {
        this.toastr.error(err.statusText, 'Error.');
      }
    );
  }

  goBack() {
    this.router.navigate(['/admin/' + this.adminId]);
  }

  focusInput() {
    document.getElementById('searchInput').focus();
  }

}
