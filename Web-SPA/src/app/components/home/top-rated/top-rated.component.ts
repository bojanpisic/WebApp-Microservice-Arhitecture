import { Component, OnInit, Input } from '@angular/core';
import { Airline } from 'src/app/entities/airline';
import { AirlineService } from 'src/services/airline.service';
import { RentACarService } from 'src/app/entities/rent-a-car-service';
import { CarRentService } from 'src/services/car-rent.service';
import { DomSanitizer } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-top-rated',
  templateUrl: './top-rated.component.html',
  styleUrls: ['./top-rated.component.scss']
})
export class TopRatedComponent implements OnInit {

  @Input() option: string;

  allRentACarServices: Array<any>;
  allAirlines: Array<any>;
  userId;

  constructor(private airlineService: AirlineService,
              private rentService: CarRentService,
              private san: DomSanitizer,
              private toastr: ToastrService,
              private route: ActivatedRoute,
              private router: Router) {
    route.params.subscribe(params => {
      this.userId = params.id;
    });
    this.allAirlines = [];
    this.allRentACarServices = [];
   }

  ngOnInit(): void {
    this.loadRentACarServices();
    // this.loadAirlines();
    this.loadAirlines();
  }

  loadAirlines() {
    let count = 0;
    const a = this.airlineService.getTopRatedAirlines().subscribe(
      (res: any[]) => {
        if (res.length > 0) {
          console.log('TOP AIRLINES' + res);
          res.forEach(element => {
            console.log(element);
            const airline = {
              airlineId: element.airlineId,
              city: element.city,
              state: element.state,
              name: element.name,
              // tslint:disable-next-line:max-line-length
              logo: (element.logo === null) ? null : this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${element.logo}`),
              about: element.about,
              destinations: element.destinations,
              rate: element.rate
            };
            count++;
            if (count <= 5) {
              this.allAirlines.push(airline);
            }
          });
        }
      },
      err => {
        this.toastr.error(err.statusText, 'Error!');
      }
    );
  }

  loadRentACarServices() {
    let count = 0;
    const a = this.rentService.getTopRatedRACs().subscribe(
      (res: any[]) => {
        if (res.length > 0) {
          res.forEach(element => {
            const rac = {
              racId: element.id,
              city: element.city,
              state: element.state,
              name: element.name,
              // tslint:disable-next-line:max-line-length
              logo: (element.logo === null) ? null : this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${element.logo}`),
              about: element.about,
              branches: element.branches,
              rate: element.rate
            };
            count++;
            if (count <= 5) {
              this.allRentACarServices.push(rac);
            }
          });
        }
        console.log(res);
      },
      err => {
        this.toastr.error(err.statusText, 'Error!');
      }
    );
  }

  onAirlines() {
    if (this.userId === undefined) {
      this.router.navigate(['/airlines']);
    } else {
      this.router.navigate(['/' + this.userId + '/airlines']);
    }
  }

  onRAC() {
    if (this.userId === undefined) {
      this.router.navigate(['/rent-a-car-services']);
    } else {
      this.router.navigate(['/' + this.userId + '/rent-a-car-services']);
    }
  }

}
