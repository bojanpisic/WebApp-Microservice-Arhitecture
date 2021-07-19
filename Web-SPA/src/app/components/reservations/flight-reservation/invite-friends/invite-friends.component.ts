import { Component, OnInit, Input, Output, EventEmitter, HostListener, ElementRef } from '@angular/core';
import { Flight } from 'src/app/entities/flight';
import { Passenger } from 'src/app/entities/passenger';
import { SeatsForFlight } from 'src/app/entities/seats-for-flight';
import { Seat } from 'src/app/entities/seat';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { RegisteredUser } from 'src/app/entities/registeredUser';
import { UserService } from 'src/services/user.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-invite-friends',
  templateUrl: './invite-friends.component.html',
  styleUrls: ['./invite-friends.component.scss']
})
export class InviteFriendsComponent implements OnInit {

  @Input() seat: Seat;
  @Input() flight: Flight;
  @Input() person: any;
  @Input() pickSeatForMe: boolean;
  @Input() invite: boolean;

  @Output() passenger = new EventEmitter<any>();
  userId: number;
  user: RegisteredUser;

  friends: Array<any>;

  closeIt = 0;

  firstNameInvalid = false;
  lastNameInvalid = false;
  passportInvalid = false;

  searchText = '';

  option = 'fill';
  btnContent = 'Invite a friend';

  form: FormGroup;

  constructor(private eRef: ElementRef, route: ActivatedRoute, private userService: UserService,
              private toastr: ToastrService) {
    route.params.subscribe(params => {
      this.userId = params.id;
    });

    this.friends = [];
  }

  ngOnInit(): void {
    const rez = this.userService.getFriends().subscribe(
      (res: any[]) => {
        if (res.length > 0) {
          res.forEach(element => {
            this.friends.push(element);
          });
        }
      }, err => {
        this.toastr.error(err.error, 'Error!');
      }
    );
    if (this.person !== undefined) {
      if (this.person.friendsId) {
        this.onClick();
      }
    }
    this.initForm();
  }

  initForm() {
    if (this.pickSeatForMe) {
      this.form = new FormGroup({
        passport: new FormControl((this.person === undefined) ? '' : this.person.passport, Validators.required),
     });
    } else {
      this.form = new FormGroup({
        firstName: new FormControl((this.person === undefined) ? '' : this.person.firstName, Validators.required),
        lastName: new FormControl((this.person === undefined) ? '' : this.person.lastName, Validators.required),
        passport: new FormControl((this.person === undefined) ? '' : this.person.passport, Validators.required),
     });
    }
  }

  confirm() {
    // proveriti da li ima putnik sa takvim pasosem na ovom letu
    if (this.validate()) {
      // tslint:disable-next-line:max-line-length
      if (this.pickSeatForMe) {
        this.passenger.emit({passport: this.form.controls.passport.value});
      } else {
        // tslint:disable-next-line:max-line-length
        this.passenger.emit(new Passenger(this.form.controls.firstName.value, this.form.controls.lastName.value, this.form.controls.passport.value));
      }
    }
  }

  validate() {
    let retVal = true;
    if (!this.pickSeatForMe) {
      if (this.form.controls.firstName.value === '') {
        this.firstNameInvalid = true;
        retVal = false;
      }
      if (this.form.controls.lastName.value === '') {
        this.lastNameInvalid = true;
        retVal = false;
      }
    }
    if (this.form.controls.passport.value === '') {
      this.passportInvalid = true;
      retVal = false;
    }
    return retVal;
  }

  focusInput() {
    document.getElementById('searchInput').focus();
  }

  inviteFriend(friend: any) {
    // tslint:disable-next-line:max-line-length
    this.passenger.emit({friendsId: friend.id});
  }

  close() {
    this.passenger.emit('CLOSE');
  }

  onClick() {
    this.option = (this.option === 'fill') ? 'invite' : 'fill';
    this.btnContent = (this.btnContent === 'Fill info') ? 'Invite a friend' : 'Fill info';
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
