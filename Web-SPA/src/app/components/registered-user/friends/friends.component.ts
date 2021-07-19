import { Component, OnInit, Output, HostListener } from "@angular/core";
import { User } from "src/app/entities/user";
import { UserService } from "src/services/user.service";
import { ActivatedRoute, Router } from "@angular/router";
import { RegisteredUser } from "src/app/entities/registeredUser";
import { ToastrService } from "ngx-toastr";
import { ChatService } from "src/services/chat-service.service";

@Component({
  selector: "app-friends",
  templateUrl: "./friends.component.html",
  styleUrls: ["./friends.component.scss"],
})
export class FriendsComponent implements OnInit {
  modal = "unfriend";
  activeButton = "all";
  myProps = { friend: undefined, show: false };

  friends: Array<any>;
  nonFriends: Array<any>;
  user: RegisteredUser;
  userId: number;
  searchText = "";

  isOk = false;
  friendsId;

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
    this.nonFriends = new Array<RegisteredUser>();
    this.myProps.show = false;
  }

  ngOnInit(): void {
    this.loadAll();
  }

  loadAll() {
    this.nonFriends = [];
    this.friends = [];
    this.isOk = false;
    const a = this.userService.getNonFriends().subscribe(
      (res: any[]) => {
        if (res.length) {
          res.forEach((element) => {
            const b = {
              firstName: element.firstname,
              lastName: element.lastname,
              email: element.email,
              id: element.id,
            };
            this.nonFriends.push(b);
          });
        }
        const c = this.userService.getFriends().subscribe(
          (res1: any[]) => {
            console.log(res1);
            if (res1.length) {
              res1.forEach((element1) => {
                const b1 = {
                  firstName: element1.firstname,
                  lastName: element1.lastname,
                  email: element1.email,
                  id: element1.id,
                };
                this.friends.push(b1);
              });
              this.isOk = true;
            }
            this.isOk = true;
          },
          (err) => {
            this.toastr.error(err.error, "ERROR");
          }
        );
      },
      (err) => {
        this.toastr.error(err.error, "ERROR");
      }
    );
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
