import { Injectable } from '@angular/core';
import { Ticket } from 'src/app/entities/ticket';
import { AirlineService } from './airline.service';
import { Flight } from 'src/app/entities/flight';
import { ChangeOver } from 'src/app/entities/changeOver';
import { Destination } from 'src/app/entities/destination';
import { Seat } from 'src/app/entities/seat';

@Injectable({
  providedIn: 'root'
})
export class TicketService {

  tickets: Array<Ticket>;
  constructor(private airlines: AirlineService) {
    this.tickets = new Array<Ticket>();
    this.mockedTickets();
  }

  getTicketsOfSpecificAirline(airlineId: number) {
    const tickets = new Array<Ticket>();
    // this.tickets.forEach(ticket => {
    //   if (ticket.airlineId == airlineId) {
    //     tickets.push(ticket);
    //   }
    // });
    return tickets;
  }


  mockedTickets() {
    // const f1 = new Flight( 0, new Date(Date.now()), new Date(Date.now()), '5h 53min', 12,
    // [new ChangeOver('11:20', '10:30', new Destination('', 'Paris', 'France', 'PAR'))], 300.00, '234T',
    // new Destination('', 'Madrid', 'Spain', 'MAD'), new Destination('', 'Belgrade', 'Serbia', 'BG'), '06:20', '12:13',
    // [new Seat(0, '33R')]);

    // const f2 = new Flight( 0, new Date(Date.now()), new Date(Date.now()), '8h 10min', 12,
    // [], 200.00, '234T',
    // new Destination('', 'New York', 'USA', 'NY'), new Destination('', 'Novi Sad', 'Serbia', 'NS'), '15:15', '22:15',
    // [new Seat(0, '33R')]);

    // const f3 = new Flight( 0, new Date(Date.now()), new Date(Date.now()), '10h 0min', 12,
    // [new ChangeOver('10:00', '09:00', new Destination('', 'Paris', 'France', 'PAR')),
    // new ChangeOver('12:10', '11:20', new Destination('', 'London', 'England', 'LON'))], 700.00, '234T',
    // new Destination('', 'Novi Sad', 'Serbia', 'NS'), new Destination('', 'New York', 'USA', 'NY'), '06:00', '16:00',
    // [new Seat(0, '33R')]);

    // const f4 = new Flight( 1, new Date(Date.now()), new Date(Date.now()), '1h 10min', 12,
    // [], 220.00, '234T',
    // new Destination('', 'Vienna', 'Austria', 'VIE'), new Destination('', 'Belgrade', 'Serbia', 'BG'), '14:10', '15:20',
    // [new Seat(0, '33R')]);

    // const f5 = new Flight( 1, new Date(Date.now()), new Date(Date.now()), '0h 50min', 12,
    // [], 200.00, '234T',
    // new Destination('', 'Sofia', 'Bulgaria', 'BUG'), new Destination('', 'Belgrade', 'Serbia', 'BG'), '12:15', '13:05',
    // [new Seat(0, '33R')]);

    // AKO JE ROUND TRIP MORA IMATI DVA LETA, NE JEDAN
    // const t1 = new Ticket([f1.flightNumber], [new Seat(0, '33R')], 'oneWay', 300, 0);
    // const t2 = new Ticket([f3.flightNumber, f2.flightNumber], [new Seat(0, '33R')], 'roundTrip', 700.00, 0);
    // const t3 = new Ticket([f3.flightNumber, f1.flightNumber, f2.flightNumber], [new Seat(0, '33R')], 'multicity', 700.00, 0);
    // const t4 = new Ticket([f4.flightNumber], [new Seat(0, '33R')], 'oneWay', 220.00, 1);
    // const t5 = new Ticket([f5.flightNumber], [new Seat(0, '33R')], 'oneWay', 200.00, 1);

    // this.tickets.push(t1);
    // this.tickets.push(t2);
    // this.tickets.push(t3);
    // this.tickets.push(t4);
    // this.tickets.push(t5);
  }
}
