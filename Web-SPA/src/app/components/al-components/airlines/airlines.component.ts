import { Component, OnInit, Input } from '@angular/core';
import { Airline } from 'src/app/entities/airline';
import { AirlineService } from 'src/services/airline.service';
import { Location } from '@angular/common';
import { DomSanitizer } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-airlines',
  templateUrl: './airlines.component.html',
  styleUrls: ['./airlines.component.scss']
})
export class AirlinesComponent implements OnInit {
  allAirlines: Array<{
    airlineId: number,
    city: string,
    state: string,
    name: string,
    logo: any,
    about: string,
    destinations: Array<{
      city: string,
      state: string
    }>
  }>;
  colorsOfArilineDest: Array<string>;
  rotateArrow = false;
  nameup = false;
  namedown = false;
  cityup = false;
  citydown = false;
  scrolledY: number;

  constructor(private airlineService: AirlineService,
              private location: Location,
              private san: DomSanitizer,
              private toastr: ToastrService) {
    this.allAirlines = [];
    this.colorsOfArilineDest = new Array<string>();
  }

  ngOnInit(): void {
    this.loadAirlines();
    this.addColors();
  }

  goBack() {
    this.location.back();
  }

  loadAirlines() {
    const a = this.airlineService.getAirlines().subscribe(
      (res: any[]) => {
        if (res.length > 0) {
          console.log('AIRLINES' + res);
          res.forEach(element => {
            console.log(element);
            const airline = {
              airlineId: element.airlineId,
              city: element.city,
              state: element.state,
              name: element.name,
              logo: (element.logo === null) ? null : this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${element.logo}`),
              about: element.about,
              destinations: element.destinations,
              rate: element.rate
            };
            this.allAirlines.push(airline);
          });
        }
      },
      err => {
        this.toastr.error(err.statusText, 'Error!');
      }
    );
  }

  addColors() {
    this.colorsOfArilineDest.push('#998AD3');
    this.colorsOfArilineDest.push('#E494D3');
    this.colorsOfArilineDest.push('#CDF1AF');
    this.colorsOfArilineDest.push('#87DCC0');
    this.colorsOfArilineDest.push('#88BBE4');
  }

  applySort(sortList: Array<boolean>) {
    this.namedown = sortList[0];
    this.nameup = sortList[1];
    this.citydown = sortList[2];
    this.cityup = sortList[3];
    this.sortBy();
  }

  sortBy() {
    if (this.namedown && this.citydown) {
      // tslint:disable-next-line:max-line-length
      this.allAirlines.sort((a, b) => (a.name.toUpperCase() > b.name.toUpperCase()) ? 1 : (a.name.toUpperCase() === b.name.toUpperCase()) ? ((a.city.toUpperCase() > b.city.toUpperCase()) ? 1 : -1) : -1);
    }
    if (this.namedown && this.cityup) {
      // tslint:disable-next-line:max-line-length
      this.allAirlines.sort((a, b) => (a.name.toUpperCase() > b.name.toUpperCase()) ? 1 : (a.name.toUpperCase() === b.name.toUpperCase()) ? ((a.city.toUpperCase() < b.city.toUpperCase()) ? 1 : -1) : -1);
    }
    if (this.namedown && !this.citydown && !this.cityup) {
      this.allAirlines.sort((a, b) => (a.name.toUpperCase() > b.name.toUpperCase()) ? 1 : -1);
    }
    if (this.nameup && this.citydown) {
      // tslint:disable-next-line:max-line-length
      this.allAirlines.sort((a, b) => (a.name.toUpperCase() < b.name.toUpperCase()) ? 1 : (a.name.toUpperCase() === b.name.toUpperCase()) ? ((a.city.toUpperCase() > b.city.toUpperCase()) ? 1 : -1) : -1);
    }
    if (this.nameup && this.cityup) {
      // tslint:disable-next-line:max-line-length
      this.allAirlines.sort((a, b) => (a.name.toUpperCase() < b.name.toUpperCase()) ? 1 : (a.name.toUpperCase() === b.name.toUpperCase()) ? ((a.city.toUpperCase() < b.city.toUpperCase()) ? 1 : -1) : -1);
    }
    if (this.nameup && !this.citydown && !this.cityup) {
      this.allAirlines.sort((a, b) => (a.name.toUpperCase() < b.name.toUpperCase()) ? 1 : -1);
    }
    if (this.citydown && !this.namedown && !this.nameup) {
      this.allAirlines.sort((a, b) => (a.city.toUpperCase() > b.city.toUpperCase()) ? 1 : -1);
    }
    if (this.cityup && !this.namedown && !this.nameup) {
      this.allAirlines.sort((a, b) => (a.city.toUpperCase() < b.city.toUpperCase()) ? 1 : -1);
    }

    // if (this.namedown) {
    //   if (!this.cityup && !this.citydown) {
    //     this.allAirlines.sort((a, b) => (a.name.toUpperCase() > b.name.toUpperCase()) ? 1 : -1);
    //   } else if (this.citydown) {
    //     // tslint:disable-next-line:max-line-length
    //     this.allAirlines.sort((a, b) => (a.name.toUpperCase() > b.name.toUpperCase()) ? 1 : (a.name.toUpperCase() === b.name.toUpperCase()) ? ((a.city.toUpperCase() > b.city.toUpperCase()) ? 1 : -1) : -1);
    //   } else {
    //     // tslint:disable-next-line:max-line-length
    //     this.allAirlines.sort((a, b) => (a.name.toUpperCase() > b.name.toUpperCase()) ? 1 : (a.name.toUpperCase() === b.name.toUpperCase()) ? ((a.city.toUpperCase() < b.city.toUpperCase()) ? 1 : -1) : -1);
    //   }
    // } else {
    //   if (this.nameup) {
    //     if (!this.cityup && !this.citydown) {
    //       this.allAirlines.sort((a, b) => (a.name.toUpperCase() < b.name.toUpperCase()) ? 1 : -1);
    //     } else if (this.cityup) {
    //       // tslint:disable-next-line:max-line-length
    //       this.allAirlines.sort((a, b) => (a.name.toUpperCase() < b.name.toUpperCase()) ? 1 : (a.name.toUpperCase() === b.name.toUpperCase()) ? ((a.city.toUpperCase() < b.city.toUpperCase()) ? 1 : -1) : -1);
    //     } else {
    //       // tslint:disable-next-line:max-line-length
    //       this.allAirlines.sort((a, b) => (a.name.toUpperCase() < b.name.toUpperCase()) ? 1 : (a.name.toUpperCase() === b.name.toUpperCase()) ? ((a.city.toUpperCase() > b.city.toUpperCase()) ? 1 : -1) : -1);
    //     }
    //   }
    // }
  }
}
