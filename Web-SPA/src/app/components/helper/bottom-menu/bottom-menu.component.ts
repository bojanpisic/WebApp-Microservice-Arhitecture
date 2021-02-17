import { Component, OnInit, HostListener, Input } from '@angular/core';
import * as jwt_decode from 'jwt-decode';
@Component({
  selector: 'app-bottom-menu',
  templateUrl: './bottom-menu.component.html',
  styleUrls: ['./bottom-menu.component.scss']
})
export class BottomMenuComponent implements OnInit {

  scrolled =  false;
  lastPosition: number = window.innerHeight;
  @Input() userId;
  decoded;

  constructor() { }

  ngOnInit(): void {
    this.scrolled =  true;
    const token = localStorage.getItem('token');
    this.decoded = this.getDecodedAccessToken(token);
  }

  getDecodedAccessToken(token: string): any {
    try {
        return jwt_decode(token);
    } catch (Error) {
        return null;
    }
  }

  @HostListener('window:scroll', [])
  onWindowScroll() {
      if (this.scrolled === true) {
        if (this.lastPosition > window.scrollY + window.innerHeight) {
          this.scrolled = true;
          this.lastPosition = window.scrollY + window.innerHeight;
        } else {
          this.scrolled = false;
        }
      } else {
        if (this.lastPosition > window.scrollY + window.innerHeight) {
          this.scrolled = true;
        } else {
          this.scrolled = false;
          this.lastPosition = window.scrollY + window.innerHeight;
        }
      }
    }
}
