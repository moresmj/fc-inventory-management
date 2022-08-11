import { Observable } from 'rxjs/Observable';

export class StoreOrder {
	constructor(init?:Partial<StoreOrder>)
	{
		Object.assign(this,init);
	}

	public Id : number;
	public StoreId : number;
	public StoreName : string;
	public WarehouseId : number;
	public WarehouseName : string;

	public OrderType : number;
	public OrderTypeStr : string;
	public TransactionNo : string
	public TransactionType : number;
	public TransactionTypeStr : string;
	public PONumber : string;
	public PODate : string;
	public WHDRNumber : string;

	public DRNumber : string;

	public RequestStatus : number;
	public RequestStatusStr : string;

	public store : Store;
	public warehouse : Warehouse;

	public OrderedDate : string;

	// unknown

	public DeliverFrom : string;
	public DeliverTo : string;
	public DeliveryStatus : number;
	public DeliveryStatusStr : number;
	public DeliveryTypeStr : string;
    public DeliveryDate : string;
    public DeliveryQuantity : string;

    public ApprovedDeliveryDate : string;
    public ReturnFormNumber : string

}


export class Store {
	public Name : string;
}

export class Warehouse {
	public Name : string;
}


