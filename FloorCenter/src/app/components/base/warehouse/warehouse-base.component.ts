import { Component,OnInit } from '@angular/core';
import { PageModuleService } from '@services/common/pageModule.service' 


@Component({
	selector: 'app-warehouse-base',
	templateUrl: './warehouse-base.html'
})

export class WarehouseBaseComponent  implements OnInit {

	constructor(private pageModuleService: PageModuleService) { }
	ngOnInit() {
		this.pageModuleService.loadScripts();
	}

}
