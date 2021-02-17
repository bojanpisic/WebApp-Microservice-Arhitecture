import { Component, OnInit, Input, EventEmitter, Output, HostListener, ElementRef } from '@angular/core';
import { Message } from 'src/app/entities/message';

@Component({
  selector: 'app-message-info',
  templateUrl: './message-info.component.html',
  styleUrls: ['./message-info.component.scss']
})
export class MessageInfoComponent implements OnInit {

  @Output() response = new EventEmitter<string>();
  @Input() props: {message: any, show: boolean};
  closeIt = 0;

  constructor(private eRef: ElementRef) { }

  ngOnInit(): void {
  }

  decline() {
    this.response.emit('decline');
  }

  close() {
    this.response.emit('close');
  }

  accept() {
    this.response.emit('accept');
  }

  @HostListener('document:click', ['$event'])
  clickout(event) {
    if (this.closeIt < 1) {
      this.closeIt = 1;
    } else {
      this.closeIt = 2;
    }
    if (!this.eRef.nativeElement.contains(event.target)) {
      if (this.closeIt === 2) {
        this.close();
      }
    }
  }

}
