import { Component, OnInit } from '@angular/core';
import { Airline } from 'src/app/entities/airline';
import { AirlineService } from 'src/services/airline.service';
import { UserService } from 'src/services/user.service';
import { ActivatedRoute } from '@angular/router';
import { AirlineAdmin } from 'src/app/entities/airlineAdmin';
import { Destination } from 'src/app/entities/destination';

@Component({
  selector: 'app-admin-home',
  templateUrl: './admin-home.component.html',
  styleUrls: ['./admin-home.component.scss']
})
export class AdminHomeComponent implements OnInit {

  airline: Airline;
  admin: AirlineAdmin;
  adminId: string;
  home = true;
  isOk = false;

  constructor(private airlineService: AirlineService, private userService: UserService, private routes: ActivatedRoute) {
    routes.params.subscribe(route => {
      this.adminId = route.id;
    });
    this.isOk = true;
    console.log(this.adminId);
  }

  ngOnInit(): void {
    // this.airline = this.airlineService.getAirline(0);
    // this.admin = this.userService.getAirlineAdmin(this.airline.adminid);
    // document.getElementById('ul').scrollLeft = document.getElementById('li-flights').offsetLeft;
  }

}
