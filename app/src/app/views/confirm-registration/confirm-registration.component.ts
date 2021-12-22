import {Component, OnInit} from '@angular/core';
import {LoginService} from "../../service/login.service";
import {AppToastService} from "../../service/toast.service";
import {Router, ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-confirm-registration',
  templateUrl: './confirm-registration.component.html',
  styleUrls: ['./confirm-registration.component.scss']
})
export class ConfirmRegistrationComponent implements OnInit {
  id: string = '';
  token: string = '';

  constructor(
		private loginService: LoginService,
		private toastService: AppToastService,
		private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id');
		this.token = this.route.snapshot.paramMap.get('token');

		this.confirmEmail();
  }

	confirmEmail() {
		let data = {
			userId: this.id,
			code: this.token
		}
		this.loginService.confirmEmail(data).subscribe(
			(data: any) => {  }, 
			error => this.handleError(error.error.Message)
		);
	}

	handleSuccess(message: string) {
		this.toastService.show('Atenção!', message, 'success');
	}
	
	handleError(message: string) {
		this.toastService.show('Atenção!', message, 'danger');
	}
}
