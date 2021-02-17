import { Seat } from './seat';
import { Flight } from './flight';

export class TripId {

    airlineIds: Array<number>;
    flightsIds: Array<string>;

    constructor(airlineIds: Array<number>, flightsIds: Array<string>) {
        this.airlineIds = airlineIds;
        this.flightsIds = flightsIds;
    }
}
