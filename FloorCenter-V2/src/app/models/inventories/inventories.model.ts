import { Observable } from 'rxjs/Observable';

export class Inventories {
	constructor(init?:Partial<Inventories>)
	{
		Object.assign(this,init);
	}

	public ItemId : number;
	public SerialNumber : number;
	public Code : string;
	public ItemName : number;
	public SizeName : string;
	public Tonality : string;
	public SRP : number;
	public Description : string;
	public Remarks : string;
	public CategoryParent : number;
	public CategoryChild : number;
	public CategoryGrandChild : number;
	public OnHand : any;
	public ForRelease : number;
	public Available : number;
	public ForReturn : number;
	public Broken : number;

	public WarehouseName : string;
	public WarehouseAddress : string;
	public StoreName : string;
	public StoreAddress : string;
	public CompanyName : string;

	public transactionDate : string;
	public drNumber : string;
	public poNumber : string;
	public adjustment : number;
	public fromSupplier : number;
	public fromInterBranch : number;
	public fromInterCompany : number;
	public FromSalesReturns : number;
	public rtv : number;
	public fromOtherSupplier : number;

	public origin : string;

}

