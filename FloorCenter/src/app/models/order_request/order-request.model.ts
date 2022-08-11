import { Observable} from 'rxjs/Observable';

export class OrderRequest{
	constructor	(init?:Partial<OrderRequest>)
	{
		Object.assign(this,init);
	}


	public id : number;
	public storeId : number;
	public transactionType : number;
	public transactionTypeStr : string;
	public warehouseId : number;
	public poNumber : string;
	public poDate : Date;
	public remarks : string;
	public requestStatus : number;
	public requestStatusStr : string;
	public dateCreated : Date;
	public DateUpdated : Date;
	public requestedItems : any;




}