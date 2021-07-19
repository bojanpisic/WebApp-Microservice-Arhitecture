import { Component, OnInit} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { UserService } from 'src/services/user.service';
import * as jwt_decode from 'jwt-decode';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-signin',
  templateUrl: './signin.component.html',
  styleUrls: ['./signin.component.scss', '../signup/signup.component.scss']
})

export class SigninComponent implements OnInit {

  userId: number;
  token: any;

  loggedIn = false;

  errorUsername = false;
  errorPassword = false;

  constructor(private route: ActivatedRoute,
              private router: Router,
              public userService: UserService,
              private toastr: ToastrService) {
    route.params.subscribe(params => {
      this.userId = params.id;
      this.token = params.token;
    });
  }

  ngOnInit(): void {
    if (this.userId === undefined && this.token === undefined) {
      if (localStorage.getItem('token') != null) {

        const token = localStorage.getItem('token');
        const decoded = this.getDecodedAccessToken(token);
        if (token == null || decoded.exp >= Date.now()) {
            this.toastr.error('You are not registered.', 'Error.');
            return ;
        }
        // this.router.navigateByUrl(decoded + '/home');
      }
    }
  }

  validateForm() {
    let retVal = true;
    if (this.userService.formModel.controls.UserName.invalid) {
      this.errorUsername = true;
      retVal = false;
    }
    if (this.userService.formModel.controls.Password.invalid) {
      this.errorPassword = true;
      retVal = false;
    }
    return retVal;
  }

  signInClick() {
    if (this.validateForm()) {
      const data = {
        userId: this.userId,
        token: this.token
      };

      this.userService.logIn(data).subscribe(
        (res: any) => {
          localStorage.setItem('token', res.token);
          const decoded = this.getDecodedAccessToken(res.token);

          if (res.token == null || decoded.exp >= Date.now()) {
              this.toastr.error('You are not registered.', 'Error.');
              return ;
          }
          console.log(res);
          switch (decoded.Roles) {
            case 'RegularUser':
              this.router.navigateByUrl(decoded.UserID + '/home');
              break;
            case 'AirlineAdmin':
              console.log(decoded.PasswordChanged);
              if (decoded.PasswordChanged === 'True') {
                this.router.navigateByUrl('/admin/' + decoded.UserID);
              } else {
                this.router.navigateByUrl('/admin/' + decoded.UserID + '/profile/edit-profile');
              }
              break;
            case 'RentACarServiceAdmin':
              console.log(decoded.PasswordChanged);
              if (decoded.PasswordChanged === 'True') {
                this.router.navigateByUrl('/rac-admin/' + decoded.UserID);
              } else {
                this.router.navigateByUrl('/rac-admin/' + decoded.UserID + '/profile/edit-profile');
              }
              break;
            case 'Admin':
              this.router.navigateByUrl('/system-admin/' + decoded.UserID);
              break;
          }

        },
        err => {
          // tslint:disable-next-line: triple-equals
          this.toastr.error(err.error, 'Error.');
        }
      );
    }
  }


    getDecodedAccessToken(token: string): any {
      try {
          return jwt_decode(token);
      } catch (Error) {
          return null;
      }
    }

    // let loggedUser = this.userService.logIn((document.getElementById('email') as HTMLInputElement).value,
    // (document.getElementById('password') as HTMLInputElement).value);
    // if (loggedUser) {
    //   alert('you are logged');

    //   (loggedUser.userType === 'airlineAdmin' && this.username === undefined) ?
    //   this.router.navigate(['/airlines/' + loggedUser.id + '/flight-add']) :
    //   (loggedUser.userType === 'regular' && this.username === undefined) ?
    //   (this.router.navigate([loggedUser.id + '/home']), localStorage.setItem(loggedUser.id.toString(), JSON.stringify(loggedUser))) :
    //   this.username === undefined ? this.router.navigate(['/']) :
    //   this.router.navigate([this.username + '/' + this.option + '/register-company']);
    // } else {
    //   alert('logging error');
    // }

}
