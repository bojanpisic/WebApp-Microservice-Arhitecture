import { Component, OnInit } from '@angular/core';
import { Trip } from 'src/app/entities/trip';
import { Location } from '@angular/common';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { RegisteredUser } from 'src/app/entities/registeredUser';
import { UserService } from 'src/services/user.service';
import { AirlineService } from 'src/services/airline.service';
import { DomSanitizer } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-trips',
  templateUrl: './trips.component.html',
  styleUrls: ['./trips.component.scss']
})
export class TripsComponent implements OnInit {
  user: RegisteredUser;
  userId: number;
  trips: Array<any>;
  urlParams = [];
  filter = false;

  mySubscription;

  url: any;
  trip1: {flightsObject: Array<any>, minPrice: number};
  showModal = false;

  constructor(private userService: UserService,
              private route: ActivatedRoute, private airlineService: AirlineService,
              private router: Router, private san: DomSanitizer,
              private toastr: ToastrService) {
    const array = route.snapshot.queryParamMap.get('array');
    this.urlParams = JSON.parse(array);
    console.log(this.urlParams);

    this.route.params.subscribe(param => {
      this.userId = param.id;
    });

    this.router.routeReuseStrategy.shouldReuseRoute = () => {
      return false;
    };

    this.mySubscription = this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        // Trick the Router into believing it's last link wasn't previously loaded
        this.router.navigated = false;
      }
    });

    this.trips = new Array<any>();
    this.trip1 = {
      flightsObject: [],
      minPrice: 0
    };
   }

  ngOnInit(): void {
    let data;
    data = this.generateFilter();

    if (this.urlParams !== null) {
      this.url = {
        type: data.type,
        from: data.from,
        to: data.to,
        dep: data.dep,
        ret: data.ret,
        minPrice: data.minPrice,
        maxPrice: data.maxPrice,
        air: data.air,
        mind: data.mind,
        maxd: data.maxd
      };

      const a = this.airlineService.test(this.url).subscribe(
        (res: any[]) => {
          if (res.length > 0) {
            res.forEach(el => {
              console.log(el.flightsObject);
              this.trip1.flightsObject = [];
              this.trip1.minPrice = 0;
              el.flightsObject.forEach(element => {
                const new1 = {
                  flightId: element.flightId,
                  flightNumber: element.flightNumber,
                  // tslint:disable-next-line:max-line-length
                  airlineLogo: (element.airlineLogo === null) ? null : this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${element.airlineLogo}`),
                  airlineName: element.airlineName,
                  airlineId: element.airlineId,
                  from: element.from,
                  takeOffDate: element.takeOffDate,
                  takeOffTime: element.takeOffTime,
                  to: element.to,
                  landingDate: element.landingDate,
                  landingTime: element.landingTime,
                  flightLength: element.flightLength,
                  flightTime: element.flightTime,
                  stops: element.stops,
                  minPrice: element.minPrice
                };
                this.trip1.flightsObject.push(new1);
                this.trip1.minPrice += new1.minPrice;
              });
              this.trips.push(this.trip1);
            });
          }
          console.log(res);
        },
        err => {
          this.toastr.error(err.statusText, 'Error.');
        }
      );
    }
  }

  loadAll() {
    let data;
    const array = this.route.snapshot.queryParamMap.get('array');
    this.urlParams = JSON.parse(array);
    data = this.generateFilter();

    if (this.urlParams !== null) {
      this.url = {
        type: data.type,
        from: data.from,
        to: data.to,
        dep: data.dep,
        ret: data.ret,
        minPrice: data.minPrice,
        maxPrice: data.maxPrice,
        air: data.air,
        mind: data.mind,
        maxd: data.maxd
      };

      this.trips = [];

      console.log(this.url);

      const a = this.airlineService.test(this.url).subscribe(
        (res: any[]) => {
          if (res.length > 0) {
            res.forEach(el => {
              console.log(el.flightsObject);
              this.trip1.flightsObject = [];
              this.trip1.minPrice = 0;
              el.flightsObject.forEach(element => {
                const new1 = {
                  flightId: element.flightId,
                  flightNumber: element.flightNumber,
                  // tslint:disable-next-line:max-line-length
                  airlineLogo: (element.airlineLogo === null) ? null : this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${element.airlineLogo}`),
                  airlineName: element.airlineName,
                  airlineId: element.airlineId,
                  from: element.from,
                  takeOffDate: element.takeOffDate,
                  takeOffTime: element.takeOffTime,
                  to: element.to,
                  landingDate: element.landingDate,
                  landingTime: element.landingTime,
                  flightLength: element.flightLength,
                  flightTime: element.flightTime,
                  stops: element.stops,
                  minPrice: element.minPrice
                };
                this.trip1.flightsObject.push(new1);
                this.trip1.minPrice += new1.minPrice;
              });
              this.trips.push(this.trip1);
            });
          }
          console.log(res);
        },
        err => {
          this.toastr.error(err.statusText, 'Error.');
        }
      );
    }
  }

  generateFilter() {
    console.log('USAO');
    if (this.urlParams === null && this.userId === undefined) {
      this.router.navigate(['']);
      return;
    }
    if (this.urlParams === null && this.userId !== undefined) {
      this.router.navigate(['/' + this.userId + '/home']);
      return;
    }
    if (this.urlParams !== null) {
      console.log(this.urlParams);
      if (this.urlParams[0].type === 'one') {
        return {type: 'one', from: this.urlParams[1].from, to: this.urlParams[1].to,
                dep: this.urlParams[1].dep, ret: '', minPrice: this.urlParams[2].minPrice, maxPrice: this.urlParams[2].maxPrice,
                air: this.urlParams[2].air, mind: this.urlParams[2].mind, maxd: this.urlParams[2].maxd};
      } else if (this.urlParams[0].type === 'two') {
        // tslint:disable-next-line:max-line-length
        return {type: 'two', from: this.urlParams[1].from, to: this.urlParams[1].to,
                dep: this.urlParams[1].dep, ret: this.urlParams[1].ret,
                minPrice: this.urlParams[2].minPrice, maxPrice: this.urlParams[2].maxPrice,
                air: this.urlParams[2].air, mind: this.urlParams[2].mind, maxd: this.urlParams[2].maxd};
      } else {
        let froms = '';
        let tos = '';
        let deps = '';
        for (let i = 1; i < this.urlParams.length - 1; i++) {
          console.log(this.urlParams.length);
          if (i === this.urlParams.length - 2) {
            const element = this.urlParams[i];
            froms += this.urlParams[i].from;
            tos += this.urlParams[i].to;
            deps += this.urlParams[i].dep;
          } else {
            const element = this.urlParams[i];
            froms += this.urlParams[i].from + ',';
            tos += this.urlParams[i].to + ',';
            deps += this.urlParams[i].dep + ',';
          }
        }
        return {type: 'multi', from: froms, to: tos, dep: deps, ret: '',
                minPrice: this.urlParams[this.urlParams.length - 1].minPrice,
                maxPrice: this.urlParams[this.urlParams.length - 1].maxPrice,
                air: this.urlParams[this.urlParams.length - 1].air,
                mind: this.urlParams[this.urlParams.length - 1].mind,
                maxd: this.urlParams[this.urlParams.length - 1].maxd};
      }
    }
  }

  onApplyFilter(value: any) {
    console.log('USAO');
    this.loadAll();
    // window.location.reload();
    this.filter = false;
  }

  toggleFilter() {
    this.filter = !this.filter;
  }

  goBack() {
    if (this.userId === undefined) {
      this.router.navigate(['/']);
    } else {
      this.router.navigate(['/' + this.userId + '/home']);
    }
  }

  onShowModal(value: any) {
    this.showModal = true;
  }

  onModal(value: any){
    if (value) {
      this.router.navigate(['/signin']);
    }
    this.showModal = false;
  }

}
