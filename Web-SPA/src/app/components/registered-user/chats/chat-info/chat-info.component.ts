import {
  Component,
  OnInit,
  Input,
  HostListener,
  ViewChild,
  ElementRef,
  Inject,
} from "@angular/core";
import * as signalR from "@aspnet/signalr";
import { ActivatedRoute, Router } from "@angular/router";
import { ChatService } from "../../../../../services/chat-service.service";
import { ToastrService } from "ngx-toastr";
@Component({
  selector: "app-chat-info",
  templateUrl: "./chat-info.component.html",
  styleUrls: ["./chat-info.component.scss"],
})
export class ChatInfoComponent implements OnInit {
  @ViewChild("scrollDiv", { static: true }) scrollDiv: ElementRef;

  userId: string;
  friendId: string;
  username = "";
  messages: Array<any>;
  allMessages: Array<any>;
  newMessage = "";
  conversation: any;
  hubConnection: any;
  friendFirstName: any;
  friendLastName: any;
  connectionId: any;

  msgText: any;
  conversationId: number;
  RoomName: string;
  receiverId: any;
  newConvInfo: any;
  showTyping: boolean = false;

  showSpinner: boolean = false;
  msgs_box: HTMLElement;
  scrollCounter: number = 0;
  maxMessagesRender: number = 20;
  prevScrollHeight: number;

  constructor(
    private route: ActivatedRoute,
    private chatService: ChatService,
    private router: Router,
    private toastr: ToastrService
  ) {
    route.params.subscribe((params) => {
      this.userId = params.id;
      this.conversationId = params.conversationId;
    });
  }
  ngOnInit() {
    this.messages = [];

    if (this.conversationId.toString() == "non") {
      this.newConvInfo = this.chatService.getNewConversationInfo();
      this.friendLastName = this.newConvInfo.friendLastName;
      this.friendFirstName = this.newConvInfo.friendFirstName;
      this.RoomName = this.userId + this.newConvInfo.receiverId;
      this.receiverId = this.newConvInfo.receiverId;
      this.getHub();
    } else {
      this.getMessages();
    }
  }

  ngAfterViewInit() {
    this.scrollTo(this.scrollDiv.nativeElement.scrollHeight);
  }

  scrollTo(val: number) {
    setTimeout(() => {
      console.log(val);
      this.scrollDiv.nativeElement.scrollTo(0, val);
    }, 10);
  }

  onScroll() {
    let element = this.scrollDiv.nativeElement;

    if (
      element.scrollTop === 0 &&
      this.allMessages.length - this.scrollCounter * this.maxMessagesRender >=
        this.scrollCounter * this.maxMessagesRender
    ) {
      this.showSpinner = true;

      let newMessages = this.allMessages
        .slice(
          this.scrollCounter * this.maxMessagesRender,
          this.allMessages.length <
            (this.scrollCounter + 1) * this.maxMessagesRender
            ? this.allMessages.length
            : (this.scrollCounter + 1) * this.maxMessagesRender
        )
        .reverse();
      this.messages.unshift(...newMessages);
      this.scrollCounter++;
      this.showSpinner = false;
    }
  }

  getHub() {
    this.hubConnection = this.chatService.getHubConnection();
    this.hubConnection.invoke("getConnectionId").then((connId) => {
      this.connectionId = connId;
      this.joinToChat();
    });
    this.hubConnection.on("ReceiveMessage", (message) => {
      const msg = JSON.parse(message);

      this.messages.push({
        text: msg.Text,
        timeStamp: new Date(),
        senderId: msg.SenderId,
        received: msg.Received,
      });
    });

    this.hubConnection.on("typing", (senderId) => {
      if (this.userId !== senderId) {
        this.showTyping = true;
      }
    });
    this.hubConnection.on("stopTyping", (senderId) => {
      if (this.userId !== senderId) {
        this.showTyping = false;
      }
    });
  }

  private getTimezoneOffset(): number {
    return Number(-new Date().getTimezoneOffset());
  }

