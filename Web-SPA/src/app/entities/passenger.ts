export class Passenger {
    firstName: string;
    lastName: string;
    passport: string;

    constructor(fName: string, lName: string, pass: string) {
        this.firstName = fName;
        this.lastName = lName;
        this.passport = pass;
    }
}
