import { Component, OnInit, NgModule } from '@angular/core';
import { JsonClientService, NotificationService } from '../../services/';


@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  private lastTime: string;
  colorScheme = {
    domain: ['#00c0ef', '#00a65a', '#f39c12', '#dd4b39']
  };

  data = [

    {
      "name": "Activate Workers",
      "series": []
    }
  ];
  constructor(
    private js: JsonClientService,
    private ns: NotificationService
  ) {

    ns.onWsMessage(m => m.Type === 'Statics')
      .subscribe(m => {
        console.debug(this.data);
        const s = {
          name: new Date(m.Ticks / 10000 - 2208988800000).toLocaleTimeString(),
          value: m.Data.Capacity - m.Data.Available
        };
        if (this.lastTime !== s.name) {
          this.data[0].series.push(s);
          this.data[0].series = this.data[0].series.slice(-5);
          this.data = [...this.data];
          this.lastTime = s.name;
        }

      });
  }

  ngOnInit() {

  }


}
