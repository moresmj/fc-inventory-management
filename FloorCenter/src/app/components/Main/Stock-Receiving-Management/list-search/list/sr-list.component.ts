import { Component, AfterViewInit } from '@angular/core';
declare var jquery:any;
declare var $ :any;

@Component({
    selector: 'app-sr-list',
    templateUrl: './sr-list.html'
})

export class StockReceivingListComponent implements AfterViewInit{
    constructor(){
       //called first time before the ngOnInit()
    }
  
    ngAfterViewInit(){
        $(document).ready(function() {
            $("#drDate").daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            });
            $("#drDate2").daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            });
            $("#poDate").daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            });
            $("#poDate2").daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            });
            $("#drDate3").daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            });
            $("#drDate4").daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            });
        });
   
    }
  }