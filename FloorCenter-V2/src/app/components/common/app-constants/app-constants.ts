export class AppConstants{
    public static get skuLength(): number { return 9; }
    public static get skuErrorMessage(): string { return "Serial number must be atleast "+ this.skuLength +" Digits"}
    public static get selectWarehouse(): string { return "Please select warehouse in Ordered To"}
    public static get clientDeliverySuccessMessage(): string { return "Client Delivery Successfully Added."; }
    public static get recUpdateSuccessMessage(): string { return "Record Successfully Updated."; }
    public static get recordSaveSuccessMessage(): string { return "Records Successfully Saved."; }
    public static get requestCancelledMessage(): string { return "Request has been cancelled"; }
    public static get deliverySuccessMessage(): string { return "Delivery Record Successfully Added." ;}
    public static get itemListError(): string { return  "Please add at least one item."; }
    public static get serialDoNotExist(): string { return "Serial Number does not exist."; }
    public static get selectItemCodeErr(): string {return "Please select item code"; }
    public static get selectItemsizeErr(): string{ return "Please select item size"; }
    public static get maxItemOnListErr(): string { return "Only 7 items are allowed per purchase"; }
    public static get itemAlreadyOnListErr(): string { return "The selected item is already on the list"; }
    public static get availQtyNotSufficientErr(): string { return "The available quantity does not meet the requested quantity"; }
    public static get qtyMustGreaterThanZeroErr(): string { return "Quantity must be greater than 0"; }
    public static get ChangeTonalitySucess(): string { return "Tonality has successfully been changed.";}
    public static get AdvanceOrderUpdateSuccessMessage() : string { return "Advance Order successfully updated"; }
    public static get AdvanceOrderCancelMessage() : string { return "Advance order has been cancelled"; }
    public static get uploadMaxLength(): number { return 1000;}
    public static get uploadLengthErrorMessage() : string { return "Only 1000 items are allowed per upload"}

}