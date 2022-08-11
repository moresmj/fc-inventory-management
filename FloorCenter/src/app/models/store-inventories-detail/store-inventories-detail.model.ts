import { Observable } from 'rxjs/Observable';
import { Item } from '@models/item/item.model';

export class StoreInventoriesDetail {

	constructor(init?:Partial<StoreInventoriesDetail>)
	{
		Object.assign(this,init);
	}

	public Id : number;
	public STInventoryId : number;
	public ItemId : number;
	public RequestedQuantity : number;
	public RequestedRemarks : string;
	public ApprovedQuantity : number;
	public ApprovedRemarks : string;
	public DeliveryStatus : number;

	public DeliveryStatusStr : string
	public item : Item;

}
