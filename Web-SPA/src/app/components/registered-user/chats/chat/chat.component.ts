import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { ChatService } from "src/services/chat-service.service";
import { Location } from "@angular/common";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: "app-chat",
  templateUrl: "./chat.component.html",
  styleUrls: ["./chat.component.scss"],
})
export class ChatComponent implements OnInit {
  userId: string;
  @Input() chat;
  @Output() deleteEvent = new EventEmitter<any>();
  exit: boolean = false;
  blur: boolean = false;
  deleteChat = "deleteChat";
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private chatService: ChatService,
    private location: Location,
    private toastr: ToastrService
  ) {
    route.params.subscribe((params) => {
      this.userId = params.id;
    });
  }

  ngOnInit(): void {}

  removeConversation() {
    this.exit = true;
  }
  openChat() {
    this.router.navigate([
      "/" + this.userId + "/chat/" + this.chat.conversationId + "/chat-info",
    ]);
  }

  onDeleteChat(value: any) {
    if (value) {
      this.chatService.deleteConversation(this.chat.conversationId).subscribe(
        (res: any) => {
          console.log("Deleted");
          this.deleteEvent.emit(this.chat.conversationId);
        },
        (err) => this.toastr.error("Something went wrong", "Error.")
      );
    }
    this.exit = false;
    this.blur = false;
  }
}
