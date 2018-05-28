import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { PaginationModule } from 'ngx-bootstrap';
import { NgxChartsModule } from '@swimlane/ngx-charts';

@NgModule({
  declarations: [],
  exports: [
    CommonModule, FormsModule, ReactiveFormsModule, PaginationModule, NgxChartsModule
  ]
})
export class SharedModule { }
