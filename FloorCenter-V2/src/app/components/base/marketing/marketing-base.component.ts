import { Component,OnInit } from '@angular/core';
//import { PageModuleService } from '@services/common/pageModule.service' 


@Component({
	selector: 'app-marketing-base',
	templateUrl: './marketing-base.html'
})

export class MarketingBaseComponent  implements OnInit {

	constructor( /* private pageModuleService: PageModuleService */) { }
	ngOnInit() {
		//this.pageModuleService.loadScripts();
	}

}
