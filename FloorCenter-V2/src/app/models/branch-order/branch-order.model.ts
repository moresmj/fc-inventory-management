import { Observable } from 'rxjs/Observable';

export class BranchOrder {
	constructor(init?:Partial<BranchOrder>)
	{
		Object.assign(this,init);
	}

	public id : number;
	public transactionNo : string;
	public poNumber : string;
	public orderedBy : string;
	public orderedTo : string;
	public companyRelation : string;
	public clientName : string;
	public deliveryTypeStr : string;
	public poDate : string;
	public requestStatusStr : string;
	public deliveryStatusStr : string;

	public transactionType : number;
	public transactionTypeStr : string;
	public requestStatus : number;
	public deliveryType : number;

	public paymentModeStr : string;


}