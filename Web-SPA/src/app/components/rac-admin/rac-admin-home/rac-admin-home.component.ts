import { Component, OnInit } from '@angular/core';
import { RentACarService } from 'src/app/entities/rent-a-car-service';
import { ActivatedRoute } from '@angular/router';
import { RacAdmin } from 'src/app/entities/racAdmin';
import { UserService } from 'src/services/user.service';

@Component({
  selector: 'app-rac-admin-home',
  templateUrl: './rac-admin-home.component.html',
  styleUrls: ['./rac-admin-home.component.scss']
})
export class RacAdminHomeComponent implements OnInit {

  rac: RentACarService;
  admin: RacAdmin;
  adminId: number;
  home = true;

  constructor(private userService: UserService, private routes: ActivatedRoute) {
    routes.params.subscribe(route => {
      this.adminId = route.id;
    });
  }

  ngOnInit(): void {
    // this.admin = this.userService.getRACAdmin(this.rac.adminId);
  }

}
