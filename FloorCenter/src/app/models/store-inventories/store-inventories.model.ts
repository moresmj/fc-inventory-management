import { Observable } from 'rxjs/Observable';
import { StoreInventoriesDetail } from '@models/store-inventories-detail/store-inventories-detail.model';
import { Store } from '@models/store/store.model';
import { Warehouse } from '@models/warehouse/warehouse.model';

export class StoreInventories {

	constructor(init?:Partial<StoreInventories>)
	{
		Object.assign(this,init);
	}

	public Id : number;
	public Store : number;
	public TransactionType : number;

	public TransactionTypeStr : string;

	public WarehouseId : number;
	public PONumber : string;
	public PODate : string;
	public Remarks : string;
	public RequestStatus : number;

	public RequestStatusStr : string

	public RequestedItems : StoreInventoriesDetail[];

	public DRNumber : string;
	public DRDate : string;
	public StoreName : string;
	public WarehouseName : string;

	public store : Store;
	public warehouse : Warehouse;

}
