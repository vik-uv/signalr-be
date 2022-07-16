import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Message } from 'src/app/services/api';
import { LatencyService } from 'src/app/services/latency.service';
import { SignalRService } from 'src/app/services/signalr.service';

@Component({
  selector: 'app-add-message',
  templateUrl: './add-message.component.html',
  styleUrls: ['./add-message.component.scss']
})
export class AddMessageComponent  {
  @Input() selectedGroup: string | undefined;
  @Output() messageAdded: EventEmitter<boolean> = new EventEmitter<boolean>();
  
  public groups = [
    {name: 'logs', code: 'logs'},
    {name: 'messages', code: 'messages'}
  ];
  public form: FormGroup;
  public isBusy = false;

  constructor(private service: SignalRService, private fb: FormBuilder, private latService: LatencyService) { 
    this.form = this.fb.group({
      group: this.fb.control(undefined),
      content: this.fb.control('')
    });
  }

    public onSend(): void {
    if (this.form.invalid) return;
    this.isBusy = true;
    const data = this.form.value as Message;
    if (this.selectedGroup) {
      data.group = this.selectedGroup;
    }
    data.datetime = new Date();
    this.latService.start();
    this.service.sendMessage(data).subscribe({
      next: (data) => {
        this.form.reset();
        this.messageAdded.emit(true);
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        this.isBusy = false;
      }
    });
  }
}
