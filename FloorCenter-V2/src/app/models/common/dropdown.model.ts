import { Observable } from 'rxjs/Observable'

export class Dropdown {

	constructor(init?:Partial<Dropdown>)
	{
		Object.assign(this,init);
	}
	public id : Number;
	public name : String;
	
}