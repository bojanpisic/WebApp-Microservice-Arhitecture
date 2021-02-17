import { Injectable } from '@angular/core';
import { Car } from 'src/app/entities/car';

@Injectable({
  providedIn: 'root'
})
export class CarService {

  cars: Array<Car>;

  constructor() {
    this.cars = new Array<Car>();
    this.mockedCars();
  }

  getCar(adminId: number, carId: number) {
    return this.cars[carId];
  }

  getCarsOfSpecificRAC(racId: number) {
    return this.getAllCars();
  }

  getAllCars() {
    return this.cars;
  }

  mockedCars() {
    const c1 = new Car(0, 0, 0, 'Range Rover', 'Sport', 2020, 'SUV', 5, 4.95, 50, '../assets/cars/range.jpg');
    const c2 = new Car(1, 0, 0, 'Range Rover', 'Evoque', 2020, 'Convertible', 5, 4.60, 40, '../assets/cars/evoque.jpg');
    const c3 = new Car(2, 0, 0, 'Range Rover', 'Velar', 2020, 'Luxury', 5, 4.75, 45, '../assets/cars/velar.jpg');
    const c4 = new Car(3, 0, 0, 'Mercedes', 'AMG GT 63', 2020, 'Coupe', 5, 5.00, 70, '../assets/cars/merc.jpg');

    this.cars.push(c1);
    this.cars.push(c2);
    this.cars.push(c3);
    this.cars.push(c4);
  }
}
