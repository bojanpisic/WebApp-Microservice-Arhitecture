import { Component, OnInit } from '@angular/core';
import { RegisteredUser } from 'src/app/entities/registeredUser';
import { UserService } from 'src/services/user.service';
import { ActivatedRoute } from '@angular/router';
import { Message } from 'src/app/entities/message';

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

  isOk = false;

  senderId;

  activeButton = 'inbox';

  constructor(private route: ActivatedRoute, private userService: UserService) {
    route.params.subscribe(params => {
      this.userId = params.id;
    });
   }

  ngOnInit(): void {
    this.loadAll();
    console.log(this.requests);
    console.log(this.acceptedRequests);
    console.log(this.flightInvitations);
    console.log(this.acceptedFlightInvitations);
  }

  loadAll() {
    this.requests = [];
    this.acceptedRequests = [];
    this.isOk = false;
    const a = this.userService.getRequests().subscribe(
      (res: any[]) => {
        console.log(res);
        if (res.length) {
          res.forEach(element => {
            const b = {
              accepted: element.accepted,
              senderFirstName: element.senderFirstName,
              senderLastName: element.senderLastName,
              senderId: element.senderId,
              senderEmail: element.senderEmail,
            };
            console.log(b.accepted);
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
        console.log(res);
        if (res.length) {
          res.forEach(element => {
            const b = {
              accepted: element.accepted,
              senderFirstName: element.senderFirstName,
              senderLastName: element.senderLastName,
              senderId: element.senderId,
              senderEmail: element.senderEmail,
            };
            if (b.accepted === false) {
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
    console.log('value' + value);
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
    if (value === 'decline') {
      console.log('eo me');
      const c = this.userService.declineFriendship(this.senderId).subscribe(
        (res1: any) => {
          this.loadAll();
        },
        err => {
          alert(err.error.description);
        }
      );
    }
  }

  openMessageInfo(message: any) {
    this.myProps.message = message;
    this.myProps.show = true;
    this.senderId = message.senderId;
  }

}
