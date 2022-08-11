import { Component, AfterViewInit,OnInit,Input,Output,EventEmitter,ViewChild,ViewChildren,SimpleChanges} from '@angular/core';
import { FormGroup,FormControl,FormArray,FormBuilder,Validators,NgModel} from '@angular/forms';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';


import { CommonViewService } from '@services/common/common-view.service';


@Component({

	selector: 'app-re-dr-details',
	templateUrl: 're-dr-details.html'
})



export class ReturnDeliveryRequestDetailsComponent {

	@Input()returnDeliveryDetails: any;



}