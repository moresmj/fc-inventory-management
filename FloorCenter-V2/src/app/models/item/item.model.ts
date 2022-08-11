import { Observable } from 'rxjs/Observable';

export class Item {
	constructor(init?:Partial<Item>)
	{
		Object.assign(this,init);
	}

	public Id : number;
	public Name : string;
	public SRP : number;
	public Cost : number;
	public Code : string;
	public SerialNumber : number;
	public SerialNumberStr : string;
	public SizeId : number;
	public SizeName : string;
	public Tonality : string;
	public QtyPerBox : number;
	public QtyPerPallet : number;
	public Description : string;
	public Remarks : string;
	public DateCreated : string;

	public size : Size;

}



export class Size {
	public Name : string;
}