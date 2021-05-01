using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Login
{
    public class CustomerLogin
    {
        public bool isExist { get; set; }
        public bool isUserLoggedIn { get; set; }
        public bool hasDetailsConfirmed { get; set; }
        public string errorMessage { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string MobileNo { get; set; }
        public string Password { get; set; }
        public string EmailId { get; set; }
        public string Address { get; set; }
        public int RoleCode { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        //public string RoleId { get; set; }
        public string DateOfBirth { get; set; }
        public float Height { get; set; }
        public float weight { get; set; }
        public int BMI { get; set; }
        //public int OTP { get; set; }
        public string MobileDeviceID { get; set; }
        public string CreatedBy { get; set; }
        //public string BranchCode { get; set; }
        public string Description { get; set; }
        public string HowDidYouKnowAboutUs { get; set; }
        public string PreferredCallBackTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string HeightMeasurement { get; set; }
        public string WeightMeasurement { get; set; }
    }
}