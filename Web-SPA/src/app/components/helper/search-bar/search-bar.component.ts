import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { User } from 'src/app/entities/user';

@Component({
  selector: 'app-search-bar',
  templateUrl: './search-bar.component.html',
  styleUrls: ['./search-bar.component.scss']
})
export class SearchBarComponent implements OnInit {

  @Input() friendsList;
  @Output() emitter = new EventEmitter<Array<User>>();
  searchContent: any;
  searchedFriends: Array<User>;

  constructor() { }

  ngOnInit(): void {
    this.searchedFriends = new Array<User>();
  }

  searchFunction() {
    if (this.searchContent === '') {
      this.emitter.emit(this.friendsList);
      return;
    }
    this.searchedFriends.splice(0, this.searchedFriends.length);
    this.friendsList.forEach(friend => {
      if ((friend.firstName.toUpperCase().indexOf(this.searchContent.toUpperCase()) > -1 ||
      friend.lastName.toUpperCase().indexOf(this.searchContent.toUpperCase()) > -1 ||
      friend.email.toUpperCase().indexOf(this.searchContent.toUpperCase()) > -1 ) &&
      !this.searchedFriends.includes(friend)) {
        this.searchedFriends.push(friend);
      }
      this.emitter.emit(this.searchedFriends);
    });

  }

}
