import { Injectable } from '@angular/core';
import { Http, Response,RequestOptions,Headers} from '@angular/http';

import { StoreService } from '@services/store/store.service';
import { WarehouseService } from '@services/warehouse/warehouse.service';
import { ProductService } from '@services/product/product.service';
import { UserService } from '@services/user/user.service';
import { OrderRequestService } from '@services/order_request/order-request.service';

import { StockService } from '@services/stock/stock.service';
import { StoreInventoriesService } from '@services/store-inventories/store-inventories.service';
import { StoreRequestService } from '@services/store-request/store-request.service';
import { WarehouseInventoriesService } from '@services/warehouse/warehouse-inventories.service';


import { Observable } from 'rxjs/Observable';

@Injectable()
export class BaseService{

	private services : Observable<any>[] = [];

	constructor(
		private _storeService : StoreService,
		private _warehouseService : WarehouseService,
		private _userService : UserService,
		private _productService : ProductService,
		private _orderRequestService : OrderRequestService,
		private _stockService : StockService,
		private _storeInventoriesService : StoreInventoriesService,
		private _storeRequestService : StoreRequestService,
		private _warehouseInventoriesService: WarehouseInventoriesService

		) 		
	{
		this.services["store"] = this._storeService;
		this.services["warehouse"] = this._warehouseService;
		this.services["product"] = this._productService;
		this.services["user"] = this._userService;
		this.services["order_request"] = this._orderRequestService;
		this.services["stock"] = this._stockService;
		this.services["storeInventories"] = this._storeInventoriesService;
		this.services["storeRequest"] = this._storeRequestService;
		this.services["w-inventories"] = this._warehouseInventoriesService;

	}

	getService(serviceName : string): Object 
	{
		return this.services[serviceName];
	}

}