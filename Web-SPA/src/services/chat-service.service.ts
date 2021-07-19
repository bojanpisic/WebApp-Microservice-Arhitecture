import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Observable, of } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class ChatService {
  readonly BaseURI = "http://localhost:8084/chat";
  hubConnection: any;

  friendFirstName: any;
  friendLastName: any;
  receiverId: any;
  senderId: any;
  page: number = 0;

  constructor(private http: HttpClient) {}

  setupHubConnection(conn: any) {
    if (this.hubConnection == undefined) {
      console.log("SETTING UP HUBCONNECTION");
      this.hubConnection = conn;
    }
  }

  deleteConversation(id: number) {
    const url = `${this.BaseURI + "/chat/deleteChat"}/${id}`;
    return this.http.delete(url);
  }

  getHubConnection() {
    console.log("GETTING HUBCONNECTION");

    return this.hubConnection;
  }

  getConversationById(id: any): Observable<any> {
    const url = `${this.BaseURI + "/chat/getChats"}/${id}`;

    console.log(this.getTimezoneOffset());

    // const myParams = {
    //   page: this.page++,
    // };
    // let options = new RequestOptions({ headers: myHeaders, params: myParams });

    return this.http.get<any>(url, {
      headers: new HttpHeaders().set(
        "X-Timezone-Offset",
        this.getTimezoneOffset()
      ),
    });
  }

  private getTimezoneOffset(): string {
    return String(-(new Date().getTimezoneOffset() / 60));
  }

  getChats(): Observable<any> {
    return this.http.get<any>(this.BaseURI + "/chat/getChats");
  }

  getIds(): Observable<any> {
    return this.http.get<any>(this.BaseURI + "/chat/getChatUsers");
  }

  joinToChat(data: any) {
    const body = {
      connectionId: data.connectionId,
      roomName: data.roomName,
    };
    const url = `${this.BaseURI + "/chat/joinToChat"}`;
    return this.http.post(url, body);
  }

  leaveChat(data: any) {
    const body = {
      connectionId: data.connectionId,
      roomName: data.roomName,
    };
    const url = `${this.BaseURI + "/chat/leaveChat"}`;
    return this.http.post(url, body);
  }

  sendMessage(data: any) {
    const body = {
      ConversationId: data.ConversationId,
      SenderId: data.SenderId,
      Message: data.Message,
      RoomName: data.RoomName,
    };

    const url = `${this.BaseURI + "/chat/sendMessage"}`;
    return this.http.post(url, data);
  }
  sendMessageToNewConversation(data: any) {
    const body = {
      ReceiverId: data.ReceiverId,
      Message: data.Message,
      RoomName: data.RoomName,
      ReceiverFirstName: data.ReceiverFirstName,
      ReceiverLastName: data.ReceiverLastName,
    };
    const url = `${this.BaseURI + "/chat/sendmsgtonewconv"}`;
    return this.http.post(url, body);
  }

  createNewConversation(info: any) {
    this.receiverId = info.receiverId;
    this.friendLastName = info.friendLastName;
    this.friendFirstName = info.friendFirstName;
    this.senderId = info.senderId;
  }

  getNewConversationInfo() {
    return {
      receiverId: this.receiverId,
      friendLastName: this.friendLastName,
      friendFirstName: this.friendFirstName,
      senderId: this.senderId,
    };
  }
}
