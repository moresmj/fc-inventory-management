import { Component, AfterViewInit,OnInit,EventEmitter,Output } from '@angular/core';
import { FormControl, FormGroup,Validators, FormBuilder } from '@angular/forms';

import { Warehouse } from '@models/warehouse/warehouse.model';
import { WarehouseService } from '@services/warehouse/warehouse.service';
import { CommonViewService } from '@services/common/common-view.service';
import { CustomValidator } from '@validators/custom.validator';
declare var jquery:any;
declare var $ :any;

@Component({
  selector: 'app-warehouse-add',
  templateUrl: './warehouse-add.html'
})
export class WarehouseAddComponent implements AfterViewInit,OnInit {
@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();
@Output() hidePanelAdd: EventEmitter<boolean> = new EventEmitter<boolean>();
warehouse: Warehouse;
warehouseForm: FormGroup;
errorMessage : any;
statusMessage: string;

	async ngAfterViewInit() {

       

	}


    constructor(private warehouseService: WarehouseService,
     private fb: FormBuilder,
     private commonViewService: CommonViewService){

    }


    ngOnInit(): void{
        this.warehouseForm = this.fb.group({
        'code': new FormControl('',[Validators.required, Validators.minLength(2)]),
        'name': new FormControl('', [Validators.required, Validators.minLength(2)]),
        'address': new FormControl(''),
        'contactNumber': new FormControl('',Validators.required)

        });
    }

    createForm(){
        this.warehouseForm = this.fb.group({
            code: ['',[Validators.required, Validators.minLength(2)]],
            name: ['',[Validators.required, Validators.minLength(2)]],
            address: [''],
            contactNumber: ['',Validators.required]
        });
    }


    addWare(){
        let warehouse = this.warehouseForm.value;

        this.warehouseService.addWarehouse(warehouse)
        .subscribe(successCode =>{
    
            this.updatePage.emit("loadPageRecord");
            this.reset();
            this.errorMessage = null;
            this.statusMessage = "Warehouse successfully added";

        },
        error =>{
            this.errorMessage = this.commonViewService.getErrors(error);
    

        });
    }

    reset(){
        this.createForm();
        this.errorMessage = null;
        this.statusMessage = null;
    }

     cancelAdd(){
    this.hidePanelAdd.emit();
  }
      onChange(ch : any){

        this.errorMessage = null;
        this.statusMessage = null;
    }


}
