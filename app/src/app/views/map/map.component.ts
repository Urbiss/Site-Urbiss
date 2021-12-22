import {Component, OnInit, TemplateRef, ViewChild} from '@angular/core';
import Map from 'ol/Map';
import View from 'ol/View';
import * as olProj from 'ol/proj';
import TileLayer from 'ol/layer/Tile';
import VectorSource from "ol/source/Vector";
import {Attribution, ScaleLine, Zoom, ZoomSlider, ZoomToExtent} from "ol/control";
import {MapParameters} from "./map.parameters";
import {MoveSelectorControl} from "../../shared/open-layers-controls/move-selector-control";
import {DrawSelectorControl} from "../../shared/open-layers-controls/draw-selector-control";
import {defaults as defaultInteractions, DragRotateAndZoom, Modify, Select, Draw } from 'ol/interaction';
import VectorLayer from "ol/layer/Vector";
import {WKT} from "ol/format";
import {OSM, XYZ} from "ol/source";
import {BsModalRef, BsModalService} from "ngx-bootstrap/modal";
import {MapService} from "../../service/map.service";
import {OrderService} from "../../service/order.service";
import {City} from "../../shared/city";
import {Wkt} from "../../shared/wkt";
import {Order} from "../../shared/order";
import {AppToastService} from "../../service/toast.service";
import { fromExtent } from "ol/geom/Polygon";
import Style from 'ol/style/Style';
import Stroke from 'ol/style/Stroke';
import Fill from 'ol/style/Fill';


@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss']
})
export class MapComponent implements OnInit {

  constructor(
    private modalService: BsModalService,
    private mapService: MapService,
    private orderService: OrderService,
    private toastService: AppToastService
  ) { }

  showSidebar: boolean;
  map: Map;
  satelite: boolean = true;
  voucher: string = '';
  orderSurvey: Order;
  orderConfirmation: boolean = false;
  selectedOrder: { userSurveyId: 0, surveyId: 0 };
  cities: City[];
  suggestions = [];
  citySelected: City;
  text: string;
  mapParameters: MapParameters = new MapParameters();
  selectedArea: string;
  emptySurveys: boolean = true;
  source = new VectorSource({wrapX: false});
  raster: TileLayer;
  vector = new VectorLayer({
    source: this.source,
  });
  selection = new Select({
    layers: [this.vector],
  });
  modify = new Modify({
    source: this.source
  })

  surveysSource = new VectorSource({
    wrapX: false,
    //strategy: bbox
  });
  surveysLayer = new VectorLayer({
    source: this.surveysSource,
    style: new Style({
      stroke: new Stroke({
        color: 'rgba(255, 255, 0, 1.0)',
        width: 2,
      }),
      fill: new Fill({
        color: 'rgba(255, 255, 0, 0.1)',

      })
    })
  });

  features = [];

  modalRef: BsModalRef;
  @ViewChild('modal')
  private template: TemplateRef<any>;

  ngOnInit() {
    this.mockParams();
    this.buildMap();
    this.getCities();
  }

  buildMap(): void {
    this.vector.setZIndex(1000);
    this.surveysLayer.setZIndex(2000);

    //Esse artifício é utilizado porque dentro da função loader o this se refere a VectorSource
    /*var mapComponent = this;
    this.surveysSource.setLoader(function(extent, resolution, projection) {
      mapComponent.showMbr();
    });*/

    this.map = new Map({
      target: 'openlayers_map',
      layers: [this.vector, this.surveysLayer],
      controls: [
        new Zoom(),
        new ScaleLine(),
        new MoveSelectorControl(),
        new Attribution(),
      ],
      interactions: defaultInteractions().extend([new DragRotateAndZoom(), this.selection, this.modify]),
      view: new View({
        center: olProj.fromLonLat(this.mapParameters.center),
        zoom: this.mapParameters.maxZoomOut,
      })
    });

    var draw = new DrawSelectorControl(this.source);
    draw.onDrawStart = this.onDrawStart.bind(this);
    draw.onAddFeature = this.onAddFeature.bind(this);
    this.map.addControl(draw);

    this.loadSatelliteMap();
    this.map.setView(
      new View({
        center: olProj.fromLonLat(this.mapParameters.center),
        zoom: this.mapParameters.zoom,
      })
    );
    this.map.addInteraction(this.modify)
  }

  onDrawStart(e) {
    this.vector.getSource().clear();
  }

