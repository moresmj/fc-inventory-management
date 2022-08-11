import { Component, OnInit, OnChanges, SimpleChanges  } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { environment } from 'environments/environment';
import { Router } from '@angular/router';
import { AuthenticationService } from '@services/auth/authentication.service';
import { PageModuleService } from '@services/common/pageModule.service';

@Component({
	selector: 'app-error-401',
	templateUrl: './error-401.html'
})

export class Error401Component implements OnChanges,OnInit  {
	assignment : any;
	redirectTo : any;


	constructor(private router: Router, private _auth : AuthenticationService
		,private pageModuleService : PageModuleService) {

		if(localStorage.getItem('currentUser'))
		{
			this.pageModuleService.loadScripts();
			this.assignment = this.pageModuleService.assignment;
			if(this.assignment == 1)
			{
				this.redirectTo = environment.baseUrl + 'Main';
			}
			else if(this.assignment == 2)
			{
				this.redirectTo = environment.baseUrl + 'Warehouse';
			}
			else if(this.assignment == 3)
			{
				this.redirectTo = environment.baseUrl + 'Store';
			}
			else if(this.assignment == 4)
			{
				this.redirectTo = environment.baseUrl + 'Logistics';
			}

		}
		
		

	
	}

	ngOnInit() {

		
	  	
	}

	

	ngOnChanges(changes : SimpleChanges) {
	
	}


}