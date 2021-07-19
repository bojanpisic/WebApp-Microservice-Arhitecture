import { Component, OnInit, Input } from '@angular/core';
import { Destination } from 'src/app/entities/destination';
import { ActivatedRoute } from '@angular/router';
import { AirlineAdmin } from 'src/app/entities/airlineAdmin';
import { UserService } from 'src/services/user.service';
import { AirlineService } from 'src/services/airline.service';
import { Address } from 'src/app/entities/address';
import { DomSanitizer } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-airline-destinations',
  templateUrl: './airline-destinations.component.html',
  styleUrls: ['./airline-destinations.component.scss']
})
export class AirlineDestinationsComponent implements OnInit {

  adminId: number;
  admin: AirlineAdmin;
  destinations: Array<{destinationId: number, imageUrl: any, city: string, state: string}>;
  indexOfPickedDestination: number;
  pickedDestinationAddress: {city: string, state: string};
  showModal = false;

  searchText = '';

  // tslint:disable-next-line:max-line-length
  constructor(private routes: ActivatedRoute,
              private userService: UserService,
              private airlineService: AirlineService,
              private san: DomSanitizer,
              private toastr: ToastrService) {
    routes.params.subscribe(route => {
      this.adminId = route.id;
    });
    this.destinations = [];
  }

  ngOnInit(): void {
    this.loadDestionations();
  }

  loadDestionations() {
    this.destinations = [];
    this.airlineService.getAdminsDestinations().subscribe(
      (res: any[]) => {
        if (res.length) {
          res.forEach(element => {
            const new1 = {
              destinationId: element.destinationId,
              imageUrl: this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${element.imageUrl}`),
              city: element.city,
              state: element.state
            };
            this.destinations.push(new1);
          });
        }
      },
      err => {
        this.toastr.error(err.error, 'Error.');
      }
    );
  }

  // onDelete(index: number) {
  //   this.indexOfPickedDestination = index;
  //   this.pickedDestinationAddress = {city: this.destinations[index].city, state: this.destinations[index].state};
  //   this.showModal = true;
  // }

  // onDeleteDestination(value: boolean) {
  //   if (value) {
  //     const data = {
  //       id: this.destinations.find(x => x.city === this.pickedDestinationAddress.city).destinationId
  //     };
  //     this.airlineService.deleteDestination(data).subscribe(
  //       (res: any) => {
  //         this.destinations = [];
  //         res.forEach(element => {
  //           if (!this.destinations.find(x => x.city === element.city)) {
  //             console.log(element);
  //             const new1 = {
  //               destinationId: element.destinationId,
  //               imageUrl: this.san.bypassSecurityTrustResourceUrl(`data:image/png;base64, ${element.imageUrl}`),
  //               city: element.city,
  //               state: element.state
  //             };
  //             this.destinations.push(new1);
  //           }
  //         });
  //         this.toastr.success('Success!');
  //         console.log('deleted');
  //       },
  //       err => {
  //         this.toastr.error(err.statusText, 'Error.');
  //       }
  //     );
  //   }
  //   this.showModal = false;
  // }

  addDestination(value: any) {
    const obj = JSON.parse(value);
    const address = new Address(obj.city, obj.state, obj.longitude, obj.latitude);
    const destination = new Destination(obj.placePhoto, address);
    // tslint:disable-next-line:prefer-for-of
    for (let i = 0; i < this.destinations.length; i++) {
      if (this.destinations[i].city === obj.city && this.destinations[i].state === obj.state) {
        return;
      }
    }
    const data = {
      state: obj.state,
      city: obj.city,
      imgUrl: obj.placePhoto,
    };
    this.airlineService.addDestination(data).subscribe(
      (res: any) => {
        this.loadDestionations();
      },
      err => {
        this.toastr.error(err.error, 'Error.');
      }
    );
  }

}
