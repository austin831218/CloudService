import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { LocalStorageService } from 'ngx-webstorage';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  vm: any = {};
  constructor(
    private router: Router,
    private toastr: ToastrService,
    private ls: LocalStorageService
  ) { }

  ngOnInit() {
  }

  login() {
    this.toastr.success('login successfully');
    this.ls.store('token', 'fake');
    this.router.navigate(['/']);
  }
}
