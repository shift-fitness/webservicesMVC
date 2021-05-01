using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.RazorPay
{
    public class MarchantDetailsInput
    {
        public string MarchantName { get; set; }
        public string MarchantKey { get; set; }
        public string MarchantSecretKey { get; set; }
        public string BranchCode { get; set; }

    }
}