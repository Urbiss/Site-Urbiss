import {Injectable} from "@angular/core";
import {CanActivate, Router} from "@angular/router";
import {LoginService} from "../service/login.service";

@Injectable({
  providedIn: 'root'
})
export class MapGuard implements CanActivate {
  constructor(
	private loginService: LoginService, 
	private router: Router
  ) { }

  canActivate() {
    if(!this.loginService.isLogged()) {
	  localStorage.setItem('redirect', window.location.href)
      this.router.navigate(['/login'])
    }
    return this.loginService.isLogged()
  }
}
