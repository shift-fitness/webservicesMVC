using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.PackagesList
{
    public class PreRenewalUtlizationInput
    {
        public string Comment { get; set; }
        public string MaritialStatus { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string MobileNo { get; set; }
        public string CreatedBy { get; set; }
        public string Photo { get; set; }
        public string PhotoURL { get; set; }
        public string Email { get; set; }
        public string Address2 { get; set; }
        public int EnquireTypeNo { get; set; }
        public string BranchCode { get; set; }
        public string PackageCode { get; set; }
        public string MembershipStartDate { get; set; }
        public string EndDate { get; set; }
        public float ActualPrice { get; set; }
        public int NumberOfSession { get; set; }
        public int NumberOfDaysValidity { get; set; }
        public string MobileDeviceID { get; set; }
        public string Invoice { get; set; }
        public string MemberShipCode { get; set; }
        public string GatewayProviderName { get; set; }
        public string OrderId { get; set; }
        public string ModeOfPayment { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string DiscountAmount { get; set; }
        public string receipt { get; set; }
        public string PaymentId { get; set; }
        public string notes { get; set; }
        public int TransactionId { get; set; }
        public string TransactionStatus { get; set; }
        public string TransactionDate { get; set; }
        public string Signature { get; set; }
        public string PlanCode { get; set; }
        public string MembershipExpireDate { get; set; }
        public string SlotCode { get; set; }
        public string TrainerCode { get; set; }
        public string PlanCostCode { get; set; }
        public string DurationId { get; set; }
        public string PayableAmount { get; set; }
        public double AmountDue { get; set; }
        public string DueDate { get; set; }
        public string FinalAmount { get; set; }
        public string PromoCode { get; set; }
        public string SlotPrice { get; set; }
        public string PlanCost { get; set; }
        public string Wallet { get; set; }

        public string MembershipCode { get; set; }
    
        public int Freezing { get; set; }
        public int Change { get; set; }
        public int Upgrade { get; set; }
        public int Transfer { get; set; }
        public int Paused { get; set; }
        public int Convert { get; set; }
      
    }
}