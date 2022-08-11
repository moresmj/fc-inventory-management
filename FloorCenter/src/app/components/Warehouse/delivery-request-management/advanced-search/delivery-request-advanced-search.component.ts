import { Component,AfterViewInit } from '@angular/core';
declare var jquery:any;
declare var $ :any;

@Component({
    selector:'app-delivery-request-advanced-search',
    templateUrl:'./delivery-request-advanced-search.html'

})

export class DeliveryRequestAdvancedSearchComponent implements AfterViewInit{

    constructor(){}

    	async ngAfterViewInit() {

     $(document).ready(function(){
       
            $("#poDate").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true
            });
            $("#poDate2").daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            });
        });

    }
}