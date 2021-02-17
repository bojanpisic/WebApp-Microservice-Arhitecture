import { Component, OnInit, EventEmitter, Output, Input, HostListener, ElementRef } from '@angular/core';
import { RegisteredUser } from 'src/app/entities/registeredUser';
import { Address } from 'src/app/entities/address';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.scss']
})
export class ModalComponent implements OnInit {

  @Output() remove = new EventEmitter<boolean>();
  @Input() props: {friend: RegisteredUser, show: boolean};
  @Input() destination: {city: string, state: string};
  @Input() type: string;
  @Input() car: any;
  closeIt = 0;

  constructor(private eRef: ElementRef) { }

  ngOnInit(): void {
    console.log(this.car);
  }

  return(value: boolean) {
    this.remove.emit(value);
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
        this.return(false);
      }
    }
  }

}
