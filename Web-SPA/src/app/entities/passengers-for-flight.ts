import { Flight } from './flight';
import { Seat } from './seat';
import { Passenger } from './passenger';

export class PassengersForFlight {
    public flight: Flight; // napisano skraceno da bi izbegli predugacak url
    public passengers: Array<Passenger>;
    constructor() {
      this.passengers = new Array<Passenger>();
    }
}
