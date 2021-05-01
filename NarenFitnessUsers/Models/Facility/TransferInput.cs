using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility
{
    public class TransferInput
    {
        public string WalletAmountUsed { get; set; }
        public string Comments { get; set; }
        public string SlotPrice { get; set; }
        public string PlanCost { get; set; }
        public string PaymentDate { get; set; }
        public string InvoiceID { get; set; }
        public string RemainingAmount { get; set; }
        public string EnquireTypeNo { get; set; }
        public string ToDurationCode { get; set; }
        public string IsTransferUsed { get; set; }
        public string MFDID { get; set; }
        public string SMFMID { get; set; }

        public string PaymentReceivedBy { get; set; }
        public string ReceiverRoleID { get; set; }
        // From 

        public string FromMemberShipCode { get; set; }
        public string FromMFDID { get; set; }
        public string FromSMFMID { get; set; }
        public string FromBranchCode { get; set; }
        public string FromPackageCode { get; set; }
        public string FromPackageName { get; set; }
        public string FromSessionCode { get; set; }
        public string FromSessionName { get; set; }
        public string FromPlanCode { get; set; }
        public string FromPlanName { get; set; }
        public string FromSlotCode { get; set; }
        public string FromSlotName { get; set; }
        public string FromMembershipStartDate { get; set; }
        public string FromMembershipExpireDate { get; set; }
        public string FromPlanAmount { get; set; }
        public string FromLeftOutDays { get; set; }
        public string FromLeftOutAmount { get; set; }

        // To 
        public string ToTrainerCode { get; set; }
        public string ToMemberShipCode { get; set; }
        public string ToBranchCode { get; set; }
        public string ToPackageCode { get; set; }
        public string ToPackageName { get; set; }
        public string ToSessionCode { get; set; }
        public string ToSessionName { get; set; }
        public string ToPlanCode { get; set; }
        public string ToPlanName { get; set; }
        public string ToSlotCode { get; set; }
        public string ToSlotName { get; set; }
        public string ToMembershipStartDate { get; set; }
        public string ToMembershipExpireDate { get; set; }

        // To Amount Details
        public string ToGSTCodes { get; set; }
        public string ToFAPaymentModes { get; set; }
        public string ToFAPaymentModes2 { get; set; }
        public string ToPayableAmount { get; set; }
        public string ToPayableAmount2 { get; set; }
        public string ToFinalAmount { get; set; }
        public double ToAmountDue { get; set; }
        public string ToGymFee { get; set; }
        public string ToIGSTableAmount { get; set; }
        public string ToIGST { get; set; }
        public string ToCGST { get; set; }
        public string ToSGST { get; set; }
        public string ToDueDate { get; set; }
        public string ToPromoCode { get; set; }
        public double ToDiscountAmount { get; set; }
        public string CreatedBy { get; set; }
        public string ToTrainerFees { get; set; }

        public string ToPlanCostCode { get; set; }
    }
}