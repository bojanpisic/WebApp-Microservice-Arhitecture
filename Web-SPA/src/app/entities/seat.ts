import { Passenger } from './passenger';

export class Seat {
    passenger: Passenger;
    class: string;
    column: string;
    row: number;
    price: number;
    available: boolean;
    reserved: boolean;

    constructor(classParam: string, columnParam: string, rowParam: number, priceParam: number) {
        this.passenger = null;
        this.class = classParam;
        this.column = columnParam;
        this.row = rowParam;
        this.price = priceParam;
        this.available = true;
        this.reserved = false;
    }
}
