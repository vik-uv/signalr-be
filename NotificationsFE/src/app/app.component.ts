import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { LocalStorageAuthToken } from './services/interceptors/token.interceptor';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'notifications';

  constructor(private router: Router) {
    const token = localStorage.getItem(LocalStorageAuthToken);
    if (!token) {
      this.router.navigateByUrl('login')
    }
  }
}
