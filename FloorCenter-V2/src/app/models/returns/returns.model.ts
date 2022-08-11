import { Observable } from 'rxjs/Observable';

export class Returns {

	constructor(init?:Partial<Returns>)
	{
		Object.assign(this,init);
	}

	public id: number;
	public transactionNo : string;	
	public returnFormNumber	: string;
	public requestDate	: string;							
	public requestStatus : number;							
	public requestStatusStr : string;													
	public returnType : number;	
	public returnTypeStr : string;	
	public returnedBy : string;
	public returnedTo : string;		
	public orNumber : string;
	public orderStatusStr : string;	
	public approveDate : any;	
	public clientName : string;
	public orderedBy: any;
	public firstAddress: string;
	public secondAddress: string;
	public thirdAddress: string;
	public ContactNo: string;
	public aoDate: any;
	public deliveryStatus: string;
	public poNumber: string;
	public remarks: string;
	public salesAgent: string;
	public orderStatus: string;
	public paymentMode: string;
	public paymentModeStr: string;
	// public code: string;
	// public size: string;
	// public tonality: string;
	// public approvedQuantity: string;
	// public allocatedQuantiy: string;


}