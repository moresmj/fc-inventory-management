import { Pipe, PipeTransform } from '@angular/core';  
import { User } from '@models/user/user.model';

@Pipe({
	name: 'userListFilter',
	pure: false
})

export class UserListFilter implements PipeTransform  {
		transform( items : any[], filter : string ) : any {
			if(!items || ! filter) {
				return items; 
			}
			return items.filter(item => this.filterList(item,filter));
		}


		filterList(record : any, searchKey : string) : boolean {

			for(let key in record)
		  	{
		  		if (record[key] != null) {
		  			if(record[key].toString().toLowerCase().indexOf(searchKey.toLowerCase()) != -1)
			   		{
			    		return true;
			   		}
		  		}
		  	}
		  	return false;
		}


}