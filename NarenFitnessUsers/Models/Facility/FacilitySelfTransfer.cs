using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility
{
    public class FacilitySelfTransfer
    {
        public string Comments { get; set; }
        public string WalletAmountUsed { get; set; }
        public string RemainingAmount { get; set; }
        public string SlotPrice { get; set; }
        public string PlanCost { get; set; }
        public string PaymentDate { get; set; }
        public string EnquireTypeNo { get; set; }
        public string FromBranchCode { get; set; }
        public string ToBranchCode { get; set; }
        public string PlanCostCode { get; set; }
        public string DurationCode { get; set; }
        public string IsTransferUsed { get; set; }
        public string MFDID { get; set; }
        public string SMFMID { get; set; }
        public string PaymentReceivedBy { get; set; }
        public string ReceiverRoleID { get; set; }
        // From 
        public string MemberShipCode { get; set; }
        public string BranchCode { get; set; }
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public string SessionCode { get; set; }
        public string SessionName { get; set; }
        public string PlanCode { get; set; }
        public string PlanName { get; set; }
        public string SlotCode { get; set; }
        public string SlotName { get; set; }
        public string MembershipStartDate { get; set; }
        public string MembershipExpireDate { get; set; }
        public string PlanAmount { get; set; }
        public string LeftOutDays { get; set; }
        public string FromLeftOutAmount { get; set; }

        // To 
        public string TrainerCode { get; set; }


        // To Amount Details
        public string GSTCodes { get; set; }
        public string FAPaymentModes { get; set; }
        public string FAPaymentModes2 { get; set; }
        public string PayableAmount { get; set; }
        public string PayableAmount2 { get; set; }
        public string FinalAmount { get; set; }
        public double AmountDue { get; set; }
        public string GymFee { get; set; }
        public string IGSTableAmount { get; set; }
        public string IGST { get; set; }
        public string CGST { get; set; }
        public string SGST { get; set; }
        public string DueDate { get; set; }
        public string PromoCode { get; set; }
        public double DiscountAmount { get; set; }
        public string CreatedBy { get; set; }
        public string TrainerFees { get; set; }
    }
}