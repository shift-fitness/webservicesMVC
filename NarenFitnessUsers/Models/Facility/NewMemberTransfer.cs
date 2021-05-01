using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility
{
    public class NewMemberTransfer
    {
        public string WalletAmountUsed { get; set; }
        public string Comments { get; set; }
        public string SlotPrice { get; set; }
        public string PlanCost { get; set; }
        public string PaymentDate { get; set; }
        public string InvoiceID { get; set; }
        public string RemainingAmount { get; set; }
        public string EnquireTypeNo { get; set; }
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

        public string ToDurationCode { get; set; }
        public string ToPlanCostCode { get; set; }
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
        public double ToDueAmount { get; set; }
        public string ToGymFee { get; set; }
        public double ToIGSTableAmount { get; set; }
        public string ToIGST { get; set; }
        public string ToCGST { get; set; }
        public string ToSGST { get; set; }
        public string ToDueDate { get; set; }
        public string ToPromoCode { get; set; }
        public double ToDiscountAmount { get; set; }
        public string CreatedBy { get; set; }
        public string ToTrainerFees { get; set; }


        //// new


        public string DiscountAmount { get; set; }

        public string Photo { get; set; }
        public string Others { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string BranchCode { get; set; }
        public string SlotCode { get; set; }
        public string MembershipCode { get; set; }
        public string Mode { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string UCode { get; set; }
        public string UserName { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string DateOfJoining { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string PinCode { get; set; }
        public string MobileNo { get; set; }
        public string MaritialStatus { get; set; }
        public string Gender { get; set; }

        //Login
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }

        // Membership

        public string PlanCode { get; set; }
        public string PackageCode { get; set; }
        public string MembershipExpireDate { get; set; }
        public string MembershipStartDate { get; set; }

        //CCRMMembersHealthInfo
        public string HealthCode { get; set; }
        public string Description { get; set; }

        //CCRMPreferred
        public string PreferredCode { get; set; }

        //CCRMGoals
        public string GoalCode { get; set; }

        //CCRMFITBasicMesurements  
        public string FITCode { get; set; }

        //CCRMLifeStyle
        public string LifeStyle { get; set; }

        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}