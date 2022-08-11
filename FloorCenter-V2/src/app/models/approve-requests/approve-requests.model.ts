import { Observable } from 'rxjs/Observable';

export class ApproveRequests {
	constructor(init?:Partial<ApproveRequests>)
	{
		Object.assign(this,init);
	}

	public Id : number;
	public TransactionNo : string;
	public TransactionType : number;
	public TransactionTypeStr : string;

	public OrderType : number;
	public OrderTypeStr : string;
	public ReturnType : number;
	public ReturnTypeStr : string;

	public RequestStatus : number;
	public RequestStatusStr : string;
	public StoreId : number;
	public StoreName : string;
	public WarehouseId : number;
	public WarehouseName : string;
	public PONumber : string;
	public PODate : string;

	public DeliveryOption : number;
	public DeliveryOptionStr : string;

	public Remarks : string;

	public payMentModeStr : string;
}


export class Store {
	public Name : string;
}

export class Warehouse {
	public Name : string;
}

export class Item {
	public SerialNumber : number;
	public Code : string;
	public Name : string;
	public SizeId : number;
	public Tonality : string;
}


export class Size {
	public Name : string;
}
