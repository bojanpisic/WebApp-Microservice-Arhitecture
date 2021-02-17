import { Component, OnInit, HostListener } from '@angular/core';
import { Airline } from '../../entities/airline';
import { RouterLinkActive, ActivatedRoute, Router } from '@angular/router';
import { User } from 'src/app/entities/user';
import { UserService } from 'src/services/user.service';
// import {Chart} from 'chart.js';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  option = 'fly';
  userId: any;

  constructor(private route: ActivatedRoute, private userService: UserService, private router: Router) {
    route.params.subscribe(params => {
      this.userId = params.id;
    });
  }

  ngOnInit(): void {
  }

  onFly() {
    this.option = 'fly';
  }

  onDrive() {
    this.option = 'drive';
  }

  onSpecialOffers() {
    if (this.option === 'fly') {
      if (this.userId === undefined) {
        this.router.navigate(['/flight-special-offers']);
      } else {
        this.router.navigate(['/' + this.userId + '/flight-special-offers']);
      }
    } else {
      if (this.userId === undefined) {
        this.router.navigate(['/car-special-offers']);
      } else {
        this.router.navigate(['/' + this.userId + '/car-special-offers']);
      }
    }
  }

  onRedirect(value: any) {
    window.scroll(0, 0);
    this.option = value;
  }

}
