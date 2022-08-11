import { Component, AfterViewInit } from '@angular/core';
declare var jquery:any;
declare var $ :any;

@Component({
	selector: 'app-og-list-details',
	templateUrl: './og-list-details.html'
})

export class OutgoingListDetailsComponent implements AfterViewInit {

	async ngAfterViewInit() {

		$(document).ready(function() {           
            $("#drDate").daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
             });
         });

	}

}