import { Observable } from 'rxjs/Observable';

export class OrderStock {

	constructor(init?:Partial<OrderStock>)
	{
		Object.assign(this,init);
	}

	public id: number;
	public storeId: number;
	public transaction: number;
	public transactionType: number;
	public transactionTypeStr: string;
	public orderType: number;
	public orderTypeStr: string;	
	public transactionNo: string;
	public warehouseId: number;
 	public poNumber: string;
 	public poDate: string;
 	public remarks:  string;
 	public requestStatus: number;
 	public requestStatusStr: string;
 	public orderedItems: any
 	public store: any;
 	public warehouse: any;
 	public deliveries:any;
 	public purpose: string;
	public deliveryType: number;
	public wHDRNumber: string;
						
 	public orderedBy: string;
 	public orderedTo: string;
 	public orderStatusStr : string;

}