  onTyping() {
    if (this.msgText !== "") {
      this.hubConnection.invoke("onTyping", this.RoomName, this.userId);
    }
  }
  focusOut() {
    if (this.msgText == "") {
      this.hubConnection.invoke("stopTyping", this.RoomName, this.userId);
    }
  }

  sendMessage() {
    if (this.conversationId.toString() == "non") {
      this.sendMessageToNewConversation();
    } else {
      this.sendMessageToExistingConversation();
    }

    this.msgText = "";
    this.focusOut(); //to hide typing msg
    this.scrollTo(this.scrollDiv.nativeElement.scrollHeight);
  }

  sendMessageToExistingConversation() {
    var message = {
      ConversationId: this.conversationId,
      SenderId: this.userId,
      Message: this.msgText,
      RoomName: this.RoomName,
    };
    this.chatService.sendMessage(message).subscribe(
      (res: any) => {},
      (err) => {
        console.log("msg failed to send");
        this.toastr.error("Something went wrong", "Error.");
      }
    );
  }

  sendMessageToNewConversation() {
    var message = {
      ReceiverId: this.receiverId,
      Message: this.msgText,
      RoomName: this.RoomName,
      ReceiverFirstName: this.friendFirstName,
      ReceiverLastName: this.friendLastName,
    };
    this.chatService.sendMessageToNewConversation(message).subscribe(
      (res: any) => {
        console.log("msg sent");
      },
      (err) => {
        console.log("msg failed to send");
        this.toastr.error("Something went wrong", "Error.");
      }
    );
  }
  joinToChat() {
    var data = {
      connectionId: this.connectionId,
      roomName: this.RoomName,
    };

    this.chatService.joinToChat(data).subscribe(
      (res: any) => {
        console.log("Joined");
      },
      (err) => {
        console.log("failed to join");
        this.toastr.error("Something went wrong", "Error.");
      }
    );
  }
  public leaveChat = () => {
    var data = {
      connectionId: this.connectionId,
      roomName: this.RoomName,
    };

    this.chatService.leaveChat(data).subscribe(
      (res: any) => {
        console.log("Leaved");
      },
      (err) => {
        console.log("failed to leave");
        this.toastr.error("Something went wrong", "Error.");
      }
    );
  };

  close() {
    this.leaveChat();
    this.router.navigate(["/" + this.userId + "/chat"]);
  }

  getMessages() {
    this.messages = [];
    this.allMessages = [];
    this.showSpinner = true;

    this.chatService.getConversationById(this.conversationId).subscribe(
      (res: any) => {
        const b = {
          conversationId: res.conversationId,
          friendFirstName: res.friendFirstName,
          friendLastName: res.friendLastName,
          receiverId: res.receiverId,
          roomName: res.roomName,
          receiverUserName: res.receiverUserName,
          senderUserName: res.senderUserName,
          messages: res.messages,
          sender:
            res.messages.senderId == this.userId ? "You" : res.friendFirstName,
        };
        this.conversation = b;
        this.allMessages = this.conversation.messages;
        this.friendId = this.conversation.friendId;
        this.friendFirstName = this.conversation.friendFirstName;
        this.friendLastName = this.conversation.friendLastName;
        this.RoomName = this.conversation.roomName;

        this.messages = this.allMessages
          .slice(
            this.scrollCounter * this.maxMessagesRender,
            this.allMessages.length <
              (this.scrollCounter + 1) * this.maxMessagesRender
              ? this.allMessages.length
              : (this.scrollCounter + 1) * this.maxMessagesRender
          )
          .reverse();

        this.scrollCounter++;
        this.getHub();
        this.closeSpinner();
      },
      (err) => {
        console.log("failed to get messages");
        this.toastr.error("Something went wrong", "Error.");
        this.showSpinner = false;
      }
    );
  }

  closeSpinner() {
    setTimeout(() => (this.showSpinner = false), 500);
  }
}
