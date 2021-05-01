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
using NarenFitnessUsers.Models.Trainer;
using NarenFitnessUsers.Models.Trainer.TrainerList;
using NarenFitnessUsers.Models.Trainer.TrainerDetails;
using NarenFitnessUsers.Models.Trainer.OnlineTrainerList;
using NarenFitnessUsers.Models.Trainer.OnlineTrainerDetails;
using NarenFitnessUsers.Class;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;
using System.Web.Helpers;
using System.Collections;

namespace NarenFitnessUsers.Controllers
{
    public class TrainersController : Controller
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

        //OffLine
        // Trainers List

        public List<OffLinePerviousTrainerDetailsObject> OffLinePerviousTrainerDetailsObject(string MobileNo)
        {
            List<OffLinePerviousTrainerDetailsObject> _OffLinePerviousTrainerDetailsObject = new List<OffLinePerviousTrainerDetailsObject>();

            DataTable dt = getdata(string.Format("select top 1 CCRM.TrainerCode as TrainerCode,HRE.EmployeeName as TrainerName,HRE.PhotoURL as Image  from CCRMMembership CCRM,HREmployee HRE ,Users U where CCRM.TrainerCode=HRE.EmployeeCode and U.UCode=CCRM.MembershipCode and U.MobileNo ='{0}'", MobileNo));

            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                _OffLinePerviousTrainerDetailsObject.Add(new OffLinePerviousTrainerDetailsObject
                {
                    trainerCode = Convert.ToString(dt.Rows[i]["TrainerCode"].ToString())
                                                 ,
                    trainerName = Convert.ToString(dt.Rows[i]["TrainerName"].ToString())
                     ,
                    trainerImage = Convert.ToString(dt.Rows[i]["Image"].ToString())

                });
            }
            return _OffLinePerviousTrainerDetailsObject;
        }
        public List<OffLineTrainerListObject> OffLineTrainerListObject(string MobileNo, string BranchCode)
        {
            List<OffLineTrainerListObject> _OffLineTrainerListObject = new List<OffLineTrainerListObject>();

            DataTable dt = getdata(string.Format("select distinct HRE.EmployeeCode as TrainerCode,HRE.EmployeeName as TrainerName,HRE.PhotoURL as Image from HREmployee HRE, TrainerSlotSpecializationMapping OLTSM where OLTSM.TrainerCode=HRE.EmployeeCode and OLTSM.IsDeleted=0 and OLTSM.BranchCode='{1}' and OLTSM.TrainerCode not in(select top 1 CCRM.TrainerCode as TrainerCode  from CCRMMembership CCRM,HREmployee HRE ,Users U where CCRM.TrainerCode=HRE.EmployeeCode and U.UCode=CCRM.MembershipCode and U.MobileNo ='{0}') '  order by OLPU.ID desc)", MobileNo, BranchCode));

            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                _OffLineTrainerListObject.Add(new OffLineTrainerListObject
                {
                    trainerCode = Convert.ToString(dt.Rows[i]["TrainerCode"].ToString())
                                                 ,
                    trainerName = Convert.ToString(dt.Rows[i]["TrainerName"].ToString())
                     ,
                    trainerImage = Convert.ToString(dt.Rows[i]["Image"].ToString())

                });
            }
            return _OffLineTrainerListObject;
        }

        public Object OffLineGetTrainersList([FromBody]OffLineGetTrainerInput Trainer)
        {

            DataTable dt_Trainer = new DataTable();
            string sJSONResponse = "";
            DataSet dt_Packageresposne = new DataSet();

            OffLineGetTraineroutput topt = new OffLineGetTraineroutput();
            List<OffLineTrainerObjects> TrainerObjects = new List<OffLineTrainerObjects>();

            try
            {
                dt_Trainer = getdata(string.Format("select distinct HRE.EmployeeCode as TrainerCode,HRE.EmployeeName as TrainerName from HREmployee HRE,TrainerSlotSpecializationMapping OLTSM where OLTSM.TrainerCode=HRE.EmployeeCode and OLTSM.IsDeleted=0", ""));

                OffLineTrainerObjects OTO = new OffLineTrainerObjects { previousTrainers = OffLinePerviousTrainerDetailsObject(Trainer.MobileNo), trainersList = OffLineTrainerListObject(Trainer.MobileNo, Trainer.BranchCode) };
                TrainerObjects.Add(OTO);

                topt.status = "success";
                topt.value = TrainerObjects;
                sJSONResponse = JsonConvert.SerializeObject(topt);

            }
            catch (Exception ec)
            {
                topt.status = "success";

                sJSONResponse = JsonConvert.SerializeObject(topt);

            }



            return sJSONResponse;

        }

        //public Object GetTrainersList([FromBody]GetTrainerInput Trainer)
        //{

        //    DataTable dt_Trainer = new DataTable();
        //    string sJSONResponse = "";
        //    DataSet dt_Packageresposne = new DataSet();

        //    GetTraineroutput topt = new GetTraineroutput();
        //    List<TrainersFields> TrainerList = new List<TrainersFields>();
        //    try
        //    {
        //        dt_Trainer = getdata(string.Format("select distinct HRE.EmployeeCode as TrainerCode,HRE.EmployeeName as TrainerName,TSSM.PackageCode,TSSM.PackageName from HREmployee HRE,TrainerSlotSpecializationMapping TSSM where HRE.EmployeeCode=TSSM.TrainerCode and TSSM.BranchCode='{0}'", Trainer.BranchCode));

        //        for (int i = 0; i < dt_Trainer.Rows.Count; i++)
        //        {
        //            TrainersFields SlotsDetails = new TrainersFields { trainerCode = dt_Trainer.Rows[i]["TrainerCode"].ToString(), trainerName = dt_Trainer.Rows[i]["TrainerName"].ToString(), packageCode = dt_Trainer.Rows[i]["PackageCode"].ToString(), packageName = dt_Trainer.Rows[i]["PackageName"].ToString() };
        //            TrainerList.Add(SlotsDetails);
        //        }

        //        topt.status = "success";
        //        topt.value = TrainerList;
        //        sJSONResponse = JsonConvert.SerializeObject(topt);

        //    }
        //    catch (Exception ec)
        //    {
        //        topt.status = "success";

        //        sJSONResponse = JsonConvert.SerializeObject(topt);

        //    }


        //    // string val = json.DataTableToJSONWithStringBuilder(dt_Packageresposne.Tables[0]);

        //    return sJSONResponse;

        //}

        // Trainers Details
        public Object GetTrainerDetails([FromBody]GetTrainerDetailsInput Trainer)
        {
            DataTable dt_Trainer = new DataTable();
            string sJSONResponse = "";
            DataSet dt_Packageresposne = new DataSet();

            GetTrainerDetailsoutput tdopt = new GetTrainerDetailsoutput();
            List<TrainerAttributes> TrainerAttribute = new List<TrainerAttributes>();
            //

            //DataTable dt = getdata(string.Format("select Top 1 TrainerCode,TrainerName,PackageCode,PackageName from TrainerSlotSpecializationMapping where TrainerCode='{0}'  order by cast(SUBSTRING(PackageCode, 1,  case when patindex('%[^0-9]%',PackageCode) > 0 then patindex('%[^0-9]%',PackageCode) - 1 else LEN(PackageCode) end) as int), PackageCode", Trainer.TrainerCode));
            DataTable dt = getdata(string.Format("select distinct CMSSWA.TrainerCode,HRE.EmployeeName as TrainerName from  CMSSlotWiseAllocation CMSSWA,HREmployee HRE where HRE.EmployeeCode=CMSSWA.TrainerCode and CMSSWA.TrainerCode='{0}' ", Trainer.TrainerCode));


            try
            {
                //packages
                TrainerAttributes SlotsDetails = new TrainerAttributes { trainerCode = dt.Rows[0]["TrainerCode"].ToString(), trainerName = dt.Rows[0]["TrainerName"].ToString(), certificates = GetCertificatesDetails(Trainer.TrainerCode), images = GetImageDetails(Trainer.TrainerCode), specialist = GetSpecialistsDetails(Trainer.TrainerCode), videos = GetVideoDetails(Trainer.TrainerCode),packages= GetPackageDetails(Trainer.TrainerCode) };
                TrainerAttribute.Add(SlotsDetails);

                tdopt.status = "success";
                tdopt.value = TrainerAttribute;
                sJSONResponse = JsonConvert.SerializeObject(tdopt);

            }
            catch (Exception ec)
            {
                tdopt.status = "success";

                sJSONResponse = JsonConvert.SerializeObject(tdopt);

            }

            return sJSONResponse;

        }
        public List<Images> GetImageDetails(string TrainerCode)
        {
            List<Images> _Images = new List<Images>();

            DataTable dt = getdata(string.Format("select ImageName,ImageUrl from HREmpImages where EmployeeCode='{0}'", TrainerCode));
            try
            {
                _Images.Add(new Images
                {
                    imageName = Convert.ToString(dt.Rows[0]["ImageName"].ToString())
                                                 ,
                    imageUrl = Convert.ToString(dt.Rows[0]["ImageUrl"].ToString())

                });
            }
            catch (Exception ec)
            {

            }
            return _Images;
        }
        public List<Videos> GetVideoDetails(string TrainerCode)
        {
            List<Videos> _videos = new List<Videos>();

            DataTable dt = getdata(string.Format("select VideoName,VideoUrl from HREmpVideos where EmployeeCode='{0}' ", TrainerCode));
            try
            {
                _videos.Add(new Videos
                {
                    videoName = Convert.ToString(dt.Rows[0]["VideoName"].ToString())
                                                          ,
                    videoUrl = Convert.ToString(dt.Rows[0]["VideoUrl"].ToString())

                });
            }
            catch (Exception ec)
            {

            }
            return _videos;
        }
        public List<Specialist> GetSpecialistsDetails(string TrainerCode)
        {
            List<Specialist> _Specialist = new List<Specialist>();
            DataTable dt = getdata(string.Format("select SpecializationName from HREmpSpecialists where EmployeeCode='{0}'", TrainerCode));
            try
            {
                _Specialist.Add(new Specialist
                {
                    specializationName = Convert.ToString(dt.Rows[0]["SpecializationName"].ToString())

                });
            }
            catch (Exception ec)
            {

            }

            return _Specialist;
        }
        public List<certificates> GetCertificatesDetails(string TrainerCode)
        {
            List<certificates> _certificates = new List<certificates>();
            DataTable dt = getdata(string.Format("select CertificateName,CertificateUrl,UniversityName ,CertificationDate from HREmpCertificates where EmployeeCode='{0}' ", TrainerCode));

            try
            {
                _certificates.Add(new certificates
                {
                    certificatesName = Convert.ToString(dt.Rows[0]["CertificateName"].ToString())
                                                     ,
                    certificateUrl = Convert.ToString(dt.Rows[0]["CertificateUrl"].ToString())
                    ,
                    certificateInstitute = Convert.ToString(dt.Rows[0]["UniversityName"].ToString())
                    ,
                    CertificationDate = Convert.ToString(dt.Rows[0]["CertificationDate"].ToString())

                });
            }
            catch (Exception ec)
            {

            }

            return _certificates;
        }

        public List<TrainerPackages> GetPackageDetails(string TrainerCode)
        {
            List<TrainerPackages> _packages = new List<TrainerPackages>();
            DataTable dt = getdata(string.Format("select distinct CSMA.PackageCode,CMSP.PackageName from CMSSlotWiseAllocation CSMA,CMSPACKAGES CMSP where CSMA.PackageCode=CMSP.PackageCode and CSMA.TrainerCode='{0}' ", TrainerCode));

            try
            {
                _packages.Add(new TrainerPackages
                {
                    packageCode = Convert.ToString(dt.Rows[0]["PackageCode"].ToString())
                                                     ,
                    packageName = Convert.ToString(dt.Rows[0]["PackageName"].ToString())

                });
            }
            catch (Exception ec)
            {

            }

            return _packages;
        }

        // Online
        // Trainers List
        //28/08/20: Task - Need to add Pervioustrainerdetailsobject column(TrainerCode,TrainerName,TrainerImage),TrainerListObject(same columns as pervioustrainer)
        public Object GetOnlineTrainersList([FromBody]OnlineGetTrainerInput Trainer)
        {

            DataTable dt_Trainer = new DataTable();
            string sJSONResponse = "";
            DataSet dt_Packageresposne = new DataSet();

            OnlineGetTraineroutput topt = new OnlineGetTraineroutput();
            List<OnlineTrainerObjects> TrainerObjects = new List<OnlineTrainerObjects>();

            try
            {
                dt_Trainer = getdata(string.Format("select distinct HRE.EmployeeCode as TrainerCode,HRE.EmployeeName as TrainerName from HREmployee HRE,OnlineTrainersSlotMapping OLTSM where OLTSM.TrainerCode=HRE.EmployeeCode and OLTSM.IsDeleted=0", ""));

                OnlineTrainerObjects OTO = new OnlineTrainerObjects { previousTrainers = PerviousTrainerDetailsObject(Trainer.MobileNo), trainersList = TrainerListObject(Trainer.MobileNo) };
                TrainerObjects.Add(OTO);

                topt.status = "success";
                topt.value = TrainerObjects;
                sJSONResponse = JsonConvert.SerializeObject(topt);

            }
            catch (Exception ec)
            {
                topt.status = "success";

                sJSONResponse = JsonConvert.SerializeObject(topt);

            }


            // string val = json.DataTableToJSONWithStringBuilder(dt_Packageresposne.Tables[0]);

            return sJSONResponse;

        }
        public List<PerviousTrainerDetailsObject> PerviousTrainerDetailsObject(string MobileNo)
        {
            List<PerviousTrainerDetailsObject> _PerviousTrainerDetailsObject = new List<PerviousTrainerDetailsObject>();

            DataTable dt = getdata(string.Format("select top 1 OLPU.TrainerID as TrainerCode,HRE.EmployeeName as TrainerName,HRE.PhotoURL as Image  from OnlinePackageUsed OLPU,HREmployee HRE where OLPU.TrainerID=HRE.EmployeeCode and OLPU.MobileNo='" + MobileNo + "'  order by OLPU.ID desc", MobileNo));

            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                _PerviousTrainerDetailsObject.Add(new PerviousTrainerDetailsObject
                {
                    trainerCode = Convert.ToString(dt.Rows[i]["TrainerCode"].ToString())
                                                 ,
                    trainerName = Convert.ToString(dt.Rows[i]["TrainerName"].ToString())
                     ,
                    trainerImage = Convert.ToString(dt.Rows[i]["Image"].ToString())

                });
            }
            return _PerviousTrainerDetailsObject;
        }
        public List<TrainerListObject> TrainerListObject(string MobileNo)
        {
            List<TrainerListObject> _TrainerListObject = new List<TrainerListObject>();

            DataTable dt = getdata(string.Format("select distinct HRE.EmployeeCode as TrainerCode,HRE.EmployeeName as TrainerName,HRE.PhotoURL as Image from HREmployee HRE,OnlineTrainersSlotMapping OLTSM where OLTSM.TrainerCode=HRE.EmployeeCode and OLTSM.IsDeleted=0 and OLTSM.TrainerCode not in(select top 1 OLPU.TrainerID as TrainerCode  from OnlinePackageUsed OLPU,HREmployee HRE where OLPU.TrainerID=HRE.EmployeeCode and MobileNo='" + MobileNo + "'  order by OLPU.ID desc)", MobileNo));

            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                _TrainerListObject.Add(new TrainerListObject
                {
                    trainerCode = Convert.ToString(dt.Rows[i]["TrainerCode"].ToString())
                                                 ,
                    trainerName = Convert.ToString(dt.Rows[i]["TrainerName"].ToString())
                     ,
                    trainerImage = Convert.ToString(dt.Rows[i]["Image"].ToString())

                });
            }
            return _TrainerListObject;
        }

        // Trainers Details
        public Object GetOnlineTrainerDetails([FromBody]OnlineGetTrainerDetailsInput Trainer)
        {
            DataTable dt_Trainer = new DataTable();
            string sJSONResponse = "";
            DataSet dt_Packageresposne = new DataSet();

            OnlineGetTrainerDetailsoutput tdopt = new OnlineGetTrainerDetailsoutput();
            List<OnlineTrainerAttributes> TrainerAttribute = new List<OnlineTrainerAttributes>();

            DataTable dt = getdata(string.Format("select Top 1 TrainerCode,TrainerName,PackageCode,PackageName from OnlineTrainersSlotMapping where TrainerCode='170309' order by PackageCode desc", Trainer.TrainerCode));
            try
            {

                OnlineTrainerAttributes SlotsDetails = new OnlineTrainerAttributes { trainerCode = dt.Rows[0]["TrainerCode"].ToString(), trainerName = dt.Rows[0]["TrainerName"].ToString(), packageCode = dt.Rows[0]["PackageCode"].ToString(), packageName = dt.Rows[0]["PackageName"].ToString(), certificates = GetOnlineCertificatesDetails(Trainer.TrainerCode), images = GetOnlineImageDetails(Trainer.TrainerCode), specialist = GetOnlineSpecialistsDetails(Trainer.TrainerCode), videos = GetOnlineVideoDetails(Trainer.TrainerCode) };
                TrainerAttribute.Add(SlotsDetails);

                tdopt.status = "success";
                tdopt.value = TrainerAttribute;
                sJSONResponse = JsonConvert.SerializeObject(tdopt);

            }
            catch (Exception ec)
            {
                tdopt.status = "success";

                sJSONResponse = JsonConvert.SerializeObject(tdopt);

            }


            // string val = json.DataTableToJSONWithStringBuilder(dt_Packageresposne.Tables[0]);

            return sJSONResponse;

        }
        public List<OnlineImages> GetOnlineImageDetails(string TrainerCode)
        {
            List<OnlineImages> _Images = new List<OnlineImages>();

            DataTable dt = getdata(string.Format("select ImageName,ImageUrl from HREmpImages where EmployeeCode='170309'", TrainerCode));

            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                _Images.Add(new OnlineImages
                {
                    imageName = Convert.ToString(dt.Rows[i]["ImageName"].ToString())
                                                 ,
                    imageUrl = Convert.ToString(dt.Rows[i]["ImageUrl"].ToString())

                });
            }
            return _Images;
        }
        public List<OnlineVideos> GetOnlineVideoDetails(string TrainerCode)
        {
            List<OnlineVideos> _videos = new List<OnlineVideos>();

            DataTable dt = getdata(string.Format("select VideoName,VideoUrl from HREmpVideos where EmployeeCode='170309' ", TrainerCode));

            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                _videos.Add(new OnlineVideos
                {
                    videoName = Convert.ToString(dt.Rows[i]["VideoName"].ToString())
                                                  ,
                    videoUrl = Convert.ToString(dt.Rows[i]["VideoUrl"].ToString())

                });
            }
            return _videos;
        }
        public List<OnlineSpecialist> GetOnlineSpecialistsDetails(string TrainerCode)
        {
            List<OnlineSpecialist> _Specialist = new List<OnlineSpecialist>();
            DataTable dt = getdata(string.Format("select ImageName,ImageUrl from HREmpImages where EmployeeCode='170309'", TrainerCode));
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                _Specialist.Add(new OnlineSpecialist
                {
                    specializationName = Convert.ToString(dt.Rows[i]["ImageName"].ToString())

                });
            }

            return _Specialist;
        }
        public List<Onlinecertificates> GetOnlineCertificatesDetails(string TrainerCode)
        {
            List<Onlinecertificates> _certificates = new List<Onlinecertificates>();
            DataTable dt = getdata(string.Format("select CertificatesName,CertificateUrl,CertificateInstitute,Completion from HREmployeeCertificates where EmployeeCode='170309' ", TrainerCode));
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                _certificates.Add(new Onlinecertificates
                {
                    certificatesName = Convert.ToString(dt.Rows[i]["CertificatesName"].ToString())
                                                 ,
                    certificateUrl = Convert.ToString(dt.Rows[i]["CertificateUrl"].ToString())
                ,
                    certificateInstitute = Convert.ToString(dt.Rows[i]["CertificateInstitute"].ToString())
                ,
                    completion = Convert.ToString(dt.Rows[i]["Completion"].ToString())

                });
            }

            return _certificates;
        }
        public Object GetTrainersListBranchwise([FromBody]OffLineGetTrainerInput Trainer)

        {
            TrainerlistOutput daOP = new TrainerlistOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Trainers = new DataTable();

            List<TrainerlistResponse> dalots = new List<TrainerlistResponse>();

            try
            {
                dt_Trainers = getdata(string.Format("select distinct HRE.EmployeeCode as TrainerCode,HRE.EmployeeName as TrainerName,HRES.GradeName as PackageName,HRE.PhotoURL from HREmployee HRE,CMSSlotWiseAllocation CMSSWA,EmployeeBranches EB,HREmpSpecialists HRES where HRES.EmployeeCode=HRE.EmployeeCode and HRES.EmployeeCode=CMSSWA.TrainerCode and  CMSSWA.TrainerCode=HRE.EmployeeCode and CMSSWA.IsDeleted=0 and CMSSWA.IsActive=1 and EB.EmployeeCode=HRE.EmployeeCode and HRE.EmployeeCode!=100001 and CMSSWA.BranchCode=EB.BranchCode and EB.BranchCode='{0}' ", Trainer.BranchCode));

                for (int i = 0; i < dt_Trainers.Rows.Count; i++)
                {
                    TrainerlistResponse SlotsDetails = new TrainerlistResponse { trainerCode = dt_Trainers.Rows[i]["TrainerCode"].ToString(), trainerName = dt_Trainers.Rows[i]["TrainerName"].ToString(), packageName = dt_Trainers.Rows[i]["PackageName"].ToString(), imageUrl = dt_Trainers.Rows[i]["PhotoURL"].ToString() };
                    dalots.Add(SlotsDetails);
                }

                daOP.status = "success";
                daOP.value = dalots;
                sJSONResponse = JsonConvert.SerializeObject(daOP);


            }
            catch (Exception ec)
            {
                daOP.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(daOP);
            }


            return sJSONResponse;
        }




    }
}