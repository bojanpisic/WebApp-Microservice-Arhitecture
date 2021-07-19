import { Injectable } from '@angular/core';
import { User } from 'src/app/entities/user';
import { ThrowStmt, ConditionalExpr } from '@angular/compiler';
import { RegisteredUser } from 'src/app/entities/registeredUser';
import { Flight } from 'src/app/entities/flight';
import { Destination } from 'src/app/entities/destination';
import { ChangeOver } from 'src/app/entities/changeOver';
import { Seat } from 'src/app/entities/seat';
import { Address } from 'src/app/entities/address';
import { Message } from '../app/entities/message';
import { AirlineAdmin } from 'src/app/entities/airlineAdmin';
import { RacAdmin } from 'src/app/entities/racAdmin';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CustomValidationService } from './custom-validation.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  readonly BaseURI = 'http://localhost:8084/user';

  constructor(private customValidator: CustomValidationService, private fb: FormBuilder, private http: HttpClient) {

   }

   formModel = this.fb.group({
    UserName: ['', Validators.required],
    Email: ['', [Validators.required, Validators.pattern('^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$')]],
    FirstName: ['', Validators.required],
    LastName: ['', Validators.required],
    Phone: ['', [Validators.required, Validators.pattern('^[(][+][0-9]{3}[)][0-9]{2}[/][0-9]{3}[-][0-9]{3,4}')]],
    City: ['', Validators.required],
    Password: ['', [Validators.required, this.customValidator.patternValidator()]],
    ConfirmPassword: ['', Validators.required],
    ImageUrl: [''],
  });

   formModelLogin = this.fb.group({
    UserNameOrEmail: ['', Validators.required],
    Password: ['', [Validators.required]],
  });


  getUser(data: any): Observable<any> {
    return this.http.get<any>(this.BaseURI + '/profile/get-profile');
    // return this.http.get<any>(this.BaseURI + '/systemadmin/register-systemadmin', body);
  }

  getNonFriends(): Observable<any> {
    console.log('nonfriends');
    return this.http.get<any>(this.BaseURI + '/get-all-users');
    // return this.http.get<any>(this.BaseURI + '/systemadmin/register-systemadmin', body);
  }

  getFriends(): Observable<any> {
    console.log('friends');
    return this.http.get<any>(this.BaseURI + '/get-friends');
    // return this.http.get<any>(this.BaseURI + '/systemadmin/register-systemadmin', body);
  }

  getRequests(): Observable<any> {
    return this.http.get<any>(this.BaseURI + '/get-user-requests');
    // return this.http.get<any>(this.BaseURI + '/systemadmin/register-systemadmin', body);
  }

  getFlightInvitations(): Observable<any> {
    return this.http.get<any>(this.BaseURI + '/get-trip-invitations');
    // return this.http.get<any>(this.BaseURI + '/systemadmin/register-systemadmin', body);
  }

  addFriend(data: any) {
    console.log(data);
    const body = {
      UserId: data
    };
    const url = this.BaseURI + '/send-friendship-invitation';
    return this.http.post(url, body);
  }

  removeFriend(data: any) {
    console.log(data);
    const body = {
      UserId: data
    };
    const url = this.BaseURI + '/delete-friend';
    return this.http.post(url, body);
  }

  acceptFriendship(data: any) {
    const body = {
      UserId: data
    };
    const url = this.BaseURI + '/accept-friendship';
    return this.http.post(url, body);
  }

  declineFriendship(data: any) {
    const body = {
      UserId: data
    };
    const url = this.BaseURI + '/reject-request';
    return this.http.post(url, body);
  }
  acceptTripInvitation(data: any) {
    const body = {
      Id: data.id,
      Passport: data.passport
    };
    const url = this.BaseURI + '/accept-trip-invitation';
    return this.http.post(url, body);
  }

  declineTripInvitation(data: any) {
    const url = `${this.BaseURI + '/reject-trip-invitation'}/${data.id}`;
    return this.http.delete(url);
  }
  changePhoto(data: any) {
    const formData = new FormData();
    formData.append('img', data.image);

    const url = `${this.BaseURI + '/profile/change-img'}/${data.id}`;
    return this.http.put(url, formData);
  }

  changeFirstName(data: any) {
    const body = {
      FirstName: data.FirstName
    };
    const url = `${this.BaseURI + '/profile/change-firstname'}/${data.id}`;
    return this.http.put(url, body);
  }

  changeLastName(data: any) {
    const body = {
      LastName: data.LastName
    };
    const url = `${this.BaseURI + '/profile/change-lastname'}/${data.id}`;
    return this.http.put(url, body);
  }

  changeEmail(data: any) {
    const body = {
      Email: data.Email
    };
    const url = `${this.BaseURI + '/profile/change-email'}/${data.id}`;
    return this.http.put(url, body);
  }

  changeUsername(data: any) {
    const body = {
      UserName: data.UserName
    };
    const url = `${this.BaseURI + '/profile/change-username'}/${data.id}`;
    return this.http.put(url, body);
  }

  changePhone(data: any) {
    const body = {
      Phone: data.Phone
    };
    const url = `${this.BaseURI + '/profile/change-phone'}/${data.id}`;
    return this.http.put(url, body);
  }

  changeAddress(data: any) {
    const body = {
      City: data.City
    };
    const url = `${this.BaseURI + '/profile/change-city'}/${data.id}`;
    return this.http.put(url, body);
  }

  changePassword(data: any) {
    console.log(data);
    const body = {
      OldPassword: data.OldPassword,
      Password: data.Password,
      PasswordConfirm: data.PasswordConfirm,
    };
    const url = `${this.BaseURI + '/profile/change-passw'}/${data.id}`;
    return this.http.put(url, body);
  }

  comparePasswords(fb: FormGroup) {
    const confirmPswrdCtrl = fb.get('ConfirmPassword');
    // passwordMismatch
    // confirmPswrdCtrl.errors={passwordMismatch:true}
    if (confirmPswrdCtrl.errors == null || 'passwordMismatch' in confirmPswrdCtrl.errors) {
      if (fb.get('Password').value != confirmPswrdCtrl.value) {
        confirmPswrdCtrl.setErrors({ passwordMismatch: true });
      } else {
        confirmPswrdCtrl.setErrors(null);
      }
    }
  }



  AirlineAdminRegistration() {
    // tslint:disable-next-line: prefer-const
    let body = {
      UserName: this.formModel.value.UserName,
      Email: this.formModel.value.Email,
      FirstName: this.formModel.value.FirstName,
      LastName: this.formModel.value.LastName,
      Phone: this.formModel.value.Phone,
      City: this.formModel.value.City,
      ConfirmPassword: this.formModel.value.ConfirmPassword,
      Password: this.formModel.value.Passwords.Password,
    };
    return this.http.post(this.BaseURI + '/authentication/RegisterAirlineAdmin', body);
  }

  userRegistration() {
    // tslint:disable-next-line: prefer-const
    const body = {
      UserName: this.formModel.value.UserName,
      Email: this.formModel.value.Email,
      FirstName: this.formModel.value.FirstName,
      LastName: this.formModel.value.LastName,
      Phone: this.formModel.value.Phone,
      City: this.formModel.value.City,
      ConfirmPassword: this.formModel.value.ConfirmPassword,
      Password: this.formModel.value.Password,
    };
    console.log(body);
    return this.http.post(this.BaseURI + '/authentication/register-user', body);
  }

  RentCarAdminRegistration() {
    // tslint:disable-next-line: prefer-const
    let body = {
      UserName: this.formModel.value.UserName,
      Email: this.formModel.value.Email,
      FirstName: this.formModel.value.FirstName,
      LastName: this.formModel.value.LastName,
      Phone: this.formModel.value.Phone,
      City: this.formModel.value.City,
      ConfirmPassword: this.formModel.value.ConfirmPassword,
      Password: this.formModel.value.Passwords.Password,
    };
    return this.http.post(this.BaseURI + '/authentication/RegisterRentCarAdmin', body);
  }

  SystemAdminRegistration() {
    // tslint:disable-next-line: prefer-const
    const body = {
      UserName: this.formModel.value.UserName,
      Email: this.formModel.value.Email,
      FirstName: this.formModel.value.FirstName,
      LastName: this.formModel.value.LastName,
      Phone: this.formModel.value.Phone,
      City: this.formModel.value.City,
      ConfirmPassword: this.formModel.value.ConfirmPassword,
      Password: this.formModel.value.Password,
    };
    return this.http.post(this.BaseURI + '/authentication/RegisterSytemAdmin', body);
  }

  updateUser(user: RegisteredUser) {
    // const index = this.allUsers.indexOf(user);
    // this.allUsers[index] = user;
  }

  logIn(data: any) {
    const body = {
      UserNameOrEmail: this.formModel.value.UserName,
      Password: this.formModel.value.Password,
      UserId: data.userId,
      Token: data.token
    };
    console.log("body:" + body.UserId + "     " + body.Token);
    return this.http.post(this.BaseURI + '/authentication/login', body);
  }

  externalLogin(formData) {
    return this.http.post(this.BaseURI + '/authentication/social-login', formData);
  }

  getAllUsers() {
    // return this.allUsers;
    return null;
  }



  // getUser(id: number) {
  //   // return this.allUsers.find(x => x.id == id);
  //   return null;

  // }

  getAirlineAdmin(id: number) {
    // return this.airlineAdmins.find(x => x.id == id);
    return null;

  }

  getRACAdmin(id: number) {
    // return this.racAdmins.find(x => x.id == id);
    return null;

  }

  roleMatch(allowedRoles): boolean {
    console.log(allowedRoles);
    let isMatch = false;
    const payLoad = JSON.parse(window.atob(localStorage.getItem('token').split('.')[1]));
    const userRole = payLoad.Roles;
    console.log(userRole);
    allowedRoles.forEach(element => {
      if (userRole == element) {
        isMatch = true;
      }
    });
    return isMatch;
  }

  hasChangedPassword(): boolean {
    const payLoad = JSON.parse(window.atob(localStorage.getItem('token').split('.')[1]));
    console.log(payLoad);
    const passwordChanged = payLoad.PasswordChanged;
    if (passwordChanged === "False") {
      return false;
    } else {
      localStorage.setItem('PasswordChanged', 'True');
      const payLoad1 = JSON.parse(window.atob(localStorage.getItem('token').split('.')[1]));
      console.log(payLoad1);
    }
    return true;
  }

}
