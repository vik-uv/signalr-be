import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { HomePageComponent } from './pages/home/home.page.component';
import { AddMessageComponent } from './components/add-message/add-message.component';
import { ReactiveFormsModule } from '@angular/forms';
import { TokenInterceptor } from './services/interceptors/token.interceptor';
import { LoginComponent } from './pages/login/login.component';
import { ApiService, API_BASE_URL } from './services/api';
import { environment } from 'src/environments/environment';

import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { TableModule } from 'primeng/table';
import { TabViewModule } from 'primeng/tabview';
import { DialogModule } from 'primeng/dialog';

@NgModule({
  declarations: [
    AppComponent,
    HomePageComponent,
    AddMessageComponent,
    LoginComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    InputTextModule,
    ButtonModule,
    DropdownModule,
    InputTextareaModule,
    TableModule,
    TabViewModule,
    DialogModule
  ],
  providers: [
    HttpClientModule, 
    { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true },
    { provide: API_BASE_URL, useValue: environment.apiConnection },
    ApiService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
