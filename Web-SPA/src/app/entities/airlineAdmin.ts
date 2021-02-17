import { User } from './user';

export class AirlineAdmin extends User {

    airlineId: number;

    constructor(fname: string, lname: string, pasw: string, email: string, city: string, phone: string) {
        super(fname, lname, pasw, email, city, phone);
    }
}
