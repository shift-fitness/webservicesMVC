using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility.AllFacilities.PersonTransfer
{
    public class PersonTransferDetails
    {
        public string Name { get; set; }
        public int MFDID { get; set; }
        public int CompleteDays { get; set; }
        public float TransferFees { get; set; }
        public int LeftOutDays { get; set; }
        public int TotalDays { get; set; }
        public float FinalAmount { get; set; }
        public float RemainingAmount { get; set; }
        public Boolean IsTransferUsed { get; set; }




    }
}