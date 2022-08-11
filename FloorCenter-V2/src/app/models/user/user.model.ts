import { Observable } from 'rxjs/Observable';

export class User {

	constructor(init?:Partial<User>)
	{
		Object.assign(this,init);
	}

	public Id : number;
	public FullName : string;
	public UserType : number;
	public UserTypeStr : string;
	public UserName : string;
	public Password : string
	public Password_confirm : string;
	public EmailAddress : string;
	public Address : string;
	public ContactNumber : string;
	public Assignment : number;
	public AssignmentStr : string;
	public StoreId : number;
	public WarehouseId : number;
	public DateCreated : string;
	public DateUpdated : string;

	public LastLogin : string;
}