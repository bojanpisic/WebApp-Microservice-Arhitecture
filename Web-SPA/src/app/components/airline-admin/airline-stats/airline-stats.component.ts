import { Component, OnInit } from '@angular/core';
import {Chart} from 'chart.js';
import { Router, ActivatedRoute } from '@angular/router';
import { AirlineService } from 'src/services/airline.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-airline-stats',
  templateUrl: './airline-stats.component.html',
  styleUrls: ['./airline-stats.component.scss']
})
export class AirlineStatsComponent implements OnInit {

  myChart;
  adminId;
  types: Array<any>;
  dropdown = false;
  pickedType;
  pickedWeek = '2020-W35';
  pickedMonth = '2020-09';
  pickedYear = '2020';
  labels: Array<any>;
  dataset: Array<any>;

  myChartTickets;
  typesTickets: Array<any>;
  dropdownTickets = false;
  pickedTypeTickets;
  pickedDateTickets = '2020-08-30';
  pickedWeekTickets = '2020-W35';
  pickedMonthTickets = '2020-09';
  labelsTickets: Array<any>;
  datasetTickets: Array<any>;

  constructor(private router: Router,
              private routes: ActivatedRoute,
              private airlineService: AirlineService,
              private toastr: ToastrService) {
    routes.params.subscribe(route => {
      this.adminId = route.id;
    });
    this.types = new Array<any>();
    this.types.push({
      type: 'week', displayedType: 'By week'
    });
    this.types.push({
      type: 'month', displayedType: 'By month'
    });
    this.types.push({
      type: 'year', displayedType: 'By year'
    });
    this.typesTickets = new Array<any>();
    this.typesTickets.push({
      type: 'date', displayedType: 'By date'
    });
    this.typesTickets.push({
      type: 'week', displayedType: 'By week'
    });
    this.typesTickets.push({
      type: 'month', displayedType: 'By month'
    });
    this.pickedType = this.types[0];
    this.pickedTypeTickets = this.typesTickets[0];
    this.labels = new Array<any>();
    this.labelsTickets = new Array<any>();
    this.dataset = new Array<any>();
    this.datasetTickets = new Array<any>();
  }

  ngOnInit(): void {
    this.onWeekSelected();
    this.onDateTicketsSelected();
    this.updateChart();
    this.updateChartTickets();
  }

  updateChart() {
    this.myChart = new Chart('myChart', {
      type: 'line',
      data: {
        labels: this.labels,
        datasets: [{
          data: this.dataset,
          label: this.pickedType.type === 'week' ? this.pickedWeek :
                this.pickedType.type === 'month' ? this.pickedMonth :
                this.pickedYear,
          borderColor: 'rgba(75, 192, 192, 1)',
          backgroundColor: 'rgba(75, 192, 192, 0.2)',
        }
        ]
      },
      options: {
        title: {
          display: true,
          text: this.pickedType.type === 'week' ? 'Income for  ' + this.pickedWeek :
                this.pickedType.type === 'month' ? 'Income for  ' + this.pickedMonth :
                'Income for  ' + this.pickedYear,
        }
      }
    });
  }

  onYearSelected() {
    const data = {
      year: this.pickedYear
    };
    const a = this.airlineService.getStatsForYear(data).subscribe(
      (res: any) => {
        console.log(res);
        res.forEach(element => {
          this.labels.push(element.item1);
          this.dataset.push(element.item2);
        });
        this.updateChart();
        // tslint:disable-next-line:forin
        // for (const key in res.days) {
        //   this.labels.push(key);
        // }
        // // tslint:disable-next-line:forin
        // for (const key in res.tickets) {
        //   this.dataset.push(key);
        // }
      },
      err => {
        this.toastr.error(err.statusText, 'Error.');
      }
    );
  }

