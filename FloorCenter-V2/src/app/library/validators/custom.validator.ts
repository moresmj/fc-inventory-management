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

		let isValidPhoneNumber = /^\d{3,3}-\d{3,3}-\d{3,3}$/.test(c.value) || /^\d{3,3}-\d{3,3}$/.test(c.value) ;
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

	static shouldNotContainDot(c: FormControl): ValidationErrors{
		let isValid = /^[^.]*$/.test(c.value);

		if(c.value == "")
		{
			isValid = true;
		}

		const message = {
			'telephoneNumber' : {
				'message': 'This field should not contain any decimal point'
			} 
		};

		return isValid ? null:message;


	}

	static emailAddress(c: FormControl): ValidationErrors{

		let isValidEmail = false;

		if (c.value == "") {
			isValidEmail = true;
		}
		else{
			isValidEmail = /^[_a-z0-9]+(\.[_a-z0-9+]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$/.test(c.value);
			if(isValidEmail)
			{
				isValidEmail = true;
			}
		}

		const message = {
			'emailAddress' : {
				'message': 'Email Address is not a valid email address.'
			} 
		};
		return isValidEmail ? null : message;
	}

	static SRP(c : FormControl) : ValidationErrors {
		
		const message = {
			'SRP' : {
				'message': 'SRP must be greater than 0'
			} 
		}
		return (c.value > 0) ? null : message;
	}

	static Cost(c : FormControl) : ValidationErrors {
		
		const message = {
			'Cost' : {
				'message': 'Cost must be greater than 0'
			} 
		}
		return (c.value > 0) ? null : message;
	}

	static quantity(c : FormControl) : ValidationErrors {
		
		const message = {
			'quantity' : {
				'message': 'Quantity must not be less than 0'
			} 
		}
		return (c.value >= 0) ? null : message;
	}

	static requestedQuantity(c : FormControl) : ValidationErrors {
		
		const message = {
			'requestedQuantity' : {
				'message': 'Quantity must be greater than 0'
			} 
		}
		return (c.value > 0) ? null : message;
	}	

	static approvedQuantity(c : FormControl) : ValidationErrors {
		
		const message = {
			'approvedQuantity' : {
				'message': 'Qty must not be less than 0'
			} 
		}
		return (c.value >= 0) ? null : message;
	}

	static deliveredQuantity(c : FormControl) : ValidationErrors {
		
		const message = {
			'deliveredQuantity' : {
				'message': 'Quantity must not be less than 0'
			} 
		}
		return (c.value >= 0) ? null : message;
	}

	static receivedQuantity(c : FormControl) : ValidationErrors {
		
		const message = {
			'receivedQuantity' : {
				'message': 'Quantity must not be less than 0'
			} 
		}
		return (c.value >= 0) ? null : message;
	}

	static physicalCount(c : FormControl) : ValidationErrors {
		
		const message = {
			'physicalCount' : {
				'message': 'Quantity must be greater than 0'
			} 
		}
		return (c.value > 0) ? null : message;
	}

	static SerialNumber(c : FormControl) : ValidationErrors {
		
		const message = {
			'SerialNumber' : {
				'message': 'Serial Number must be 9 digits'
			} 
		}

		let isSerialValid = false;

		if (c.value != null) {
			isSerialValid = (c.value.length == 9) ? true : false;
		}

		return (isSerialValid) ? null : message;
	}	


}
