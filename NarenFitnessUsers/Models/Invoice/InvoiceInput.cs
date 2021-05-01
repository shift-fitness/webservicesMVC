using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Invoice
{
    public class InvoiceInput
    {
        public string RemainingAmount { get; set; }
        public string Invoice { get; set; }
        public string WriteOFF { get; set; }
        public string Wallet { get; set; }
        public string SlotPrice { get; set; }
        public string PlanCost { get; set; }

        public string PaymentDate { get; set; }
        public string DuePaidAmount { get; set; }
        public string Comments { get; set; }

        public string ID { get; set; }
        public string Address2 { get; set; }
        public string DurationCode { get; set; }
        public string PlanCostCode { get; set; }
        public string TrainerCode { get; set; }
        public string EnquireTypeNo { get; set; }

        public string DiscountAmount { get; set; }
        public string PaymentReceivedBy { get; set; }
        public string ReceiverRoleID { get; set; }


        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string BranchCode { get; set; }
        public string SlotCode { get; set; }
        public string MembershipCode { get; set; }
        public string Mode { get; set; }

        public string UCode { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string DateOfBirth { get; set; }
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


        // Membership

        public string PlanCode { get; set; }
        public string PackageCode { get; set; }
        public string MembershipExpireDate { get; set; }
        public string MembershipStartDate { get; set; }



        // GST
        public double FinalAmount { get; set; }
        public double GymFee { get; set; }
        public string IGSTCode { get; set; }
        public string FAPaymentModes { get; set; }
        public double PayableAmount { get; set; }
        public string FAPaymentModes2 { get; set; }
        public double PayableAmount2 { get; set; }
        public double PersonalTrainerFee { get; set; }
        public double DueAmount { get; set; }
        public string DueDate { get; set; }
        public string Trainer { get; set; }
        public double IGSTableAmount { get; set; }
        public double IGST { get; set; }
        public double CGST { get; set; }
        public double SGST { get; set; }
        public string GSTCodes { get; set; }
        public string PromoCode { get; set; }
    }
}