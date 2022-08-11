import { Observable } from 'rxjs/Observable';

export class SalesOrder {

	constructor(init?:Partial<SalesOrder>)
	{
		Object.assign(this,init);
	}
	public Id : Number;
	public StoreId : String;
	public TransactionNo : String;
	public SINumber : String;
	public ORNumber : String;
	public DRNumber : String;
	public DeliveryType : String;
	public DeliveryTypeStr : String;
	public SalesDate : String;
	public SalesAgent : string;
	public ClientName : string;
	public ContactNumber : String;
	public Address1 : String;
	public Address2 : String;
	public Address3 : String;
	public Transaction : String;
	public OrderStatusStr : String;

	public SalesTypeStr : string;
	public OrderedBy : string;
	public RequestedDeliveryDate : string;
	public ApprovedDeliveryDate : string;
	public DeliveryQty : number;

	public DeliverFrom : string;
	public DeliverTo : string;
	public DeliveryStatusStr : string;


}
