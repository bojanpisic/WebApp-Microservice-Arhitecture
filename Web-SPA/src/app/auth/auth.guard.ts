import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';
import { UserService } from 'src/services/user.service';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {


  constructor(private router: Router, private cookieService: CookieService,
              private userService: UserService, private toastr: ToastrService) {
  }
  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      const jwt = this.cookieService.get('jwt');
      if (localStorage.getItem('token') != null) {
        const roles = next.data['permittedRoles'] as Array<string>;
        if (roles) {
          if (this.userService.roleMatch(roles)) {
            if (roles.includes('AirlineAdmin' || 'RentACarServiceAdmin')) {
              if (!this.userService.hasChangedPassword()) {
                const payload = JSON.parse(window.atob(localStorage.getItem('token').split('.')[1]));
                const userId = payload.UserID;
                this.toastr.error('You have to change password first!', 'Error!');
                if (roles.includes('AirlineAdmin')) {
                  this.router.navigate(['/admin/' + userId + '/profile/edit-profile']);
                } else {
                  this.router.navigate(['/rac-admin/' + userId + '/profile/edit-profile']);
                }
                return false;
              }
            }
            return true;
          } else {
            this.toastr.error('You have no permission for this route', 'Error!');
            return false;
          }
        }
        // const userRole = JSON.parse(localStorage.getItem('sessionUserRole'));
        // console.log(userRole);
        // if (userRole === 'Admin') {
        //   return true;
        // } else {
        //   alert('You have no premission!');
        // }
      } else {
        this.toastr.error('You are not loggen in!', 'Error!');
        this.router.navigate(['/signin']);
        return false;
      }
  }
}
