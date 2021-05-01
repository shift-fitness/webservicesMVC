using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility
{
    public class FacilityChangeInput
    {

        public string Comments { get; set; }
        public string WalletAmountUsed { get; set; }
        public string SlotPrice { get; set; }
        public string PlanCost { get; set; }

        public string PaymentDate { get; set; }
        public string InvoiceID { get; set; }
        public string RemainingAmount { get; set; }

        public string EnquireTypeNo { get; set; }
        public string PlanCostCode { get; set; }

        public string IsChange { get; set; }

        //CCRMMembersHealthInfo
        public string HealthCode { get; set; }


        //CCRMPreferred
        public string PreferredCode { get; set; }

        //CCRMGoals
        public string GoalCode { get; set; }

        //CCRMFITBasicMesurements  
        public string FITCode { get; set; }

        //CCRMLifeStyle
        public string LifeStyle { get; set; }
        public string BranchCode { get; set; }
        public string MemberShipCode { get; set; }
        public string SMFMID { get; set; }
        public string MFDID { get; set; }
        public string ChangeStartDate { get; set; }
        public string ChangeEndDate { get; set; }
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public string SessionCode { get; set; }
        public string SessionName { get; set; }
        public string PlanCode { get; set; }
        public string PlanName { get; set; }
        public string PlanAmount { get; set; }
        public string PlanStartDate { get; set; }
        public string SlotCode { get; set; }
        public string SlotName { get; set; }
        public string DurationCode { get; set; }
        public string DurationName { get; set; }
        public string Description { get; set; }
        public string FAPaymentModes { get; set; }
        public string PayableAmount2 { get; set; }
        public double PayableAmount { get; set; }
        public string FAPaymentModes2 { get; set; }
        public double IGSTableAmount { get; set; }
        public string GSTCodes { get; set; }
        public double IGST { get; set; }
        public double CGST { get; set; }
        public double SGST { get; set; }
        public string GymFee { get; set; }
        public string IGSTCode { get; set; }
        public string DiscountAmount { get; set; }
        public string PromoCode { get; set; }
        public string PromoCodeDescription { get; set; }
        public string DueDate { get; set; }
        public double DueAmount { get; set; }
        public string FinalAmount { get; set; }
        public string PaymentReceivedBy { get; set; }
        public string DowngradeDate { get; set; }
        public string LeftOutDays { get; set; }
        public string LeftOutAmount { get; set; }
        public string DifferentAmount { get; set; }
        public string ReceiverRoleID { get; set; }
        public string TrainerFees { get; set; }
        public string TrainerCode { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }


    }
}