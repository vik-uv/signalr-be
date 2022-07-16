import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { ApiService, Message, SignalRConnectionInfo } from 'src/app/services/api';
import { HubConnection } from '@microsoft/signalr';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: HubConnection | undefined;
  
  public messages: Subject<Message> = new Subject();

  constructor(private apiClient: ApiService) {
  }

  private getConnectionInfo(): Observable<SignalRConnectionInfo> {
    return this.apiClient.negotiate();
  }

  init() {
    this.getConnectionInfo().subscribe((info) => {
      const options = {
        accessTokenFactory: () => info.accessToken ?? '',
      };

      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(info.url ?? '', options)
        .configureLogging(signalR.LogLevel.Information)
        .build();

      this.hubConnection.start().catch((err) => console.error(err.toString()));

      this.hubConnection.on("newMessage", (message: Message) => {
        this.messages.next(message);
      });
    });
  }

  getMessages(count: number = 100): Observable<Message[]> {
    return this.apiClient.getMessagesList(count);
  }

  sendMessage(message: Message): Observable<Message> {
    return this.apiClient.addMessage(message);
  }

  getIndex(): Observable<string> {
    return this.apiClient.getHomePage();
  }

  connectionOk(): Observable<string> {
    return this.apiClient.connectionOk();
  }
}
