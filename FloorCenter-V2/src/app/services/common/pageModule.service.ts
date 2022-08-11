import { Injectable, Inject  } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

@Injectable()
export class PageModuleService {

	cookieService: CookieService;

	public username : string;
	public userType : string;
	public assignedAt : string;
	public assignment : string;
	public whId : any;

	public storesHandled : any;

	constructor(@Inject(CookieService) private _cookieService: CookieService) {
		this.cookieService = _cookieService;
	}


	public loadScripts() {        

		this.getAssignedAt();

		let pageScripts = [];

	    var scripts = document.getElementsByTagName("script")
		for(let i = 0; i < scripts.length; i++) {

			// Saves all the current script of the page into the array
			pageScripts.push(scripts[i].getAttribute("src"));

		}	

        var dynamicScripts = [ 
        						"assets/vendor/jquery/jquery.min.js",
						        "assets/vendor/bootstrap/js/bootstrap.min.js",
						        "assets/vendor/metisMenu/metisMenu.min.js",
						        "assets/vendor/datatables/js/jquery.dataTables.min.js",
						        "assets/vendor/datatables-plugins/dataTables.bootstrap.min.js",
						        "assets/vendor/datatables-responsive/dataTables.responsive.js",
						        "assets/vendor/select2/js/select2.full.min.js",
						        "assets/vendor/chartist/chartist.min.js",
						        "assets/vendor/datepicker/moment.min.js",
						        "assets/vendor/datepicker/daterangepicker.js",
						        "assets/dist/js/sb-admin-2.js",
						        "assets/dist/js/script.js"
						     ];


		for(let i = 0; i < dynamicScripts.length; i++) {

			// Checks the page scripts array if the scripts that will be appended is existing
			let scriptIndex = pageScripts.indexOf(dynamicScripts[i]);

			// Item on dynamicScripts do not exist on the current page. 
			if (scriptIndex == -1) {
	 			this.appendScript(dynamicScripts[i]);
			}

			else {
				scripts[scriptIndex].remove();
				this.appendScript(dynamicScripts[i]);
			}
		}				     
    
	}

	public getAssignedAt() {
		this.username = this.cookieService.get('username');
		this.userType = this.cookieService.get('userType');
		this.assignedAt = this.cookieService.get('assignedAt');
		this.assignment = this.cookieService.get('assignment');
		this.whId = this.cookieService.get('warehouseId');
	}


	private appendScript(src : string) {
        let node = document.createElement('script');
        node.type = 'text/javascript';
        node.src = src;
        node.async = false;
        document.getElementsByTagName('head')[0].appendChild(node);
	}


}
