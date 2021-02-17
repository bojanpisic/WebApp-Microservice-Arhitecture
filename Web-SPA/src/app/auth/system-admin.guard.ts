import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class SystemAdminGuard implements CanActivate {

  constructor(private router: Router, private cookieService: CookieService) {}

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      const jwt = this.cookieService.get('jwt');
      if (localStorage.getItem('token') != null) {
        const userRole = JSON.parse(localStorage.getItem('sessionUserRole'));
        console.log(userRole);
        if (userRole === 'Admin') {
          return true;
        } else {
          alert('You have no premission!');
        }
      } else {
        alert('You are not logged in!');
        this.router.navigate(['/signin']);
        return false;
      }
  }
}
