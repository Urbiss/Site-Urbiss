import {Injectable} from '@angular/core';
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {catchError} from "rxjs/operators";
import {throwError} from "rxjs";
import {Router} from "@angular/router";
import {ApiService} from "./api.service";

@Injectable({
  providedIn: 'root'
})
export class LoginService extends ApiService {
  constructor(private http: HttpClient, private router: Router) { 
	super();
  }

  login(user) {
    return this.http.post(this.baseUrl + '/api/user/authenticate', JSON.stringify(user), this.httpOptions).pipe(catchError(this.handleError))
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    this.router.navigate(['/login'])
  }

  register(user) {
    return this.http.post(this.baseUrl + '/api/user/register', JSON.stringify(user), this.httpOptions).pipe(catchError(this.handleError))
  }

  forgotPassword(email) {
    this.http.post(this.baseUrl + '/api/user/forgotpassword', JSON.stringify({email: email}), this.httpOptions).pipe(catchError(this.handleError)).subscribe()
  }

  resetPassword(data) {
	return this.http.post(this.baseUrl + '/api/user/resetpassword', JSON.stringify(data), this.httpOptions).pipe(catchError(this.handleError))
  }

  confirmEmail(data) {
	let url = this.baseUrl + '/api/user/confirmemail?userId=' + data.userId + '&code=' + data.code;
	return this.http.get(url, this.httpOptions).pipe(catchError(this.handleError))
  }

  handleError(error: HttpErrorResponse) {
    return throwError(error);
  }

  setToken(token, refreshToken) {
    localStorage.setItem('token', token);
    localStorage.setItem('refreshToken', refreshToken);
  }

  getToken() {
    return localStorage.getItem('token');
  }

  isLogged() {
    return !!this.getToken();
  }
}
