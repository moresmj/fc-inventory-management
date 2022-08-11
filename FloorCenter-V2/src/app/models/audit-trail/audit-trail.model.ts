import { Observable } from 'rxjs/Observable';

export class AuditTrail {
	constructor(init?:Partial<AuditTrail>)
	{
		Object.assign(this,init);
	}

	public id : number;
	public date : string;

	public userName : number;
	public action : string;
	public transaction : number;
	public detail : string;

}