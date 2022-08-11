import { Component, OnInit, OnChanges, SimpleChanges  } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { environment } from 'environments/environment';
import { Router } from '@angular/router';
import { AuthenticationService } from '@services/auth/authentication.service';
import { PageModuleService } from '@services/common/pageModule.service';

@Component({
	selector: 'app-error-404',
	templateUrl: './error-404.html'
})

export class Error404Component implements OnChanges,OnInit  {
	assignment : any;
	redirectTo : any;


	constructor(private router: Router, private _auth : AuthenticationService
		,private pageModuleService : PageModuleService) {

		
		

	
	}

	ngOnInit() {

		
	  	
	}

	goBack() {
  		window.history.back();
	}

	ngOnChanges(changes : SimpleChanges) {
	
	}


}