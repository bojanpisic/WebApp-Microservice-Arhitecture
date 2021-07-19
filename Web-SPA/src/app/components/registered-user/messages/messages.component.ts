import { Component, OnInit } from '@angular/core';
import { RegisteredUser } from 'src/app/entities/registeredUser';
import { UserService } from 'src/services/user.service';
import { ActivatedRoute } from '@angular/router';
import { Message } from 'src/app/entities/message';
import { cwd } from 'process';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.scss']
})
export class MessagesComponent implements OnInit {

  userId: number;
  user: RegisteredUser;
  myProps = {message: undefined, show: false};

  requests: Array<any>;
  acceptedRequests: Array<any>;
  flightInvitations: Array<any>;
  acceptedFlightInvitations: Array<any>;
  passenger: any;
  isOk = false;
  senderId;
  invitationId;
  fillInfo = false;

  activeButton = 'inbox';

  constructor(private route: ActivatedRoute, private userService: UserService) {
    route.params.subscribe(params => {
      this.userId = params.id;
    });
   }

  ngOnInit(): void {
    this.loadAll();
    this.flightInvitations = [];
    this.acceptedFlightInvitations = [];
    this.acceptedRequests = [];


  }

  loadAll() {
    this.requests = [];
    this.acceptedRequests = [];
    this.isOk = false;
    const a = this.userService.getRequests().subscribe(
      (res: any[]) => {
        if (res.length) {
          res.forEach(element => {
            const b = {
              accepted: element.accepted,
              senderFirstName: element.senderFirstName,
              senderLastName: element.senderLastName,
              senderId: element.senderId,
              senderEmail: element.senderEmail,
              text: 'You have friendship request!',
              content: 'Would you like to becoma a friend with me? ',
              type: 'friendshipRequest'
            };


            if (b.accepted === false) {
              this.acceptedRequests.push(b);
            } else {
              this.requests.push(b);
            }
          });
          this.isOk = true;
        }
      }
    );

    const n = this.userService.getFlightInvitations().subscribe(
      (res: any[]) => {
        if (res.length) {
          res.forEach(element => {
            const b = {
              accepted: element.accepted,
              senderFirstName: element.senderFirstName,
              senderLastName: element.senderLastName,
              senderId: element.senderId,
              senderEmail: element.senderEmail,
              text: 'You have trip invite!',
              content: 'Would you like to travel with me? ',
              type: 'tripInvite',
              invitationId:element.invitationId,

            };

            if (b.accepted === true) {
              this.acceptedFlightInvitations.push(b);
            } else {
              this.flightInvitations.push(b);
            }
          });
          this.isOk = true;
        }
      }
    );
  }

  toggleButton(value: string) {
    this.activeButton = value;
  }

  getResponse(value: string) {
    this.myProps.show = false;
    if (value === 'accept') {
      const c = this.userService.acceptFriendship(this.senderId).subscribe(
        (res1: any) => {
          this.loadAll();
        },
        err => {
          alert(err.error.description);
        }
      );
    }
    else if (value === 'decline') {
      const c = this.userService.declineFriendship(this.senderId).subscribe(
        (res1: any) => {
          this.loadAll();
        },
        err => {
          alert(err.error.description);
        }
      );
    }
    else if (value === 'declineTrip') {
      const c = this.userService.declineTripInvitation(this.invitationId).subscribe(
        (res1: any) => {
          this.loadAll();
        },
        err => {
          alert(err.error.description);
        }
      );
    }
    else if (value === 'acceptTrip') {

      this.fillInfo = true;

      
    }
  }

  openMessageInfo(message: any) {
    this.myProps.message = message;
    this.myProps.show = true;
    this.senderId = message.senderId;
    this.invitationId = message.invitationId;
  }

  addPassenger(passenger: any) {
    if (passenger === 'CLOSE') {
      this.fillInfo = false;
      return;
    }
    var data = {
      id : this.invitationId,
      passport: passenger.passport
    };
    console.log(data.passport);
    console.log(data.id);

    const c = this.userService.acceptTripInvitation(data).subscribe(
      (res1: any) => {
        this.loadAll();
      },
      err => {
        alert(err.error.description);
      }
    );
  }
}
