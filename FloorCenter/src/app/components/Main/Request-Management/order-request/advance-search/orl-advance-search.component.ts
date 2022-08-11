import { Component, AfterViewInit } from '@angular/core';
declare var $query: any;
declare var $: any;

@Component({
    selector: 'app-orl-advance-search',
    templateUrl: './orl-advance-search.html'
})

export class OrderRequestListAdvanceSearchComponent implements AfterViewInit{

    constructor() {

    }

    async ngAfterViewInit() {
        $(document).ready(function(){
            $('#poDate').daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            });
            $('#poDate2').daterangepicker({
                singleDatePicker: true,
                showDropdowns: true
            })
        });
    } 
}
