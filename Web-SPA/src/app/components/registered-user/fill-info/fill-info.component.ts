import { Component, OnInit ,Input, Output, EventEmitter, HostListener, ElementRef } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { UserService } from 'src/services/user.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-fill-info',
  templateUrl: './fill-info.component.html',
  styleUrls: ['./fill-info.component.scss']
})
export class FillInfoComponent implements OnInit {

  form: FormGroup;
  @Input() person: any;
  @Output() passenger = new EventEmitter<any>();
  closeIt = 0;
  userId: any;

  constructor(private eRef: ElementRef, route: ActivatedRoute, private userService: UserService,
    private toastr: ToastrService) {
      route.params.subscribe(params => {
      this.userId = params.id;
      });
    }

  ngOnInit(): void {
    this.initForm();

  }

  confirm() {
    this.passenger.emit({passport: this.form.controls.passport.value});
  }

  initForm() {
      this.form = new FormGroup({
        passport: new FormControl((this.person === undefined) ? '' : this.person.passport, Validators.required),
     });
  }

  close() {
    this.passenger.emit('CLOSE');
  }

  onDiscard() {
    this.passenger.emit(null);
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
