import { Message } from './message';

export class User {
    id: number;
    imageUrl: string;
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    userType: string;
    phone: string;
    address: string;
    messages: Array<Message>;

    constructor(fname: string, lname: string, pasw: string, email: string, city: string, phone: string) {
        this.firstName = fname;
        this.lastName = lname;
        this.email = email;
        this.password = pasw;
        this.phone = phone;
        this.address = city;
        this.messages = new Array<Message>();
    }
}
