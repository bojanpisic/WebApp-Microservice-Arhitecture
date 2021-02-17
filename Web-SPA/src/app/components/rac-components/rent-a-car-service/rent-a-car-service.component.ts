import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-rent-a-car-service',
  templateUrl: './rent-a-car-service.component.html',
  styleUrls: ['./rent-a-car-service.component.scss', '../../al-components/airline/airline.component.scss']
})
export class RentACarServiceComponent implements OnInit {

  @Input() data;

  colorsOfBranches: Array<string>;
  userId;

  constructor(private route: ActivatedRoute, private router: Router) {
    this.colorsOfBranches = new Array<string>();
    route.params.subscribe(params => {
      this.userId = params.id;
    });
  }

  ngOnInit(): void {
    this.addColors();
  }

  addColors() {
    this.colorsOfBranches.push('#998AD3');
    this.colorsOfBranches.push('#E494D3');
    this.colorsOfBranches.push('#CDF1AF');
    this.colorsOfBranches.push('#87DCC0');
    this.colorsOfBranches.push('#88BBE4');
  }

  onRAC() {
    if (this.userId === undefined) {
      this.router.navigate(['/rent-a-car-services/' + this.data.racId + '/rent-a-car-service-info']);
    } else {
      this.router.navigate(['/' + this.userId + '/rent-a-car-services/' + this.data.racId + '/rent-a-car-service-info']);
    }
  }

}
