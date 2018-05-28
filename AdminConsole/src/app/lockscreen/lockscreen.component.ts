import { Component, OnInit, OnDestroy } from '@angular/core';
import { NotificationService } from '../services/';
import { Subscription } from 'rxjs/Subscription'

@Component({
  selector: 'app-screen-locker',
  templateUrl: './lockscreen.component.html',
  styleUrls: ['./lockscreen.component.css']
})
export class LockscreenComponent implements OnInit, OnDestroy {
  private sub$: Subscription;
  locked = false;
  constructor(
    private ns: NotificationService
  ) {
    this.sub$ = this.ns.on<boolean>('lock').subscribe(lock => this.locked = lock);
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    this.sub$.unsubscribe();
  }
}
