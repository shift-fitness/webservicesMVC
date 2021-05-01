using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility.AllFacilities.LocationTransfer
{
    public class LocationTransferDetails
    {
        public string Name { get; set; }
        public int MFDID { get; set; }
        public double TransferFees { get; set; }
        public double CompleteDays { get; set; }
        public double LeftOutDays { get; set; }
        public double TotalDays { get; set; }
        public double FinalAmount { get; set; }
        public double RemainingAmount { get; set; }
        public Boolean IsTransferUsed { get; set; }
    }
}