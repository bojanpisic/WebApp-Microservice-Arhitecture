import { Injectable } from '@angular/core';
import { Flight } from 'src/app/entities/flight';
import { ChangeOver } from 'src/app/entities/changeOver';
import { Destination } from 'src/app/entities/destination';
import { Seat } from 'src/app/entities/seat';
import { TripId } from 'src/app/entities/trip-id';
import { Address } from 'src/app/entities/address';
import { SeatsService } from './seats.service';
import { Airline } from 'src/app/entities/airline';

@Injectable({
  providedIn: 'root'
})
export class FlightService {

  flights: Array<Flight>;

  constructor(private seats: SeatsService) {
    this.flights = new Array<Flight>();
    this.mockedFlights();
  }

  addFlightToAirline(flight: Flight, newPrice: number) {
    alert('adding spec offer. Not implemented');
  }

  getFlightsOfSpecificAirline(airlineId: number) {
    const flights = new Array<Flight>();
    this.flights.forEach(f => {
      if (f.airlineId == airlineId) {
        flights.push(f);
      }
    });
    return flights;
  }

  getFlight(id: number) {
    let retVal;
    this.flights.forEach(flight => {
      if (flight.id == id) {
        retVal = flight;
      }
    });

    return retVal;
  }

  getAllFlights() {
    return this.flights;
  }


  mockedFlights() {

    // const seats1 = this.makeSeats();
    // const seats2 = this.makeSeats();
    // const seats3 = this.makeSeats();
    // const seats4 = this.makeSeats();
    // const seats5 = this.makeSeats();
    // const seats6 = this.makeSeats();

    // const f1 = new Flight(0, new Date(2020, 4, 28), new Date(Date.now()), '03h 40min', 12,
    // [new ChangeOver('08:40', '08:10', new Address('Paris', 'France', 'PAR', 0, 0))], 300.00, '234A',
    // new Address('Madrid', 'Spain', 0, 0), new Address('Belgrade', 'Serbia', 'BEG', 0, 0), '06:20', '10:00',
    // seats1, [new Address('Paris', 'France', 'PAR', 0, 0)], new Airline('Turkish Airlines', new Address('Istanbul', 'Turkey', 'IST', 0, 0))
    // , 0);

    // const f2 = new Flight( 0, new Date(Date.now()), new Date(Date.now()), '03h 10min', 12,
    // [], 200.00, '234B',
    // new Address('Belgrade', 'Serbia', 'BEG', 0, 0), new Address( 'Barcelona', 'Spain', 'BAR', 0, 0), '04:00', '07:10',
    // seats2, [], new Airline('Turkish Airlines', new Address('Istanbul', 'Turkey', 'IST', 0, 0)), 1);

    // const f3 = new Flight( 0, new Date(Date.now()), new Date(Date.now()), '10h 0min', 12,
    // [new ChangeOver('10:00', '09:00', new Address('Paris', 'France', 'PAR', 0, 0)),
    // new ChangeOver('12:10', '11:20', new Address('London', 'England', 'LON', 0, 0))], 700.00, '234C',
    // new Address('Novi Sad', 'Serbia', 'NS', 0, 0), new Address('New York', 'USA', 'NY', 0, 0), '06:00', '16:00',
    // seats3, [new Address('Paris', 'France', 'PAR', 0, 0), new Address('London', 'England', 'LON', 0, 0)],
    // new Airline('Turkish Airlines', new Address('Istanbul', 'Turkey', 'IST', 0, 0)), 2);

    // const f4 = new Flight( 0, new Date(Date.now()), new Date(Date.now()), '8h 10min', 12,
    // [], 700.00, '234D',
    // new Address('New York', 'USA', 'NY', 0, 0), new Address('Novi Sad', 'Serbia', 'NS', 0, 0), '08:00', '16:10',
    // seats4, [], new Airline('Turkish Airlines', new Address('Istanbul', 'Turkey', 'IST', 0, 0)), 3);

    // const f5 = new Flight( 1, new Date(Date.now()), new Date(Date.now()), '1h 10min', 12,
    // [], 220.00, 'V11T',
    // new Address('Vienna', 'Austria', 'VIE', 0, 0), new Address('Belgrade', 'Serbia', 'BG', 0, 0), '14:10', '15:20',
    // seats5, [], new Airline('Qatar Airlines', new Address('Doha', 'Qatar', 'IST', 0, 0)), 4);

    // const f6 = new Flight( 1, new Date(Date.now()), new Date(Date.now()), '0h 50min', 12,
    // [], 200.00, 'V11R',
    // new Address('Sofia', 'Bulgaria', 'BUG', 0, 0), new Address('Belgrade', 'Serbia', 'BG', 0, 0), '12:15', '13:05',
    // seats6, [], new Airline('Qatar Airlines', new Address('Doha', 'Qatar', 'IST', 0, 0)), 5);

    // this.flights.push(f1);
    // this.flights.push(f2);
    // this.flights.push(f3);
    // this.flights.push(f4);
    // this.flights.push(f5);
    // this.flights.push(f6);

    // console.log(this.flights);
  }

