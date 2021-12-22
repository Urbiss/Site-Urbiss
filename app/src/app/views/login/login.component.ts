import {Component, HostListener, OnInit, TemplateRef, ViewChild} from '@angular/core';
import {User} from "../../shared/user";
import {LoginService} from "../../service/login.service";
import {FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";
import {AppToastService} from "../../service/toast.service";
import {BsModalRef, BsModalService} from "ngx-bootstrap/modal";
import {Router} from "@angular/router";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  @ViewChild('modal')
  private template: TemplateRef<any>;

  @ViewChild('modalForgot')
  private templateForgotPassword: TemplateRef<any>;

  loginForm: FormGroup;
  registerForm: FormGroup;
  forgotPasswordForm = new FormControl('', [Validators.required, Validators.email])

  user: User = new User();
  mobile: boolean;

  modalRef: BsModalRef;
  modalMessage = {
    header: '',
    message: ''
  };
  tabViewIndex: number = 0;

  constructor(private loginService: LoginService,
              private fb: FormBuilder, private toastService: AppToastService,
              private modalService: BsModalService,
              private router: Router) { }

  ngOnInit(): void {
    this.mobile = window.screen.width <= 1024;
    this.loginForm = this.fb.group({
      'email': new FormControl('', Validators.required),
      'password': new FormControl('', [Validators.required]),
    });

    this.registerForm = this.fb.group({
      'fullName': new FormControl('', Validators.required),
      'email': new FormControl('', [Validators.required, Validators.email]),
      'password': new FormControl('', [Validators.required, Validators.minLength(8)]),
      'confirmPassword': new FormControl('', [Validators.required, Validators.minLength(8)]),
    })
  }

  @HostListener('window:resize', ['$event'])
  onResize(event) {
    this.mobile = window.screen.width <= 1024;
  }

  login() {
    this.user = new User(this.loginForm.value)
    if(!this.user.email.match('[a-z0-9.]+@[a-z0-9]+\\.[a-z]+')) {
      this.loginForm.controls['email'].setErrors({'incorrect': true})
    }

    if(this.user.password.length < 6) {
      this.loginForm.controls['password'].setErrors({'incorrect': true});
    }

    if(this.loginForm.valid) {
      this.user = new User(this.loginForm.value)
      this.loginService.login(this.user).subscribe((data:any) => this.loginConfirmation(data), error => this.showError(error.error.Message))
    }
  }

  register() {
    this.user = new User(this.registerForm.value)
    if(this.verifyPassword(this.user.password, this.user.confirmPassword)) {
      this.loginService.register(this.user).subscribe((data: any) => this.registerConfirmation(data), error => this.showError(error.error.Message))
    }
    this.registerForm.reset()
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

  registerConfirmation(data: any) {
    if(data.succeeded == true) {
      this.modalMessage = {header: 'Confirmação de registro', message: 'Usuário registrado com sucesso!'};
      this.modalRef = this.modalService.show(this.template);
      this.tabViewIndex = 0;
    }
  }

  loginConfirmation(data: any) {
    if(data.succeeded == true) {      
	  this.loginService.setToken(data.data.jwToken, data.data.refreshToken)
      localStorage.setItem('userName', data.data.userName)

	  let redirect = localStorage.getItem('redirect')

	  if (redirect) {
		localStorage.removeItem('redirect')
		window.location.href = redirect
	  } else {
      	this.router.navigate([''])
	  }
    }
  }

  showError(message) {
    this.toastService.show('Atenção!', message, 'danger')
  }

  forgotPassword() {
    this.modalRef = this.modalService.show(this.templateForgotPassword);
  }

  sendPasswordRedefinition() {
    this.loginService.forgotPassword(this.forgotPasswordForm.value)
    this.modalRef.hide();
    this.modalMessage = {header: 'Redefinição de senha', message: 'Foi enviado para seu email as instruções de redefinição de senha!'}
    this.modalRef = this.modalService.show(this.template);
    this.forgotPasswordForm.reset();
  }
}
