import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {NavbarComponent} from './shared/navbar/navbar.component';
import {MapComponent} from './views/map/map.component';
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {BsDropdownModule} from "ngx-bootstrap/dropdown";
import {ButtonModule} from "primeng-lts/button";
import {SidebarModule} from "primeng-lts/sidebar";
import {ModalModule} from "ngx-bootstrap/modal";
import {LoginComponent} from "./views/login/login.component";
import {CardModule} from "primeng-lts/card";
import {InputTextModule} from "primeng-lts/inputtext";
import {TabViewModule} from "primeng-lts/tabview";
import {RippleModule} from "primeng-lts/ripple";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {KeyFilterModule} from "primeng-lts/keyfilter";
import {MessageModule} from "primeng-lts/message";
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import {ToastComponent} from './shared/toast/toast.component';
import {HttpClientModule} from "@angular/common/http";
import {AutoCompleteModule} from "primeng-lts/autocomplete";
import localePt from '@angular/common/locales/pt';
import {registerLocaleData} from '@angular/common';
import {NgxMaskModule} from 'ngx-mask';
import {DownloadOrderComponent} from './views/download-order/download-order.component';
import {RecoverPasswordComponent} from './views/recover-password/recover-password.component';
import {ConfirmRegistrationComponent} from './views/confirm-registration/confirm-registration.component';

registerLocaleData(localePt);

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    MapComponent,
    LoginComponent,
	DownloadOrderComponent,
	RecoverPasswordComponent,
	ConfirmRegistrationComponent,
    ToastComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    AutoCompleteModule,
    ButtonModule,
    SidebarModule,
    InputTextModule,
    KeyFilterModule,
    MessageModule,
    RippleModule,
    CardModule,
    TabViewModule,
    BsDropdownModule.forRoot(),
    ModalModule.forRoot(),
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    HttpClientModule,
	NgxMaskModule.forRoot()
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {
}
