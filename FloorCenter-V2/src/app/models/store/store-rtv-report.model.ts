import { Observable } from 'rxjs/Observable';

export class StoreRTVReport {

	constructor(init?:Partial<StoreRTVReport>)
	{
		Object.assign(this,init);
	}
	public returnFormNumber : String;
	public drNumber : Number;
	public code : String;
	public sizeName : String;
	public receivedGoodQuantity : String;
	public receivedBrokenQuantity : String;

}
