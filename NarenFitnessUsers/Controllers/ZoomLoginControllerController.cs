using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft;
using Newtonsoft.Json;
using NarenFitnessUsers.Models.Applications;
using NarenFitnessUsers.Models.Slots.Slot;
using NarenFitnessUsers.Models.Booking;
using NarenFitnessUsers.Class;
using NarenFitnessUsers.Models.Login;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;

namespace NarenFitnessUsers.Controllers
{
    public class ZoomLoginControllerController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        public Object MeetingDetails([FromBody]ZoomRequest zoomreq)
        {

            FinalOutPutB FOutputA = new FinalOutPutB();
            DataSet ds_SlotBooking = new DataSet();
            string sJSONResponse = "";
            ZoomResponse zp = new ZoomResponse();
            List<zoombody> zoomslist = new List<zoombody>();

            try
            {
                zoombody zooms = new zoombody { meetingDetails = Getmeetingdetails(zoomreq.mobileNo, zoomreq.slotId) };
                zoomslist.Add(zooms);

                zp.status = "success";
                zp.value = zoomslist;
                sJSONResponse = JsonConvert.SerializeObject(zp);

            }
            catch (Exception ec)
            {
                zp.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(zp);
            }

            return sJSONResponse;
        }
        public List<ZoomLogin> Getmeetingdetails(string MobileNo, string SlotId)
        {
            string meetingId = "";
            string meetingUsesrName = "";
            string meetingPassword = "";
            string meetingUrl = "";

            List<ZoomLogin> ftd = new List<ZoomLogin>();
            DataTable dt = new DataTable();

            dt = getdata(string.Format("select top 1  OLPU.ZoomId,OLPU.ZoomPassword,OLPU.ZoomUserName,OLPU.ZoomUrl,OLPU.GoogleId,OLPU.GooglePassword,OLPU.GoogleUserName,OLPU.GoogleUrl,convert(varchar(10), OLST.SlotStartTime, 108) as slotStartTime,convert(varchar(10), OLST.SlotEndTime, 108) as slotEndTime,OLPU.ID,OLPU.SlotID,OLPU.TrainerID from OnlinePackageUsed OLPU,OnlineSlotTimings OLST where  OLPU.SlotID=OLST.SlotCode and  OLPU.IsDeleted=0 and OLPU.MobileNo='{0}' and OLPU.SlotID='{1}' order by OLST.ID  desc ", MobileNo, SlotId));

            if (dt.Rows[0]["ZoomId"].ToString() != null)
            {
                meetingId = dt.Rows[0]["ZoomId"].ToString();
                meetingUsesrName = dt.Rows[0]["ZoomId"].ToString();
                meetingPassword = dt.Rows[0]["ZoomId"].ToString();
                meetingUrl = dt.Rows[0]["ZoomUrl"].ToString();
            }
            else
            {
                meetingId = dt.Rows[0]["GoogleId"].ToString();
                meetingUsesrName = dt.Rows[0]["GooglePassword"].ToString();
                meetingPassword = dt.Rows[0]["GoogleUserName"].ToString();
                meetingUrl = dt.Rows[0]["GoogleUrl"].ToString();
            }


            ftd.Add(new ZoomLogin
            {
                meetingId = Convert.ToString(meetingId)
                 ,
                meetingPassword = Convert.ToString(meetingPassword)
                 ,
                meetingUserName = Convert.ToString(meetingUsesrName)
                ,
                meetingUrl = Convert.ToString(meetingUrl)
                 ,
                slotStartTime = Convert.ToString(dt.Rows[0]["slotStartTime"])
                ,
                slotEndTime = Convert.ToString(dt.Rows[0]["slotEndTime"])
                ,
                id = Convert.ToString(dt.Rows[0]["ID"])
                ,
                trainerId = Convert.ToString(dt.Rows[0]["TrainerID"])
            });

            return ftd;

        }
        private DataTable getdata(string query)
        {

            SqlCommand cmd = new SqlCommand(query);

            using (SqlDataAdapter sda = new SqlDataAdapter())
            {
                cmd.Connection = cnn;
                sda.SelectCommand = cmd;
                using (DataTable dt = new DataTable())
                {
                    sda.Fill(dt);
                    return dt;
                }
            }
        }
    }
}