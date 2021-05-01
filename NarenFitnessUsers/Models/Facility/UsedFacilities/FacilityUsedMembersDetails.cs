using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility.UsedFacilities
{
    public class FacilityUsedMembersDetails
    {
       
        public string BranchCode { get; set; }
        public string MFDID { get; set; }
        public string InvoiceID { get; set; }
        public string RemainingAmount { get; set; }
        public string PreviousExpiryDate { get; set; }
        public string FacilityStartDate { get; set; }
        public string FacilityExpireDate { get; set; }
        public string LeftOutDays { get; set; }
        public string LeftOutAmount { get; set; }
        public string FreezingDays { get; set; }
        public string Days { get; set; }
        public string NewMembersCode { get; set; }
        public string NewInvoiceId { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }



    }
}