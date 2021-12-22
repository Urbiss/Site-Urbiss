import {Component, OnInit} from '@angular/core';
import {LoginService} from "../../service/login.service";

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {

  username: string;

  constructor(private loginService: LoginService) {
  }

  ngOnInit(): void {
    this.username = localStorage.getItem('userName');
  }

  logout() {
    this.loginService.logout();
  }
}
