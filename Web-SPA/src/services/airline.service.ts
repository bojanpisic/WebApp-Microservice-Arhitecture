import { Injectable } from '@angular/core';
import { Airline } from 'src/app/entities/airline';
import { Destination } from 'src/app/entities/destination';
import { Flight } from 'src/app/entities/flight';
import { TripId } from 'src/app/entities/trip-id';
import { Address } from 'src/app/entities/address';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AirlineService {

  readonly BaseURI = 'http://localhost:8084';

  airlines: Array<Airline>;
  constructor(private http: HttpClient) {
    this.airlines = new Array<Airline>();
    this.allMockedAirlines();
   }

   reserveTrip(data) {
    console.log('SALJEM:', data);
    const body = {
      MySeatsIds: data.mySeatsIds,
      MyPassport: data.myPassport,
      Friends: data.friends,
      UnregisteredFriends: data.unregisteredFriends,
      CarReservation: data.carReservation,
      WithBonus: data.withBonus
    };
    const url = this.BaseURI + '/airline/reservation/flight-reservation';
    return this.http.post(url, body);
  }

  reserveSpecialOffer(data) {
    console.log(data);
    const body = {
      Id: data.id,
      Passport: data.passport
    };
    const url = this.BaseURI + '/airline/reservation/reserve-special-offer-flight';
    return this.http.post(url, body);
  }

  getPreviousFlights(): Observable<any> {
    return this.http.get<any>(this.BaseURI + '/airline/reservation/get-previous-flights');
  }

  getUpcomingTrips(): Observable<any> {
    return this.http.get<any>(this.BaseURI + '/airline/reservation/get-upcoming-trips');
  }

  quitReservation(data: any) {
    const body = {
      ReservationId: data.reservationId
    };

    const url = `${this.BaseURI + '/airline/reservation/cancel-flight-reservation'}/${data.reservationId}`;
    return this.http.delete(url); // promeni u delete ako treba
  }

  rateFlight(data) {
    console.log(data);
    const body = {
      Id: data.id,
      Rate: data.rate.rate,
    };
    const url = this.BaseURI + '/user/rate-flight';
    return this.http.post(url, body);
  }

  rateAirline(data) {
    console.log(data);
    const body = {
      Id: data.id,
      Rate: data.rate,
    };
    const url = this.BaseURI + '/user/rate-airline';
    return this.http.post(url, body);
  }

   test(data: any): Observable<any> {
    console.log(data);
    const param = {
      type: data.type,
      from: data.from,
      to: data.to,
      dep: data.dep,
      ret: data.ret,
      minPrice: data.minPrice,
      maxPrice: data.maxPrice,
      class: data.class,
      air: data.air,
      mind: data.mind,
      maxd: data.maxd
    };
    const url = this.BaseURI + '/airline/home/flights';
    return this.http.get<any>(url, {params: param});
  }

  getTripInfo(data: any): Observable<any> {
    console.log(data);
    const body = {
      MySeatsIds: data.mySeatsIds,
      Friends: data.friends,
      UnregisteredFriends: data.unregisteredFriends
    };
    const url = this.BaseURI + '/user/get-trip-info';
    return this.http.post(url, body);
  }

  getAirline(data: any): Observable<any> {
    const url = this.BaseURI + '/airline/get-airline';
    console.log(url);
    return this.http.get<any>(url);
  }

  getAirlines(): Observable<any> {
    const url = this.BaseURI + '/airline/home/all-airlines';
    return this.http.get<any>(url);
  }

  getAirlineProfile(data: any) {
    const url = `${this.BaseURI + '/airline/home/airline'}/${data}`;
    return this.http.get(url);
  }

  getTopRatedAirlines(): Observable<any> {
    const url = this.BaseURI + '/airline/home/get-toprated-airlines';
    console.log(url);
    return this.http.get<any>(url);
  }

  editAirline(data: any) {
    const body = {
      Name: data.name,
      Address: data.address,
      PromoDescription: data.promoDescription,
    };
    const url = this.BaseURI + '/airlineadmin/change-airline-info';
    return this.http.put(url, body);
  }


  getAirlinePhoto(data: any): Observable<any> {
    const url = `${this.BaseURI + '/airline/get-airline-logo'}/${data}`;
    console.log(url);
    return this.http.get(url);
  }

  changePhoto(data: any) {
    const formData = new FormData();
    formData.append('img', data.image);

    const url = this.BaseURI + '/airline/change-airline-logo';
    return this.http.put(url, formData);
  }


  getAdminsFlights(): Observable<any> {
    const url = this.BaseURI + '/airline/flight/get-flights';
    console.log(url);
    return this.http.get<any>(url);
  }

  getFlightsSeats(data: any): Observable<any> {
    const url = `${this.BaseURI + '/airline/seat/get-seats'}/${data}`;
    return this.http.get<any>(url);
  }

  addFlight(data: any) {
    console.log(data);
    const body = {
      FlightNumber: data.FlightNumber,
      TakeOffDateTime: data.TakeOffDateTime,
      LandingDateTime: data.LandingDateTime,
      StopIds: data.StopIds, // lista ideja destinacija
      FromId: data.FromId,
      ToId: data.ToId,
      Seats: data.Seats, // Column, Row, Class, Price
      TripLength: data.TripLength
    };
    const url = this.BaseURI + '/airline/flight/add-flight';
    return this.http.post(url, body);
  }

  getStatsForYear(data: any): Observable<any> {
    const param = {
      year: data.year // 2020
    };
    const url = this.BaseURI + '/airline/chart/get-income-year';
    return this.http.get<any>(url, {params: param});
  }

  getStatsForWeek(data: any): Observable<any> {
    const param = {
      week: data.week // 2020-W34 to je 34. nedelja
    };
    const url = this.BaseURI + '/airline/chart/get-income-week';
    return this.http.get<any>(url, {params: param});
  }

  getStatsForMonth(data: any): Observable<any> {
    const param = {
      month: data.month // 2020-09
    };
    const url = this.BaseURI + '/airline/chart/get-income-month';
    return this.http.get<any>(url, {params: param});
  }

  getTicketStatsForDate(data: any): Observable<any> {
    const param = {
      date: data.date // 2020
    };
    const url = this.BaseURI + '/airline/chart/get-stats-date';
    return this.http.get<any>(url, {params: param});
  }

  getTicketStatsForWeek(data: any): Observable<any> {
    const param = {
      week: data.week // 2020-W34 to je 34. nedelja
    };
    const url = this.BaseURI + '/airline/chart/get-stats-week';
    return this.http.get<any>(url, {params: param});
  }

  getTicketStatsForMonth(data: any): Observable<any> {
    const param = {
      month: data.month // 2020-09
    };
    const url = this.BaseURI + '/airline/chart/get-stats-month';
    return this.http.get<any>(url, {params: param});
  }


  getAdminsDestinations(): Observable<any> {
    const url = this.BaseURI + '/airline/destination/get-airline-destinations';
    console.log(url);
    return this.http.get<any>(url);
  }

  addDestination(data: any) {
    console.log(data);
    const body = {
      State: data.state,
      City: data.city,
      ImgUrl: data.imgUrl,
    };
    const url = this.BaseURI + '/airline/destination/add-destination';
    return this.http.post(url, body);
  }

  deleteDestination(data: any) {
    const url = this.BaseURI + '/airline/destination/delete-destination/' + data.id;
    return this.http.delete(url);
  }

  addSeat(data: any) {
    const body = {
      Class: data.class,
      Column: data.column,
      Row: data.row,
      Price: data.price,
      FlightId: data.flightId
    };
    const url = this.BaseURI + '/airline/seat/add-seat';
    return this.http.post(url, body);
  }

  deleteSeat(data: any) {
    const url = this.BaseURI + '/airline/seat/delete-seat/' + data;
    return this.http.delete(url);
  }

  changeSeat(data: any) {
    const url = `${this.BaseURI + '/airline/seat/change-seat'}/${data.id}`;
    const body = {
      Price: data.price,
    };
    return this.http.put(url, body);
  }

  getAdminsSpecialOffers(): Observable<any> {
    const url = this.BaseURI + '/airline/get-specialoffer';
    console.log(url);
    return this.http.get<any>(url);
  }

  getAirlineSpecialOffers(data): Observable<any> {
    const url = `${this.BaseURI + 'airline/home/airline-special-offers'}/${data}`;
    console.log(url);
    return this.http.get<any>(url);
  }

  getAllSpecialOffers(): Observable<any> {
    const url = this.BaseURI + '/airline/home/all-airlines-specialoffers';
    console.log(url);
    return this.http.get<any>(url);
  }

  addSpecialOffer(data: any) {
    console.log(data);
    const body = {
      NewPrice: data.NewPrice,
      SeatsIds: data.SeatsIds
    };
    const url = this.BaseURI + '/airline/add-specialoffer';
    return this.http.post(url, body);
  }

  loadAllAirlines() {
    const retValue = new Array<Airline>();
    for (const item of this.airlines) {
      retValue.push(item);
    }
    return retValue;
  }

  getAdminsAirlineId(adminId: number) {
    let retVal;
    this.airlines.forEach(airline => {
      if (airline.adminid == adminId) {
        retVal = airline.id;
      }
    });
    return retVal;
  }

  getAdminsAirline(adminId: number) {
    let retVal;
    this.airlines.forEach(airline => {
      if (airline.adminid == adminId) {
        retVal = airline;
      }
    });
    return retVal;
  }

  getFlightSeats(data): Observable<any> {
    const param = {
      ids: data
    };
    const url = this.BaseURI + '/airline/home/flight-seats';
    return this.http.get<any>(url, {params: param});
  }

  getFlights(airlineId: number) {
    let f;
    this.airlines.forEach(airline => {
      if (airline.id === airlineId) {
        f = airline.flights;
      }
    });
    return f;
  }

  getAirlineId(adminId: number) {
    let retVal;
    this.airlines.forEach(airline => {
      if (airline.adminid === adminId) {
        retVal = airline.id;
      }
    });
    return retVal;
  }

  allMockedAirlines() {
  }
}
