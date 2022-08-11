import { Observable } from 'rxjs/Observable';

export class Store {

	constructor(init?:Partial<Store>)
	{
		Object.assign(this,init);
	}
	public Id : Number;
	public CompanyId : Number;
	public WarehouseId : Number;
	public Code : String;
	public Name : String;
	public Address : String;
	public ContactNumber : String;
	public DateCreated : string;
	public DateUpdated : string;

	public company : Company;
	public warehouse : Warehouse;

	public CompanyName : String;
	public WarehouseName : String;
	public Type : any;

}

export class Company {
	private Name : String;
}

export class Warehouse {
	private Name : String;
}