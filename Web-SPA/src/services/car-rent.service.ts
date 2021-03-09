import { Injectable } from '@angular/core';
import { RentACarService } from 'src/app/entities/rent-a-car-service';
import { Address } from 'src/app/entities/address';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CarRentService {
  readonly BaseURI = 'http://localhost:8084/racs';

  cars: Array<{
    brand: string,
    carId: number,
    city: string,
    model: string,
    name: string,
    pricePerDay: number,
    seatsNumber: number,
    state: string,
    type: string,
    year: number,
    priceForSelectedDates: number
  }>;

  constructor(private http: HttpClient) {
    this.cars = [];
    this.mock();
  }

  editRAC(data: any) {
    const body = {
      Name: data.name,
      Address: data.address,
      PromoDescription: data.promoDescription,
    };
    const url = this.BaseURI + '/rentacarserviceadmin/change-racs-info';
    return this.http.put(url, body);
  }

  changeLogo(data: any) {
    const formData = new FormData();
    formData.append('img', data.image);

    const url = this.BaseURI + '/rentacarserviceadmin/change-racs-logo';
    return this.http.put(url, formData);
  }

  getRAC(): Observable<any> {
    const url = this.BaseURI + '/rentacarserviceadmin/get-racs';
    console.log(url);
    return this.http.get<any>(url);
  }

  getRACCityState(): Observable<any> {
    const url = this.BaseURI + '/rentacarserviceadmin/get-racs-address';
    console.log(url);
    return this.http.get<any>(url);
  }

  getRACs(): Observable<any> {
    const url = this.BaseURI + '/home/rent-car-services';
    return this.http.get<any>(url);
  }

  getTopRatedRACs(): Observable<any> {
    const url = this.BaseURI + '/home/get-toprated-racs';
    console.log(url);
    return this.http.get<any>(url);
  }

  getRACProfile(data: any) {
    const url = `${this.BaseURI + '/home/rent-car-service'}/${data}`;
    return this.http.get(url);
  }

  // *************************************************************************************************************

  addBranch(data: any) {
    console.log(data);
    const body = {
      State: data.State,
      City: data.City,
    };
    const url = this.BaseURI + '/rentacarserviceadmin/add-branch';
    return this.http.post(url, body);
  }

  deleteBranch(data: any) {
    const url = this.BaseURI + '/rentacarserviceadmin/delete-branch/' + data.id;
    return this.http.delete(url);
  }

  getAdminsBranches(): Observable<any> {
    const url = this.BaseURI + '/rentacarserviceadmin/get-racs-branches';
    console.log(url);
    return this.http.get<any>(url);
  }

  // addCarsPhoto(data: any) {
  //   const formData = new FormData();
  //   formData.append('img', data.image);

  //   const url = this.BaseURI + '/rentacarserviceadmin/add-car-img';
  //   return this.http.put(url, formData);

  // }

  // *************************************************************************************************************

  test(data: any): Observable<any> {
    // console.log(data);
    const param = {
      from: data.from,
      to: data.to,
      dep: data.dep,
      ret: data.ret,
      seatfrom: data.seatFrom,
      seatto: data.seatTo,
      minprice: data.minPrice,
      maxprice: data.maxPrice,
      racs: data.racs,
      type: data.type
    };
    const url = this.BaseURI + '/home/cars';
    return this.http.get<any>(url, {params: param});
  }

  getAllCars() {

    return this.cars;
  }

  addCar(data: any) {
    const body = {
      Brand: data.Brand,
      Model: data.Model,
      Year: data.Year,
      Type: data.Type,
      SeatsNumber: data.SeatsNumber,
      PricePerDay: data.PricePerDay,
    };
    const url = this.BaseURI + '/rentacarserviceadmin/add-car';
    return this.http.post(url, body);
  }

  editCar(data: any) {
    const body = {
      Brand: data.Brand,
      Model: data.Model,
      Year: data.Year,
      Type: data.Type,
      SeatsNumber: data.SeatsNumber,
      PricePerDay: data.PricePerDay,
    };
    const url = `${this.BaseURI + '/rentacarserviceadmin/change-car-info'}/${data.id}`;
    return this.http.put(url, body);
  }

  addCarToBranch(data: any) {
    const body = {
      BranchId: data.BranchId,
      Brand: data.Brand,
      Model: data.Model,
      Year: data.Year,
      Type: data.Type,
      SeatsNumber: data.SeatsNumber,
      PricePerDay: data.PricePerDay,
    };
    const url = this.BaseURI + '/rentacarserviceadmin/add-car-to-branch';
    return this.http.post(url, body);
  }

  deleteCar(data: any) {
    const url = this.BaseURI + '/rentacarserviceadmin/delete-car/' + data.id;
    return this.http.delete(url);
  }

  getCar(data: any) {
    const url = `${this.BaseURI + '/rentacarserviceadmin/get-car'}/${data}`;
    return this.http.get(url);
  }

  getAdminsCars(): Observable<any> {
    const url = this.BaseURI + '/rentacarserviceadmin/get-racs-cars';
    return this.http.get<any>(url);
  }

  getBranchesCars(data: any): Observable<any> {
    const url = `${this.BaseURI + '/rentacarserviceadmin/get-branch-cars'}/${data}`;
    return this.http.get<any>(url);
  }

  // *************************************************************************************************************

  addSpecialOffer(data: any) {
    console.log(data);
    const body = {
      FromDate: data.FromDate,
      ToDate: data.ToDate,
      NewPrice: data.NewPrice,
    };
    const url = `${this.BaseURI + '/rentacarserviceadmin/add-car-specialoffer'}/${data.id}`;
    return this.http.post(url, body);
  }

  getAdminsSpecialOffers(): Observable<any> {
    const url = this.BaseURI + '/rentacarserviceadmin/get-cars-specialoffers';
    return this.http.get<any>(url);
  }

  getRACSpecialOffers(data): Observable<any> {
    const url = `${this.BaseURI + '/home/racs-specialoffers'}/${data}`;
    console.log(url);
    return this.http.get<any>(url);
  }

  getAllSpecialOffers(): Observable<any> {
    // let params = {
    //   param1: param1Value,
    //   param2: param2Value
    // };
    // this.router.navigate('/segment1/segment2', { queryParams: params });
    const url = this.BaseURI + '/home/all-racs-specialoffers';
    console.log(url);
    return this.http.get<any>(url);
  }

  getStats(data): Observable<any> {
    // saljem kao query ili ovako?
    const param = {
      from: data.from,
      to: data.to,
      isFree: data.isFree
    };
    const url = this.BaseURI + '/rentacarserviceadmin/get-car-report';
    console.log(url);
    return this.http.get<any>(url, {params: param});
  }

  getStatsForYear(data: any): Observable<any> {
    const param = {
      year: data.year // 2020-09-25
    };
    const url = this.BaseURI + '/rentacarserviceadmin/get-income-year';
    return this.http.get<any>(url, {params: param});
  }

  getStatsForWeek(data: any): Observable<any> {
    const param = {
      week: data.week // 2020-W34 to je 34. nedelja
    };
    const url = this.BaseURI + '/rentacarserviceadmin/get-income-week';
    return this.http.get<any>(url, {params: param});
  }

  getStatsForMonth(data: any): Observable<any> {
    const param = {
      month: data.month // 2020-09
    };
    const url = this.BaseURI + '/rentacarserviceadmin/get-income-month';
    return this.http.get<any>(url, {params: param});
  }


  getReservationStatsForDate(data: any): Observable<any> {
    const param = {
      date: data.date // 2020-09-25
    };
    const url = this.BaseURI + '/rentacarserviceadmin/get-stats-date';
    return this.http.get<any>(url, {params: param});
  }

  getReservationStatsForWeek(data: any): Observable<any> {
    const param = {
      week: data.week // 2020-W34 to je 34. nedelja
    };
    const url = this.BaseURI + '/rentacarserviceadmin/get-stats-week';
    return this.http.get<any>(url, {params: param});
  }

  getReservationStatsForMonth(data: any): Observable<any> {
    const param = {
      month: data.month // 2020-09
    };
    const url = this.BaseURI + '/rentacarserviceadmin/get-stats-month';
    return this.http.get<any>(url, {params: param});
  }




  getTotalPriceForResevation(data: any) {
    console.log(data);
    const param = {
      from: data.from,
      to: data.to,
      dep: data.dep,
      ret: data.ret,
      carId: data.carId
    };
    const url = this.BaseURI + '/user/rent-total-price';
    return this.http.get(url, {params: param});
  }

  // getUsersCarReservations(): Observable<any> {
  //   return this.http.get<any>(this.BaseURI + '/user/get-car-reservations');
  // }

  getUsersCarReservations(): Observable<any> {
    return this.http.get<any>(this.BaseURI + '/user/get-car-reservations');
  }

  // getUsersPreviousCarReservations(): Observable<any> {
  //   return this.http.get<any>(this.BaseURI + '/user/get-previous-car-reservations');
  // }

  rateCar(data) {
    console.log(data);
    const body = {
      Id: data.carId,
      Rate: data.rate,
    };
    const url = this.BaseURI + '/user/rate-car';
    return this.http.post(url, body);
  }

  rateCarService(data) {
    console.log(data);
    const body = {
      Id: data.carServiceId,
      Rate: data.rate,
    };
    const url = this.BaseURI + '/user/rate-car-service';
    return this.http.post(url, body);
  }

  quitReservation(data: any) {
    console.log('bla');
    const body = {
      ReservationId: data.reservationId
    };

    const url = `${this.BaseURI + '/user/cancel-rent'}/${data.reservationId}`;
    return this.http.delete(url); // promeni u delete ako treba
  }

  getFriends(): Observable<any> {
    return this.http.get<any>(this.BaseURI + '/user/get-friends');
    // return this.http.get<any>(this.BaseURI + '/systemadmin/register-systemadmin', body);
  }

  reserveCar(data) {
    console.log(data);
    const body = {
      CarRentId: data.carId,
      TakeOverCity: data.from,
      ReturnCity: data.to,
      TakeOverDate: data.dep, // pocetni datum
      ReturnDate: data.ret, // datum vracanja auta
      TotalPrice: data.totalPrice, // uzracunata na osnovu cene po danu
    };
    const url = this.BaseURI + '/user/rent-car';
    return this.http.post(url, body);
  }

  reserveCarSpecialOffer(data) {
    console.log(data);
    const body = {
      Id: data.id
    };
    const url = this.BaseURI + '/user/reserve-special-offer-car';
    return this.http.post(url, body);
  }


  allMockedRentServices() {}

  mock() {
    const c1 = {
      brand: 'Range Rover',
      carId: 0,
      city: 'Berlin',
      model: 'Evoque',
      name: 'Hertz',
      pricePerDay: 50,
      seatsNumber: 4,
      state: 'Germany',
      type: 'Luxury',
      year: 2020
    };
    const c2 = {
      brand: 'Range Rover',
      carId: 1,
      city: 'Berlin',
      model: 'Evoque',
      name: 'Hertz',
      pricePerDay: 50,
      seatsNumber: 4,
      state: 'Germany',
      type: 'Luxury',
      year: 2020
    };
    const c3 = {
      brand: 'Range Rover',
      carId: 2,
      city: 'Berlin',
      model: 'Evoque',
      name: 'Hertz',
      pricePerDay: 50,
      seatsNumber: 4,
      state: 'Germany',
      type: 'Luxury',
      year: 2020
    };
    const c4 = {
      brand: 'Range Rover',
      carId: 3,
      city: 'Berlin',
      model: 'Evoque',
      name: 'Hertz',
      pricePerDay: 50,
      seatsNumber: 4,
      state: 'Germany',
      type: 'Luxury',
      year: 2020
    };

    // this.cars.push(c1);
    // this.cars.push(c2);
    // this.cars.push(c3);
    // this.cars.push(c4);
  }
}
