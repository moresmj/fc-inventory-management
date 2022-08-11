import { Injectable } from '@angular/core';
import { Http, Response,RequestOptions,Headers} from '@angular/http';

import { UserService } from '@services/user/user.service';
import { StoreService } from '@services/store/store.service';
import { WarehouseService } from '@services/warehouse/warehouse.service';
import { ItemService } from '@services/item/item.service';
import { ApproveRequestsService } from '@services/approve-requests/approve-requests.service';
import { ReceiveItemsService } from '@services/transactions/receive-items.service';
import { OrderStockService } from '@services/order-stock/order-stock.service';
import { DeliveriesService } from '@services/deliveries/deliveries.service';
import { ReleaseItemsService } from '@services/release-item/release-items.service';
import { OrderService } from '@services/order/order.service';
import { ReleasingService } from '@services/releasing/releasing.service';
import { InventoriesService } from '@services/inventories/inventories.service';
import { SalesOrderService } from '@services/sales-order/sales-order.service';
import { BranchOrderService } from '@services/branch-order/branch-order.service';
import { RequestService } from '@services/request.service';
import { ClientReleasingService } from '@services/releasing/client-releasing.service';
import { SalesOrderReleasingService } from '@services/releasing/sales-order-releasing.service';
import { SameDaySalesReleasingService } from '@services/releasing/same-day-sales-releasing.service';
import { TransferReleasingService } from '@services/releasing/transfer-releasing.service';

import { ApiBaseService } from '@services/api-base.service';

import { Observable } from 'rxjs/Observable';

@Injectable()
export class BaseService{

	public param : any;
	private services : Observable<any>[] = [];

	constructor(
		private _userService : UserService,
		private _storeService : StoreService,
		private _warehouseService : WarehouseService,
		private _itemService : ItemService,
		private _approveRequestService : ApproveRequestsService,
		private _receiveItemsService : ReceiveItemsService,
		private _orderStockService : OrderStockService,
		private _deliveriesService : DeliveriesService,
		private _releaseItemsService : ReleaseItemsService,
		private _orderService : OrderService,
		private _releasingService : ReleasingService,
		private _inventoriesService : InventoriesService,
		private _salesOrderService : SalesOrderService,
		private _branchOrderService : BranchOrderService,
		private _requestService : RequestService,
		private _clientReleasingService : ClientReleasingService,
		private _salesOrderReleasingService : SalesOrderReleasingService,
		private _sameDaySalesReleasingService : SameDaySalesReleasingService,
		private _transferReleasingService : TransferReleasingService,
		private _apiBaseService : ApiBaseService,

		) 			
	{
		this.services["user"] = this._userService;
		this.services["user-dealer"] = this._userService;
		this.services["store"] = this._storeService;
		this.services["warehouse"] = this._warehouseService;
		this.services["item"] = this._itemService;
		this.services["approveRequestsOrders"] = this._apiBaseService;
		this.services["approveRequestsReturns"] = this._apiBaseService;
		this.services["approveModifyTonality"] = this._apiBaseService;
		this.services["approveRequestsTransfers"] = this._apiBaseService;
		// this.services["receive-items"] = this._receiveItemsService;
		this.services["receive-items"] = this._apiBaseService;
		this.services["deliveries-orders"] = this._apiBaseService;
		this.services["deliveries-sales"] = this._apiBaseService;
		this.services["order-stock"] = this._apiBaseService ;
		this.services["release-items"] = this._releaseItemsService;
		this.services["order"] = this._orderService;
		this.services["inventories"] = this._inventoriesService;
		this.services["releasing"] = this._releasingService;
		this.services["sales-order"] = this._apiBaseService;
		// this.services["branchOrders"] = this._branchOrderService;
		this.services["branchOrders"] = this._apiBaseService;
		this.services["item-test"] = this._requestService;
		this.services["client-releasing"] = this._clientReleasingService;
		this.services["salesorder-releasing"] = this._apiBaseService;
		this.services["same-day-sales-releasing"] = this._apiBaseService;
		this.services["transfer-releasing"] = this._apiBaseService;
		this.services["returns"] = this._apiBaseService;
		this.services["deliveries-returns"] = this._apiBaseService;
		this.services["returns-releasing"] = this._apiBaseService;
		this.services["receive-return-items"] = this._apiBaseService;// warehouse
		this.services["clientreturn"] = this._apiBaseService;
		this.services["return-receive-items"] = this._apiBaseService; //store
		this.services["return-client-delivery"] = this._apiBaseService; //store
		this.services["store-inventory"] = this._apiBaseService;
		this.services["warehouse-inventory"] = this._apiBaseService;	
		this.services["m-inv-store-stock-history"] = this._apiBaseService;
		this.services["m-inv-warehouse-stock-history"] = this._apiBaseService;		
		this.services["s-stock-history"] = this._apiBaseService;		
		this.services["w-stock-history"] = this._apiBaseService;	
		this.services["stock-history"] = this._apiBaseService;		
		this.services["w-stock-history"] = this._apiBaseService;
		this.services["adjust-inventory"] = this._apiBaseService;
		this.services["w-adjust-inventory"] = this._apiBaseService;
		this.services["incoming-inventory"] = this._apiBaseService;
		this.services["outgoing-inventory"] = this._apiBaseService;
		this.services["assign-dr"] = this._apiBaseService;
		this.services["audit-trail"] = this._apiBaseService;
		this.services["main-store-rtv"] = this._apiBaseService;
		this.services["company"] = this._apiBaseService;
		this.services["items2"] = this._apiBaseService;

		this.services["w-releasing-details"] = this._apiBaseService;
		this.services["s-releasing-details"] = this._apiBaseService;

		this.services["change-tonality"] = this._apiBaseService;
		this.services["approve-advance-order"] = this._apiBaseService;
		this.services["warehouse-advance-order"] = this._apiBaseService;


	}

