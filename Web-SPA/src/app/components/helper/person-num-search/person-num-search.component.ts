import { Component, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-person-num-search',
  templateUrl: './person-num-search.component.html',
  styleUrls: ['./person-num-search.component.scss']
})
export class PersonNumSearchComponent implements OnInit {

  rotateArrow = false;
  numOfAdults = 1;
  buttonContent = '1 person';
  @Output() updateTravellers = new EventEmitter<number>();

  constructor() { }

  ngOnInit(): void {
  }

  travellersBtnClick() {
    this.rotateArrow = this.rotateArrow === true ? false : true;

    if (this.rotateArrow) {
      document.getElementById('travellers-box').classList.remove('hide-travellers-box');
      document.getElementById('travellers-box').classList.add('show-travellers-box');
    } else {
      document.getElementById('travellers-box').classList.add('hide-travellers-box');
      document.getElementById('travellers-box').classList.remove('show-travellers-box');
    }
  }

  minus(type: string) {
    if (type === 'adults' && this.numOfAdults > 1) {
      this.numOfAdults = this.numOfAdults - 1;
    }

    this.changeContentOfBtn();
  }

  plus(type: string) {
    if (type === 'adults') {
      this.numOfAdults = this.numOfAdults + 1;
    }

    this.changeContentOfBtn();
  }

  changeContentOfBtn() {
    this.buttonContent = this.numOfAdults.toString() + ' people';
    this.updateTravellers.emit(this.numOfAdults);
  }

}
