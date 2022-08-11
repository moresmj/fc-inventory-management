using AutoMapper;
using FC.Api.DTOs.Company;
using FC.Api.DTOs.Item;
using FC.Api.DTOs.Size;
using FC.Api.DTOs.Store;
using FC.Api.DTOs.Store.AdvanceOrder;
using FC.Api.DTOs.Store.BranchOrders;
using FC.Api.DTOs.Store.Releasing;
using FC.Api.DTOs.Store.Returns;
using FC.Api.DTOs.Store.Returns.ClientReturn;
using FC.Api.DTOs.User;
using FC.Api.DTOs.Warehouse;
using FC.Api.DTOs.Warehouse.AllocateAdvanceOrder;
using FC.Core.Domain.Companies;
using FC.Core.Domain.Items;
using FC.Core.Domain.Sizes;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Users;
using FC.Core.Domain.Warehouses;

namespace FC.Api.Helpers
{
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();

            CreateMap<User, UserDealerDTO>();
            CreateMap<UserDealerDTO, User>();

            CreateMap<UserDealer, UserDealerDTO>();
            CreateMap<UserDealerDTO, UserDealer>();

            CreateMap<Warehouse, WarehouseDTO>();
            CreateMap<WarehouseDTO, Warehouse>();

            CreateMap<Company, CompanyDTO>();
            CreateMap<CompanyDTO, Company>();

            CreateMap<Store, StoreDTO>();
            CreateMap<StoreDTO, Store>();

            CreateMap<Item, ItemDTO>();
            CreateMap<ItemDTO, Item>();

            CreateMap<ItemAttribute, ItemAttributeDTO>();
            CreateMap<ItemAttributeDTO, ItemAttribute>();

            CreateMap<Size, SizeDTO>();
            CreateMap<SizeDTO, Size>();
            
            CreateMap<CategoryParent, CategoryParentDTO>();
            CreateMap<CategoryParentDTO, CategoryParent>();

            CreateMap<CategoryChild, CategoryChildDTO>();
            CreateMap<CategoryChildDTO, CategoryChild>();

            CreateMap<CategoryGrandChild, CategoryGrandChildDTO>();
            CreateMap<CategoryGrandChildDTO, CategoryGrandChild>();

            CreateMap<WHReceive, WHReceiveDTO>();
            CreateMap<WHReceiveDTO, WHReceive>();

            CreateMap<WHReceiveDetail, WHReceiveDetailDTO>();
            CreateMap<WHReceiveDetailDTO, WHReceiveDetail>();

            CreateMap<WHStock, WHStockDTO>();
            CreateMap<WHStockDTO, WHStock>();

            CreateMap<STOrder, STOrderDTO>();
            CreateMap<STOrderDTO, STOrder>();
            CreateMap<STOrder, UpdateRequestDTO>();
            CreateMap<UpdateRequestDTO, STOrder>();
            CreateMap<STOrder, ForClientOrderDTO>();
            CreateMap<ForClientOrderDTO, STOrder>();

            CreateMap<STOrderDetail, STOrderDetailDTO>();
            CreateMap<STOrderDetailDTO, STOrderDetail>();

            CreateMap<STDelivery, STDeliveryDTO>();
            CreateMap<STDeliveryDTO, STDelivery>();
            CreateMap<STShowroomDelivery, STShowroomDeliveryDTO>();
            CreateMap<STShowroomDeliveryDTO, STShowroomDelivery>();
            CreateMap<STDelivery, UpdateForDeliveryDTO>();
            CreateMap<UpdateForDeliveryDTO, STDelivery>();
            CreateMap<STClientDelivery, AddClientDeliveryItems>();
            CreateMap<AddClientDeliveryItems, STClientDelivery>();
            CreateMap<STDelivery, AddClientDeliveryDTO>();
            CreateMap<AddClientDeliveryDTO, STDelivery>();

            CreateMap<STSales, STSalesDTO>();
            CreateMap<STSalesDTO, STSales>();

            CreateMap<STDelivery, AddShowroomDeliveryDTO>();
            CreateMap<AddShowroomDeliveryDTO, STDelivery>();
            CreateMap<STShowroomDelivery, AddShowroomDeliveryItems>();
            CreateMap<AddShowroomDeliveryItems, STShowroomDelivery>();

            CreateMap<STSales, AddSalesDTO>();
            CreateMap<AddSalesDTO, STSales>();
            CreateMap<STSalesDetail, AddSalesItems>();
            CreateMap<AddSalesItems, STSalesDetail>();

            CreateMap<STSales, AddSalesOrderDTO>();
            CreateMap<AddSalesOrderDTO, STSales>();
            CreateMap<STSalesDetail, AddSalesOrderItems>();
            CreateMap<AddSalesOrderItems, STSalesDetail>();

            CreateMap<STDelivery, AddSalesOrderDeliveryDTO>();
            CreateMap<AddSalesOrderDeliveryDTO, STDelivery>();
            CreateMap<STClientDelivery, AddSalesOrderDeliveryItems>();
            CreateMap<AddSalesOrderDeliveryItems, STClientDelivery>();

