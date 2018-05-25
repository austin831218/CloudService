import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { PaginationModule } from 'ngx-bootstrap';

@NgModule({
  declarations: [],
  exports: [
    CommonModule, FormsModule, ReactiveFormsModule, PaginationModule
  ]
})
export class SharedModule { }
