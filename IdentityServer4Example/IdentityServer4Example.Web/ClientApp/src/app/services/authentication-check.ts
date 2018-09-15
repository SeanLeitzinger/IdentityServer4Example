import { Injectable } from "@angular/core";
import { CanActivate, RouterStateSnapshot, ActivatedRouteSnapshot, Router } from "@angular/router";
import { Observable } from 'rxjs/Observable';

import { AuthenticationService } from './auth';

@Injectable()
export class AuthenticationCheck implements CanActivate {
    constructor(
        private router: Router,
        private authenticationService: AuthenticationService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {

        let isLoggedIn = this.authenticationService.isLoggedInObs();

        isLoggedIn.subscribe((loggedin) => {
            if (!loggedin) {
                this.authenticationService.startSigninMainWindow();
            }
        });

        return isLoggedIn;
    }
}
