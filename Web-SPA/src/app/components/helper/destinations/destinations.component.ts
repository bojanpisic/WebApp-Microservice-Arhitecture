import { Component, OnInit, Input } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-destinations',
  templateUrl: './destinations.component.html',
  styleUrls: ['./destinations.component.scss']
})
export class DestinationsComponent implements OnInit {

  @Input() data: any;
  selected = false;
  imgUrl;
  itsOk = false;
  constructor() { }

  ngOnInit(): void {
    setTimeout(() => {
      this.itsOk = true;
    }, 50);
  }

  select() {
    this.selected = !this.selected;
  }

}
