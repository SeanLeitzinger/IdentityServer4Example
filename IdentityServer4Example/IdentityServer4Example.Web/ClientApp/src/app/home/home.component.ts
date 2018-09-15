import { Component } from '@angular/core';
import { UserApiService } from '../apiservices/user.apiservice';
import { ApplicationUser } from '../models/application-user';
import { AuthenticationService } from '../services/auth';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  users: ApplicationUser[] = new Array<ApplicationUser>();

  constructor(private userApiService: UserApiService, private authService: AuthenticationService) {

  }

  getUsers() {
    this.userApiService.getUsers().subscribe(data => {
      this.users = data;
    });
  }

  signOut() {
    this.authService.startSignoutMainWindow();
  }
}
