import { Pipe, PipeTransform} from '@angular/core';
import { Product } from '@models/main/product.model';


@Pipe({
	name:'productListFilter',
	pure: false
})



export class ProductListFilter implements PipeTransform {
	transform(items : any[] , filter : string) : any{
		if(!items || ! filter){
			return items;
		}

		return items.filter(item => this.filterList(item,filter));
	}


	filterList(record: any, searchKey: string) : boolean{
		for(let key in record)
		{
			if(record[key] != null){
				if(record[key].toString().toLowerCase().indexOf(searchKey.toLowerCase()) != -1)
				{
					return true;
				}
			}
		}
		return false;
	}
}