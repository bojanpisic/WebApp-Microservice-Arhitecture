import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { Airline } from 'src/app/entities/airline';
import { AirlineService } from 'src/services/airline.service';
import { Location } from '@angular/common';
import { Router, NavigationExtras, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-filter',
  templateUrl: './filter.component.html',
  styleUrls: ['./filter.component.scss']
})
export class FilterComponent implements OnInit {

  slidingMinPriceValue: number;
  slidingMaxPriceValue: number;
  minHoursDuration = 0;
  minMinutesDuration = 0;
  maxHoursDuration = 40;
  maxMinutesDuration = 0;

  airlinesButton: string;
  allAirlines: Array<any>;
  checkedAirlines: Array<boolean>;

  fromString: string;
  toString: string;
  depString: string;

  urlParams: any;
  userId: number;
  url: any;
  allAirlinesBool = false;
  @Output() closeFilter = new EventEmitter<boolean>();
  @Output() appliedFilters = new EventEmitter<any>();

  constructor(private airlineService: AirlineService, private location: Location,
              private router: Router, private route: ActivatedRoute,
              private toastr: ToastrService) {
    const array = route.snapshot.queryParamMap.get('array');
    this.urlParams = JSON.parse(array);
    this.route.params.subscribe(param => {
      this.userId = param.id;
    });

    this.slidingMinPriceValue = 0;
    this.slidingMaxPriceValue = 100;
    this.allAirlines = new Array<Airline>();
    this.checkedAirlines = new Array<boolean>();
  }

  ngOnInit(): void {
    let data;
    data = this.generateFilter();

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

    this.slidingMinPriceValue = (this.url.minPrice === 0) ? 0 : this.url.minPrice / 30;
    this.slidingMaxPriceValue = (this.url.maxPrice === 0) ? 0 : this.url.maxPrice / 30;
    this.minHoursDuration = this.url.mind.split('h')[0];
    this.minMinutesDuration = this.url.mind.split(' ')[1].split('min')[0];
    this.maxHoursDuration = this.url.maxd.split('h')[0];
    this.maxMinutesDuration = this.url.maxd.split(' ')[1].split('min')[0];
    this.allAirlinesBool = (this.url.air === '') ? true : false;

    console.log(this.url);
    console.log(this.slidingMinPriceValue);
    console.log(this.slidingMaxPriceValue);
    console.log(this.minHoursDuration);
    console.log(this.minMinutesDuration);
    console.log(this.maxHoursDuration);
    console.log(this.maxMinutesDuration);

    this.loadAirlines();
  }

  generateFilter() {
    if (this.urlParams === null && this.userId === undefined) {
      this.router.navigate(['']);
      return;
    }
    if (this.urlParams === null && this.userId !== undefined) {
      this.router.navigate(['/' + this.userId + '/home']);
      return;
    }
    if (this.urlParams !== null) {
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

  getLeftStylePrice() {
    return this.slidingMinPriceValue + '%';
  }

  getRightStylePrice() {
    return (100 - +this.slidingMaxPriceValue).toString() + '%';
  }

  leftValuePrice() {
    return Math.min(+this.slidingMinPriceValue, +this.slidingMaxPriceValue) - 1;
  }

  rightValuePrice() {
    return Math.max(+this.slidingMinPriceValue, +this.slidingMaxPriceValue) + 1;
  }

  allAirlinesButton() {
    this.checkedAirlines.forEach((v, i, a) => a[i] = true);
  }

  loadAirlines() {
    if (this.allAirlinesBool) {
      const a = this.airlineService.getAirlines().subscribe(
        (res: any[]) => {
          if (res.length > 0) {
            res.forEach(element => {
              const airline = {
                airlineId: element.airlineId,
                name: element.name,
              };
              this.allAirlines.push(airline);
              this.checkedAirlines.push(true);
              console.log(airline);
            });
          }
        },
        err => {
          this.toastr.error(err.statusText, 'Error.');
        }
      );
    } else {
      const a = this.airlineService.getAirlines().subscribe(
        (res: any[]) => {
          if (res.length > 0) {
            res.forEach(element => {
              const airline = {
                airlineId: element.airlineId,
                name: element.name,
              };
              this.allAirlines.push(airline);
              if (this.url.air.split(',').includes(airline.airlineId.toString())) {
                this.checkedAirlines.push(true);
              } else {
                this.checkedAirlines.push(false);
              }
            });
            console.log('AAAAAAAA', this.checkedAirlines);
          }
        },
        err => {
          this.toastr.error(err.statusText, 'Error.');
        }
      );
    }
  }

  toggleAirlineCheckBox(index: number) {
    this.checkedAirlines[index] = !this.checkedAirlines[index];
  }

  goBack() {
    this.closeFilter.emit(true);
  }

  onApplyFilters() {
    let airIds = '';
    const airlines = this.getIdsOfCheckedAirlines();
    for (let i = 0; i < airlines.length; i++) {
      const element = airlines[i];
      if (i === airlines.length - 1) {
        airIds += element;
      } else {
        airIds += element + ',';
      }
    }

    const queryParams: any = {};
    const array = [];
    if (this.url.type === 'one') {
      array.push({type: 'one'});
      array.push({from: this.url.from, to: this.url.to, dep: this.url.dep});
    }
    if (this.url.type === 'two') {
      array.push({type: 'two'});
      array.push({from: this.url.from, to: this.url.to,
                  dep: this.url.dep, ret: this.url.ret});
    }
    if (this.url.type === 'multi') {
      array.push({type: 'multi'});
      this.fromString = this.url.from;
      this.toString = this.url.to;
      this.depString = this.url.dep;
      const fromSplitted = this.fromString.split(',');
      const toSplitted = this.toString.split(',');
      const depSplitted = this.depString.split(',');
      for (let i = 0; i < fromSplitted.length - 1; i++) {
        const element = fromSplitted[i];
        array.push({from: element, to: toSplitted[i], dep: depSplitted[i]});
      }
    }

    array.push({
      minPrice: this.slidingMinPriceValue * 30,
      maxPrice: this.slidingMaxPriceValue * 30,
      mind: this.minHoursDuration + 'h ' + this.minMinutesDuration + 'min',
      maxd: this.maxHoursDuration + 'h ' + this.maxMinutesDuration + 'min',
      air: airIds
    });

    queryParams.array = JSON.stringify(array);

    const navigationExtras: NavigationExtras = {
      queryParams
    };
    if (this.userId !== undefined) {
      this.router.navigate(['/' + this.userId + '/trips'], navigationExtras);
    } else {
      this.router.navigate(['/trips'], navigationExtras);
    }

    console.log(array);

    this.appliedFilters.emit(true);
  }

  getIdsOfCheckedAirlines() {
    const ids = [];
    for (let i = 0; i < this.checkedAirlines.length; i++) {
      const element = this.checkedAirlines[i];
      if (element) {
        ids.push(this.allAirlines[i].airlineId);
      }
    }
    return ids;
  }
}
