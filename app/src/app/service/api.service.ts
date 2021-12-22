import {HttpHeaders} from "@angular/common/http";
import {environment} from "../../environments/environment";

export class ApiService { 
  baseUrl = environment.api;
	
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + localStorage.getItem('token')
    })
  };

  httpOptionsFile = {
    headers: new HttpHeaders({
	  'Authorization': 'Bearer ' + localStorage.getItem('token')
	}),
	responseType: 'arraybuffer' as 'json'
  };

  constructor() { }
}