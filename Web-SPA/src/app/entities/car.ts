export class Car {

    id: number;
    idOfService: number; // id glavnog servisa
    idOfAffiliate: number; // id filijale u kojoj je, ako je u glavnoj onda je 0
    brand: string;
    model: string;
    year: number;
    type: string; // small, medium, large, suv, van, luxury, convertible
    numberOfSeats: number;
    averageRate: number;
    pricePerDay: number;
    automatic: true; // false ako je manual gearbox
    image: string;

    constructor(id?: number, idOfService?: number, idOfAffiliate?: number, brand?: string, model?: string, year?: number, type?: string,
                numberOfSeats?: number, averageRate?: number, pricePerDay?: number, image?: string) {
        this.id = id || null;
        this.idOfService = idOfService || null;
        this.idOfAffiliate = idOfAffiliate || null;
        this.brand = brand || null;
        this.model = model || null;
        this.year = year || null;
        this.type = type || null;
        this.numberOfSeats = numberOfSeats || null;
        this.averageRate = averageRate || null;
        this.pricePerDay = pricePerDay || null;
        this.image = image || null;
    }
}
