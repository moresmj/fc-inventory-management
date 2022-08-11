import { Component,AfterViewInit } from '@angular/core';

declare var jquery:any;
declare var $ :any;

@Component({
	selector: 'app-dr-update',
	templateUrl: './dr-update.html'


})



export class RequestUpdateComponent implements AfterViewInit{

	
	async ngAfterViewInit() {

        $(document).ready(function(){
            $(".addNew").click(function(){
                $("#addItems").toggle();
            });
            $("#cancel").click(function(){
                $("#addItems").toggle();
            });
        });	

	}


}