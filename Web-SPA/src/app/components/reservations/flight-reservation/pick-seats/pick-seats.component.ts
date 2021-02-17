import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { Flight } from 'src/app/entities/flight';
import { Seat } from 'src/app/entities/seat';

@Component({
  selector: 'app-pick-seats',
  templateUrl: './pick-seats.component.html',
  styleUrls: ['./pick-seats.component.scss']
})
export class PickSeatsComponent implements OnInit, OnChanges {

  @Input() flight: any;
  @Input() blur: boolean;
  @Input() mySeats;
  @Input() index;
  @Output() bookSeat = new EventEmitter<Seat>();

  firstClassSeats: Array<any>;
  businessSeats: Array<any>;
  economySeats: Array<any>;
  basicEconomySeats: Array<any>;

  firstClassPriceRange: {minPrice: number, maxPrice: number};
  businessPriceRange: {minPrice: number, maxPrice: number};
  economyPriceRange: {minPrice: number, maxPrice: number};
  basicEconomyPriceRange: {minPrice: number, maxPrice: number};

  isOk = false;
  modifiedSeats;

  constructor() {
    this.firstClassSeats = new Array<any>();
    this.businessSeats = new Array<any>();
    this.economySeats = new Array<any>();
    this.basicEconomySeats = new Array<any>();
  }

  ngOnInit(): void {
    this.modifiedSeats = this.mySeats.map(el => +el.seats).concat([]);
  }

  ngOnChanges(changes: SimpleChanges) {
    this.firstClassSeats = [];
    this.businessSeats = [];
    this.economySeats = [];
    this.basicEconomySeats = [];
    this.setFirstClassSeats();
    this.setBusinessSeats();
    this.setEconomySeats();
    this.setBasicEconomySeats();
    this.modifiedSeats = [];
    this.mySeats.forEach(element => {
      element.seats.forEach(el => {
        this.modifiedSeats.push(el.seatId);
      });
    });
    // this.modifiedSeats = this.mySeats.map(el => {
    //   el.seats.map(element => {
    //     return element.seatId;
    //   });
    // }).concat([]);
  }

  book(seat: any) {
    this.bookSeat.emit(seat);
  }

  setFirstClassSeats() {
    const numberOfSeats = this.flight.seats.length;
    const rows = this.flight.seats.filter(x => x.class === 'F')[this.flight.seats.filter(x => x.class === 'F').length - 1].row;
    let column;
    for (let r = 1; r < rows + 1; r++) {
      for (let c = 0; c < 6; c++) {
        column = (c === 0) ? 'A' : (c === 1) ? 'B' : (c === 2) ? 'C' : (c === 3) ? 'D' : (c === 4) ? 'E' : 'F';
        if (this.flight.seats.some(seat => seat.row == r && seat.column === column && seat.class === 'F')) {
          this.firstClassSeats.push(this.flight.seats.find(seat => seat.class === 'F' && seat.column === column && seat.row == r));
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

  setBusinessSeats() {
    const numberOfSeats = this.flight.seats.length;
    if (this.flight.seats.filter(x => x.class === 'B').length > 0) {
      const rows = this.flight.seats.filter(x => x.class === 'B')[this.flight.seats.filter(x => x.class === 'B').length - 1].row;
      let column;
      for (let r = 1; r < rows + 1; r++) {
        for (let c = 0; c < 6; c++) {
          column = (c === 0) ? 'A' : (c === 1) ? 'B' : (c === 2) ? 'C' : (c === 3) ? 'D' : (c === 4) ? 'E' : 'F';
          if (this.flight.seats.some(seat => seat.row == r && seat.column === column && seat.class === 'B')) {
            this.businessSeats.push(this.flight.seats.find(seat => seat.class === 'B' && seat.column === column && seat.row == r));
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
    const numberOfSeats = this.flight.seats.length;
    if (this.flight.seats.filter(x => x.class === 'E').length > 0) {
      const rows = this.flight.seats.filter(x => x.class === 'E')[this.flight.seats.filter(x => x.class === 'E').length - 1].row;
      let column;
      for (let r = 1; r < rows + 1; r++) {
        for (let c = 0; c < 6; c++) {
          column = (c === 0) ? 'A' : (c === 1) ? 'B' : (c === 2) ? 'C' : (c === 3) ? 'D' : (c === 4) ? 'E' : 'F';
          if (this.flight.seats.some(seat => seat.row == r && seat.column === column && seat.class === 'E')) {
            this.economySeats.push(this.flight.seats.find(seat => seat.class === 'E' && seat.column === column && seat.row == r));
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
    }
  }

  setBasicEconomySeats() {
    const numberOfSeats = this.flight.seats.length;
    if (this.flight.seats.filter(x => x.class === 'BE').length > 0) {
      const rows = this.flight.seats.filter(x => x.class === 'BE')[this.flight.seats.filter(x => x.class === 'BE').length - 1].row;
      let column;
      for (let r = 1; r < rows + 1; r++) {
        for (let c = 0; c < 6; c++) {
          column = (c === 0) ? 'A' : (c === 1) ? 'B' : (c === 2) ? 'C' : (c === 3) ? 'D' : (c === 4) ? 'E' : 'F';
          if (this.flight.seats.some(seat => seat.row == r && seat.column === column && seat.class === 'BE')) {
            this.basicEconomySeats.push(this.flight.seats.find(seat => seat.class === 'BE' && seat.column === column && seat.row == r));
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
    this.isOk = true;
  }
}
