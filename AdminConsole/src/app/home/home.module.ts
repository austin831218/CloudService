import { NgModule } from '@angular/core';
import { SharedModule } from '../shared.module';
import { HomeRoutingModule } from './home.routing';
import { HomeComponent } from './home.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { HeaderComponent } from './header/header.component';
import { HeaderUserComponent } from './header/header-user/header-user.component';
import { HeaderMessageComponent } from './header/header-message/header-message.component';
import { HeaderNotificationComponent } from './header/header-notification/header-notification.component';
import { HeaderTaskComponent } from './header/header-task/header-task.component';

@NgModule({
  imports: [
    SharedModule,
    HomeRoutingModule
  ],
  declarations: [
    HomeComponent,
    DashboardComponent,
    HeaderComponent,
    HeaderUserComponent,
    HeaderMessageComponent,
    HeaderNotificationComponent,
    HeaderTaskComponent
  ]
})
export class HomeModule { }
