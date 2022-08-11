using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using System;
using System.Linq;

namespace FC.Api.Validators.Store
{
    public class SaveReceiveItemDeliveriesValidator : AbstractValidator<SaveReceiveItemDeliveries>
    {

        private readonly DataContext _context;

        public SaveReceiveItemDeliveriesValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate Id & ItemId
            RuleFor(p => p)
                .NotEmpty()
                .Must(ValidRecord)
                    .WithMessage("Record is not valid")
                    .WithName("Id and ItemId");


            // Validate STOrderId & STOrderDeatailId
            RuleFor(p => p)
                .Must(STOrderIdAndSTOrderDeatailIdValid)
                    .WithMessage("Record is not valid")
                    .WithName("STOrderId and STOrderDeatailId");


            //  Validate ReceivedQuantity
            RuleFor(p => p.DeliveredQuantity)
                    .NotEmpty()
                    .GreaterThanOrEqualTo(0)
                    .WithName("Received Quantity");


            RuleFor(p => p)
                    .Must(DeliveredQuantityNotMoreThanQuantity)
                        .WithMessage("Received Quantity cannot be more than delivered quantity")
                        .WithName("ReceivedQuantity");


            RuleFor(p => p)
                    .Must(CannotReceiveMoreThanApprovedQty)
                        .WithMessage("Received Quantity cannot be more than delivered quantity")
                        .WithName("ReceivedQuantity");

        }


        /// <summary>
        /// This is used to check if STShowroomDelivery.Id and STShowroomDelivery.ItemId exist in WHStocks
        /// </summary>
        /// <param name="model">SaveReceiveItemDeliveries</param>
        /// <returns>True if record is valid, otherwise False</returns>
        private bool ValidRecord(SaveReceiveItemDeliveries model)
        {
            int count = 0;

            var order = _context.STOrders
                                .Where(p => p.Id == model.STOrderId)
                                .FirstOrDefault();
            if (order == null)
            {
                return false;
            }

            //if other vendor

            if(order.TransactionType != TransactionTypeEnum.Transfer)
            {
                if (_context.Warehouses.Where(p => p.Id == order.WarehouseId).SingleOrDefault().Vendor)
                {
                    return true;
                }
            }
            

            if (order.OrderType != OrderTypeEnum.InterbranchOrIntercompanyOrder)
            {
                count = _context.WHStocks.Where(p => p.STShowroomDeliveryId == model.Id
                                                     && p.ItemId == model.ItemId
                                                     && p.DeliveryStatus == DeliveryStatusEnum.Waiting
                                                     && p.ReleaseStatus == ReleaseStatusEnum.Released).Count();
            }
            else
            {
                count = _context.STStocks.Where(p => p.STShowroomDeliveryId == model.Id
                                                     && p.ItemId == model.ItemId
                                                     && p.DeliveryStatus == DeliveryStatusEnum.Waiting
                                                     && p.ReleaseStatus == ReleaseStatusEnum.Released).Count();
            }

            if (count != 1)
            {
                if(model.IsRemainingForReceiving)
                {
                    return true;
                }
                return false;
            }

            return true;
        }


        /// <summary>
        /// This is used to check if STOrderDetailId and STOrderId exist in STOrderDetails
        /// </summary>
        /// <param name="model">SaveReceiveItemDeliveries</param>
        /// <returns>True if record is valid, otherwise False</returns>
        private bool STOrderIdAndSTOrderDeatailIdValid(SaveReceiveItemDeliveries model)
        {

            var count = _context.STOrderDetails.Where(p => p.Id == model.STOrderDetailId && p.STOrderId == model.STOrderId).Count();
            if(count != 1)
            {
                return false;
            }

            return true;

        }


        /// <summary>
        /// This is used to check if delivered quantity is not more than approved quantity (STShowroomDelivery.Quantity)
        /// </summary>
        /// <param name="dto">SaveReceiveItemDeliveries</param>
        /// <returns>True if delivered quantity is valid, otherwise False</returns>
        private bool DeliveredQuantityNotMoreThanQuantity(SaveReceiveItemDeliveries dto)
        {
            if (dto.DeliveredQuantity.HasValue && dto.DeliveredQuantity > 0)
            {
                var obj = _context.STShowroomDeliveries.Where(p => p.STOrderDetailId == dto.STOrderDetailId
                                                    && p.STDeliveryId == dto.STDeliveryId
                                                    && p.DeliveryStatus == DeliveryStatusEnum.Waiting
                                                    && p.ReleaseStatus == ReleaseStatusEnum.Released).FirstOrDefault();

                if (obj != null)
                {
                    if (dto.DeliveredQuantity > obj.Quantity.Value)
                    {
                        return false;
                    }
                    

                    return true;
                }

                return false;
            }

            return true;
        }


        private bool CannotReceiveMoreThanApprovedQty(SaveReceiveItemDeliveries dto)
        {


            var orderDetail = _context.STOrderDetails.Where(p => p.Id == dto.STOrderDetailId
                                                     && p.ItemId == dto.ItemId).FirstOrDefault();


            var totalDelivered = Convert.ToInt32(
                                            _context.STShowroomDeliveries
                                            .Where(p => p.STOrderDetailId == dto.STOrderDetailId
                                                    && p.DeliveryStatus == DeliveryStatusEnum.Delivered)
                                            .Sum(p => p.DeliveredQuantity)
                                        );

            var order = _context.STOrders
                                .Where(p => p.Id == dto.STOrderId)
                                .FirstOrDefault();

            if(order.OrderType != OrderTypeEnum.InterbranchOrIntercompanyOrder && order.Warehouse.Vendor)
            {
                 if(totalDelivered > orderDetail.ApprovedQuantity)
                {
                    return false;
                }
            }

            return true;
            
        }
    }
}
