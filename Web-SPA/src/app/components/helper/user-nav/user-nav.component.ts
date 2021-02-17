import { Component, OnInit, Input } from '@angular/core';
import { RegisteredUser } from 'src/app/entities/registeredUser';

@Component({
  selector: 'app-user-nav',
  templateUrl: './user-nav.component.html',
  styleUrls: ['./user-nav.component.scss']
})
export class UserNavComponent implements OnInit {

  @Input() user: RegisteredUser;
  constructor() { }

  ngOnInit(): void {
  }

}
