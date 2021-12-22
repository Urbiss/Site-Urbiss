import {Component, OnInit} from '@angular/core';
import {MapService} from "../../service/map.service";
import {AppToastService} from '../../service/toast.service';
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-download-order',
  templateUrl: './download-order.component.html',
  styleUrls: ['./download-order.component.scss']
})
export class DownloadOrderComponent implements OnInit {
  id: string = '';

  constructor(
	private mapService: MapService,
	private toastService: AppToastService,
	private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id');
	this.download();
  }

  download() {
	this.mapService.downloadOrder(this.id).subscribe(
	  (data: any) => this.getFile(data), 
	  error => this.handleError(error.error.Message)
	);
  }

  getFile(data) {
	let blob = new Blob([data], {type: 'application/zip'});
	let fileName = this.id + '.zip';
	
	let url = window.URL.createObjectURL(blob);
	
	let a = document.createElement('a');
	document.body.appendChild(a);
	a.href = url;
	a.download = fileName;
	a.click();
	
	window.URL.revokeObjectURL(url);
	a.remove();
  } 

  handleError(message: string) {
	this.toastService.show('Atenção!', message, 'danger');
  }
}
