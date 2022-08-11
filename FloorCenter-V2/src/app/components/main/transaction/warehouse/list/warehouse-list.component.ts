import { Component,OnInit,ViewChild } from '@angular/core';
import { FormControl, FormGroup,Validators, FormBuilder,AbstractControl  } from '@angular/forms';
import { Warehouse } from '@models/warehouse/warehouse.model';
import { WarehouseService } from '@services/warehouse/warehouse.service';
import { Angular2Csv } from 'angular2-csv/Angular2-csv';


import { PageModuleService } from '@services/common/pageModule.service'
import { PagerComponent } from '@common/pager/pager.component';

import { CustomValidator } from '@validators/custom.validator';

declare var jquery:any;
declare var $ :any;

@Component({
  selector: 'app-warehouse-list',
  templateUrl: './warehouse-list.html',
})
export class WarehouseListComponent  implements OnInit{
public now: Date = new Date();

userType : number;
search : string;
warehouseForm: FormGroup;

allRecords : Warehouse[] = [];
warehouses: Warehouse[] = [];
totalRecordMessage : string;
pageRecordMessage : string;

selectedWarehouse: Warehouse;
downloadDetails:  Warehouse[] = [];

module : string = "warehouse";
showAddPanel : boolean  = false;	
constructor(private warehouseService: WarehouseService, private fb: FormBuilder, private pageModuleService: PageModuleService){

}

  code : string;

  @ViewChild(PagerComponent)
  private pager: PagerComponent;
	

	ngOnInit(){
    	this.userType = parseInt(this.pageModuleService.userType);
	}

	 addPanelShow(): void {
    this.showAddPanel = !this.showAddPanel;
  }
    onSearch()
	  {
	    if (this.showAddPanel) {
	      this.showAddPanel = false;
	    }
	  }


	loadAllWarehouse(pagerModel : any){

  		this.allRecords = pagerModel["allRecords"];
		this.warehouses =  pagerModel["pageRecord"]; 
        this.totalRecordMessage =  pagerModel["totalRecordMessage"]; 
        this.pageRecordMessage =  pagerModel["pageRecordMessage"]; 
	}

   reloadRecord(event : string) {
    if (this.pager[event]) {
      this.pager[event]();    
    }
  }	

   filterRecord() {


    if (this.search == "" && this.warehouses.length == 0) {
      this.pager["loadPageRecord"](1);
    }
    else{
      this.pager["filterPageRecord"](this.search);
    }
       
  }
 	



//get warehouse details
	public onSelect(warehouse: Warehouse): void{
		 this.showAddPanel = !this.showAddPanel;
		this.warehouseForm = this.fb.group({
		code: new FormControl(warehouse.code,[Validators.required, Validators.minLength(2),Validators.maxLength(50)]),
		name: new FormControl(warehouse.name, [Validators.required, Validators.minLength(2),Validators.maxLength(50)]),
		address: new FormControl(warehouse.address),
		 contactNumber: new FormControl(warehouse.contactNumber,Validators.required),

		id: new FormControl(warehouse.id,Validators.required)
		});

		
		
		
		
	}

	private mapWarehouse(r:any): Warehouse[] {
		return r.map(ul=>this.toWarehouse(ul));
	}

	private toWarehouse(r:any): Warehouse{
		let wh = new Warehouse({
			id: r.id,
			code: r.code,
			name: r.name,
			address: r.address,
			contactNumber: r.contactNumber,
			DateCreated : (r.dateCreated != null) ? new Date(r.dateCreated).toLocaleString() : ''
		});

		return wh;
	}

	  onChange(ch : any){
    }


	downloadWarehouseList(){
		
	

		var options = {
			fieldSeparator: ',',
			quoteStrings: '"',
			decimalseparator: '.',
			showLabels: true,
			headers: ['ID','Warehouse Code','Warehouse Name', 'Address', 'Contact No.', 'Date Added']
				
		};
		let title = this.now;
		let record= this.allRecords.map( r => this.toWarehouse(r));


		new Angular2Csv(record, title.toISOString(), options);
	}


}
