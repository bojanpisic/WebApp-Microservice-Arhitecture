import { Pipe, PipeTransform } from '@angular/core';
import { Seat } from '../entities/seat';

@Pipe({
  name: 'placeSeats'
})
export class PlaceSeatsPipe implements PipeTransform {

  transform(seats: Array<Seat>, classParam?: string, columnParam?: string): Array<Seat> {
    const retVal = new Array<Seat>();
    seats.forEach(seat => {
      if (seat.column === columnParam && seat.class === classParam) {
        retVal.push(seat);
      }
    });
    return retVal;
  }

}
