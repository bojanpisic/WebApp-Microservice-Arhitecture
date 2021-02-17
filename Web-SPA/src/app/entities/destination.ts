import { Address } from './address';

export class Destination {
    imageUrl: string;
    address: Address;

    constructor(imgUrl: string, address: Address) {
        this.imageUrl = imgUrl;
        this.address = address;
    }
}
