import { Component, OnInit } from '@angular/core';
// import { AuthService, GoogleLoginProvider, FacebookLoginProvider } from 'angular-6-social-login';
import {GoogleLoginProvider, SocialAuthService, FacebookLoginProvider} from 'angularx-social-login';
import { UserService } from 'src/services/user.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import * as jwt_decode from 'jwt-decode';

@Component({
  selector: 'app-social-network',
  templateUrl: './social-network.component.html',
  styleUrls: ['./social-network.component.scss']
})
export class SocialNetworkComponent implements OnInit {

  constructor( private userService: UserService,
               private router: Router,
               private OAuth: SocialAuthService,
               private toastr: ToastrService)
     {
     }

  ngOnInit(): void {
  }

  LoginWithGoogle() {
    let socialPlatformProvider;
    socialPlatformProvider = GoogleLoginProvider.PROVIDER_ID;

    // this.OAuth.signIn(GoogleLoginProvider.PROVIDER_ID).then(s => {
    //   console.log(s);
    // });
    this.OAuth.signIn(socialPlatformProvider).then(socialusers => {
      console.log(socialusers);
      this.userService.externalLogin(socialusers).subscribe(
        (res: any) => {
        localStorage.setItem('token', res.token);
        const decoded = this.getDecodedAccessToken(res.token);
        console.log(decoded.Roles);
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
        this.toastr.error(err.error, 'Error!');
      });

      console.log(socialusers);
    });

  }

  LoginWithFacebook() {
    let socialPlatformProvider;
    socialPlatformProvider = FacebookLoginProvider.PROVIDER_ID;

    // this.OAuth.signIn(FacebookLoginProvider.PROVIDER_ID).then(socialusers => {
    //   console.log(socialusers);
    // });

    this.OAuth.signIn(socialPlatformProvider).then(socialusers => {
      console.log(socialusers);

      this.userService.externalLogin(socialusers).subscribe(
        (res: any) => {
        localStorage.setItem('token', res.token);
        this.router.navigateByUrl('/home');
      });

      console.log(socialusers);
    });

  }

  getDecodedAccessToken(token: string): any {
    try {
        return jwt_decode(token);
    } catch (Error) {
        return null;
    }
  }
}
