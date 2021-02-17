export class Address {
    city: string;
    state: string;
    lon: number;
    lat: number;

    constructor(city?: string, state?: string, lon?: number, lat?: number) {
        this.city = city || '';
        this.state = state || '';
        this.lat = lat || 0;
        this.lon = lon || 0;
    }
}
