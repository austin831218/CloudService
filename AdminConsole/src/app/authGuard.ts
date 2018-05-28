import { LocalStorageService } from 'ngx-webstorage';
import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';


@Injectable()
export class AuthGuard implements CanActivate {

    constructor(
        private router: Router,
        private ls: LocalStorageService
    ) {

    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | Observable<boolean> | Promise<boolean> {

        const token = this.ls.retrieve('token');

        if (token) {
            return true;
        }
        this.router.navigate(['/login']);
        return false;
    }
}