  onAddFeature(e) {
    const geometry = e.feature.getGeometry().clone().transform('EPSG:3857', 'EPSG:4326');
    const wkt = new WKT().writeGeometry(geometry);
    this.selectedArea = wkt;

    var wktDto = new Wkt(wkt, 4326);

    this.mapService.getSurveys(wktDto).subscribe((data: any) => {
      this.selectedArea = data.data;

  	  if (data.data.surveys.length > 0) {
	  	this.emptySurveys = false;
  	  }
    });

    this.openModal();
  }

  showMbr() {
    var extent = this.map.getView().calculateExtent();
    var geometry = fromExtent(extent).transform('EPSG:3857', 'EPSG:4326');
    const wkt = new WKT().writeGeometry(geometry);
    var wktDto = new Wkt(wkt, 4326);

    this.surveysLayer.getSource().clear();

    this.features = [];

    this.mapService.getSurveysBounds(wktDto).subscribe((data: any) => {
      data.data.forEach(surveyItem => {
        var newFeature = new WKT().readFeature(surveyItem.wkt, {
          dataProjection: 'EPSG:4326',
          featureProjection: 'EPSG:3857'
        });
        this.features.push(newFeature);
      });
      this.surveysSource.addFeatures(this.features);
    },
    error => this.showError(error.error.Message)
    );
  }

  // TODO: remove mocked map parameters
  mockParams(): void {
    this.mapParameters.center = [-44.19, -19.96];
    this.mapParameters.maxZoomOut = 12;
    this.mapParameters.zoom = 14;
  }

  updateMapParams(city): void {
    this.mapParameters.center = [city.longitudeCenter, city.latitudeCenter];
    this.mapParameters.zoom = city.zoom;
  }

  loadStreetMap() {
    const layer = new TileLayer({source: new OSM()})
    this.map.removeLayer(this.raster)
    this.map.addLayer(layer)
    this.raster = layer;
  }

  loadSatelliteMap() {
    const layer = new TileLayer({
      source: new XYZ({
        url: 'https://mt1.google.com/vt/lyrs=y&x={x}&y={y}&z={z}',
        attributions:
          '© <a href="https://www.google.com.br/maps" target="_blank">Google Maps</a>',
        attributionsCollapsible: false,
      })
    })
    this.map.removeLayer(this.raster)
    this.map.addLayer(layer)
    this.raster = layer;
  }

  openModal() {
    this.modalRef = this.modalService.show(this.template, { class: 'order-survey'});
  }

  closeModal() {
	this.voucher = '';
	this.selectedArea = '';
	this.emptySurveys = true;
	this.orderConfirmation = false;
	this.modalRef.hide();
  }

  order(userSurveyId, surveyId) {
    this.selectedOrder = {
	  	userSurveyId: userSurveyId,
	  	surveyId: surveyId
		};
		this.orderConfirmation = true;
  }

  confirmOrder() {
		const data = new Order(
			this.selectedOrder.userSurveyId,
			this.selectedOrder.surveyId,
			this.voucher
	  );

	this.orderService.orderSurvey(data)
	  .subscribe((data: any) => {
			this.closeModal();
			this.showSuccess('Processamento criado com sucesso, as informações serão enviadas para seu e-mail.')
	  },
	  error => this.showError(error.error.Message)
	);
  }

  getCities() {
    this.mapService.getCities().subscribe((data: any) => {
      this.cities = data.data;
    })
  }

  search(event) {
    this.suggestions = []
    this.cities.forEach(city => this.suggestions.push(city.name))
    this.suggestions = this.suggestions.filter(city => city.includes(event.query));
  }

  showError(message) {
    this.toastService.show('Atenção!', message, 'danger')
  }

  showSuccess(message) {
    this.toastService.show('Operação concluída com sucesso!', message, 'success')
  }

  handleVoucher(event: any) {
	this.voucher = event.target.value;
  }

  handleMap() {
    this.satelite ? this.loadStreetMap() : this.loadSatelliteMap();
		this.satelite = ! this.satelite;
  }

  handleCity(value) {
		let city = this.cities.find(function(item, index) {
	  	if(item.id == value)
			return true;
		});

		if (!city) 
			return;

		this.updateMapParams(city);

		this.map.setView(
			new View({
				center: olProj.fromLonLat(this.mapParameters.center),
				zoom: this.mapParameters.zoom,
			})
		);
  }
}
