import { Observable } from 'rxjs/Observable';

export class Warehouse {

	constructor(init?:Partial<Warehouse>)
	{
		Object.assign(this,init);
	}

	public id: number;
	public code: string;
	public name: string;
	public contactNumber: string;
	public address: string;
	public DateCreated: string;
}