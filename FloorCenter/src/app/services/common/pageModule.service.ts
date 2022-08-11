import { Injectable } from '@angular/core';

@Injectable()
export class PageModuleService {

	public loadScripts() {        
	    var isFound = false;
	    var scripts = document.getElementsByTagName("script")
	    for (var i = 0; i < scripts.length; ++i) {
	        if (scripts[i].getAttribute('src') != null && scripts[i].getAttribute('src').includes("loader")) {
	            isFound = true;
	        }
	    }

	    if (!isFound) {
	        var dynamicScripts = ["assets/vendor/jquery/jquery.min.js",
							        "assets/vendor/bootstrap/js/bootstrap.min.js",
							        "assets/vendor/metisMenu/metisMenu.min.js",
							        "assets/vendor/datatables/js/jquery.dataTables.min.js",
							        "assets/vendor/datatables-plugins/dataTables.bootstrap.min.js",
							        "assets/vendor/datatables-responsive/dataTables.responsive.js",
							        "assets/vendor/chartist/chartist.min.js",
							        "assets/vendor/datepicker/moment.min.js",
							        "assets/vendor/datepicker/daterangepicker.js",
							        "assets/dist/js/sb-admin-2.js",
							        "assets/dist/js/script.js"
							     ];

	        for (var i = 0; i < dynamicScripts .length; i++) {
	            let node = document.createElement('script');
	            node.type = 'text/javascript';
	            node.src = dynamicScripts [i];
	            node.async = false;
	            document.getElementsByTagName('head')[0].appendChild(node);
	        }

	    }
	}


}
