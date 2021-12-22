import {Component, OnInit} from '@angular/core';
import {PrimeNGConfig} from "primeng-lts/api";
import {Router} from "@angular/router";
import {LoginService} from "./service/login.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'urbiss-v2';

  constructor(private primengConfig: PrimeNGConfig, private router: Router, private loginService: LoginService) {}

  ngOnInit() {
    this.primengConfig.ripple = true;
  }

  showNavbar() {
    return this.loginService.isLogged();
  }
}
