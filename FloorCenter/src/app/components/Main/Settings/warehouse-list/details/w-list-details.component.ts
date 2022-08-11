import { Component, AfterViewInit,Input,Output,OnInit,OnChanges,SimpleChanges,EventEmitter } from '@angular/core';
import { FormControl, FormGroup,Validators, FormBuilder,AbstractControl  } from '@angular/forms';
import { Warehouse } from '@models/warehouse/warehouse.model';

import { WarehouseService } from '@services/warehouse/warehouse.service';
import { CommonViewService } from '@services/common/common-view.service';


@Component({
  selector: 'app-w-list-details',
  templateUrl: './w-list-details.html',
})
export class WarehouseListDetailsComponent implements OnInit,OnChanges {
	@Input()warehouseUpdateForm: FormGroup;
	@Input()warehouse: Warehouse;
	@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();
	errorMessage: any;
	statusMessage: string;
	

	ngOnChanges(changes: SimpleChanges){
		if(changes["warehouseUpdateForm"])
		{
			this.errorMessage = null;
			this.statusMessage = null;
		}
	}
	constructor(private warehouseService: WarehouseService, private commonViewService: CommonViewService, private fb: FormBuilder)
	{
	

	}

	ngOnInit(): void{
		
	
	}


	updateEmp(){

		let warehouse = this.warehouseUpdateForm.value;
		this.warehouseService.updateWarehouse(warehouse.id, warehouse)
		.subscribe(successcode =>{
		this.statusMessage ="Warehouse successfully updated"
		this.errorMessage = null;
		this.updatePage.emit("loadPageRecord");
	

		},
		error=>{
	
	
			this.errorMessage = this.commonViewService.getErrors(error);
	
		});
	}

}
