import { Injectable, EventEmitter, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Rx';

import { UserManager, User } from 'oidc-client';

@Injectable()
export class AuthenticationService {
  settings: any;

  userManager: UserManager;
  userLoadededEvent: EventEmitter<User> = new EventEmitter<User>();
  currentUser: User;
  loggedIn = false;

  constructor(private http: Http) {
    this.settings = {
      authority: 'https://localhost:44386',
      client_id: 'examplewebclient',
      redirect_uri: `https://localhost:44322/signin-callback.html`,
      post_logout_redirect_uri: `https://localhost:44322/signout-callback-oidc`,
      response_type: 'id_token token',
      scope: 'openid profile exampleapi',

      silent_redirect_uri: `https://localhost:44322/silent-renew-callback.html`,
      automaticSilentRenew: true,
      accessTokenExpiringNotificationTime: 10,
      silentRequestTimeout: 10000,

      filterProtocolClaims: true,
      loadUserInfo: true
    };

    this.userManager = new UserManager(this.settings);

    this.userManager.getUser()
      .then((user) => {
        if (user) {
          this.loggedIn = true;
          this.currentUser = user;
          this.userLoadededEvent.emit(user);
        }
        else {
          this.loggedIn = false;
        }
      })
      .catch((err) => {
        this.loggedIn = false;
      });

    this.userManager.events.addUserLoaded((user) => {
      this.currentUser = user;
    });

    this.userManager.events.addUserUnloaded((e) => {
      this.loggedIn = false;
    });

  }

  isLoggedInObs(): Observable<boolean> {
    return Observable.fromPromise(this.userManager.getUser()).map<User, boolean>((user) => {
      if (user) {
        return true;
      } else {
        return false;
      }
    });
  }

  clearState() {
    this.userManager.clearStaleState().then(function () {
    }).catch(function (e) {
      console.log('clearStateState error', e.message);
    });
  }

  getUser() {
    this.userManager.getUser().then((user) => {
      this.currentUser = user;
      this.userLoadededEvent.emit(user);
    }).catch(function (err) {
      console.log(err);
    });
  }

  removeUser() {
    this.userManager.removeUser().then(() => {
      this.userLoadededEvent.emit();
    }).catch(function (err) {
      console.log(err);
    });
  }

  startSigninMainWindow() {
    this.userManager.signinRedirect({ data: '' }).then(function () {
    }).catch(function (err) {
      console.log(err);
    });
  }

  endSigninMainWindow() {
    this.userManager.signinRedirectCallback().then(function (user) {
    }).catch(function (err) {
      console.log(err);
    });
  }

  startSignoutMainWindow() {
    this.userManager.signoutRedirect().then(function (resp) {

    }).catch(function (err) {
      console.log(err);
    });
  };

  endSignoutMainWindow() {
    this.userManager.signoutRedirectCallback().then(function (resp) {
      console.log('signed out', resp);
    }).catch(function (err) {
      console.log(err);
    });
  };
}
