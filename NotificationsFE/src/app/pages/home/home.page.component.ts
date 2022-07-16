import { Component, OnInit } from '@angular/core';
import { Message } from 'src/app/services/api';
import { LatencyService } from 'src/app/services/latency.service';
import { SignalRService } from 'src/app/services/signalr.service';

interface GridMessage extends Message {
  latency: string;
}

@Component({
  templateUrl: './home.page.component.html',
  styleUrls: ['./home.page.component.scss']
})
export class HomePageComponent implements OnInit {
  public messages: GridMessage[] = [];
  public groups: string[] = ['logs', 'messages', 'other'];
  public tabIndex: number = 0;
  public isModalVisible: boolean = false;

  constructor(private service: SignalRService, private latService: LatencyService) { }

  ngOnInit(): void {
    this.service.init();
    this.service.messages.subscribe((message) => {
      const m = message as GridMessage;
      m.latency = this.latService.finish();
      this.messages.unshift(m);
    });

    this.service.getMessages(100).subscribe((messages) => {
      this.messages = messages as GridMessage[];
    });
  }

  public getGroupMessaged(group: string): GridMessage[] {
    if (group === 'other') {
      return this.messages.filter(f => !this.groups.some(g => g === f.group));
    }
    return this.messages.filter(f => f.group === group);
  }

  public onAdd(): void {
    this.isModalVisible = true;
  }

  public onAdded($event: boolean): void {
    if ($event) {
      this.isModalVisible = false;
    }
  }
}
