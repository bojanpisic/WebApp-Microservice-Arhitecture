import { Component, OnInit } from '@angular/core';
import { RentACarService } from 'src/app/entities/rent-a-car-service';
import { CarRentService } from 'src/services/car-rent.service';
import { Location } from '@angular/common';
import { DomSanitizer } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-rent-a-car-services',
  templateUrl: './rent-a-car-services.component.html',
  styleUrls: ['./rent-a-car-services.component.scss', '../../al-components/airlines/airlines.component.scss']
})
export class RentACarServicesComponent implements OnInit {

  allRentServices: Array<{
    racId: number,
    city: string,
    state: string,
    name: string,
    logo: any,
    about: string,
    branches: Array<{
      city: string,
      state: string
    }>
  }>;
  colorsOfBranches: Array<string>;
  rotateArrow = false;

  nameup = false;
  namedown = false;
  cityup = false;
  citydown = false;
  scrolledY: number;

  constructor(private service: CarRentService, private san: DomSanitizer, private location: Location,
              private toastr: ToastrService) {
    this.allRentServices = [];
    this.colorsOfBranches = new Array<string>();
   }

  ngOnInit(): void {
    this.loadRACs();
    this.addColors();
  }

  loadRACs() {
    const a = this.service.getRACs().subscribe(
      (res: any[]) => {
        console.log(res);
        if (res.length > 0) {
          res.forEach(element => {
            const rac = {
              racId: element.id,
              city: element.city,
              state: element.state,
              name: element.name,
              logo: (element.logo === null) ? null : this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${element.logo}`),
              about: element.about,
              branches: element.branches,
              rate: element.rate
            };
            this.allRentServices.push(rac);
          });
        }
      },
      err => {
        this.toastr.error(err.error, 'Error!');
        console.log(err);
      }
    );
  }


  goBack() {
    this.location.back();
  }

  addColors() {
    this.colorsOfBranches.push('#998AD3');
    this.colorsOfBranches.push('#E494D3');
    this.colorsOfBranches.push('#CDF1AF');
    this.colorsOfBranches.push('#87DCC0');
    this.colorsOfBranches.push('#88BBE4');
  }


  applySort(sortList: Array<boolean>) {
    this.namedown = sortList[0];
    this.nameup = sortList[1];
    this.citydown = sortList[2];
    this.cityup = sortList[3];
    this.sortBy();
  }

  sortBy() {
    if (this.namedown) {
      if (!this.cityup && !this.citydown) {
        this.allRentServices.sort((a, b) => (a.name > b.name) ? 1 : -1);
      } else if (this.citydown) {
        this.allRentServices.sort((a, b) => (a.name > b.name) ? 1 : (a.name === b.name) ? ((a.city > b.city) ? 1 : -1) : -1);
      } else {
        this.allRentServices.sort((a, b) => (a.name > b.name) ? 1 : (a.name === b.name) ? ((a.city < b.city) ? 1 : -1) : -1);
      }
    } else {
      if (this.nameup) {
        if (!this.cityup && !this.citydown) {
          this.allRentServices.sort((a, b) => (a.name < b.name) ? 1 : -1);
        } else if (this.cityup) {
          this.allRentServices.sort((a, b) => (a.name < b.name) ? 1 : (a.name === b.name) ? ((a.city < b.city) ? 1 : -1) : -1);
        } else {
          this.allRentServices.sort((a, b) => (a.name < b.name) ? 1 : (a.name === b.name) ? ((a.city > b.city) ? 1 : -1) : -1);
        }
      }
    }
  }
}
