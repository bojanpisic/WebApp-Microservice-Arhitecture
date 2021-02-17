import { Seat } from './seat';
import { Flight } from './flight';

export class Ticket {
    flight: Flight;
    seat: Seat;

    constructor(flight: Flight, seat: Seat) {
        this.flight = flight;
        this.seat = seat;
    }

}
