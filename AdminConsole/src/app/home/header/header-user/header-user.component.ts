import { Component, OnInit, AfterViewInit, AfterViewChecked } from '@angular/core';
import { LocalStorageService } from 'ngx-webstorage';
import { Router } from '@angular/router';

@Component({
  selector: 'li[app-header-user]',
  templateUrl: './header-user.component.html',
  styleUrls: ['./header-user.component.css']
})
export class HeaderUserComponent implements OnInit, AfterViewInit, AfterViewChecked {
  user: any = { name: {} };
  constructor(
    private ls: LocalStorageService,
    private router: Router
  ) { }

  ngOnInit() {

  }

  ngAfterViewInit() {

  }
  ngAfterViewChecked() {
    // this.log.info('header-user checked');\
    // this callback is called by angular very frequently, be careful!

  }

  logout() {
    this.ls.clear('token');
    this.router.navigate(['/login']);
  }

}
