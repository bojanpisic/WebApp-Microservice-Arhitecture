import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'findCar'
})
export class FindCarPipe implements PipeTransform {

  transform(cars: Array<any>, inputString?: string): Array<any> {
    if (inputString === '') {
      return cars;
    }
    const retVal = new Array<any>();
    cars.forEach(car => {
      if (car.model.toLowerCase().startsWith(inputString.toLowerCase())) {
        if (!retVal.includes(car)) {
          retVal.push(car);
        }
      }
    });
    return retVal;
  }

}
