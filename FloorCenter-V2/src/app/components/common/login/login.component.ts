import { Component, OnInit, OnChanges, SimpleChanges  } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';

import { Router } from '@angular/router';
import { AuthenticationService } from '@services/auth/authentication.service';

import { PageModuleService } from '@services/common/pageModule.service';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';

import { CommonViewService } from '@services/common/common-view.service';
@Component({
	selector: 'app-login',
	templateUrl: './login.html'
})

export class LoginComponent implements OnChanges  {
	template: string = '<img class="custom-spinner-template" src="assets/images/loader.gif">';
	loginForm : FormGroup;

	errorMessage : string;
	errorMessage2 : string;
	assignedAt : any;
	assignment : any;


	constructor(private router: Router,
		 private _auth : AuthenticationService,
		 private fb : FormBuilder, private pageModuleService : PageModuleService,
		 private _spinnerService : Ng4LoadingSpinnerService,
		 private _commonViewService : CommonViewService) {
	
		this.loginForm = this.fb.group({
			username : new FormControl('',Validators.required),
			password : new FormControl('',Validators.required)
		});
		this.pageModuleService.getAssignedAt();
		this.assignedAt = this.pageModuleService.assignedAt;
		this.assignment = this.pageModuleService.assignment;

		
	}

	ngOnInit() {
		// if(this.assignment != undefined || this.assignment != "")
		// {
		// 	this.Navigator(this.assignment);
		// }
		
	  	this.loginForm.valueChanges.subscribe(form => {
	    	this.errorMessage = null;
		});
		

		console.log(this.assignment);
		
	}

	onSubmit() {
		this._spinnerService.show();
		let formData = this.loginForm.value;
		this._auth.login(formData).subscribe(result => {
			if (result["status"] === true) {

				// this._spinnerService.hide();
				this.Navigator(result["assignment"]);
				
				// switch(result["assignment"]) { 
				//    case 1: { 
				//       this.router.navigateByUrl('/Main/dashboard'); 
				//       break; 
				//    } 
				//    case 2: { 
				//       this.router.navigateByUrl('/Warehouse/dashboard');
				//       break; 
				//    } 
				//    case 3: { 
				//       this.router.navigateByUrl('/Store/dashboard'); 
				//       break; 
				//    } 
				//    case 4: { 
				//       this.router.navigateByUrl('/Logistics/dashboard');
				//       break; 
				//    } 
				//    default: { 
				//       this.router.navigateByUrl('/Main/dashboard');
				//       break; 
				//    } 
				// }
			}
		},
        error =>{
            formData["password"] = null;
			this.loginForm.reset(formData);
			if(error !== null)
			{
			this.errorMessage2 = this._commonViewService.getErrors(error);
			console.log(this.errorMessage2);
			}

			if(error === null)
			{
				this.errorMessage = 'Username or password is incorrect';
			}
			
			this._spinnerService.hide();
        });
	}


	Navigator(assignment : any)
	{
		
		switch(parseInt(assignment)) { 
			case 1: { 
			   this.router.navigateByUrl('/Main/dashboard'); 
			   break; 
			} 
			case 2: { 
			   this.router.navigateByUrl('/Warehouse/dashboard');
			   break; 
			} 
			case 3: { 
			   this.router.navigateByUrl('/Store/dashboard'); 
			   break; 
			} 
			case 4: { 
			   this.router.navigateByUrl('/Logistics/dashboard');
			   break; 
			} 
			default: { 
			   this.router.navigateByUrl('/Main/dashboard');
			   break; 
			} 
		 }
	}

	onChange() {
		this.errorMessage2 = null;
		this.errorMessage = null;

  }
	ngOnDestroy() {
			
		this._spinnerService.hide();

	}

	ngOnChanges(changes : SimpleChanges) {
		this.errorMessage = null;
		this.errorMessage2 = null;
	}


}