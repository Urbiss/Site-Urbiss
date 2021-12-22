import {Injectable} from '@angular/core';
import {HttpClient, HttpErrorResponse, HttpHeaders} from "@angular/common/http";
import {throwError} from "rxjs";
import {catchError} from "rxjs/operators";
import { Order } from '../shared/order';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class OrderService extends ApiService {
  constructor(private http: HttpClient) {
	  super();
  }

  orderSurvey(order: Order) {
    return this.http.post(this.baseUrl + '/api/order/create', JSON.stringify(order), this.httpOptions).pipe(catchError(this.handleError));
  }

  handleError(error: HttpErrorResponse) {
    return throwError(error);
  }
}