	getService(serviceName : string): Object 
	{
		if(serviceName == "user")
		{
			this.services[serviceName].action = "";
		}
		else if(serviceName == "user-dealer")
		{
			this.services[serviceName].action = "dealer";
		}
		else if(serviceName == "item-test")
		{
			this.services[serviceName].action = "items";
		}
		else if (serviceName == "returns")
		{
			this.services[serviceName].action = "returns";
		}
		else if (serviceName == "deliveries-returns")
		{
			this.services[serviceName].action = "deliveries/returns";
		}
		else if (serviceName == "approveRequestsOrders")
		{
			this.services[serviceName].action = "transactions/approverequests";
		}
		else if (serviceName == "approveRequestsReturns")
		{
			this.services[serviceName].action = "transactions/approverequests/returns";
		}
		else if (serviceName == "approveRequestsTransfers")
		{
			this.services[serviceName].action = "transactions/approverequests/transfers";
		}
		else if (serviceName == "returns-releasing")
		{
			this.services[serviceName].action = "transactions/releasing/returns";
		}
		else if (serviceName == "receive-return-items")
		{
			this.services[serviceName].action = "transactions/receiveitems/returns";
		}
		else if(serviceName == "clientreturn")
		{
			this.services[serviceName].action = "returns/clientreturn";
		}
		else if(serviceName == "deliveries-orders")
		{
			this.services[serviceName].action = "deliveries";
		}
		else if(serviceName == "deliveries-sales")
		{
			this.services[serviceName].action = "deliveries/sales";
		}
		else if(serviceName == "return-receive-items")
		{
			this.services[serviceName].action = "transactions/receiveitems/clientreturn";
		}
		else if(serviceName == "return-client-delivery")
		{
			this.services[serviceName].action = "deliveries/storereturns";
		}
		else if(serviceName == "store-inventory")
		{
			this.services[serviceName].action = "inventories/main/stores";
		}
		else if(serviceName == "warehouse-inventory")
		{
			this.services[serviceName].action = "inventories/main/warehouses";
		}
		else if(serviceName == "m-inv-store-stock-history")
		{
			this.services[serviceName].action = "inventories/main/s_stockhistory";
		}
		else if(serviceName == "m-inv-warehouse-stock-history")
		{
			this.services[serviceName].action = "inventories/main/w_stockhistory";
		}
		else if(serviceName == "s-stock-history")
		{
			this.services[serviceName].action = "inventories/store/stockhistory/" + this.param;
		}
		else if(serviceName == "w-stock-history")
		{
			this.services[serviceName].action = "inventories/warehouse/stockhistory/" + this.param;
		}
		else if(serviceName == "adjust-inventory")
		{
			this.services[serviceName].action = "imports/physicalcount/store";
		}
		else if(serviceName == "w-adjust-inventory")
		{
			this.services[serviceName].action = "imports/physicalcount/warehouse";
		}
		else if(serviceName == "incoming-inventory")
		{
			this.services[serviceName].action = "inventories/main/inventory-monitoring/incoming";
		}		
		else if(serviceName == "outgoing-inventory")
		{
			this.services[serviceName].action = "inventories/main/inventory-monitoring/outgoing";
		}
		else if(serviceName == "assign-dr")
		{
			this.services[serviceName].action = "assign/main/whdrnumber";
		}
		else if(serviceName == "audit-trail")
		{
			this.services[serviceName].action = "Users/usertrail";
		}		
		else if(serviceName == "main-store-rtv")
		{
			this.services[serviceName].action = "returns/rtv/main";
		}
		else if(serviceName == "company")
		{
			this.services[serviceName].action = "Companies";
		}
		else if(serviceName == "items2")
		{
			this.services[serviceName].action = "items/items2";
		}

		else if(serviceName == "w-releasing-details")
		{
			this.services[serviceName].action = "inventories/release/details/" + this.param;;
		}		

		else if(serviceName == "s-releasing-details")
		{
			this.services[serviceName].action = "inventories/STrelease/details/" + this.param;;
		}		

		else if(serviceName == "change-tonality")
		{
			this.services[serviceName].action = "transactions/releaseitems/changetonality";
		}
		else if(serviceName == "approveModifyTonality")
		{
			this.services[serviceName].action =  "transactions/approverequests/modifytonality";
		}
		else if(serviceName == "branchOrders")
		{
			this.services[serviceName].action ="branchorders";
		}
		else if(serviceName == "salesorder-releasing")
		{
			this.services[serviceName].action ="transactions/releasing/salesorder";
		}
		else if(serviceName == "same-day-sales-releasing")
		{
			this.services[serviceName].action ="transactions/releasing/samedaysales";
		}
		else if(serviceName == "sales-order")
		{
			this.services[serviceName].action ="transactions/salesorder";
		}
		else if(serviceName == "order-stock")
		{
			this.services[serviceName].action ="transactions/orders";
		}
		else if(serviceName == "transfer-releasing")
		{
			this.services[serviceName].action = 'transactions/releasing/transfer';
		}
		else if(serviceName == "approve-advance-order")
		{
			this.services[serviceName].action = 'transactions/approverequests/advanceorders';
		}
		else if(serviceName == "warehouse-advance-order")
		{
			this.services[serviceName].action = 'transactions/orders/warehouse/advanceorder';
		}
		else if(serviceName == "receive-items")
		{
			this.services["receive-items"].action = 'transactions/receiveitems';
		}	

	


		
		
		
		return this.services[serviceName];
	}

	// implemented with api base service
	// - returns
	// - items
	// - approve request [ orders and returns ]


}