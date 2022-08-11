import { Observable }  from 'rxjs/Observable';

export class Product{
	constructor(init?: Partial<Product>){
		Object.assign(this,init);
	}


	public id: number;
	public code: string;
	public name : string;
	public remarks: string;
	public serialNumber: number;
	public sizeId: number;
	public tonality: string;
	public description: string;
	public image: any;
	public dateadded: string;
	public size: Size;

}

class Size{
	public id: number;
	public name: string;
}