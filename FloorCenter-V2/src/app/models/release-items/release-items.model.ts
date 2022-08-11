import { Observable } from 'rxjs/Observable';

export class ReleaseItems {

	constructor(init?:Partial<ReleaseItems>)
	{
		Object.assign(this,init);
	}

	public id: number;
	public StoreId:number ;
    public STOrderId:number;
    public DeliveryType : string;
    public DRNumber:string;
    public DeliveryDate:string;
    public ApprovedDeliveryDate : string;
    public DriverName:string;
    public PlateNumber:string;
    public ShowroomDeliveries : any;
    public transaction_no : any;
    public transaction : any;
    public orderType : any;
    public purpose : any;
    public po_num : any;
    public ordered_by : any;
    public ordered_to : any;
    public WHDRNumber: string;

						

}