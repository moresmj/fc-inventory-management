import { FormArray, FormGroup, FormControl, ValidationErrors } from '@angular/forms';

export class CustomValidator {
	static checkConfirmPassword(form: FormGroup): ValidationErrors {
		const passwordControl = form.get("Password").value;
		const confirmPasswordControl = form.get("ConfirmPassword").value;

		let error = null;
		if ((passwordControl !== confirmPasswordControl) && confirmPasswordControl != "") {
			error = 'Password and Confirm Password do not match.';
		}

		const message = { 
							'checkConfirmPassword' : 
								{ 'message' : error } 
						};

		return error ? message : null;
	}


	static telephoneNumber(c: FormControl): ValidationErrors{

		let isValidPhoneNumber = /^\d{3,3}-\d{3,3}-\d{3,3}$/.test(c.value);
		if(c.value == "")
		{
			isValidPhoneNumber = true;

		}

		const message = {
		'telephoneNumber' : {
			'message': 'The phone number be valid (XXX-XXX-XXX, where X is a digit)'
		} 

		};



		return isValidPhoneNumber ? null:message;

	}
}