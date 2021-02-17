export class TripParameter {
    public a: number; // napisano skraceno da bi izbegli predugacak url
    public f: string;
    constructor(airlineId: number, flightNumber: string) {
      this.a = airlineId;
      this.f = flightNumber;
    }
}
