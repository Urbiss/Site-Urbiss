import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";
import {LoginService} from "../../service/login.service";
import {AppToastService} from "../../service/toast.service";

@Component({
  selector: 'app-recover-password',
  templateUrl: './recover-password.component.html',
  styleUrls: ['./recover-password.component.scss']
})
export class RecoverPasswordComponent implements OnInit {
  token: string = '';
  email: string = '';
  resetPasswordForm: FormGroup;

  constructor(
	private fb: FormBuilder,
	private route: ActivatedRoute,
	private router: Router,
	private loginService: LoginService,
	private toastService: AppToastService
  ) { }

  ngOnInit(): void {

console.log('entrou');

    this.token = this.route.snapshot.paramMap.get('token');
    this.email = this.route.snapshot.paramMap.get('email');

    this.resetPasswordForm = this.fb.group({
      'password': new FormControl('', [
        Validators.required, 
        Validators.minLength(6)
      ]),
      'confirmPassword': new FormControl('', Validators.required)
    });
  }

  resetPassword() {
    let dataForm = this.resetPasswordForm.value;
    
		if(! this.verifyPassword(dataForm.password, dataForm.confirmPassword)) {
			return;
		}

		let data = {
			email: this.email,
			token: this.token,
			password: dataForm.password,
			confirmPassword: dataForm.confirmPassword
		}

		this.loginService.resetPassword(data).subscribe((data: any) => this.redirect(), error => this.handleError(error.error.Message));
		this.resetPasswordForm.reset();
  }

  verifyPassword(password, confirmPassword): boolean {
    if(!password || !confirmPassword) {
      return false;
    } else if (password != confirmPassword) {
      this.toastService.show('Atenção!', 'As senhas não correspondem', 'danger');
      return false;
    }
    return true
  }

  redirect(){
    window.location.href = '/';
  }

  handleSuccess(message: string) {
	  this.toastService.show('Atenção!', message, 'success');
  }

  handleError(message: string) {
	  this.toastService.show('Atenção!', message, 'danger');
  }
}
