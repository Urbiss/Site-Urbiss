import {Injectable} from "@angular/core";
import {CanActivate, Route, Router} from "@angular/router";
import {LoginService} from "../service/login.service";

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private loginService: LoginService, private router: Router) {
  }

  canActivate() {
    return !this.loginService.isLogged()
  }
}
