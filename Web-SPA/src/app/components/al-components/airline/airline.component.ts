import { Component, OnInit, Input } from '@angular/core';
import { AirlineService } from 'src/services/airline.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-airline',
  templateUrl: './airline.component.html',
  styleUrls: ['./airline.component.scss']
})
export class AirlineComponent implements OnInit {

  @Input() data;

  colorsOfArilineDest: Array<string>;
  userId;

  constructor(private airlineService: AirlineService,
              private route: ActivatedRoute,
              private router: Router) {
    this.colorsOfArilineDest = new Array<string>();
    route.params.subscribe(params => {
      this.userId = params.id;
    });
   }

  ngOnInit(): void {
    this.addColors();
  }

  addColors() {
    this.colorsOfArilineDest.push('#998AD3');
    this.colorsOfArilineDest.push('#E494D3');
    this.colorsOfArilineDest.push('#CDF1AF');
    this.colorsOfArilineDest.push('#87DCC0');
    this.colorsOfArilineDest.push('#88BBE4');
  }

  onAirlines() {
    if (this.userId === undefined) {
      this.router.navigate(['/airlines/' + this.data.airlineId + '/airline-info']);
    } else {
      this.router.navigate(['/' + this.userId + '/airlines/' + this.data.airlineId + '/airline-info']);
    }
  }

}
