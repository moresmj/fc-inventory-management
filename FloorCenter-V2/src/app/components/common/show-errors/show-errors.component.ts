import { Component, Input }  from '@angular/core';
import { AbstractControlDirective, AbstractControl }  from '@angular/forms';


@Component({
	selector: 'show-errors',
	template:`
	<ul *ngIf="shouldShowErrors()">
		<li style="color: red" *ngFor="let error of listOfErrors()">{{error}}</li>
	</ul>`
})

export class ShowErrorsComponent{

	private static readonly errorMesage ={
		'required': () => 'This field is required',
		'minlength': (params) => 'The min number of characters is ' + params.requiredLength,
    	'maxlength': (params) => 'The max allowed number of characters is ' + params.requiredLength,
    	'checkConfirmPassword' : (params) => params.message,
    	'telephoneNumber': (params) => params.message,
    	'emailAddress': (params) => params.message,
    	'SRP': (params) => params.message,
    	'quantity': (params) => params.message,
    	'requestedQuantity': (params) => params.message,
    	'approvedQuantity': (params) => params.message,
    	'deliveredQuantity': (params) => params.message,
    	'receivedQuantity': (params) => params.message,
		'SerialNumber': (params) => params.message,
		'Cost': (params) => params.message,
	};


	@Input()
	private control: AbstractControlDirective | AbstractControl;

	shouldShowErrors(): boolean{
		return this.control && 
			this.control.errors &&
				(this.control.dirty || this.control.touched);
	}


	listOfErrors(): string[]{
		return Object.keys(this.control.errors)
		.map(field => this.getMessage(field, this.control.errors[field]));
	}


	private getMessage(type: string, params: any){
		return ShowErrorsComponent.errorMesage[type](params);
	}
}