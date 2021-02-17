import { Flight } from './flight';
import { Seat } from './seat';

export class SeatsForFlight {
    public flight: Flight; // napisano skraceno da bi izbegli predugacak url
    public seats: Array<Seat>;
    constructor(flightParam: Flight) {
      this.flight = flightParam;
      this.seats = new Array<Seat>();
    }
}
