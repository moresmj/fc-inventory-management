import { Component, AfterViewInit } from '@angular/core';
declare var jquery:any;
declare var $ :any;

@Component({
    selector:'app-rft-update',
    templateUrl: './rft-update.html'
})

export class ReceiveFromTransferUpdateComponent implements AfterViewInit  {

    async ngAfterViewInit() {

         $(document).ready(function() {
            
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