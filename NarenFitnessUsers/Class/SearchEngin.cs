using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Net;
using System.Web.Script.Serialization;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace NarenFitnessUsers.Class
{
    public class SearchEngin
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // BranchCode || PromoCode || DurationId || PackageCode || SlotCode  || MobileNo
        public string PromocodeQueryBuilder(Dictionary<string, string> PromoCodeSearch)
        {
            string Query = "select PID,PromoCode,PromoCodeName,PromoCodeDescription,FacilityApplicable,AddDays,DiscountAmount,DiscountPercentage,TermsAndConditions,PromoCodeStartDate,PromoCodeEndDate from PromoCodeMaster where";
            string QueryBuilder = "";
            string Column = "";
            string Field = "";
            int i = 0;

            foreach (var item in PromoCodeSearch)
            {
                Column = item.Key;
                Field = item.Value;
                if (Field != null)
                {
                    if(i > 0)
                    {
                        QueryBuilder = QueryBuilder +" AND";
                    }
                    QueryBuilder = QueryBuilder + " " + Column + "='" + Field + "'";


                    i = 1;
                }

            }

            Query = Query + QueryBuilder;
            return Query;
        }



    }
}