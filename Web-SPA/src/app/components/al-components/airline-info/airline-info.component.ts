import { Component, OnInit, Input } from '@angular/core';
import { AirlineService } from 'src/services/airline.service';
import { Airline } from '../../../entities/airline';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { Destination } from 'src/app/entities/destination';
import { DomSanitizer } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-airline-info',
  templateUrl: './airline-info.component.html',
  styleUrls: ['./airline-info.component.scss']
})
export class AirlineInfoComponent implements OnInit {

  id: number;
  userId;
  airline: any;

  isOk = false;

  // tslint:disable-next-line:max-line-length
  constructor(private route: ActivatedRoute, private san: DomSanitizer,
              private airlineService: AirlineService, private location: Location,
              private toastr: ToastrService, private router: Router) {
    route.params.subscribe(params => { this.id = params.airline; this.userId = params.id; });
  }

  ngOnInit(): void {
    window.scroll(0, 0);
    const a = this.airlineService.getAirlineProfile(this.id).subscribe(
      (res: any) => {
        console.log(res);
        const destina = [];
        res.destinations.forEach(element => {
          const des = {
            imageUrl: this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${element.imageUrl}`),
            city: element.city,
            state: element.state
          };
          destina.push(des);
        });
        this.airline = {
          rate: res.rate,
          city: res.city,
          state: res.state,
          lat: res.lat,
          lon: res.lon,
          name: res.name,
          logo: (res.logo === null) ? null : this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${res.logo}`),
          about: res.about,
          destinations: destina
        };
        this.isOk = true;
      },
      err => {
        this.toastr.error(err.error, 'Error!');
      }
    );
  }

  goBack() {
    if (this.userId === undefined) {
      this.router.navigate(['/airlines']);
    } else {
      this.router.navigate(['/' + this.userId + '/airlines']);
    }
  }

  seeOffers() {
    if (this.userId === undefined) {
      this.router.navigate(['/airlines/' + this.id + '/airline-info/flight-special-offers']);
    } else {
      this.router.navigate(['/' + this.userId + '/airlines/' + this.id + '/airline-info/flight-special-offers']);
    }
  }
}
