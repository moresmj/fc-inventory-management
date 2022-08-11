import { Component, AfterViewInit,OnInit,EventEmitter,Output } from '@angular/core';
import { FormControl, FormGroup,Validators, FormBuilder } from '@angular/forms';

import { Warehouse } from '@models/warehouse/warehouse.model';
import { WarehouseService } from '@services/warehouse/warehouse.service';

import { CustomValidator } from '@validators/custom.validator';
declare var jquery:any;
declare var $ :any;

@Component({
  selector: 'app-w-list-add',
  templateUrl: './w-list-add.html'
})
export class WarehouseListAddComponent implements AfterViewInit,OnInit {
@Output() updatePage: EventEmitter<String> = new EventEmitter<String>();
warehouse: Warehouse;
warehouseForm: FormGroup;
errorMessage : any;
statusMessage: string;

	async ngAfterViewInit() {

        $(document).ready(function(){
            $(".addNew").click(function(){
                $("#addItems").toggle();
            });
            $("#cancel").click(function(){
                $("#addItems").toggle();
            });
        });	

	}


    constructor(private warehouseService: WarehouseService, private fb: FormBuilder){

    }


    ngOnInit(): void{
        this.warehouseForm = this.fb.group({
        'code': new FormControl('',Validators.required),
        'name': new FormControl('', Validators.required),
        'address': new FormControl(''),
        'contactNumber': new FormControl('', Validators.compose([Validators.minLength(6),Validators.maxLength(12),CustomValidator.telephoneNumber]))

        });
    }

    createForm(){
        this.warehouseForm = this.fb.group({
            code: ['',Validators.required],
            name: ['',Validators.required],
            address: [''],
            contactNumber: ['', Validators.compose([Validators.minLength(6),Validators.maxLength(12),CustomValidator.telephoneNumber])]
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
            this.errorMessage = error.Code;

        });
    }

    reset(){
        this.createForm();
        this.errorMessage = null;
        this.statusMessage = null;
    }

}
