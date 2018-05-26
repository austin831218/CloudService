import { Component, OnInit } from '@angular/core';
import { NotificationService } from '../services/';

@Component({
  selector: '.wrapper',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private ns: NotificationService) {

  }

  ngOnInit() {
    console.log(1)
  }

}