            CreateMap<STTransfer, AddTransferOrderDTO>();
            CreateMap<AddTransferOrderDTO, STTransfer>();
            CreateMap<STTransferDetail, AddTransferOrderItems>();
            CreateMap<AddTransferOrderItems, STTransferDetail>();

            CreateMap<STTransfer, AddTransferOrderStoreDTO>();
            CreateMap<AddTransferOrderStoreDTO, STTransfer>();
            CreateMap<STTransferDetail, AddTransferOrderStoreItemsDTO>();
            CreateMap<AddTransferOrderStoreItemsDTO, STTransferDetail>();

            CreateMap<WHAllocateAdvanceOrder, AllocateAdvanceOrderDTO>();
            CreateMap<AllocateAdvanceOrderDTO, WHAllocateAdvanceOrder>();

            CreateMap<WHAllocateAdvanceOrderDetail, AllocateAdvanceOrderDetailsDTO>();
            CreateMap<AllocateAdvanceOrderDetailsDTO, WHAllocateAdvanceOrderDetail>();


            #region Advance Order

            CreateMap<STAdvanceOrder, STAdvanceOrderDTO>();
            CreateMap<STAdvanceOrderDTO, STAdvanceOrder>();

            CreateMap<STAdvanceOrderDetails, STAdvanceOrderDetailsDTO>();
            CreateMap<STAdvanceOrderDetailsDTO, STAdvanceOrderDetails>();

            #endregion


            #region Releasing

            #region For Client Order
            CreateMap<STSales, ReleaseForClientOrderDTO>();
            CreateMap<ReleaseForClientOrderDTO, STSales>();
            #endregion

            #region Same Day Sales
            CreateMap<STSales, UpdateSameDaySalesDTO>();
            CreateMap<UpdateSameDaySalesDTO, STSales>();
            #endregion

            #endregion

            #region Returns

            #region RTV/Purchase Return
            CreateMap<STReturn, AddPurchaseReturnDTO>();
            CreateMap<AddPurchaseReturnDTO, STReturn>();
            CreateMap<STPurchaseReturn, AddPurchaseReturnItems>();
            CreateMap<AddPurchaseReturnItems, STPurchaseReturn>();

            CreateMap<STReturn, AddBreakageDTO>();
            CreateMap<AddBreakageDTO, STReturn>();
            CreateMap<AddBreakageItems, STPurchaseReturn>();
            CreateMap<STPurchaseReturn, AddBreakageItems>();

            CreateMap<WHDelivery, AddPurchaseReturnDeliveryDTO>();
            CreateMap<AddPurchaseReturnDeliveryDTO, WHDelivery>();
            CreateMap<WHDeliveryDetail, AddPurchaseReturnDeliveryItems>();
            CreateMap<AddPurchaseReturnDeliveryItems, WHDeliveryDetail>();

            #endregion

            #endregion

            #region Physical Count

            CreateMap<WHImport, DTOs.Warehouse.PhysicalCount.UploadWarehousePhysicalCountDTO>();
            CreateMap<DTOs.Warehouse.PhysicalCount.UploadWarehousePhysicalCountDTO, WHImport>();
            CreateMap<WHImportDetail, DTOs.Warehouse.PhysicalCount.UploadWarehousePhysicalCountItems>();
            CreateMap<DTOs.Warehouse.PhysicalCount.UploadWarehousePhysicalCountItems, WHImportDetail>();

            CreateMap<WHImport, DTOs.Warehouse.PhysicalCount.UploadWarehouseBreakageDTO>();
            CreateMap<DTOs.Warehouse.PhysicalCount.UploadWarehouseBreakageDTO, WHImport>();
            CreateMap<WHImportDetail, DTOs.Warehouse.PhysicalCount.UploadWarehouseBreakageItems>();
            CreateMap<DTOs.Warehouse.PhysicalCount.UploadWarehouseBreakageItems, WHImportDetail>();

            CreateMap<STImport, DTOs.Store.PhysicalCount.UploadStorePhysicalCountDTO>();
            CreateMap<DTOs.Store.PhysicalCount.UploadStorePhysicalCountDTO, STImport>();
            CreateMap<STImportDetail, DTOs.Store.PhysicalCount.UploadStorePhysicalCountItems>();
            CreateMap<DTOs.Store.PhysicalCount.UploadStorePhysicalCountItems, STImportDetail>();

            CreateMap<STImport, DTOs.Store.PhysicalCount.RegisterStoreBreakageDTO>();
            CreateMap<DTOs.Store.PhysicalCount.RegisterStoreBreakageDTO, STImport>();
            CreateMap<STImportDetail, DTOs.Store.PhysicalCount.RegisterStoreBreakageItems>();
            CreateMap<DTOs.Store.PhysicalCount.RegisterStoreBreakageItems, STImportDetail>();

            #endregion


        }

    }
}
