import { Component, OnInit  } from '@angular/core';
import { PageModuleService } from '@services/common/pageModule.service';


@Component({
	selector: 'app-main-base',
	templateUrl: './main-base.html'
})

export class MainBaseComponent implements OnInit   {

	constructor(private pageModuleService: PageModuleService) { }
	ngOnInit() {
		this.pageModuleService.loadScripts();
	}
		
}
