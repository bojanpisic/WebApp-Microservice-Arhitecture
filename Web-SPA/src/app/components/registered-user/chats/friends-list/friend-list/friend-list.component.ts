import { Component, OnInit, Output, HostListener } from "@angular/core";
import { User } from "src/app/entities/user";
import { UserService } from "src/services/user.service";
import { ActivatedRoute, Router } from "@angular/router";
import { RegisteredUser } from "src/app/entities/registeredUser";
import { ToastrService } from "ngx-toastr";
import { ChatService } from "src/services/chat-service.service";

@Component({
  selector: "app-friend-list",
  templateUrl: "./friend-list.component.html",
  styleUrls: ["./friend-list.component.scss"],
})
export class FriendListComponent implements OnInit {
  modal = "unfriend";
  activeButton = "all";
  myProps = { friend: undefined, show: false };

  friends: Array<any>;
  friendsIDs: Array<any>;
  user: RegisteredUser;
  userId: number;
  searchText = "";

  show = false;
  isOk = false;
  friendsId;

  showSpinner: boolean = false;
  showEmpty: boolean = false;
  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private toastr: ToastrService,
    private chatService: ChatService,
    private router: Router
  ) {
    route.params.subscribe((params) => {
      this.userId = params.id;
    });
    this.friends = new Array<RegisteredUser>();
    this.friendsIDs = new Array<any>();
    this.myProps.show = false;
  }

  ngOnInit(): void {
    this.loadAll();
  }

  loadAll() {
    this.friends = [];
    this.isOk = false;
    this.showSpinner = true;

    this.chatService.getIds().subscribe(
      (res: any[]) => {
        if (res.length) {
          res.forEach((element) => {
            this.friendsIDs.push({ id: element.id });
          });
        }
        this.closeSpinner();
      },
      (err) => {
        this.toastr.error("Something went wrong.", "ERROR");
        this.showSpinner = false;
      }
    );

    this.userService.getFriends().subscribe(
      (res1: any[]) => {
        if (res1.length) {
          res1.forEach((element1) => {
            if (this.friendsIDs.find((x) => x.id == element1.id) == undefined) {
              const b1 = {
                firstName: element1.firstname,
                lastName: element1.lastname,
                email: element1.email,
                id: element1.id,
              };
              this.friends.push(b1);
            }
          });
          this.isOk = true;
          if (this.friends.length === 0) {
            this.showEmpty = true;
          }
        } else {
          this.show = true;
        }
        this.isOk = true;
      },
      (err) => {
        this.toastr.error(err.error, "ERROR");
      }
    );
  }

  closeSpinner() {
    setTimeout(() => (this.showSpinner = false), 500);
  }

  focusInput() {
    document.getElementById("searchInput").focus();
  }

  toggleButton(value: string) {
    this.activeButton = value;
  }

  addFriend(id: number) {
    const c = this.userService.addFriend(id).subscribe(
      (res1: any) => {
        console.log("SUCCESS");
        this.loadAll();
      },
      (err) => {
        console.log("ERROR");
        console.log(err.error);
        console.log(err);
        this.toastr.error(err.error, "ERROR");
      }
    );
  }

  removeFriend(remove: boolean) {
    if (remove) {
      const c = this.userService.removeFriend(this.myProps.friend.id).subscribe(
        (res1: any) => {
          console.log("SUCCESS");
          this.loadAll();
        },
        (err) => {
          console.log("ERROR");
          this.toastr.error(err.error, "ERROR");
        }
      );
    }
    this.myProps.show = !this.myProps.show;
  }

  toggleModal(friend: any) {
    this.myProps.friend = friend;
    this.myProps.show = !this.myProps.show;
  }

  sendMessage(friend: any) {
    const data = {
      receiverId: friend.id,
      friendFirstName: friend.firstName,
      friendLastName: friend.lastName,
      senderId: this.userId,
    };
    this.chatService.createNewConversation(data);

    this.router.navigate(["/" + this.userId + "/chat/non/chat-info"]);
  }
}
