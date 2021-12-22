import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {MapComponent} from "./views/map/map.component";
import {LoginComponent} from "./views/login/login.component";
import {DownloadOrderComponent} from "./views/download-order/download-order.component";
import {RecoverPasswordComponent} from "./views/recover-password/recover-password.component";
import {ConfirmRegistrationComponent} from "./views/confirm-registration/confirm-registration.component";
import {RecoverGuard} from "./guards/recover.guard";
import {AuthGuard} from "./guards/auth.guard";
import {MapGuard} from "./guards/map.guard";

const routes: Routes = [
  {path: 'login', component: LoginComponent, canActivate: [AuthGuard]},
  {path: '', component: MapComponent, canActivate: [MapGuard]},
  {path: 'order/:id/download', component: DownloadOrderComponent, canActivate: [MapGuard]},
  {path: 'user/:id/confirmemail/:token', component: ConfirmRegistrationComponent, canActivate: [AuthGuard]},
  {path: 'user/:email/forgotpassword/:token', component: RecoverPasswordComponent, canActivate: [RecoverGuard]},  
  {path: '**', redirectTo: '/login'},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
