import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-redirect-to',
  templateUrl: './redirect-to.component.html',
  styleUrls: ['./redirect-to.component.scss']
})
export class RedirectToComponent implements OnInit {

  @Input() option: string;
  @Output() redirect = new EventEmitter<any>();

  constructor() { }

  ngOnInit(): void {
  }

  onRedirect(value: any) {
    this.redirect.emit(value);
  }

}
