using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FC.Core.Domain.Views
{
    [NotMapped]
    public class ApproveTransferView 
    {
        public int? Id { get; set; }

        public string TransactionNo { get; set; }

        public TransactionTypeEnum? TransactionType { get; set; }


        public string TransactionTypeStr
        {
            get
            {
                if (this.TransactionType.HasValue)
                {
                    return EnumConverter.SplitName(Enum.GetName(typeof(TransactionTypeEnum), this.TransactionType));
                }
                return null;


            }
        }

        public RequestStatusEnum? RequestStatus { get; set; }

        public string RequestStatusStr {
            get
            {
                if (this.RequestStatus.HasValue)
                {
                    return EnumConverter.SplitName(Enum.GetName(typeof(RequestStatusEnum), this.RequestStatus));
                }
                return null;


            }
        }

        public string PONumber { get; set; }

        public DateTime?  PODate { get; set; }

        public DeliveryTypeEnum? DeliveryType { get; set; }

        public string DeliveryTypeStr
        {
            get
            {
                if (this.DeliveryType.HasValue)
                {
                    return EnumConverter.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), this.DeliveryType));
                }
                return null;


            }
        }

        public int  OrderedBy { get; set; }

        public int OrderedTo{ get; set; }

        public PaymentModeEnum? PaymentMode { get; set; }

        public string PaymentModeStr {
            get
            {
                if (this.PaymentMode.HasValue)
                {
                    return EnumConverter.SplitName(Enum.GetName(typeof(PaymentModeEnum), this.PaymentMode));
                }
                return null;


            }
        }

        public string SalesAgent { get; set; }

        public string ClientName { get; set; }

        public string Address { get; set; }

        public string Remarks { get; set; }

        public string ORNumber { get; set; }

        public string SINumber { get; set; }

        public string WHDRNumber { get; set; }

        public bool IsInterbranch { get
            {
                if(OrderedBy == OrderedTo)
                {
                    return true;
                }
                return false;
                
            }
        }

        public StoreCompanyRelationEnum? StoreCompanyRelation { get
            {
                if(OrderedTo == OrderedBy)
                {
                    return StoreCompanyRelationEnum.InterBranch;
                }

                return StoreCompanyRelationEnum.InterCompany;
            } }

        public string StoreCompanyRelationStr {
                get
                {
                    if (this.StoreCompanyRelation.HasValue)
                    {
                        return EnumConverter.SplitName(Enum.GetName(typeof(StoreCompanyRelationEnum), this.StoreCompanyRelation));
                    }
                    return null;


                }
        }


        public int? StoreId { get; set; }

        public int? OrderToStoreId { get; set; }



    }
}
