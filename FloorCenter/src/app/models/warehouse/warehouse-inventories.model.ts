import { Observable } from 'rxjs/Observable';

export class WarehouseInventories {

	constructor(init?:Partial<WarehouseInventories>)
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
	public receivedDate: Date;
	public remarks: string;
	public checkedBy: string;
	public dateCreated: Date;
	public dateUpdated: Date;
						

}