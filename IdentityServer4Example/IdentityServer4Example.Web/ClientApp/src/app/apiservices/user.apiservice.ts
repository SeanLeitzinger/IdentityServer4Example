import { Inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpResponse, HttpHeaders } from '@angular/common/http';
import { Observable } from "rxjs/Rx";
import { ApplicationUser } from '../models/application-user';
import { AuthenticationService } from '../services/auth';

@Injectable()
export class UserApiService {
  apiUrl: string = 'https://localhost:44356';
  httpOptions: HttpHeaders;

  constructor(private httpClient: HttpClient, private authService: AuthenticationService) {
  }

  getUsers(): Observable<ApplicationUser[]> {

    return this.httpClient.get<ApplicationUser[]>(`${this.apiUrl}/api/User/GetUsers`, {
      headers: new HttpHeaders({
        'Authorization': `Bearer ${this.authService.currentUser.access_token}`
      })
    });
  }
}
