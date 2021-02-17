import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CarRentService } from 'src/services/car-rent.service';
import {Chart} from 'chart.js';

@Component({
  selector: 'app-rac-stats',
  templateUrl: './rac-stats.component.html',
  styleUrls: ['./rac-stats.component.scss']
})
export class RacStatsComponent implements OnInit {

  adminId;
  types: Array<any>;
  chartTypes: Array<any>;
  dropdown = false;
  dropdownChart = false;
  pickedType;
  pickedTypeChart;
  from = '2020-08-30';
  to = '2020-10-30';
  pickedYear = '2020';
  pickedWeek = '2020-W35';
  pickedMonth = '2020-09';
  cars: Array<any>;
  labels; dataset; myChart;
  noCars = false;

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
              private carService: CarRentService,
              private toastr: ToastrService) {
    routes.params.subscribe(route => {
      this.adminId = route.id;
    });
    this.cars = new Array<any>();
    this.types = new Array<any>();
    this.chartTypes = new Array<any>();
    this.types.push({
      type: 'free', displayedType: 'Free vehicles'
    });
    this.types.push({
      type: 'rented', displayedType: 'Rented vehicles'
    });
    this.pickedType = this.types[0];
    this.chartTypes.push({
      type: 'week', displayedType: 'By week'
    });
    this.chartTypes.push({
      type: 'month', displayedType: 'By month'
    });
    this.chartTypes.push({
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
    this.pickedTypeChart = this.chartTypes[0];
    this.pickedTypeTickets = this.typesTickets[0];
    this.labels = new Array<any>();
    this.labelsTickets = new Array<any>();
    this.dataset = new Array<any>();
    this.datasetTickets = new Array<any>();
  }

  ngOnInit(): void {
    this.getValues();
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
          label: this.pickedTypeChart.type === 'year' ? this.pickedYear :
                this.pickedTypeChart.type === 'week' ? this.pickedWeek :
                this.pickedMonth,
          borderColor: 'rgba(75, 192, 192, 1)',
          backgroundColor: 'rgba(75, 192, 192, 0.2)',
        }
        ]
      },
      options: {
        title: {
          display: true,
          text: this.pickedTypeChart.type === 'year' ? 'Income for  ' + this.pickedYear :
                this.pickedTypeChart.type === 'week' ? 'Income for  ' + this.pickedWeek :
                'Income for  ' + this.pickedMonth,
        }
      }
    });
  }

  onYearSelected() {
    const data = {
      year: this.pickedYear
    };
    const a = this.carService.getStatsForYear(data).subscribe(
      (res: any) => {
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

  onWeekSelected() {
    const data = {
      week: this.pickedWeek
    };
    const a = this.carService.getStatsForWeek(data).subscribe(
      (res: any) => {
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

  onMonthSelected() {
    const data = {
      month: this.pickedMonth
    };
    const a = this.carService.getStatsForMonth(data).subscribe(
      (res: any) => {
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

  setChartType(value: any) {
    this.labels = [];
    this.dataset = [];
    if (value.type === 'year' && this.pickedYear !== null) {
      this.onYearSelected();
    } else if (value.type === 'week') {
      this.onWeekSelected();
    } else {
      this.onMonthSelected();
    }
    this.pickedTypeChart = value;
  }

  toggleChartDropDown() {
    this.dropdownChart = !this.dropdownChart;
  }

  getValues() {
    const data = {
      from: this.from,
      to: this.to,
      isFree: (this.pickedType.type === 'free' ? true : false)
    };
    console.log(data);
    const a = this.carService.getStats(data).subscribe(
      (res: any) => {
        console.log(res);
        if (res.length === 0) {
          this.noCars = true;
        }
        if (res.length > 0) {
          res.forEach(el => {
            const r = {
              brand: el.brand,
              carId: el.carId,
              city: el.city,
              model: el.model,
              name: el.name,
              pricePerDay: el.pricePerDay,
              seatsNumber: el.seatsNumber,
              state: el.state,
              type: el.type,
              year: el.year,
              rate: el.rate
            };
            this.cars.push(r);
          });
      }
    },
    err => {
      this.toastr.error(err.statusText, 'Error.');
    });
  }

  onExit() {
    this.router.navigate(['/rac-admin/' + this.adminId]);
  }

  setType(value: any) {
    this.pickedType = value;
    this.getValues();
  }

  toggleDropDown() {
    this.dropdown = !this.dropdown;
  }

  onSearch() {
    this.cars = [];
    this.getValues();
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
    const a = this.carService.getReservationStatsForDate(data).subscribe(
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
    const a = this.carService.getReservationStatsForWeek(data).subscribe(
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
    const a = this.carService.getReservationStatsForMonth(data).subscribe(
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
