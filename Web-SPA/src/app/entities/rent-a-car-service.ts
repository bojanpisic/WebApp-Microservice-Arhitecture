import { Destination } from './destination';
import { Address } from './address';
import { Car } from './car';

export class RentACarService {

    id: number;
    adminId: number;
    name: string;
    address: Address;
    about: string;
    promoDescription: Array<string>;
    branches: Array<RentACarService>;
    averageRating: number;
    rates: Array<number>;
    cars: Array<Car>;
    logoUrl: string;

    constructor(name: string, address: Address) {
        this.name = name;
        this.address = address;
        this.about = '';
        this.promoDescription = new Array<string>();
        this.branches = new Array<RentACarService>();
        this.averageRating = 0.0;
        this.rates = new Array<number>();
        this.cars = new Array<Car>();
        this.logoUrl = '';
    }
}
