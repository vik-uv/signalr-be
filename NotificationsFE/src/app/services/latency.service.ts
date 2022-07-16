import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LatencyService {
  private timer: number = 0;

  constructor() { }

  public start(): void {
    this.timer = Date.now();
  }

  public finish(): string {
    if (this.timer === 0) return 'No start set';
    let msec = Date.now() - this.timer;
    const hh = Math.floor(msec / 1000 / 60 / 60);
    msec -= hh * 1000 * 60 * 60;
    const mm = Math.floor(msec / 1000 / 60);
    msec -= mm * 1000 * 60;
    const ss = Math.floor(msec / 1000);
    msec -= ss * 1000;
    return `Latency is ${hh > 0 ? hh + ':': ''}${mm > 0 ? mm + ':' : ''}${ss > 0 ? ss + '.' : '0.'}${msec} s`;
  }
}
