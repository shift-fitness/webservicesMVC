using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility
{
    public class FreezingAllInput
    {
        public string Sno { get; set; }
        public string InvoiceId { get; set; }
        public string PreviousExpiryDate { get; set; }
        public string Comments { get; set; }
        public string EnquireTypeNo { get; set; }
        public string PlanCostCode { get; set; }
        public string Status { get; set; }
        public string FreezingID { get; set; }
        public string BranchCode { get; set; }
        public string FreezingName { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string FreezingStartDate { get; set; }
        public string FreezingEndDate { get; set; }
        public string PlanStartDate { get; set; }
        public string MemberShipCode { get; set; }
        public int MFDID { get; set; }
        public string Description { get; set; }
        public int NoOfOptionCurrentlyInUse { get; set; }
        public int OptionsUsed { get; set; }
        public int NoOfOptions { get; set; }
        public int FreezingDays { get; set; }
        public string Mode { get; set; }
        public string Frequency { get; set; }
    }
}