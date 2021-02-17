import { Pipe, PipeTransform } from '@angular/core';
import { Flight } from '../entities/flight';

@Pipe({
  name: 'findFlight'
})
export class FindFlightPipe implements PipeTransform {

  transform(flights: Array<any>, inputString?: string): Array<Flight> {
    if (inputString === '') {
      return flights;
    }
    const retVal = new Array<Flight>();
    flights.forEach(flight => {
      if (flight.flightNumber.toLowerCase().startsWith(inputString.toLowerCase())) {
        if (!retVal.includes(flight)) {
          retVal.push(flight);
        }
      }
    });
    return retVal;
  }

}
