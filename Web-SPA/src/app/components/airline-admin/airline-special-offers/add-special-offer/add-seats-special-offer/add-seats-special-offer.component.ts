import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Flight } from 'src/app/entities/flight';
import { Seat } from 'src/app/entities/seat';
import { AirlineService } from 'src/services/airline.service';

@Component({
  selector: 'app-add-seats-special-offer',
  templateUrl: './add-seats-special-offer.component.html',
  styleUrls: ['./add-seats-special-offer.component.scss']
})
export class AddSeatsSpecialOfferComponent implements OnInit {

  @Input() flightId: number;
  @Input() takenSeats: Array<any>;
  @Output() addSeat = new EventEmitter<any>();

  seats: Array<{column: string, row: string, class: string, price: number, seatId: number}>;

  pickedSeat: any;
  showModal = false;
  blur: boolean;

  firstClassSeats: Array<any>;
  businessSeats: Array<any>;
  economySeats: Array<any>;
  basicEconomySeats: Array<any>;

  itsOk = false;

  constructor(private airlineService: AirlineService) {

    this.seats = [];
    this.takenSeats = [];

    this.firstClassSeats = new Array<any>();
    this.businessSeats = new Array<any>();
    this.economySeats = new Array<any>();
    this.basicEconomySeats = new Array<any>();
  }

  ngOnInit(): void {
    // tslint:disable-next-line:curly
    if (this.flightId !== undefined) {
      const air1 = this.airlineService.getFlightsSeats(this.flightId).subscribe(
        (res: any[]) => {
          console.log(res);
          if (res.length) {
            console.log('alo1111');
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
          console.log('dada' + err.status);
          // tslint:disable-next-line: triple-equals
          if (err.status == 400) {
            console.log(err);
          // tslint:disable-next-line: triple-equals
          } else if (err.status == 401) {
            console.log(err);
          } else {
            console.log(err);
          }
        }
      );
    }
  }

  reload() {
    console.log('AAAAAAA');
    console.log(this.takenSeats);
    this.setEconomySeats();
    this.setFirstClassSeats();
    this.setBusinessSeats();
    this.setBasicEconomySeats();
  }

  onSeat(seat: Seat) {
  }

  onMakeItSpecial(seat: Seat) {
    this.pickedSeat = seat;
    this.blur = true;
    this.showModal = true;
  }

  onFinish(value: number) {
    this.blur = false;
    this.showModal = false;
    const data = {
      column: this.pickedSeat.column,
      row: this.pickedSeat.row,
      class: this.pickedSeat.class,
      price: value,
      seatId: this.pickedSeat.seatId,
      oldPrice: this.pickedSeat.price
    };
    this.addSeat.emit(data);
  }

  onCloseModal(value: any) {
    this.blur = false;
    this.showModal = false;
  }

  setFirstClassSeats() {
    if (this.seats.filter(x => x.class === 'F').length > 0) {
      let row1 = this.seats.find(x => x.class === 'F');
      this.seats.filter(x => x.class === 'F').forEach(element => {
        if (+element.row > +row1.row) {
          row1 = element;
        }
      });
      const rows = row1.row;
      let column;
      for (let r = 1; r < +rows + 1; r++) {
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
    if (this.seats.filter(x => x.class === 'B').length > 0) {
      let row1 = this.seats.find(x => x.class === 'B');
      this.seats.filter(x => x.class === 'B').forEach(element => {
        if (+element.row > +row1.row) {
          row1 = element;
        }
      });
      const rows = row1.row;
      let column;
      for (let r = 1; r < +rows + 1; r++) {
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
    if (this.seats.filter(x => x.class === 'E').length > 0) {
      let row1 = this.seats.find(x => x.class === 'E');
      this.seats.filter(x => x.class === 'E').forEach(element => {
        if (+element.row > +row1.row) {
          row1 = element;
        }
      });
      const rows = row1.row;
      let column;
      for (let r = 1; r < +rows + 1; r++) {
        for (let c = 0; c < 6; c++) {
          column = (c === 0) ? 'A' : (c === 1) ? 'B' : (c === 2) ? 'C' : (c === 3) ? 'D' : (c === 4) ? 'E' : 'F';
          if (this.seats.some(seat => seat.row === r.toString() && seat.column === column && seat.class === 'E')) {
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
      for (let r = 1; r < +rows + 1; r++) {
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
