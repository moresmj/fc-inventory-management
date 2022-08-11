import { Observable } from 'rxjs/Observable';

export class ReceiveItems {

	constructor(init?:Partial<ReceiveItems>)
	{
		Object.assign(this,init);
	}

	public id: number;
	public transactionNo: string;
	public warehouseId: string;
	public poNumber: string;
	public poDate: Date;
	public drNumber: number;
	public drDate: Date;
	public receivedDate: any;
	public remarks: string;
	public checkedBy: string;
	public dateCreated: Date;
	public dateUpdated: Date;
	public receivedItems: any;
	public Transaction : string;
	public returnFormNumber	: string;
	public requestedDate : string;
	public returnedBy : string;
	public deliveryDate : string;
						

}