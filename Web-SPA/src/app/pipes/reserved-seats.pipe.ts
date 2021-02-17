import { Pipe, PipeTransform } from '@angular/core';
import { Seat } from '../entities/seat';

@Pipe({
  name: 'reservedSeats'
})
export class ReservedSeatsPipe implements PipeTransform {

  transform(seats: Array<Seat>): Array<Seat> {
    const retVal = new Array<Seat>();
    seats.forEach(seat => {
      if (seat.reserved) {
        retVal.push(seat);
      }
    });
    return retVal;
  }

}
