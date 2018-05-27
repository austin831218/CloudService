import { Component, OnInit } from '@angular/core';
import { NotificationService } from '../../../services/';


@Component({
  selector: 'li[app-header-notification]',
  templateUrl: './header-notification.component.html',
  styleUrls: ['./header-notification.component.css']
})
export class HeaderNotificationComponent implements OnInit {
  
  notifications = [];
  constructor(private ns: NotificationService) {
    this.ns.onWsMessage(x => x.Type === 'Notification')
      .subscribe(x => this.notifications.push(x));
  }

  ngOnInit() {
  }

  clear() {
    this.notifications = [];
  }

}
