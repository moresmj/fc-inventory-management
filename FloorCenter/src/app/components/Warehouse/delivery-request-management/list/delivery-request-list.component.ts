import { Component, AfterViewInit }  from '@angular/core';
declare var jquery:any;
declare var $ :any;

@Component({
    selector:'delivery-request-list',
    templateUrl:'./delivery-request-list.html'

})


export class DeliveryRequestListComponent implements AfterViewInit {

    constructor(){

    }
	async ngAfterViewInit() {

     $(document).ready(function(){
       
            $("#rDate").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true
            });
            $("#rDate2").daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            });
        });

    }
    
}