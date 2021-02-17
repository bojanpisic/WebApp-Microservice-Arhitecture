import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Message } from 'src/app/entities/message';

@Component({
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.scss']
})
export class MessageComponent implements OnInit {

  @Input() message: any;
  @Output() action = new EventEmitter<Message>();

  constructor() { }

  ngOnInit(): void {
  }

  readMessage() {
    this.action.emit(this.message);
    this.message.read = true;
  }

}
