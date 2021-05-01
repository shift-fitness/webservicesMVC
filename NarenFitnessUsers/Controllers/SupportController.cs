using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization;
using System.Collections;
using System.Web.Script.Serialization;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft;
using Newtonsoft.Json;
using NarenFitnessUsers.Models.Support;


namespace NarenFitnessUsers.Controllers
{
    public class SupportController : Controller
    {

        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
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
        public Object TicketTypesUpdate([FromBody]SupportInput si)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string olPackage_Query = "";
            SupportOutput ffopt = new SupportOutput();

            try
            {
                cnn.Open();
                olPackage_Query = "update TicketTypes set RequestType='" + si.RequestType + "' where ID=" + si.ID + " ";
                SqlCommand tm_cmd = new SqlCommand(olPackage_Query, cnn);
                tm_cmd.ExecuteNonQuery();
                ffopt.status = "Success";

            }
            catch (Exception ex)
            {
                ffopt.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object TicketTypesPost([FromBody]SupportInput si)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string olPackage_Query = "insert into TicketTypes(RequestType) values('" + si.RequestType + "')";

            SupportOutput ffopt = new SupportOutput();

            try
            {
                cnn.Open();

                SqlCommand tm_cmd = new SqlCommand(olPackage_Query, cnn);
                tm_cmd.ExecuteNonQuery();
                ffopt.status = "Success";

            }
            catch (Exception ex)
            {
                ffopt.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object GetTicketTypes([FromBody]SupportInput si)
        {
            TicketTypeOpt fdop = new TicketTypeOpt();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_TicketTypes = new DataTable();

            List<TicketTypes> ttdetails = new List<TicketTypes>();

            try
            {
                dt_TicketTypes = getdata(string.Format("Select ID,RequestType from TicketTypes"));

                for (int i = 0; i < dt_TicketTypes.Rows.Count; i++)
                {
                    TicketTypes fd = new TicketTypes { ID = dt_TicketTypes.Rows[i]["ID"].ToString(), RequestType = dt_TicketTypes.Rows[i]["RequestType"].ToString() };
                    ttdetails.Add(fd);
                }

                fdop.status = "success";
                fdop.value = ttdetails;
                sJSONResponse = JsonConvert.SerializeObject(fdop);


            }
            catch (Exception ec)
            {
                fdop.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(fdop);
            }
            return sJSONResponse;
        }
        public Object TicketTypesDelete([FromBody]SupportInput si)
        {
            string sJSONResponse = "";
            SupportOutput ffopt = new SupportOutput();
            SqlCommand command = cnn.CreateCommand();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            try
            {

                cnn.Close();
                cnn.Open();
                SqlTransaction transaction;
                transaction = cnn.BeginTransaction("SampleTransaction");
                command.Connection = cnn;
                command.Transaction = transaction;
                try
                {
                    command.CommandText = "delete from TicketTypes where ID=" + si.ID + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
                finally
                {
                    ffopt.status = "success";

                }

            }
            catch (Exception ec)
            {

            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object TicketBookingUpdate([FromBody]SupportInput si)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string olPackage_Query = "";
            SupportOutput ffopt = new SupportOutput();

            try
            {
                cnn.Open();
                olPackage_Query = "update TicketBooking set TicketId='" + si.TicketId + "',RequestTypeId='" + si.RequestTypeId + "',BranchCode='" + si.BranchCode + "',MemberShipCode='" + si.MemberShipCode + "',InvoiceId='" + si.InvoiceId + "',Comments='" + si.Comments + "' where ID=" + si.ID + " ";
                SqlCommand tm_cmd = new SqlCommand(olPackage_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                ffopt.status = "Success";

            }
            catch (Exception ex)
            {
                ffopt.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object TicketBookingPost([FromBody]SupportInput si)
        {

            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;

            DataTable dt_LID = new DataTable();
            dt_LID = getdata(string.Format("select ISNULL(MAX(ID),0) as LID from TicketBooking"));

            string TicketId = "";
            int count = 0;

            count = Convert.ToInt32(dt_LID.Rows[0]["LID"].ToString()) + 1;
            TicketId = si.BranchCode + Convert.ToString(count);

            SupportOutput ffopt = new SupportOutput();

            cnn.Open();
            SqlCommand command = cnn.CreateCommand();
            SqlTransaction transaction;
            transaction = cnn.BeginTransaction("SampleTransaction");
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {
                command.CommandText = "insert into TicketBooking(TicketId,RequestTypeId,BranchCode,MemberShipCode,InvoiceId,Comments,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + TicketId + "','" + si.RequestTypeId + "','" + si.BranchCode + "','" + si.MemberShipCode + "','" + si.InvoiceId + "','" + si.Comments + "','" + si.CreatedBy + "','" + ServerDateTime + "',0,1)";
                command.ExecuteNonQuery();
                command.CommandText = "insert into TicketTransaction(TicketId,RequestTypeId,BranchCode,MemberShipCode,InvoiceId,StatusId,StatusName,PostPoneDate,Comments,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + TicketId + "','" + si.RequestTypeId + "','" + si.BranchCode + "','" + si.MemberShipCode + "','" + si.InvoiceId + "','1','Pending','','" + si.Comments + "','" + si.CreatedBy + "','" + ServerDateTime + "',0,1)";
                command.ExecuteNonQuery();
                transaction.Commit();
                ffopt.status = "Success";
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();

                }
                catch (Exception ex2)
                {

                }
                ffopt.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object GetTicketBooking([FromBody]SupportInput si)
        {
            TicketBookingOutput fdop = new TicketBookingOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_TicketTypes = new DataTable();

            List<TicketBooking> ttdetails = new List<TicketBooking>();

            try
            {
                dt_TicketTypes = getdata(string.Format("Select ID,TicketId,RequestTypeId,BranchCode,MemberShipCode,MemberName,Comments from TicketBooking  where BranchCode='{0}' and CreatedOn between '{1}' and '{2}' ", si.BranchCode, si.FromDate, si.ToDate));

                for (int i = 0; i < dt_TicketTypes.Rows.Count; i++)
                {
                    TicketBooking fd = new TicketBooking { ID = Convert.ToInt32(dt_TicketTypes.Rows[i]["ID"].ToString()), TicketId = dt_TicketTypes.Rows[i]["TicketId"].ToString(), RequestTypeId = Convert.ToInt32(dt_TicketTypes.Rows[i]["RequestTypeId"].ToString()), BranchCode = dt_TicketTypes.Rows[i]["BranchCode"].ToString(), MemberShipCode = dt_TicketTypes.Rows[i]["MemberShipCode"].ToString(), MemberName = dt_TicketTypes.Rows[i]["MemberName"].ToString(), Comments = dt_TicketTypes.Rows[i]["Comments"].ToString() };
                    ttdetails.Add(fd);
                }

                fdop.status = "success";
                fdop.value = ttdetails;
                sJSONResponse = JsonConvert.SerializeObject(fdop);

            }
            catch (Exception ec)
            {

                sJSONResponse = JsonConvert.SerializeObject(fdop);
            }
            return sJSONResponse;
        }
        public Object TicketBookingDelete([FromBody]SupportInput si)
        {
            string sJSONResponse = "";
            SupportOutput ffopt = new SupportOutput();
            SqlCommand command = cnn.CreateCommand();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            try
            {

                cnn.Close();
                cnn.Open();
                SqlTransaction transaction;
                transaction = cnn.BeginTransaction("SampleTransaction");
                command.Connection = cnn;
                command.Transaction = transaction;
                try
                {
                    command.CommandText = "";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
                finally
                {
                    ffopt.status = "fail";

                }

            }
            catch (Exception ec)
            {

            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object TicketTransactionUpdate([FromBody]SupportInput si)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string olPackage_Query = "";
            SupportOutput ffopt = new SupportOutput();

            try
            {

                cnn.Open();

                olPackage_Query = "update TicketTransaction set TicketId='" + si.TicketId + "',RequestTypeId='" + si.RequestTypeId + "',BranchCode='" + si.BranchCode + "',MemberShipCode='" + si.MemberShipCode + "',InvoiceId='" + si.InvoiceId + "',Comments='" + si.Comments + "', StatusId = '" + si.StatusId + "',StatusName = '" + si.StatusName + "' where ID=" + si.ID + " ";
                SqlCommand tm_cmd = new SqlCommand(olPackage_Query, cnn);
                tm_cmd.ExecuteNonQuery();
                ffopt.status = "Success";

            }
            catch (Exception ex)
            {
                ffopt.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object TicketTransactionPost([FromBody]SupportInput si)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string olPackage_Query = "insert into TicketTransaction(TicketId,RequestTypeId,BranchCode,MemberShipCode,InvoiceId,StatusId,StatusName,PostPoneDate,Comments,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + si.TicketId + "','" + si.RequestTypeId + "','" + si.BranchCode + "','" + si.MemberShipCode + "','" + si.InvoiceId + "','" + si.StatusId + "','" + si.StatusName + "','" + si.PostPoneDate + "','" + si.Comments + "','" + si.CreatedBy + "','" + ServerDateTime + "',0,1)";

            SupportOutput ffopt = new SupportOutput();

            try
            {
                cnn.Open();

                SqlCommand tm_cmd = new SqlCommand(olPackage_Query, cnn);
                tm_cmd.ExecuteNonQuery();
                ffopt.status = "Success";

            }
            catch (Exception ex)
            {
                ffopt.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object GetTicketTransaction([FromBody]SupportInput si)
        {
            TicketTransactionOutput fdop = new TicketTransactionOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_TicketTypes = new DataTable();

            List<TicketTransaction> ttdetails = new List<TicketTransaction>();

            try
            {
                dt_TicketTypes = getdata(string.Format("Select ID,TicketId,RequestTypeId,BranchCode,MemberShipCode,MemberName,StatusId,StatusName,PostPoneDate,Comments from TicketTransaction where BranchCode='" + si.BranchCode + "' and PostPoneDate between '" + si.FromDate + "' and '" + si.ToDate + "'"));

                for (int i = 0; i < dt_TicketTypes.Rows.Count; i++)
                {
                    TicketTransaction fd = new TicketTransaction { ID = Convert.ToInt32(dt_TicketTypes.Rows[i]["ID"].ToString()), TicketId = dt_TicketTypes.Rows[i]["TicketId"].ToString(), RequestTypeId = Convert.ToInt32(dt_TicketTypes.Rows[i]["RequestTypeId"].ToString()), BranchCode = dt_TicketTypes.Rows[i]["BranchCode"].ToString(), MemberShipCode = dt_TicketTypes.Rows[i]["MemberShipCode"].ToString(), MemberName = dt_TicketTypes.Rows[i]["MemberName"].ToString(), Comments = dt_TicketTypes.Rows[i]["Comments"].ToString(), StatusId = dt_TicketTypes.Rows[i]["StatusId"].ToString(), StatusName = dt_TicketTypes.Rows[i]["StatusName"].ToString() };
                    ttdetails.Add(fd);
                }

                fdop.status = "success";
                fdop.value = ttdetails;
                sJSONResponse = JsonConvert.SerializeObject(fdop);


            }
            catch (Exception ec)
            {
                fdop.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(fdop);
            }
            return sJSONResponse;
        }
        public Object TicketTransactionDelete([FromBody]SupportInput si)
        {
            string sJSONResponse = "";
            SupportOutput ffopt = new SupportOutput();
            SqlCommand command = cnn.CreateCommand();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            try
            {

                cnn.Close();
                cnn.Open();
                SqlTransaction transaction;
                transaction = cnn.BeginTransaction("SampleTransaction");
                command.Connection = cnn;
                command.Transaction = transaction;
                try
                {
                    command.CommandText = "delete from TicketTransaction where ID=" + si.ID + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
                finally
                {
                    ffopt.status = "success";

                }

            }
            catch (Exception ec)
            {

            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        //FunName : GetBookingDetails
        //Mode : Offline   || Offline =  Request : MoboleNo  || Table : CCRMMembership  || Reponse :  PackageName , StartDate , InvoiceId  
        //Mode : Online    || Online  = Request  : MobileNo  || Table : OnlinePackagePurchase  || Reponse :  PackageName , StartDate , InvoiceId
        public Object GetBookingDetails([FromBody]BookingDetailsInput bdi)
        {
            BookingDetailsOpt bdo = new BookingDetailsOpt();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_BookingDetails = new DataTable();
            List<BookingDetails> bdetails = new List<BookingDetails>();

            try
            {
                if (bdi.Mode == "OffLine")
                {
                    dt_BookingDetails = getdata(string.Format("select InvoiceID,CCRMM.PackageCode,CMSP.PackageName,StartDate,CCRMM.BranchCode,B.BranchName from CCRMMembership CCRMM,Users U,CMSPACKAGES CMSP,Branch B where  CMSP.PackageCode=CCRMM.PackageCode and B.BranchCode=CCRMM.BranchCode and  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' ", bdi.MobileNo));
                }
                else if (bdi.Mode == "OnLine")
                {
                    dt_BookingDetails = getdata(string.Format("select Invoice as InvoiceID,OPP.PackageID as PackageCode,OP.PackageName,OPP.BranchCode,BranchName='Online', StartDate from OnlinePackagePurchase OPP,OnlinePackages OP where OPP.PackageID=OP.PackageID and OPP.MobileNo='{0}'", bdi.MobileNo));
                }
                else
                { }
                for (int i = 0; i < dt_BookingDetails.Rows.Count; i++)
                {
                    BookingDetails fd = new BookingDetails { InvoiceId = dt_BookingDetails.Rows[i]["InvoiceID"].ToString(), PackageCode = dt_BookingDetails.Rows[i]["PackageCode"].ToString(), PackageName = dt_BookingDetails.Rows[i]["PackageName"].ToString(), BranchName = dt_BookingDetails.Rows[i]["BranchName"].ToString(), StartDate = Convert.ToDateTime(dt_BookingDetails.Rows[i]["StartDate"].ToString()), BranchCode = dt_BookingDetails.Rows[i]["BranchCode"].ToString(), };
                    bdetails.Add(fd);
                }

                bdo.status = "success";
                bdo.value = bdetails;
                sJSONResponse = JsonConvert.SerializeObject(bdo);


            }
            catch (Exception ec)
            {
                bdo.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(bdo);
            }
            return sJSONResponse;
        }

        // RaiseTicket : 

        // FunctoionName  :  GetMembersTickets || Table : 

        //

        public Object GetMembersTickets([FromBody]BookingDetailsInput bdi)
        {
            MembersOpt mo = new MembersOpt();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_MembersTickets = new DataTable();
            List<MembersTicketDetails> bdetails = new List<MembersTicketDetails>();

            try
            {

                dt_MembersTickets = getdata(string.Format("select TIT.ID,TT.RequestType as RequestName,TB.CreatedOn as RaisedOn,TIT.CreatedOn as CloserDated,TIT.PostPoneDate as NextUpdate,TIT.StatusId,TIT.StatusName,TIT.Comments from  TicketTypes TT,TicketBooking TB,TicketTransaction TIT where TT.ID=TB.RequestTypeId and TB.TicketId=TIT.TicketId and TB.InvoiceId='{0}'", bdi.InvoiceId));

                for (int i = 0; i < dt_MembersTickets.Rows.Count; i++)
                {
                    MembersTicketDetails fd = new MembersTicketDetails { Id = dt_MembersTickets.Rows[i]["ID"].ToString(), RequestName = dt_MembersTickets.Rows[i]["RequestName"].ToString(), StatusId = dt_MembersTickets.Rows[i]["StatusId"].ToString(), StatusName = dt_MembersTickets.Rows[i]["StatusName"].ToString(), CloserDated = Convert.ToDateTime(dt_MembersTickets.Rows[i]["CloserDated"].ToString()), RaisedOn = Convert.ToDateTime(dt_MembersTickets.Rows[i]["RaisedOn"].ToString()), NextUpdate = Convert.ToDateTime(dt_MembersTickets.Rows[i]["NextUpdate"].ToString()), Comments = dt_MembersTickets.Rows[i]["Comments"].ToString() };
                    bdetails.Add(fd);
                }

                mo.status = "success";
                mo.value = bdetails;
                sJSONResponse = JsonConvert.SerializeObject(mo);


            }
            catch (Exception ec)
            {
                mo.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(mo);
            }
            return sJSONResponse;
        }
        public Object GetOpenTickets([FromBody]BookingDetailsInput bdi)
        {
            OpenTicketOpts mo = new OpenTicketOpts();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_MembersTickets = new DataTable();
            List<OpenTickets> bdetails = new List<OpenTickets>();

            try
            {

                dt_MembersTickets = getdata(string.Format("select TT.MemberShipCode,TT.TicketId,TT.RequestTypeId,TTP.RequestType,TT.Comments,TT.PostPoneDate,TT.InvoiceId,TT.CreatedBy,TT.CreatedOn,TT.StatusId from TicketTransaction TT,TicketTypes TTP where  TT.ID=TTP.ID and BranchCode='{0}' and StatusId between 1 and 3  and StatusId!=2 ", bdi.BranchCode));

                for (int i = 0; i < dt_MembersTickets.Rows.Count; i++)
                {
                    OpenTickets fd = new OpenTickets { memberShipCode = dt_MembersTickets.Rows[0]["MemberShipCode"].ToString(), TicketId = Convert.ToInt32(dt_MembersTickets.Rows[0]["TicketId"].ToString()), RequestTypeId = Convert.ToInt32(dt_MembersTickets.Rows[0]["RequestTypeId"].ToString()), RequestType = dt_MembersTickets.Rows[0]["RequestType"].ToString(), Comments = dt_MembersTickets.Rows[0]["Comments"].ToString(), PostPoneDate = dt_MembersTickets.Rows[0]["PostPoneDate"].ToString(), InvoiceId = dt_MembersTickets.Rows[0]["InvoiceId"].ToString(), CreatedBy = dt_MembersTickets.Rows[0]["CreatedBy"].ToString(), CreatedOn = Convert.ToDateTime(dt_MembersTickets.Rows[0]["CreatedOn"].ToString()), StatusId = Convert.ToInt32(dt_MembersTickets.Rows[0]["StatusId"].ToString()) };
                    bdetails.Add(fd);
                }

                mo.status = "success";
                mo.value = bdetails;
                sJSONResponse = JsonConvert.SerializeObject(mo);


            }
            catch (Exception ec)
            {
                mo.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(mo);
            }
            return sJSONResponse;
        }
        public Object GetClosedTickets([FromBody]BookingDetailsInput bdi)
        {
            OpenTicketOpts mo = new OpenTicketOpts();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_MembersTickets = new DataTable();
            List<OpenTickets> bdetails = new List<OpenTickets>();

            try
            {

                dt_MembersTickets = getdata(string.Format("select TT.MemberShipCode,TT.TicketId,TT.RequestTypeId,TTP.RequestType,TT.Comments,TT.PostPoneDate,TT.InvoiceId,TT.CreatedBy,TT.CreatedOn,TT.StatusId from TicketTransaction TT,TicketTypes TTP where  TT.ID=TTP.ID and BranchCode='{0}' and StatusId=2", bdi.BranchCode));

                for (int i = 0; i < dt_MembersTickets.Rows.Count; i++)
                {
                    OpenTickets fd = new OpenTickets { memberShipCode = dt_MembersTickets.Rows[0]["MemberShipCode"].ToString(), TicketId = Convert.ToInt32(dt_MembersTickets.Rows[0]["TicketId"].ToString()), RequestTypeId = Convert.ToInt32(dt_MembersTickets.Rows[0]["RequestTypeId"].ToString()), RequestType = dt_MembersTickets.Rows[0]["RequestType"].ToString(), Comments = dt_MembersTickets.Rows[0]["Comments"].ToString(), PostPoneDate = dt_MembersTickets.Rows[0]["PostPoneDate"].ToString(), InvoiceId = dt_MembersTickets.Rows[0]["InvoiceId"].ToString(), CreatedBy = dt_MembersTickets.Rows[0]["CreatedBy"].ToString(), CreatedOn = Convert.ToDateTime(dt_MembersTickets.Rows[0]["CreatedOn"].ToString()), StatusId = Convert.ToInt32(dt_MembersTickets.Rows[0]["StatusId"].ToString()) };
                    bdetails.Add(fd);
                }

                mo.status = "success";
                mo.value = bdetails;
                sJSONResponse = JsonConvert.SerializeObject(mo);


            }
            catch (Exception ec)
            {
                mo.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(mo);
            }
            return sJSONResponse;
        }



    }
}