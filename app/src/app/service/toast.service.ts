import {Injectable} from "@angular/core";

@Injectable({providedIn: 'root'})
export class AppToastService {
  toasts: any[] = [];

  show(header: string, body: string, type?: string) {

    switch (type) {
      case 'danger': {
        this.toasts.push({header, body, class: ['bg-danger', 'text-light']});
        break;
      }
      case 'success' : {
        this.toasts.push({header, body, class: ['bg-success', 'text-light']});
        break;
      }
      case 'warning': {
        this.toasts.push({header, body, class: ['bg-warning']});
        break;
      }
      default: {
        this.toasts.push({header, body});
        break;
      }
    }
  }

  remove(toast) {
    this.toasts = this.toasts.filter(t => t != toast);
  }
}
