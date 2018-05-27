import { Component, OnInit, NgModule } from '@angular/core';
import { JsonClientService, NotificationService } from '../../services/';
import { Message } from '../../Models/Message';
import { observable, Observable, Unsubscribable, Subscription } from 'rxjs';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  loginfo = 'all';
  capacity = 0;
  private lastTime: string;
  private statics: Message;
  colorScheme = {
    domain: ['#00c0ef', '#00a65a', '#f39c12', '#dd4b39']
  };

  data = [

    {
      'name': 'Activate Workers',
      'series': []
    }
  ];

  logs = [];
  private logsSubscription$: Subscription;
  constructor(
    private js: JsonClientService,
    private ns: NotificationService
  ) {

    ns.onWsMessage(m => m.Type === 'Statics')
      .subscribe(m => {
        console.debug(m);
        const s = {
          name: new Date(m.Ticks / 10000 - 2208988800000).toLocaleTimeString(),
          value: m.Data.Capacity - m.Data.Available
        };
        if (this.lastTime !== s.name) {
          this.statics = m;
          this.data[0].series.push(s);
          this.data[0].series = this.data[0].series.slice(-5);
          this.data = [...this.data];
          this.lastTime = s.name;
        }
      });
    this.subscribLog(m => true);
  }

  ngOnInit() {

  }

  subscribLog(f: (m: Message) => boolean) {
    if (this.logsSubscription$) {
      this.logsSubscription$.unsubscribe();
    }
    this.logsSubscription$ = this.ns.onWsMessage(f).subscribe(m => {
      this.logs.unshift(m);
    });
  }

  viewJobLog(name) {
    this.logs = [];
    this.subscribLog(m => m.JobName === name);
  }

  changeCapacity() {
    console.debug(this.capacity);
    this.ns.sendWSCommand('{}');
  }

}
