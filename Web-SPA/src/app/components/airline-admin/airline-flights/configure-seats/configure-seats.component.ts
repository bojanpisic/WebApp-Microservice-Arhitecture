import { Component, OnInit } from '@angular/core';
import { Flight } from 'src/app/entities/flight';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { Seat } from 'src/app/entities/seat';
import { AirlineService } from 'src/services/airline.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-configure-seats',
  templateUrl: './configure-seats.component.html',
  styleUrls: ['./configure-seats.component.scss']
})
export class ConfigureSeatsComponent implements OnInit {

  seats: Array<{column: string, row: string, class: string, price: number, seatId: number}>;
  adminId: any;
  flightId: any;

  pickedSeat: {
    column: string,
    row: string,
    class: string,
    price: number,
    seatId: number
  };
  showModify = false;
  blur = false;

  firstClassSeats: Array<any>;
  businessSeats: Array<any>;
  economySeats: Array<any>;
  basicEconomySeats: Array<any>;

  firstClassPriceRange: {minPrice: number, maxPrice: number};
  businessPriceRange: {minPrice: number, maxPrice: number};
  economyPriceRange: {minPrice: number, maxPrice: number};
  basicEconomyPriceRange: {minPrice: number, maxPrice: number};

  start = false;
  itsOk = false;

  constructor(private airlineService: AirlineService,
              private route: ActivatedRoute,
              private router: Router,
              private toastr: ToastrService) {
    route.params.subscribe(param => {
      this.flightId = param.flight;
      this.adminId = param.id;
    });

    this.seats = [];

    this.firstClassSeats = new Array<any>();
    this.businessSeats = new Array<any>();
    this.economySeats = new Array<any>();
    this.basicEconomySeats = new Array<any>();
  }

  ngOnInit(): void {
    window.scroll(0, 0);
    this.loadAll();
  }

  loadAll() {
    this.seats = [];
    this.firstClassSeats = [];
    this.businessSeats = [];
    this.economySeats = [];
    this.basicEconomySeats = [];
    const air1 = this.airlineService.getFlightsSeats(this.flightId).subscribe(
      (res: any[]) => {
        if (res.length) {
          res.forEach(element => {
            const new1 = {
              column: element.column,
              row: element.row,
              class: element.class,
              price: element.price,
              seatId: element.seatId
            };
            this.seats.push(new1);
          });
          this.setEconomySeats();
          this.setFirstClassSeats();
          this.setBusinessSeats();
          this.setBasicEconomySeats();
          this.itsOk = true;
        }
      },
      err => {
        this.toastr.error(err.statusText, 'Error.');
      }
    );
  }

  addSeat(index?: number, side?: string, classParam?: string) {
    const row = ((index + 1) % 3 === 0) ? (index + 1) / 3 : Math.floor((index + 1) / 3) + 1;
    let column = '';
    if (side === 'left') {
      column = ((index + 1) % 3 === 1) ? 'A' : ((index + 1) % 3 === 2) ? 'B' : 'C';
    } else {
      column = ((index + 1) % 3 === 1) ? 'D' : ((index + 1) % 3 === 2) ? 'E' : 'F';
    }
    console.log(row, column, classParam);
    let previousSeat = this.findPreviosSeat(row, column, classParam);
    if (previousSeat === 'NEMA') {
      previousSeat = {price: 100};
    }
    console.log(previousSeat);
    const data = {
      // tslint:disable-next-line:object-literal-shorthand
      column: column,
      // tslint:disable-next-line:object-literal-shorthand
      row: row,
      class: classParam,
      price: previousSeat.price,
      flightId: this.flightId
    };
    this.airlineService.addSeat(data).subscribe(
      (res: any) => {
        this.loadAll();
      },
      err => {
        this.toastr.error(err.statusText, 'Error.');
      }
    );
  }

