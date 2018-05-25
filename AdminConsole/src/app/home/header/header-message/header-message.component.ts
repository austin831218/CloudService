import { Component, OnInit, AfterViewInit, ElementRef } from '@angular/core';
declare var $: any // NOTE: this show how to use jQuey plugin...

@Component({
  selector: 'li[app-header-message]',
  templateUrl: './header-message.component.html',
  styleUrls: ['./header-message.component.css']
})
export class HeaderMessageComponent implements OnInit, AfterViewInit {

  constructor(private elRef: ElementRef) { }

  ngOnInit() {
  }

  ngAfterViewInit() {
    $(this.elRef.nativeElement).find('ul.menu').slimScroll({
      height: '200px',
      alwaysVisible: true,
      size: '3px'
    }).css('width', '100%');
  }
}
