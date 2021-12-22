import {Injectable} from '@angular/core';
import {HttpClient, HttpErrorResponse, HttpHeaders} from "@angular/common/http";
import {throwError} from "rxjs";
import {catchError} from "rxjs/operators";
import { Wkt } from '../shared/wkt';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class MapService extends ApiService {
  constructor(private http: HttpClient) {
	super();
  }

  getCities() {
    return this.http.get(this.baseUrl + '/api/city/list', this.httpOptions).pipe(catchError(this.handleError));
  }

  getSurveys(wkt: Wkt) {
    return this.http.post(this.baseUrl + '/api/survey/list', JSON.stringify(wkt), this.httpOptions).pipe(catchError(this.handleError));
  }

  getSurveysBounds(wkt: Wkt) {
    return this.http.post(this.baseUrl + '/api/survey/listbounds', JSON.stringify(wkt), this.httpOptions).pipe(catchError(this.handleError));
  }

  downloadOrder(token: string) {
	let url = this.baseUrl + '/api/order/download/' + token;
	return this.http.get(url, this.httpOptionsFile).pipe(catchError(this.handleError));
  }

  handleError(error: HttpErrorResponse) {
    return throwError(error);
  }
}
