import { Observable } from 'rxjs/Observable';

export class Releasing {

	constructor(init?:Partial<Releasing>)
	{
		Object.assign(this,init);
	}

	public id: number;
	public PONumber	: string;							
	public SINumber	: string;
	public ClientSINumber	: string;							
	public ORNumber	: string;							
	public DRNumber	:string;							
	public ClientName :string;	
	public transactionNo : string;	
	public transaction : string;
	public poDate : string;	
	public ReturnFormNumber : string;
	public whDrNumber : string;			

						

}