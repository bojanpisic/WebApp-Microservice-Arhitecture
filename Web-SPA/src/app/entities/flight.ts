import { Time } from '@angular/common';
import { Destination } from './destination';
import { ChangeOver } from './changeOver';
import { Seat } from './seat';
import { Address } from './address';
import { Airline } from './airline';

export class Flight {
    id: number;
    airlineId: number;
    airline: Airline;
    flightNumber: string;
    takeOffDate: Date;
    landingDate: Date;
    tripTime: string;
    tripLength: number;
    changeOverLocations: Array<ChangeOver>; //obtisi
    stops: Array<Address>;
    ticketPrice: number; // obrisi
    from: Address;
    to: Address;
    takeOffTime: string;
    landingTime: string;
    seats: Array<Seat>;

    constructor(airlineId?: number, takeOffDate?: Date, landingDate?: Date, tripTime?: string,
                tripLength?: number, changeOverLocations?: Array<ChangeOver>,
                ticketPrice?: number, flightNumber?: string,
                from?: Address, to?: Address, takeofftime?: string, landingtime?: string,
                seats?: Array<Seat>, stops?: Array<Address>, airline?: Airline, id?: number) {
            this.id = id;
            this.airlineId = airlineId;
            this.takeOffDate = takeOffDate || null;
            this.landingDate = landingDate || null;
            this.tripTime = tripTime || null;
            this.tripLength = tripLength || null;
            this.changeOverLocations = changeOverLocations || new Array<ChangeOver>();
            this.ticketPrice = ticketPrice || null;
            this.flightNumber = flightNumber || null;
            this.from = from || null;
            this.to = to || null;
            this.landingTime = landingtime || null;
            this.takeOffTime = takeofftime || null;
            this.seats = seats || new Array<Seat>();
            this.stops = stops || new Array<Address>();
            this.airline = airline || null;
    }
}
