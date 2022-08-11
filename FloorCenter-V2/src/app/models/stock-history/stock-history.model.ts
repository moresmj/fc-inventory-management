import { Observable } from 'rxjs/Observable';

export class StockHistory {

	constructor(init?:Partial<StockHistory>)
	{
		Object.assign(this,init);
	}
	public id : Number;
  	public transactionNo: String;
  	public transaction: String;
	public poNumber : String;
	public drNumber : String;
	public siNumber : String;
	public type : String;
	public origin : String;
	public destination : String;
	public date : String;
	public transactionDate : string;
	public stock : String;
	public condition : string;
	public releaseDate : String;
	public remarks : String;

}
