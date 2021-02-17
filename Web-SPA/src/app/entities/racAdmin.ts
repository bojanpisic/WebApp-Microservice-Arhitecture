import { User } from './user';

export class RacAdmin extends User {

    racId: number;

    constructor(fname: string, lname: string, pasw: string, email: string, city: string, phone: string) {
        super(fname, lname, pasw, email, city, phone);
    }
}
