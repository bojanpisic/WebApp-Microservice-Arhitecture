import { RegisteredUser } from './registeredUser';
import { Trip } from './trip';


export class Message {
    accepted: boolean;
    expires: Date;
    sends: RegisteredUser;
    receives: RegisteredUser;
    trip: Trip;
    text: string;
    read: boolean;

    constructor(sends: RegisteredUser, receives: RegisteredUser, trip: Trip, text: string) {
        this.accepted = false;
        this.sends = sends;
        this.receives = receives;
        this.trip = trip;
        this.text = text;
        if (trip.flights[0].takeOffDate.getTime() - Date.now() > 259200000) { // 3 dana u milisekundama
            const now = new Date();
            this.expires = new Date(now.getTime() + (3 * 24 * 60 * 60 * 1000)); // 3 dana od sad
        } else {
            this.expires = new Date(trip.flights[0].takeOffDate.getTime() - (3 * 60 * 60 * 1000)); // 3 sata pre putovanja
        }
        this.read = false;
    }
}