import { Observable } from 'rxjs/Observable';

export class Stock {

	constructor(init?:Partial<Stock>)
	{
		Object.assign(this,init);
	}


public  id: number;
public storeId : number;
public stInventory	: String;
public deliveryDate : Date;
public dateCreated : Date;
public dateUpdated : Date;



}