  findPreviosSeat(row: number, column: string, classParam: string) {
    let indexOfSeat = ((row - 1) * 6) + ((column === 'A') ? 0 : (column === 'B') ? 1 : (column === 'C') ? 2 :
                        (column === 'D') ? 3 : (column === 'E') ? 4 : 5);
    if (indexOfSeat === 0) {
      while (indexOfSeat >= 0) {
        const seat = (classParam === 'F') ? this.firstClassSeats[indexOfSeat] : (classParam === 'B') ? this.businessSeats[indexOfSeat] :
                     (classParam === 'E') ? this.economySeats[indexOfSeat] : this.basicEconomySeats[indexOfSeat];
        console.log(seat);
        if (seat !== 'left' && seat !== 'right') {
          return seat;
        }
        indexOfSeat++;
      }
      return 'NEMA';
    }
    while (indexOfSeat >= 0) {
      const seat = (classParam === 'F') ? this.firstClassSeats[indexOfSeat] : (classParam === 'B') ? this.businessSeats[indexOfSeat] :
                   (classParam === 'E') ? this.economySeats[indexOfSeat] : this.basicEconomySeats[indexOfSeat];
      console.log(seat);
      if (seat !== 'left' && seat !== 'right') {
        return seat;
      }
      indexOfSeat--;
    }

    return null;
  }

  configure(seat: any) {
    this.pickedSeat = {
      column: seat.column,
      row: seat.row,
      class: seat.class,
      price: seat.price,
      seatId: seat.seatId
    };
    this.blur = true;
    this.showModify = true;
  }

  onDelete(value: boolean) {
    this.blur = false;
    this.showModify = false;
    if (value) {
      this.airlineService.deleteSeat(this.pickedSeat.seatId).subscribe(
        (res: any) => {
          this.loadAll();
        },
        err => {
          this.toastr.error(err.statusText, 'Error.');
        }
      );
    }
  }

  onChangePrice(value: number) {
    const data = {
      id: this.pickedSeat.seatId,
      price: value,
    };
    this.airlineService.changeSeat(data).subscribe(
      (res: any) => {
        this.loadAll();
      },
      err => {
        this.toastr.error(err.statusText, 'Error.');
        this.blur = false;
        this.showModify = false;
      }
    );
    // if (this.pickedSeat.class === 'F') {
    //   this.updateFirstClassPriceRange(value);
    // } else if (this.pickedSeat.class === 'B') {
    //   this.updateBusinessPriceRange(value);
    // } else if (this.pickedSeat.class === 'E') {
    //   this.updateEconomyPriceRange(value);
    // } else {
    //   this.updateBasicEconomyPriceRange(value);
    // }
  }

  exit() {
    this.router.navigate(['/admin/' + this.adminId + '/flights']);
  }

  setFirstClassSeats() {
    console.log('usa');
    const numberOfSeats = this.seats.length;
    if (this.seats.filter(x => x.class === 'F').length > 0) {
      let row1 = this.seats.find(x => x.class === 'F');
      this.seats.filter(x => x.class === 'F').forEach(element => {
        if (+element.row > +row1.row) {
          row1 = element;
        }
      });
      const rows = row1.row;
      let column;
      console.log(rows);
      for (let r = 1; r < (+rows + 1); r++) {
        for (let c = 0; c < 6; c++) {
          column = (c === 0) ? 'A' : (c === 1) ? 'B' : (c === 2) ? 'C' : (c === 3) ? 'D' : (c === 4) ? 'E' : 'F';
          if (this.seats.some(seat => seat.row === r.toString() && seat.column === column && seat.class === 'F')) {
            this.firstClassSeats.push(this.seats.find(seat => seat.class === 'F' && seat.column === column && seat.row === r.toString()));
          } else {
            this.firstClassSeats.push((column === 'A' || column === 'B' || column === 'C') ? 'left' : 'right');
          }
        }
      }
      let value = this.firstClassSeats[this.firstClassSeats.length - 1];
      let lastValue = (value === 'left' || value === 'right') ? value :
                      (value.column === 'F' || value.column === 'A' || value.column === 'B') ? 'left' : 'right';
      while (value === 'left' || value === 'right') {
        lastValue = value;
        this.firstClassSeats.splice(this.firstClassSeats.length - 1, 1);
        value = this.firstClassSeats[this.firstClassSeats.length - 1];
      }
      this.firstClassSeats.push(lastValue);
    }
  }

