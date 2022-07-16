import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { LocalStogageAuthToken } from 'src/app/services/interceptors/token.interceptor';
import { SignalRService } from 'src/app/services/signalr.service';

@Component({
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent  {
  public form: FormGroup;
  public isBusy = false;
  public error: string | undefined;

  constructor(private service: SignalRService, private fb: FormBuilder, private router: Router) {
    this.form = this.fb.group({
      login: this.fb.control(undefined),
      password: this.fb.control(undefined)
    });
  }

  public onSend(): void {
    if (this.form.invalid) return;
    this.isBusy = true;
    const { login, password } = this.form.value;
    localStorage.setItem(LocalStogageAuthToken, this.getToken(login, password));
    this.service.connectionOk().subscribe({
      next: (data) => {
        this.router.navigateByUrl('');
      },
      error: (err) => {
        this.error = "Cannot login now, server is down or credintial are not correct";
        this.isBusy = false;
        console.error(err);
      },
      complete: () => {
        this.isBusy = false;
      }
    });
  }

  private getToken(user: string, password: string)
  {
    return btoa(user + ":" + password);
  }
}
