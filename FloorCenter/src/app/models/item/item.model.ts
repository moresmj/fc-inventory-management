import { Observable } from 'rxjs/Observable';

export class Item {
	constructor(init?:Partial<Item>)
	{
		Object.assign(this,init);
	}

	public Id : number;
	public Name : string;
	public Code : string;
	public SerialNumber : number;
	public SerialNumberStr : string;
	public SizeId : number;
	public SizeStr : string;
	public Tonality : string;
	public Description : string;
	public Remarks : string;
	public DateCreated : Date;
	public size : Size;

}

export class Size {
	private Name : String;
}