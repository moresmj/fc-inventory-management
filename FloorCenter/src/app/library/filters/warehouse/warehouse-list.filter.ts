import {Pipe, PipeTransform } from '@angular/core';
import { Warehouse } from '@models/warehouse/warehouse.model';


@Pipe({
	name: 'warehouseFilter',
	pure: false
})

export class WarehouseListFilter implements PipeTransform{

	transform(items: any[], filter: string): any{
		if(!items || !filter){
			return items;

		}

		return items.filter(item=> this.likeylikey(item,filter));
	}


	likeylikey(record : any, searchKey : string) : boolean
	{
		for(let key in record)
		{
			if(record[key])
			{
				if(record[key].toString().toLowerCase().indexOf(searchKey.toLowerCase()) != -1)
					{
						return true;
					}
			}
			
		}
		return false;
	} 


}