  onWeekSelected() {
    const data = {
      week: this.pickedWeek
    };
    const a = this.airlineService.getStatsForWeek(data).subscribe(
      (res: any) => {
        console.log(res);
        res.forEach(element => {
          this.labels.push(element.item1.split('T')[0]);
          this.dataset.push(element.item2);
        });
        console.log(this.labels);
        console.log(this.dataset);
        this.updateChart();
      },
      err => {
        this.toastr.error(err.statusText, 'Error.');
      }
    );
  }

  onMonthSelected() {
    const data = {
      month: this.pickedMonth
    };
    const a = this.airlineService.getStatsForMonth(data).subscribe(
      (res: any) => {
        console.log(res);
        res.forEach(element => {
          this.labels.push(element.item1.split('T')[0]);
          this.dataset.push(element.item2);
        });
        this.updateChart();
      },
      err => {
        this.toastr.error(err.statusText, 'Error.');
      }
    );
  }

  onExit() {
    this.router.navigate(['/admin/' + this.adminId]);
  }

  setType(value: any) {
    this.labels = [];
    this.dataset = [];
    console.log(this.pickedYear);
    if (value.type === 'year' && this.pickedYear !== null) {
      this.onYearSelected();
    } else if (value.type === 'week') {
      this.onWeekSelected();
    } else {
      this.onMonthSelected();
    }
    this.pickedType = value;
  }

  toggleDropDown() {
    this.dropdown = !this.dropdown;
  }

  updateChartTickets() {
    this.myChartTickets = new Chart('myChartTickets', {
      type: 'line',
      data: {
        labels: this.labelsTickets,
        datasets: [{
          data: this.datasetTickets,
          label: this.pickedTypeTickets.type === 'date' ? this.pickedDateTickets :
                this.pickedTypeTickets.type === 'week' ? this.pickedWeekTickets :
                this.pickedMonthTickets,
          borderColor: 'rgba(75, 192, 192, 1)',
          backgroundColor: 'rgba(75, 192, 192, 0.2)',
        }]
      },
      options: {
        title: {
          display: true,
          text: this.pickedTypeTickets.type === 'date' ? 'Income for  ' + this.pickedDateTickets :
                this.pickedTypeTickets.type === 'week' ? 'Income for  ' + this.pickedWeekTickets :
                'Income for  ' + this.pickedMonthTickets,
        }
      }
    });
  }

  onDateTicketsSelected() {
    const data = {
      date: this.pickedDateTickets
    };
    const a = this.airlineService.getTicketStatsForDate(data).subscribe(
      (res: any) => {
        this.labelsTickets.push(res.item1.split('T')[0]);
        this.datasetTickets.push(res.item2);
        this.updateChartTickets();
      },
      err => {
        this.toastr.error(err.statusText, 'Error.');
      }
    );
  }

  onWeekTicketsSelected() {
    const data = {
      week: this.pickedWeekTickets
    };
    const a = this.airlineService.getTicketStatsForWeek(data).subscribe(
      (res: any) => {
        res.forEach(element => {
          this.labelsTickets.push(element.item1.split('T')[0]);
          this.datasetTickets.push(element.item2);
        });
        this.updateChartTickets();
      },
      err => {
        this.toastr.error(err.statusText, 'Error.');
      }
    );
  }

  onMonthTicketsSelected() {
    const data = {
      month: this.pickedMonthTickets
    };
    const a = this.airlineService.getTicketStatsForMonth(data).subscribe(
      (res: any) => {
        console.log(res);
        res.forEach(element => {
          this.labelsTickets.push(element.item1.split('T')[0]);
          this.datasetTickets.push(element.item2);
        });
        this.updateChartTickets();
      },
      err => {
        this.toastr.error(err.statusText, 'Error.');
      }
    );
  }

  setTypeTickets(value: any) {
    this.labelsTickets = [];
    this.datasetTickets = [];
    if (value.type === 'date') {
      this.onDateTicketsSelected();
    } else if (value.type === 'week') {
      this.onWeekTicketsSelected();
    } else {
      this.onMonthTicketsSelected();
    }
    this.pickedTypeTickets = value;
  }

  toggleDropDownTickets() {
    this.dropdownTickets = !this.dropdownTickets;
  }
}
