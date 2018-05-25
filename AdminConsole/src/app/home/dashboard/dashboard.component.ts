import { Component, OnInit } from '@angular/core';
import { JsonClientService, NotificationService } from '../../services/';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  constructor(
    private js: JsonClientService,
    private ns: NotificationService
  ) { }

  ngOnInit() {

  }

  test() {
    this.ns.cast('lock', true);
    this.js.get('accounts1').then(data => {
      console.log(data);
    }).catch(err => {
      console.error(err);
    });
  }
}