  makeSeats() {
    return this.mockedSeats();
  }

  mockedSeats() {

    const seats = new Array<Seat>();

    const s1 = new Seat('F', 'A', 1, 400);
    const s2 = new Seat('F', 'B', 1, 400);
    const s3 = new Seat('F', 'C', 1, 400);
    const s4 = new Seat('F', 'D', 1, 400);
    const s5 = new Seat('F', 'E', 1, 400);
    const s6 = new Seat('F', 'F', 1, 400);
    const s7 = new Seat('F', 'A', 2, 400);
    const s8 = new Seat('F', 'B', 2, 400);
    const s9 = new Seat('F', 'C', 2, 400);
    const s10 = new Seat('F', 'D', 2, 400);
    const s11 = new Seat('F', 'E', 2, 400);
    const s12 = new Seat('F', 'F', 2, 400);
    const s13 = new Seat('F', 'A', 3, 400);
    const s14 = new Seat('F', 'B', 3, 400);
    const s15 = new Seat('F', 'C', 3, 400);
    const s16 = new Seat('F', 'D', 3, 400);
    const s17 = new Seat('F', 'E', 3, 400);
    const s18 = new Seat('F', 'F', 3, 400);
    const s19 = new Seat('F', 'A', 4, 400);
    const s20 = new Seat('F', 'B', 4, 400);
    const s21 = new Seat('F', 'C', 4, 400);
    const s22 = new Seat('F', 'D', 4, 400);
    const s23 = new Seat('F', 'E', 4, 400);
    const s24 = new Seat('F', 'F', 4, 400);
    const s25 = new Seat('F', 'A', 5, 400);
    const s26 = new Seat('F', 'B', 5, 400);
    const s27 = new Seat('F', 'C', 5, 400);
    const s28 = new Seat('F', 'D', 5, 400);
    const s29 = new Seat('F', 'E', 5, 400);
    const s30 = new Seat('F', 'F', 5, 400);
    const s301 = new Seat('F', 'A', 6, 400);
    const s302 = new Seat('F', 'B', 6, 400);
    const s303 = new Seat('F', 'C', 6, 400);
    const s31 = new Seat('B', 'A', 1, 300);
    const s32 = new Seat('B', 'B', 1, 300);
    const s33 = new Seat('B', 'C', 1, 300);
    const s34 = new Seat('B', 'D', 1, 300);
    const s35 = new Seat('B', 'E', 1, 300);
    const s36 = new Seat('B', 'F', 1, 300);
    const s37 = new Seat('B', 'A', 2, 300);
    const s38 = new Seat('B', 'B', 2, 300);
    const s39 = new Seat('B', 'C', 2, 300);
    const s40 = new Seat('B', 'D', 2, 300);
    const s41 = new Seat('B', 'E', 2, 300);
    const s42 = new Seat('B', 'F', 2, 300);
    const s43 = new Seat('B', 'A', 3, 300);
    const s44 = new Seat('B', 'B', 3, 300);
    const s45 = new Seat('B', 'C', 3, 300);
    const s46 = new Seat('B', 'D', 3, 300);
    const s47 = new Seat('B', 'E', 3, 300);
    const s48 = new Seat('B', 'F', 3, 300);
    const s49 = new Seat('B', 'A', 4, 300);
    const s50 = new Seat('B', 'B', 4, 300);
    const s51 = new Seat('B', 'C', 4, 300);
    const s52 = new Seat('B', 'D', 4, 300);
    const s53 = new Seat('B', 'E', 4, 300);
    const s54 = new Seat('B', 'F', 4, 300);
    const s55 = new Seat('B', 'A', 5, 300);
    const s56 = new Seat('B', 'B', 5, 300);
    const s57 = new Seat('B', 'C', 5, 300);
    const s58 = new Seat('B', 'D', 5, 300);
    const s59 = new Seat('B', 'E', 5, 300);
    const s60 = new Seat('B', 'F', 5, 300);
    const s61 = new Seat('E', 'A', 1, 300);
    const s62 = new Seat('E', 'B', 1, 300);
    const s63 = new Seat('E', 'C', 1, 300);
    const s64 = new Seat('E', 'D', 1, 300);
    const s65 = new Seat('E', 'E', 1, 300);
    const s66 = new Seat('E', 'F', 1, 300);
    const s67 = new Seat('E', 'A', 2, 300);
    const s68 = new Seat('E', 'B', 2, 300);
    const s69 = new Seat('E', 'C', 2, 300);
    const s70 = new Seat('E', 'D', 2, 300);
    const s71 = new Seat('E', 'E', 2, 300);
    const s72 = new Seat('E', 'F', 2, 300);
    const s73 = new Seat('E', 'A', 3, 300);
    const s74 = new Seat('E', 'B', 3, 300);
    const s75 = new Seat('E', 'C', 3, 300);
    const s76 = new Seat('E', 'D', 3, 300);
    const s77 = new Seat('E', 'E', 3, 300);
    const s78 = new Seat('E', 'F', 3, 300);
    const s79 = new Seat('E', 'A', 4, 300);
    const s80 = new Seat('E', 'B', 4, 300);
    const s81 = new Seat('E', 'C', 4, 300);
    const s82 = new Seat('E', 'D', 4, 300);
    const s83 = new Seat('E', 'E', 4, 300);
    const s84 = new Seat('E', 'F', 4, 300);
    const s85 = new Seat('E', 'A', 5, 300);
    const s86 = new Seat('E', 'B', 5, 300);
    const s87 = new Seat('E', 'C', 5, 300);
    const s88 = new Seat('E', 'D', 5, 300);
    const s89 = new Seat('E', 'E', 5, 300);
    const s90 = new Seat('E', 'F', 5, 300);
    const s91 = new Seat('BE', 'A', 1, 300);
    const s92 = new Seat('BE', 'B', 1, 300);
    const s93 = new Seat('BE', 'C', 1, 300);
    const s94 = new Seat('BE', 'D', 1, 300);
    const s95 = new Seat('BE', 'E', 1, 300);
    const s96 = new Seat('BE', 'F', 1, 300);
    const s97 = new Seat('BE', 'A', 2, 300);
    const s98 = new Seat('BE', 'B', 2, 300);
    const s99 = new Seat('BE', 'C', 2, 300);
    const s100 = new Seat('BE', 'D', 2, 300);
    const s101 = new Seat('BE', 'E', 2, 300);
    const s102 = new Seat('BE', 'F', 2, 300);
    const s103 = new Seat('BE', 'A', 3, 300);
    const s104 = new Seat('BE', 'B', 3, 300);
    const s105 = new Seat('BE', 'C', 3, 300);
    const s106 = new Seat('BE', 'D', 3, 300);
    const s107 = new Seat('BE', 'E', 3, 300);
    const s108 = new Seat('BE', 'F', 3, 300);
    const s109 = new Seat('BE', 'A', 4, 300);
    const s110 = new Seat('BE', 'B', 4, 300);
    const s111 = new Seat('BE', 'C', 4, 300);
    const s112 = new Seat('BE', 'D', 4, 300);
    const s113 = new Seat('BE', 'E', 4, 300);
    const s114 = new Seat('BE', 'F', 4, 300);
    const s115 = new Seat('BE', 'A', 5, 300);
    const s116 = new Seat('BE', 'B', 5, 300);
    const s117 = new Seat('BE', 'C', 5, 300);
    const s118 = new Seat('BE', 'D', 5, 300);
    const s119 = new Seat('BE', 'E', 5, 300);
    const s120 = new Seat('BE', 'F', 5, 300);

    seats.push(s1);
    seats.push(s2);
    // seats.push(s3);
    seats.push(s4);
    // seats.push(s5);
    seats.push(s6);
    seats.push(s7);
    seats.push(s8);
    // seats.push(s9);
    // seats.push(s10);
    s11.price = 487;
    seats.push(s11);
    // seats.push(s12);
    seats.push(s13);
    seats.push(s14);
    seats.push(s15);
    seats.push(s16);
    seats.push(s17);
    seats.push(s18);
    seats.push(s19);
    seats.push(s20);
    seats.push(s21);
    seats.push(s22);
    seats.push(s23);
    seats.push(s24);
    seats.push(s25);
    seats.push(s26);
    seats.push(s27);
    seats.push(s28);
    seats.push(s29);
    seats.push(s30);
    seats.push(s301);
    s302.reserved = true;
    seats.push(s302);
    seats.push(s303);
    seats.push(s31);
    seats.push(s32);
    seats.push(s33);
    seats.push(s34);
    seats.push(s35);
    seats.push(s36);
    seats.push(s37);
    seats.push(s38);
    seats.push(s39);
    seats.push(s40);
    seats.push(s41);
    seats.push(s42);
    seats.push(s43);
    seats.push(s44);
    seats.push(s45);
    seats.push(s46);
    seats.push(s47);
    seats.push(s48);
    seats.push(s49);
    seats.push(s50);
    seats.push(s51);
    seats.push(s52);
    seats.push(s53);
    seats.push(s54);
    seats.push(s55);
    seats.push(s56);
    seats.push(s57);
    seats.push(s58);
    seats.push(s59);
    seats.push(s60);
    seats.push(s61);
    seats.push(s62);
    seats.push(s63);
    seats.push(s64);
    seats.push(s65);
    seats.push(s66);
    seats.push(s67);
    seats.push(s68);
    seats.push(s69);
    seats.push(s70);
    seats.push(s71);
    seats.push(s72);
    seats.push(s73);
    seats.push(s74);
    seats.push(s75);
    seats.push(s76);
    seats.push(s77);
    seats.push(s78);
    seats.push(s79);
    seats.push(s80);
    seats.push(s81);
    seats.push(s82);
    seats.push(s83);
    seats.push(s84);
    s85.available = false;
    seats.push(s85);
    s86.available = false;
    seats.push(s86);
    seats.push(s87);
    seats.push(s88);
    seats.push(s89);
    seats.push(s90);
    seats.push(s91);
    seats.push(s92);
    seats.push(s93);
    seats.push(s94);
    seats.push(s95);
    seats.push(s96);
    seats.push(s97);
    seats.push(s98);
    seats.push(s99);
    seats.push(s100);
    seats.push(s101);
    seats.push(s102);
    seats.push(s103);
    seats.push(s104);
    seats.push(s105);
    seats.push(s106);
    seats.push(s107);
    seats.push(s108);
    seats.push(s109);
    seats.push(s110);
    seats.push(s111);
    s112.available = false;
    seats.push(s112);
    s113.available = false;
    seats.push(s113);
    s114.available = false;
    seats.push(s114);
    seats.push(s115);
    seats.push(s116);
    seats.push(s117);
    seats.push(s118);
    seats.push(s119);
    seats.push(s120);

    return seats;
  }
}