  setBusinessSeats() {
    const numberOfSeats = this.seats.length;
    if (this.seats.filter(x => x.class === 'B').length > 0) {
      let row1 = this.seats.find(x => x.class === 'B');
      this.seats.filter(x => x.class === 'B').forEach(element => {
        if (+element.row > +row1.row) {
          row1 = element;
        }
      });
      const rows = row1.row;
      let column;
      for (let r = 1; r < (+rows + 1); r++) {
        for (let c = 0; c < 6; c++) {
          column = (c === 0) ? 'A' : (c === 1) ? 'B' : (c === 2) ? 'C' : (c === 3) ? 'D' : (c === 4) ? 'E' : 'F';
          if (this.seats.some(seat => seat.row === r.toString() && seat.column === column && seat.class === 'B')) {
            this.businessSeats.push(this.seats.find(seat => seat.class === 'B' && seat.column === column && seat.row === r.toString()));
          } else {
            this.businessSeats.push((column === 'A' || column === 'B' || column === 'C') ? 'left' : 'right');
          }
        }
      }
      let value = this.businessSeats[this.businessSeats.length - 1];
      let lastValue = (value === 'left' || value === 'right') ? value :
                      (value.column === 'F' || value.column === 'A' || value.column === 'B') ? 'left' : 'right';
      while (value === 'left' || value === 'right') {
        lastValue = value;
        this.businessSeats.splice(this.businessSeats.length - 1, 1);
        value = this.businessSeats[this.businessSeats.length - 1];
      }
      this.businessSeats.push(lastValue);
    }
  }

  setEconomySeats() {
    const numberOfSeats = this.seats.length;
    console.log('BROJ + ' + this.seats.filter(x => x.class === 'E').length);
    if (this.seats.filter(x => x.class === 'E').length > 0) {
      let row1 = this.seats.find(x => x.class === 'E');
      this.seats.filter(x => x.class === 'E').forEach(element => {
        if (+element.row > +row1.row) {
          row1 = element;
        }
      });
      const rows = row1.row;
      let column;
      for (let r = 1; r < (+rows + 1); r++) {
        for (let c = 0; c < 6; c++) {
          column = (c === 0) ? 'A' : (c === 1) ? 'B' : (c === 2) ? 'C' : (c === 3) ? 'D' : (c === 4) ? 'E' : 'F';
          if (this.seats.some(seat => seat.row === r.toString() && seat.column === column && seat.class === 'E')) {
            console.log('ajmoppppp');
            this.economySeats.push(this.seats.find(seat => seat.class === 'E' && seat.column === column && seat.row === r.toString()));
          } else {
            this.economySeats.push((column === 'A' || column === 'B' || column === 'C') ? 'left' : 'right');
          }
        }
      }
      let value = this.economySeats[this.economySeats.length - 1];
      let lastValue = (value === 'left' || value === 'right') ? value :
                      (value.column === 'F' || value.column === 'A' || value.column === 'B') ? 'left' : 'right';
      while (value === 'left' || value === 'right') {
        lastValue = value;
        this.economySeats.splice(this.economySeats.length - 1, 1);
        value = this.economySeats[this.economySeats.length - 1];
      }
      this.economySeats.push(lastValue);
      console.log('ECONOMY' + this.economySeats);
    }
  }

  setBasicEconomySeats() {
    const numberOfSeats = this.seats.length;
    if (this.seats.filter(x => x.class === 'BE').length > 0) {
      let row1 = this.seats.find(x => x.class === 'BE');
      this.seats.filter(x => x.class === 'BE').forEach(element => {
        if (+element.row > +row1.row) {
          row1 = element;
        }
      });
      const rows = row1.row;
      let column;
      for (let r = 1; r < (+rows + 1); r++) {
        for (let c = 0; c < 6; c++) {
          column = (c === 0) ? 'A' : (c === 1) ? 'B' : (c === 2) ? 'C' : (c === 3) ? 'D' : (c === 4) ? 'E' : 'F';
          if (this.seats.some(seat => seat.row === r.toString() && seat.column === column && seat.class === 'BE')) {
            // tslint:disable-next-line:max-line-length
            this.basicEconomySeats.push(this.seats.find(seat => seat.class === 'BE' && seat.column === column && seat.row === r.toString()));
          } else {
            this.basicEconomySeats.push((column === 'A' || column === 'B' || column === 'C') ? 'left' : 'right');
          }
        }
      }
      let value = this.basicEconomySeats[this.basicEconomySeats.length - 1];
      let lastValue = (value === 'left' || value === 'right') ? value :
                      (value.column === 'F' || value.column === 'A' || value.column === 'B') ? 'left' : 'right';
      while (value === 'left' || value === 'right') {
        lastValue = value;
        this.basicEconomySeats.splice(this.basicEconomySeats.length - 1, 1);
        value = this.basicEconomySeats[this.basicEconomySeats.length - 1];
      }
      this.basicEconomySeats.push(lastValue);
    }
  }

}
