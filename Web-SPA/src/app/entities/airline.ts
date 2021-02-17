import { Flight } from './flight';
import { Destination } from './destination';
import { Address } from './address';

export class Airline {

    id: number;
    adminid: number;
    name: string;
    address: Address;
    about: string;
    promoDescription: Array<string>;
    flightDestionations: Array<Destination>;
    flights: Array<Flight>;
    averageRating: number;
    rates: Array<number>;
    logoUrl: string;

    constructor(name: string, address: Address) {
        this.name = name;
        this.address = address;
        this.promoDescription = new Array<string>();
        this.flightDestionations = new Array<Destination>();
        this.flights = new Array<Flight>();
        this.averageRating = 0.0;
        this.rates = new Array<number>();
    }

    rateAriline(rate: number) {
        this.rates.push(rate);
        let sum = 0.00;

        this.rates.forEach(element => {
            sum += element;
        });

        this.averageRating = sum / this.rates.length;
    }
}
