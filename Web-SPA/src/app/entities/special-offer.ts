import { Flight } from './flight';
import { Trip } from './trip';
import { Seat } from './seat';

export class SpecialOffer {
    flights: Array<Flight>;
    seats: Array<Seat>;
    newPrice: number;

    constructor(flights?: Array<Flight>, seats?: Array<Seat>, newPrice?: number) {
        this.flights = flights || new Array<Flight>();
        this.seats = seats || null;
        this.newPrice = newPrice || null;
    }
}
