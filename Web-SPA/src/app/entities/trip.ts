import { Seat } from './seat';
import { Flight } from './flight';
import { TripId } from './trip-id';
import { ArrayType } from '@angular/compiler';

export class Trip {
    // tripId: TripId; UMESTO FLIGHTS
    flights: Array<Flight>;
    tripType: string;
    minumumPrice: number;
    // trip treba da se konstruise na osnovu unetih parametara, znaci treba "dinamicki" sastaviti tripove
    // ako korisnik izabere BEG to NY za taj i taj datum, treba pretraziti sve letove od svih aviokompanija
    // koje ispunjavaju te kriterijume, zatim SLOZITI tripove sa tim letovima,
    // Recimo let BG to NY preko avikompanije 1 i let od NY do BG preko aviokompanije 1, to je jedan trip
    // Zatim let BG to NY preko avikompanije 1 i let od NY do BG preko aviokompanije 2, to je drugi trip
    // Zatim let BG to NY preko avikompanije 2 i let od NY do BG preko aviokompanije 1, to je cetvrti trip
    // itd itd sve koje zadovoljavaju uslove
    // fakticki te tripove NE treba cuvati u bazi vec ih DINAMICKI 'izracunati', fakticki sastaviti letove
    // na osnovu nekih parametara

    constructor(flights: Array<Flight>, tripType: string, minimumPrice: number) {
        this.flights = flights;
        this.tripType = tripType;
        this.minumumPrice = minimumPrice;
    }

}
