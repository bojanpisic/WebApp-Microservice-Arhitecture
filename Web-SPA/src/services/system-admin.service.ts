import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class SystemAdminService {

  readonly BaseURI = 'http://localhost:8084/user';


  constructor(private http: HttpClient) { }

  registerSystemAdmin(data: any) {
    const body = {
      UserName: data.username,
      Email: data.email,
      ConfirmPassword: data.confirmPassword,
      Password: data.password,
    };
    return this.http.post(this.BaseURI + '/systemadmin/register-systemadmin', body);
  }

  registerAirline(data: any) {
    const body = {
      UserName: data.username,
      Email: data.email,
      ConfirmPassword: data.confirmPassword,
      Password: data.password,
      Name: data.companyName,
      Address: { City: data.city, State: data.state, Lat: data.lat, Lon: data.lon }
    };
    return this.http.post(this.BaseURI + '/systemadmin/register-airline', body);
  }

  registerRACService(data: any): Observable<any> {
    console.log(data);
    const body = {
      UserName: data.username,
      Email: data.email,
      ConfirmPassword: data.confirmPassword,
      Password: data.password,
      Name: data.companyName,
      Address: { City: data.city, State: data.state, Lat: data.lat, Lon: data.lon }
    };
    return this.http.post(this.BaseURI + '/systemadmin/register-racservice', body);
  }

  configureBonusAndDiscount(data: any): Observable<any> {
    console.log(data);
    const body = {
      Bonus: data.bonus,
      Discount: data.discount,
    };
    const url = this.BaseURI + '/systemadmin/set-bonus';
    return this.http.post(url, body);
  }

  getBonusAndDiscount(): Observable<any> {
    const url = this.BaseURI + '/systemadmin/get-bonus';
    return this.http.get(url);
  }

  private handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error.message);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      console.error(
        `Backend returned code ${error.status}, ` +
        `body was: ${error.error}`);
    }
    // return an observable with a user-facing error message
    return throwError(
      'Something bad happened; please try again later.');
  }
}
