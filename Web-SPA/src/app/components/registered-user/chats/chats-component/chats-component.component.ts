import { Component, OnInit } from "@angular/core";
import { RegisteredUser } from "src/app/entities/registeredUser";
import { UserService } from "src/services/user.service";
import { ActivatedRoute, Router } from "@angular/router";
import { ChatService } from "../../../../../services/chat-service.service";
import { Messages } from "@progress/kendo-angular-dateinputs";
import * as signalR from "@aspnet/signalr";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: "app-chats-component",
  templateUrl: "./chats-component.component.html",
  styleUrls: ["./chats-component.component.scss"],
})
export class ChatsComponentComponent implements OnInit {
  userId: number;
  user: RegisteredUser;
  message: any;

  chats: Array<any>;
  chat: any;
  isOk = false;
  senderId;
  conversationId;
  fillInfo = false;
  connectionUrl = "http://localhost:8085/chatHub";
  activeButton = "inbox";
  hubConnection: any;
  connectionId: string = "";

  showSpinner: boolean = false;
  showEmpty: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private chatService: ChatService,
    private router: Router,
    private toastr: ToastrService
  ) {
    route.params.subscribe((params) => {
      this.userId = params.id;
    });
  }

  ngOnInit(): void {
    this.connectToHub();
    this.loadAll();
  }

  connectToHub() {
    var conn = new signalR.HubConnectionBuilder()
      .withUrl(this.connectionUrl + "?userId=" + this.userId)
      .build();

    this.hubConnection = conn;
    var connId = null;

    this.hubConnection
      .start()
      .then(function () {
        console.log("Connection started");
        conn.invoke("getConnectionId").then(function (connectionId) {
          connId = connectionId;
        });
      })
      .catch((err) => console.log("Connection aborted: " + err));

    this.connectionId = connId;

    this.chatService.setupHubConnection(this.hubConnection);
    this.listenOnNotifications();
  }

  listenOnNotifications() {
    this.hubConnection.on("Notfication", (data) => {
      const msg = JSON.parse(data);
      console.log(msg);
      let itemIndex = this.chats.findIndex(
        (item) => item.conversationId == msg.conversationId
      );

      if (itemIndex != -1) {
        this.chats[itemIndex].notification = true;
        this.chats[itemIndex].message = msg.text.Text;
        this.moveToFirstPlace(this.chats, itemIndex, 0);
      } else {
        this.chats.unshift({
          conversationId: msg.conversationId,
          friendFirstName: msg.senderFirstName,
          friendLastName: msg.senderLastName,
          message: msg.text.Text,
          sender: msg.senderFirstName,
          notification: true,
        });
      }
    });
  }

  moveToFirstPlace(arr, old_index, new_index) {
    if (new_index >= arr.length) {
      var k = new_index - arr.length + 1;
      while (k--) {
        arr.push(undefined);
      }
    }
    arr.splice(new_index, 0, arr.splice(old_index, 1)[0]);
    return arr;
  }

  loadAll() {
    this.chats = [];
    this.isOk = false;
    this.showSpinner = true;

    this.chatService.getChats().subscribe(
      (res: any[]) => {
        if (res.length) {
          res.forEach((element) => {
            this.chats.push({
              conversationId: element.conversationId,
              friendFirstName: element.friendFirstName,
              friendLastName: element.friendLastName,
              message: element.message,
              sender:
                element.sender == this.userId ? "You" : element.friendFirstName,
              notification: false,
            });
          });
          this.isOk = true;
          if (this.chats.length === 0) {
            this.showEmpty = true;
          }
        }
        this.closeSpinner();
      },
      (err) => {
        this.toastr.error("Something went wrong", "Error.");
        this.showSpinner = false;
      }
    );
  }

  openChatInfo(chat: any) {
    this.message = chat;
    this.senderId = chat.senderId;
    this.conversationId = chat.conversationId;
  }

  onBack() {
    this.hubConnection.stop().catch((err) => console.log("ERROR: " + err));
    this.router.navigate(["/" + this.userId + "/home"]);
  }

  friendList() {
    this.router.navigate(["/" + this.userId + "/friends-chat"]);
  }

  deleteChat(id: any) {
    this.chats.forEach((value, index) => {
      if (value.conversationId == id) this.chats.splice(index, 1);
    });
  }

  closeSpinner() {
    setTimeout(() => (this.showSpinner = false), 500);
  }
}
