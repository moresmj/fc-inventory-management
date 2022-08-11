import { Component,OnInit } from '@angular/core';
import { PageModuleService } from '@services/common/pageModule.service' 


@Component({
	selector: 'app-store-base',
	templateUrl: './store-base.html'
})

export class StoreBaseComponent implements OnInit {

	constructor(private pageModuleService: PageModuleService) { }
	ngOnInit() {
		this.pageModuleService.loadScripts();
	}

}
