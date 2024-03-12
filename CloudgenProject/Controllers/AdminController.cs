﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http.Routing;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using CloudgenProject.Models.admin;
using System.IO;
using OfficeOpenXml;
using System.Net.Mail;
using System.Net;
using System.Web.Helpers;
using Microsoft.Ajax.Utilities;

namespace CloudgenProject.Controllers
{
    public class AdminController : Controller
    {
         public SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myconn"].ConnectionString);
        Models.admin.DataServices db = new Models.admin.DataServices();
        // GET: Admin

        #region dashboard
        public ActionResult Dashboard()
        {
            CloudgenProject.App_Code.basic_function basic_function = new CloudgenProject.App_Code.basic_function();
            if (basic_function.adminssioncheck("") == false)
            {
                string url = Request.Url.PathAndQuery;
                return Redirect("/admin/login?url=" + HttpUtility.UrlEncode(url) + "");
            }
            List<CloudgenProject.Models.admin.leadstatusmodel> leaddetails = new List<CloudgenProject.Models.admin.leadstatusmodel>();
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_leadresponse", con);
            cmd.CommandType = CommandType.StoredProcedure; 
            cmd.Parameters.AddWithValue("@Action", "viewLeadstatusForadmin");
            SqlDataReader sdr = cmd.ExecuteReader(); 

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    leaddetails.Add(new CloudgenProject.Models.admin.leadstatusmodel()
                    {
                        leadId = sdr["LeadId"].ToString(),
                        response = sdr["response"].ToString(),
                        status = sdr["status"].ToString(),
                        next_follow_up_date = sdr["next_follow_up_date"].ToString(),
                        client = sdr["client_name"].ToString(),
                        ContactPerson = sdr["ContactPerson"].ToString(),
                        contac_no = sdr["contact_no"].ToString(), // corrected variable name
                        contactemail = sdr["eMAIL_ID"].ToString(), // corrected variable name
                        product = sdr["productname"].ToString(),
                        assignTo = sdr["assignedToName"].ToString(),
                        assignToId = sdr["assignTo"].ToString(),
                        assignby = sdr["assignedByName"].ToString(),
                        assignstatus = sdr["assignstatus"].ToString(),
                        LeadStatus = sdr["leadstatus"].ToString(),
                        generatestatus = sdr["generateStatus"].ToString(),
                    });
                }
            }

            ViewBag.list = leaddetails;
            var logintype = Session["usertype"].ToString();
           

            var hh = db.total_count_admin(logintype);
            return View(hh);
        }

        public JsonResult FollowUpByDate(string date)
        {
            List<CloudgenProject.Models.admin.leadstatusmodel> leaddetails = new List<CloudgenProject.Models.admin.leadstatusmodel>();
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_leadresponse", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "followuplistbyDateForadmin");
            cmd.Parameters.AddWithValue("@next_follow_up_date", date);
            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    leaddetails.Add(new CloudgenProject.Models.admin.leadstatusmodel()
                    {
                        leadId = sdr["LeadId"].ToString(),
                        response = sdr["response"].ToString(),
                        status = sdr["status"].ToString(),
                        next_follow_up_date = sdr["next_follow_up_date"].ToString(),
                        client = sdr["client_name"].ToString(),
                        ContactPerson = sdr["ContactPerson"].ToString(),
                        contac_no = sdr["contact_no"].ToString(), // corrected variable name
                        contactemail = sdr["eMAIL_ID"].ToString(), // corrected variable name
                        product = sdr["productname"].ToString(),
                        assignTo = sdr["assignedToName"].ToString(),
                        assignby = sdr["assignedByName"].ToString(),
                        assignstatus = sdr["assignstatus"].ToString(),
                    });
                }
            }


            return Json(leaddetails, JsonRequestBehavior.AllowGet);

        }


        public ActionResult sale_dashboard()
        {
            CloudgenProject.App_Code.basic_function basic_function = new CloudgenProject.App_Code.basic_function();
            if (basic_function.adminssioncheck("") == false)
            {
                string url = Request.Url.PathAndQuery;
                return Redirect("/admin/login?url=" + HttpUtility.UrlEncode(url) + "");
            }
            var loginId = Session["employeeId"].ToString();

            List<CloudgenProject.Models.admin.leadstatusmodel> leaddetails = new List<CloudgenProject.Models.admin.leadstatusmodel>();
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_leadresponse", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "viewLeadstatusForsales");
            cmd.Parameters.AddWithValue("@assignedTo",loginId);
            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    leaddetails.Add(new CloudgenProject.Models.admin.leadstatusmodel()
                    {
                       // leadId = sdr["LeadId"].ToString(),
                        leadId = sdr["id"].ToString(),
                        response = sdr["response"].ToString(),
                        status = sdr["status"].ToString(),
                        next_follow_up_date = sdr["next_follow_up_date"].ToString(),
                        client = sdr["client_name"].ToString(),
                        ContactPerson = sdr["ContactPerson"].ToString(),
                        contac_no = sdr["contact_no"].ToString(), // corrected variable name
                        contactemail = sdr["eMAIL_ID"].ToString(), // corrected variable name
                        product = sdr["productname"].ToString(),
                        assignTo = sdr["assignedToName"].ToString(),
                        assignToId = sdr["assignTo"].ToString(),
                        assignby = sdr["assignedByName"].ToString(),
                        assignstatus = sdr["assignstatus"].ToString(),
                        LeadStatus = sdr["leadstatus"].ToString(),
                        generatestatus = sdr["generateStatus"].ToString(),
                    });
                }
            }

            ViewBag.list = leaddetails;

            var logintype = Session["usertype"].ToString();

            var hh = db.total_count_sales(logintype, loginId);
            return View(hh);
        }

        public JsonResult SalesFollowUpByDate(string date)
        {
            var loginId = Session["employeeId"].ToString();
            List<CloudgenProject.Models.admin.leadstatusmodel> leaddetails = new List<CloudgenProject.Models.admin.leadstatusmodel>();
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_leadresponse", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "followuplistbyDateForsales");
            cmd.Parameters.AddWithValue("@next_follow_up_date", date);
            cmd.Parameters.AddWithValue("@assignedTo", loginId);
            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    leaddetails.Add(new CloudgenProject.Models.admin.leadstatusmodel()
                    {
                        leadId = sdr["id"].ToString(),
                        response = sdr["response"].ToString(),
                        status = sdr["status"].ToString(),
                        next_follow_up_date = sdr["next_follow_up_date"].ToString(),
                        client = sdr["client_name"].ToString(),
                        ContactPerson = sdr["ContactPerson"].ToString(),
                        contac_no = sdr["contact_no"].ToString(), // corrected variable name
                        contactemail = sdr["eMAIL_ID"].ToString(), // corrected variable name
                        product = sdr["productname"].ToString(),
                        assignTo = sdr["assignedToName"].ToString(),
                        assignby = sdr["assignedByName"].ToString(),
                        assignstatus = sdr["assignstatus"].ToString(),
                    });
                }
            }


            return Json(leaddetails, JsonRequestBehavior.AllowGet);

        }


        public ActionResult agent_dashboard()
        {
            CloudgenProject.App_Code.basic_function basic_function = new CloudgenProject.App_Code.basic_function();
            if (basic_function.adminssioncheck("") == false)
            {
                string url = Request.Url.PathAndQuery;
                return Redirect("/admin/login?url=" + HttpUtility.UrlEncode(url) + "");
            }
            var logintype = Session["usertype"].ToString();
            var loginId = Session["employeeId"].ToString();

        

            List<CloudgenProject.Models.admin.leadstatusmodel> leaddetails = new List<CloudgenProject.Models.admin.leadstatusmodel>();
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_leadresponse", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "viewLeadstatusForagent");
            cmd.Parameters.AddWithValue("@assignedTo", loginId);
            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    leaddetails.Add(new CloudgenProject.Models.admin.leadstatusmodel()
                    {
                        // leadId = sdr["LeadId"].ToString(),
                        leadId = sdr["id"].ToString(),
                        response = sdr["response"].ToString(),
                        status = sdr["status"].ToString(),
                        next_follow_up_date = sdr["next_follow_up_date"].ToString(),
                        client = sdr["client_name"].ToString(),
                        ContactPerson = sdr["ContactPerson"].ToString(),
                        contac_no = sdr["contact_no"].ToString(), // corrected variable name
                        contactemail = sdr["eMAIL_ID"].ToString(), // corrected variable name
                        product = sdr["productname"].ToString(),
                        assignTo = sdr["assignedToName"].ToString(),
                        assignToId = sdr["assignTo"].ToString(),
                        assignby = sdr["assignedByName"].ToString(),
                        assignstatus = sdr["assignstatus"].ToString(),
                        LeadStatus = sdr["leadstatus"].ToString(),
                        generatestatus = sdr["generateStatus"].ToString(),
                    });
                }
            }

            ViewBag.list = leaddetails;



            var hh = db.total_count_agent(logintype, loginId);
            return View(hh);
        }

        public JsonResult AgentFollowUpByDate(string date)
        {
            var loginId = Session["employeeId"].ToString();
            List<CloudgenProject.Models.admin.leadstatusmodel> leaddetails = new List<CloudgenProject.Models.admin.leadstatusmodel>();
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_leadresponse", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "followuplistbyDateForagent");
            cmd.Parameters.AddWithValue("@next_follow_up_date", date);
            cmd.Parameters.AddWithValue("@assignedTo", loginId);
            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                while (sdr.Read())
                {
                    leaddetails.Add(new CloudgenProject.Models.admin.leadstatusmodel()
                    {
                        // leadId = sdr["LeadId"].ToString(),
                        leadId = sdr["id"].ToString(),
                        response = sdr["response"].ToString(),
                        status = sdr["status"].ToString(),
                        next_follow_up_date = sdr["next_follow_up_date"].ToString(),
                        client = sdr["client_name"].ToString(),
                        ContactPerson = sdr["ContactPerson"].ToString(),
                        contac_no = sdr["contact_no"].ToString(), // corrected variable name
                        contactemail = sdr["eMAIL_ID"].ToString(), // corrected variable name
                        product = sdr["productname"].ToString(),
                        assignTo = sdr["assignedToName"].ToString(),
                        assignToId = sdr["assignTo"].ToString(),
                        assignby = sdr["assignedByName"].ToString(),
                        assignstatus = sdr["assignstatus"].ToString(),
                        LeadStatus = sdr["leadstatus"].ToString(),
                        generatestatus = sdr["generateStatus"].ToString(),
                    });
                }
            }

            ViewBag.list = leaddetails;
            return Json(leaddetails, JsonRequestBehavior.AllowGet);

        }
        #endregion

        public ActionResult Login()
        {
            return View();
        }

        #region checklogin

        [HttpPost]
        public JsonResult admin_Login(string username, string password, string url)
        {

            CloudgenProject.App_Code.basic_function basic_function = new CloudgenProject.App_Code.basic_function();


            Models.common_response Response = basic_function.login(username, password);
            if (Response.success == true)
            {
                if (url != null && url.ToString() != "")
                {
                    Response.message = (HttpUtility.HtmlDecode(url));
                }
                else
                {

                    Session["usertype"] = Response.usertype.ToString();
                    Session["employeeId"] = Response.employeeId.ToString();
                    Session["Id"] = Response.Id.ToString();
                    Session["username"] = Response.username.ToString();
                    Session["loginuseremail"] = Response.useremail.ToString();
                    Session["loginuserbranch"] = Response.employee_branch.ToString();

                    if (Response.usertype == "admin")
                    {
                        Response.message = "/Admin/Dashboard";

                    }

                    if (Response.usertype == "agent")
                    {
                        Response.message = "/admin/agent_dashboard";

                    }

                    if (Response.usertype == "sales")
                    {
                        Response.message = "/admin/sale_dashboard";

                    }


                }
                Session["adminname"] = Response.parameter.ToString();

            }
            return Json(Response);
        }

        #endregion

        #region  admin info
        //[ChildActionOnly]
        //public ActionResult topbar()
        //{
        //    macreel_setup.App_Code.Admin.basic_function basic_function = new macreel_setup.App_Code.Admin.basic_function();
        //    macreel_setup.Models.admininfo admininfo = new macreel_setup.Models.admininfo();
        //    string agencyid = "";
        //    if (Session["adminname"] != null && Session["adminname"].ToString() != "")
        //    {
        //        agencyid = Session["adminname"].ToString();
        //    }

        //    admininfo = basic_function.admininfo(agencyid);
        //    return PartialView(admininfo);

        //}
        #endregion

        #region logout

        public ActionResult logout()
        {
            if (Session["adminname"] != null)
            {
                Session["adminname"] = null;
                Session["data_con"] = null;
            }


            Session["usertype"] = null;
            Session["employeeId"] = null;
            Session["username"] = null;
            Session.Abandon();

            return RedirectToAction("login");
        }
        #endregion

        #region Manage Department

        public ActionResult department_management(string id)
        {
            //CloudgenProject.App_Code.basic_function basic_function = new CloudgenProject.App_Code.basic_function();
            //if (basic_function.adminssioncheck("") == false)
            //{
            //    string url = Request.Url.PathAndQuery;
            //    return Redirect("/admin/login?url=" + HttpUtility.UrlEncode(url) + "");
            //}

            CloudgenProject.Models.admin.manage_department manage_department = new CloudgenProject.Models.admin.manage_department();


            if (id != null && id.ToString() != "")
            {
                string strr = "select * from department_master where id='" + id + "'";
                DataTable dt = SqlHelper.ExecuteDataset(CommandType.Text, strr).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    manage_department.id = Convert.ToInt32(dt.Rows[0]["id"]);
                    manage_department.department = dt.Rows[0]["department"].ToString();
                }
            }
          
                List<CloudgenProject.Models.admin.manage_department> department_list = new List<CloudgenProject.Models.admin.manage_department>();

                string sql = "select * from department_master order by id";

                DataTable dtr = SqlHelper.ExecuteDataset(CommandType.Text, sql).Tables[0];
                if (dtr.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtr.Rows)
                    {
                        department_list.Add(new CloudgenProject.Models.admin.manage_department()
                        {
                            department = dr["department"].ToString(),
                            id = Convert.ToInt32(dr["id"]),

                        });
                    }
                }
            ViewBag.list = department_list;
            return View(manage_department);

        }

        [HttpPost]
        public ActionResult insert_department(CloudgenProject.Models.admin.manage_department manage_department)
        {
            SqlCommand cmd = new SqlCommand("sp_manage_department", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", manage_department.id);
            cmd.Parameters.AddWithValue("@department", manage_department.department);


            if (manage_department.id != 0)
            {
                cmd.Parameters.AddWithValue("@action", "update");
            }
            else
            {
                cmd.Parameters.AddWithValue("@action", "insert");

            }

            if (con.State == ConnectionState.Closed)
                con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
            {
                return RedirectToAction("department_management");
            }
            else
            {
                return RedirectToAction("department_management");
            }
        }

        public ActionResult delete_department(string id)
        {
            string str = "delete from  department_master where id='" + id + "'";

            SqlHelper.ExecuteNonQuery(CommandType.Text, str);

            return RedirectToAction("department_management");
        }

        #endregion

        #region Manage Designation

        public ActionResult designation_management(string id)
        {
            //CloudgenProject.App_Code.basic_function basic_function = new CloudgenProject.App_Code.basic_function();
            //if (basic_function.adminssioncheck("") == false)
            //{
            //    string url = Request.Url.PathAndQuery;
            //    return Redirect("/admin/login?url=" + HttpUtility.UrlEncode(url) + "");
            //}

            CloudgenProject.Models.admin.manage_designation manage_designation = new CloudgenProject.Models.admin.manage_designation();


            if (id != null && id.ToString() != "")
            {
                string strr = "select * from designation_master where id='" + id + "'";
                DataTable dt = SqlHelper.ExecuteDataset(CommandType.Text, strr).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    manage_designation.id = Convert.ToInt32(dt.Rows[0]["id"]);
                    manage_designation.designation = dt.Rows[0]["designation"].ToString();
                }
            }

            List<CloudgenProject.Models.admin.manage_designation> designation_list = new List<CloudgenProject.Models.admin.manage_designation>();

            string sql = "select * from designation_master order by id";

            DataTable dtr = SqlHelper.ExecuteDataset(CommandType.Text, sql).Tables[0];
            if (dtr.Rows.Count > 0)
            {
                foreach (DataRow dr in dtr.Rows)
                {
                    designation_list.Add(new CloudgenProject.Models.admin.manage_designation()
                    {
                        designation = dr["designation"].ToString(),
                        id = Convert.ToInt32(dr["id"]),

                    });
                }
            }
            ViewBag.list = designation_list;
            return View(manage_designation);

        }

        [HttpPost]
        public ActionResult insert_designation(CloudgenProject.Models.admin.manage_designation manage_designation)
        {
            SqlCommand cmd = new SqlCommand("sp_manage_designation", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", manage_designation.id);
            cmd.Parameters.AddWithValue("@designation", manage_designation.designation);


            if (manage_designation.id != 0)
            {
                cmd.Parameters.AddWithValue("@action", "update");
            }
            else
            {
                cmd.Parameters.AddWithValue("@action", "insert");

            }

            if (con.State == ConnectionState.Closed)
                con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
            {
                return RedirectToAction("designation_management");
            }
            else
            {
                return RedirectToAction("designation_management");
            }
        }

        public ActionResult delete_designation(string id)
        {
            string str = "delete from  designation_master where id='" + id + "'";

            SqlHelper.ExecuteNonQuery(CommandType.Text, str);

            return RedirectToAction("designation_management");
        }

        #endregion

        #region Manage Branch

        public ActionResult branch_management(string id)
        {
            //CloudgenProject.App_Code.basic_function basic_function = new CloudgenProject.App_Code.basic_function();
            //if (basic_function.adminssioncheck("") == false)
            //{
            //    string url = Request.Url.PathAndQuery;
            //    return Redirect("/admin/login?url=" + HttpUtility.UrlEncode(url) + "");
            //}

            CloudgenProject.Models.admin.manage_branch manage_branch = new CloudgenProject.Models.admin.manage_branch();


            if (id != null && id.ToString() != "")
            {
                string strr = "select * from manage_branch where id='" + id + "'";
                DataTable dt = SqlHelper.ExecuteDataset(CommandType.Text, strr).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    manage_branch.id = dt.Rows[0]["id"].ToString();
                    manage_branch.BranchName = dt.Rows[0]["BranchName"].ToString();
                    manage_branch.BranchEmail = dt.Rows[0]["BranchEmail"].ToString();
                    manage_branch.ContactPerson = dt.Rows[0]["ContactPerson"].ToString();
                    manage_branch.LandLineNo = dt.Rows[0]["LandLineNo"].ToString();
                    manage_branch.ContactPersonMobile = dt.Rows[0]["ContactPersonMobile"].ToString();
                    manage_branch.ContactPersonEmail = dt.Rows[0]["ContactPersonEmail"].ToString();
                    manage_branch.GSTNo = dt.Rows[0]["GSTNo"].ToString();
                    manage_branch.PanNo = dt.Rows[0]["PanNo"].ToString();
                    manage_branch.BranchRegistrationNo = dt.Rows[0]["BranchRegistrationNo"].ToString();
                    manage_branch.StateName = dt.Rows[0]["StateName"].ToString();
                    manage_branch.StateCode = dt.Rows[0]["StateCode"].ToString();
                    manage_branch.CityName = dt.Rows[0]["CityName"].ToString();
                    manage_branch.CityCode = dt.Rows[0]["CityCode"].ToString();
                    manage_branch.PinCode = dt.Rows[0]["PinCode"].ToString();
                    manage_branch.RegistrationDate = dt.Rows[0]["RegistrationDate"].ToString();
                    manage_branch.BranchAddress = dt.Rows[0]["BranchAddress"].ToString();

                }
            }


            return View(manage_branch);

        }

        [HttpPost]
        public ActionResult insert_branch(CloudgenProject.Models.admin.manage_branch manage_branch)
        {
            CloudgenProject.App_Code.basic_function basic_function = new CloudgenProject.App_Code.basic_function();

            Models.common_response Response = basic_function.manage_branch(manage_branch);



            return RedirectToAction("branch_management");
        }

        public ActionResult view_branch()
        {
            List<CloudgenProject.Models.admin.manage_branch> manage_branch = new List<CloudgenProject.Models.admin.manage_branch>();

            string sql = "select * from manage_branch order by id";

            DataTable dtr = SqlHelper.ExecuteDataset(CommandType.Text, sql).Tables[0];
            if (dtr.Rows.Count > 0)
            {
                foreach (DataRow dr in dtr.Rows)
                {
                    manage_branch.Add(new CloudgenProject.Models.admin.manage_branch()
                    {
                        BranchName = dr["BranchName"].ToString(),
                        LandLineNo = dr["LandLineNo"].ToString(),
                        CityName = dr["CityName"].ToString(),
                        ContactPerson = dr["ContactPerson"].ToString(),
                        ContactPersonMobile = dr["ContactPersonMobile"].ToString(),
                        id = dr["id"].ToString(),

                    });
                }
            }

            return View(manage_branch);
        }

        public ActionResult delete_branch(string id)
        {
            string str = "delete from manage_branch where id='" + id + "'";

            SqlHelper.ExecuteNonQuery(CommandType.Text, str);

            return RedirectToAction("view_branch");
        }


        #endregion

        #region employee management field
        public ActionResult employee_management(string id)
        {
            //CloudgenProject.App_Code.basic_function basic_function = new CloudgenProject.App_Code.basic_function();
            //if (basic_function.adminssioncheck("") == false)
            //{
            //    string url = Request.Url.PathAndQuery;    
            //    return Redirect("/admin/login?url=" + HttpUtility.UrlEncode(url) + "");
            //}

            CloudgenProject.Models.admin.employee_manage employee_manage = new CloudgenProject.Models.admin.employee_manage();
            List<CloudgenProject.Models.admin.manage_branch> manage_branch = new List<CloudgenProject.Models.admin.manage_branch>();
            List<CloudgenProject.Models.admin.manage_designation> manage_designation = new List<CloudgenProject.Models.admin.manage_designation>();
            List<CloudgenProject.Models.admin.manage_department> manage_department = new List<CloudgenProject.Models.admin.manage_department>();
            List<CloudgenProject.Models.admin.reportingTL_list> reportingTL_list = new List<CloudgenProject.Models.admin.reportingTL_list>();
            List<CloudgenProject.Models.admin.reporting_manger_list> reporting_manger_list = new List<CloudgenProject.Models.admin.reporting_manger_list>();

            countrybind();

            string str = "select * from manage_branch     select * from employee_management e left outer join designation_master d on e.employee_designation = d.id where d.designation = 'Team Leader' and current_status='active'    select * from employee_management e left outer join designation_master d on e.employee_designation = d.id where d.designation = 'Management' and current_status='active'   select * from designation_master       select * from  department_master";
            DataSet ds = SqlHelper.ExecuteDataset(CommandType.Text, str);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    manage_branch.Add(new CloudgenProject.Models.admin.manage_branch()
                    {
                        BranchName = dr["BranchName"].ToString(),
                        id = dr["id"].ToString()
                    });
                }
            }

            if (ds.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    reportingTL_list.Add(new CloudgenProject.Models.admin.reportingTL_list()
                    {
                        name = dr["employee_first_name"].ToString() + dr["employee_last_name"].ToString(),
                        employee_id = dr["employee_id"].ToString()
                    });
                }
            }

            if (ds.Tables[2].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[2].Rows)
                {
                    reporting_manger_list.Add(new CloudgenProject.Models.admin.reporting_manger_list()
                    {
                        name = dr["employee_first_name"].ToString() + dr["employee_last_name"].ToString(),
                        employee_id = dr["employee_id"].ToString()
                    });
                }
            }

            if (ds.Tables[3].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[3].Rows)
                {
                    manage_designation.Add(new CloudgenProject.Models.admin.manage_designation()
                    {
                        designation = dr["designation"].ToString(),
                        id = Convert.ToInt32(dr["id"])
                    });
                }
            }

            if (ds.Tables[4].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[4].Rows)
                {
                    manage_department.Add(new CloudgenProject.Models.admin.manage_department()
                    {
                        department = dr["department"].ToString(),
                        id = Convert.ToInt32(dr["id"])
                    });
                }
            }

            employee_manage.manage_branch = manage_branch;
            employee_manage.reportingTL_list = reportingTL_list;
            employee_manage.reporting_manger_list = reporting_manger_list;
            employee_manage.department_list = manage_department;
            employee_manage.designation_list = manage_designation;


            if (id != null && id.ToString() != "")
            {
                string strr = "select * from employee_management where id='" + id + "'";
                DataTable dt = SqlHelper.ExecuteDataset(CommandType.Text, strr).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    employee_manage.id = dt.Rows[0]["id"].ToString();
                    employee_manage.employee_branch = dt.Rows[0]["employee_branch"].ToString();
                    employee_manage.employee_type = dt.Rows[0]["employee_type"].ToString();
                    employee_manage.designation = dt.Rows[0]["employee_designation"].ToString();
                    employee_manage.employee_id = dt.Rows[0]["employee_id"].ToString();
                    employee_manage.login_userid = dt.Rows[0]["login_userid"].ToString();
                    employee_manage.password = dt.Rows[0]["password"].ToString();
                    employee_manage.employee_first_name = dt.Rows[0]["employee_first_name"].ToString();
                    employee_manage.employee_last_name = dt.Rows[0]["employee_last_name"].ToString();
                    employee_manage.mobile_no = dt.Rows[0]["mobile_no"].ToString();
                    employee_manage.phone_no = dt.Rows[0]["phone_no"].ToString();
                    employee_manage.gender = dt.Rows[0]["gender"].ToString();
                    employee_manage.alternate_number = dt.Rows[0]["alternate_number"].ToString();
                    employee_manage.dob = dt.Rows[0]["dob"].ToString();
                    employee_manage.join_date = dt.Rows[0]["join_date"].ToString();
                    employee_manage.salary_ctc = dt.Rows[0]["salary_ctc"].ToString();
                    employee_manage.qualification = dt.Rows[0]["qualification"].ToString();
                    employee_manage.aadhar_no = dt.Rows[0]["aadhar_no"].ToString();
                    employee_manage.pan_no = dt.Rows[0]["pan_no"].ToString();
                    employee_manage.profile_picture = dt.Rows[0]["profile_picture"].ToString();
                    employee_manage.country = dt.Rows[0]["country"].ToString();
                    employee_manage.state = dt.Rows[0]["state"].ToString();
                    employee_manage.city = dt.Rows[0]["city"].ToString();
                    employee_manage.area_code = dt.Rows[0]["area_code"].ToString();
                    employee_manage.address = dt.Rows[0]["address"].ToString();
                    employee_manage.department = dt.Rows[0]["employee_department"].ToString();
                    employee_manage.reporting_manager = dt.Rows[0]["reporting_manager"].ToString();
                    employee_manage.reporting_tl = dt.Rows[0]["reporting_tl"].ToString();
                    employee_manage.exit_date = dt.Rows[0]["exit_date"].ToString();
                    employee_manage.current_status = dt.Rows[0]["current_status"].ToString();

                }
            }
            return View(employee_manage);

        }

        [HttpPost]
        public ActionResult insert_employee(CloudgenProject.Models.admin.employee_manage employee_manage)
        {

            var path = System.IO.Path.Combine(Server.MapPath("~/tempimage/"));


            HttpPostedFileBase file1 = Request.Files["fileupload"];

            string img1 = "";

            string uploadpayslipss;

            if (employee_manage.profile_picture != null && employee_manage.profile_picture != "")
            {
                img1 = employee_manage.profile_picture;
            }

            if (file1 != null && file1.FileName.ToString() != "")
            {
                uploadpayslipss = DateTime.Now.ToString("ssMMHHmmyyyydd") + System.Guid.NewGuid() + "." + file1.FileName.Split('.')[1];
                file1.SaveAs(path + uploadpayslipss);
                img1 = uploadpayslipss;

            }

            SqlParameter[] para = new SqlParameter[30];

            para[0] = new SqlParameter("@employee_branch", SqlDbType.NVarChar);
            para[0].Value = employee_manage.employee_branch;

            para[1] = new SqlParameter("@employee_type", SqlDbType.NVarChar);
            para[1].Value = employee_manage.employee_type;

            para[2] = new SqlParameter("@employee_designation", SqlDbType.NVarChar);
            para[2].Value = employee_manage.designation;

            para[3] = new SqlParameter("@employee_id", SqlDbType.NVarChar);
            para[3].Value = employee_manage.employee_id;

            para[4] = new SqlParameter("@login_userid", SqlDbType.NVarChar);
            para[4].Value = employee_manage.login_userid;

            para[5] = new SqlParameter("@password", SqlDbType.NVarChar);
            para[5].Value = employee_manage.password;

            para[6] = new SqlParameter("@employee_first_name", SqlDbType.NVarChar);
            para[6].Value = employee_manage.employee_first_name;

            para[7] = new SqlParameter("@employee_last_name", SqlDbType.NVarChar);
            para[7].Value = employee_manage.employee_last_name;

            para[8] = new SqlParameter("@mobile_no", SqlDbType.NVarChar);
            para[8].Value = employee_manage.mobile_no;

            para[9] = new SqlParameter("@phone_no", SqlDbType.NVarChar);
            para[9].Value = employee_manage.phone_no;

            para[10] = new SqlParameter("@gender", SqlDbType.NVarChar);
            para[10].Value = employee_manage.gender;

            para[11] = new SqlParameter("@alternate_number", SqlDbType.NVarChar);
            para[11].Value = employee_manage.alternate_number;

            para[12] = new SqlParameter("@dob", SqlDbType.NVarChar);
            para[12].Value = employee_manage.dob;

            para[13] = new SqlParameter("@join_date", SqlDbType.NVarChar);
            para[13].Value = employee_manage.join_date;

            para[14] = new SqlParameter("@salary_ctc", SqlDbType.NVarChar);
            para[14].Value = employee_manage.salary_ctc;

            para[15] = new SqlParameter("@qualification", SqlDbType.NVarChar);
            para[15].Value = employee_manage.qualification;

            para[16] = new SqlParameter("@aadhar_no", SqlDbType.NVarChar);
            para[16].Value = employee_manage.aadhar_no;

            para[17] = new SqlParameter("@pan_no", SqlDbType.NVarChar);
            para[17].Value = employee_manage.pan_no;

            para[18] = new SqlParameter("@profile_picture", SqlDbType.NVarChar);
            para[18].Value = img1;

            para[19] = new SqlParameter("@country", SqlDbType.NVarChar);
            para[19].Value = employee_manage.country;

            para[20] = new SqlParameter("@state", SqlDbType.NVarChar);
            para[20].Value = employee_manage.state;

            para[21] = new SqlParameter("@city", SqlDbType.NVarChar);
            para[21].Value = employee_manage.city;

            para[22] = new SqlParameter("@area_code", SqlDbType.NVarChar);
            para[22].Value = employee_manage.area_code;

            para[23] = new SqlParameter("@address", SqlDbType.NVarChar);
            para[23].Value = employee_manage.address;

            para[24] = new SqlParameter("@employee_department", SqlDbType.NVarChar);
            para[24].Value = employee_manage.department;

            para[25] = new SqlParameter("@reporting_manager", SqlDbType.NVarChar);
            para[25].Value = employee_manage.reporting_manager;

            para[26] = new SqlParameter("@reporting_tl", SqlDbType.NVarChar);
            para[26].Value = employee_manage.reporting_tl;

            para[27] = new SqlParameter("@exit_date", SqlDbType.NVarChar);
            para[27].Value = employee_manage.exit_date;

            para[28] = new SqlParameter("@current_status", SqlDbType.NVarChar);
            para[28].Value = employee_manage.current_status;

            para[29] = new SqlParameter("@id", SqlDbType.NVarChar);
            para[29].Value = employee_manage.id;


            SqlHelper.ExecuteNonQuery(CommandType.StoredProcedure, "sp_UpdateInsertEmployee", para);
            TempData["Message"] = "Your Employee Updated Successfully";
            TempData["para"] = true;


            //catch
            //{
            //    TempData["para"] = false;
            //    TempData["Message"] = "Error, Call Your Administritive !";
            //}

            return RedirectToAction("employee_management");
        }

        public ActionResult view_employee()
        {
            List<CloudgenProject.Models.admin.employee_manage> employee_manage = new List<CloudgenProject.Models.admin.employee_manage>();

            string sql = "select e.*,b.BranchName,d.designation from employee_management e left outer join designation_master d on e.employee_designation = d.id left outer join manage_branch b on e.employee_branch =b.id WHERE e.employee_type <> 'admin' order by e.current_status asc";

            DataTable dtr = SqlHelper.ExecuteDataset(CommandType.Text, sql).Tables[0];
            if (dtr.Rows.Count > 0)
            {
                foreach (DataRow dr in dtr.Rows)
                {
                    employee_manage.Add(new CloudgenProject.Models.admin.employee_manage()
                    {
                        employee_branch = dr["BranchName"].ToString(),
                        employee_type = dr["employee_type"].ToString(),
                        designation = dr["designation"].ToString(),
                        employee_id = dr["employee_id"].ToString(),
                        login_userid = dr["login_userid"].ToString(),
                        password = dr["password"].ToString(),
                        employee_first_name = dr["employee_first_name"].ToString(),
                        employee_last_name = dr["employee_last_name"].ToString(),
                        mobile_no = dr["mobile_no"].ToString(),
                        phone_no = dr["phone_no"].ToString(),
                        gender = dr["gender"].ToString(),
                        alternate_number = dr["alternate_number"].ToString(),
                        dob = dr["dob"].ToString(),
                        join_date = dr["join_date"].ToString(),
                        salary_ctc = dr["salary_ctc"].ToString(),
                        qualification = dr["qualification"].ToString(),
                        aadhar_no = dr["aadhar_no"].ToString(),
                        pan_no = dr["pan_no"].ToString(),
                        profile_picture = dr["profile_picture"].ToString(),
                        country = dr["country"].ToString(),
                        state = dr["state"].ToString(),
                        city = dr["city"].ToString(),
                        area_code = dr["area_code"].ToString(),
                        address = dr["address"].ToString(),
                        department = dr["employee_department"].ToString(),
                        reporting_manager = dr["reporting_manager"].ToString(),
                        reporting_tl = dr["reporting_tl"].ToString(),
                        exit_date = dr["exit_date"].ToString(),
                        current_status = dr["current_status"].ToString(),
                        id = dr["id"].ToString(),

                    });
                }
            }

            return View(employee_manage);
        }

        public ActionResult delet_employee(string id)
        {
            string str = "update employee_management set current_status='Inactive' where id='" + id + "'";

            SqlHelper.ExecuteNonQuery(CommandType.Text, str);

            return RedirectToAction("view_employee");
        }


        #endregion

        #region Manage Leave Distribution Employee

        public ActionResult manage_leave(string id)
        {
            //CloudgenProject.App_Code.basic_function basic_function = new CloudgenProject.App_Code.basic_function();
            //if (basic_function.adminssioncheck("") == false)
            //{
            //    string url = Request.Url.PathAndQuery;
            //    return Redirect("/admin/login?url=" + HttpUtility.UrlEncode(url) + "");
            //}

            CloudgenProject.Models.admin.manage_leave manage_leave = new CloudgenProject.Models.admin.manage_leave();
            List<CloudgenProject.Models.admin.employe_list> employe_list = new List<CloudgenProject.Models.admin.employe_list>();



            string str = " select *  from Employee_management where employee_type <> 'admin' ";
            DataSet ds = SqlHelper.ExecuteDataset(CommandType.Text, str);


            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    employe_list.Add(new CloudgenProject.Models.admin.employe_list()
                    {
                        name = dr["employee_first_name"].ToString() + " " + dr["employee_last_name"].ToString(),
                        employee_id = dr["employee_id"].ToString()
                    });
                }
            }

            manage_leave.employe_list = employe_list;

            if (id != null && id.ToString() != "")
            {
                string strr = "select * from manage_leave where id='" + id + "'";
                DataTable dt = SqlHelper.ExecuteDataset(CommandType.Text, strr).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    manage_leave.id = Convert.ToInt32(dt.Rows[0]["id"].ToString());
                    manage_leave.Employeeid = dt.Rows[0]["Employeeid"].ToString();
                    manage_leave.EmployeeType = dt.Rows[0]["EmployeeType"].ToString();
                    manage_leave.Department = dt.Rows[0]["Department"].ToString();
                    manage_leave.EarnedOrPrivilegeLeave = Convert.ToInt32(dt.Rows[0]["EarnedOrPrivilegeLeave"].ToString());
                    manage_leave.EarnedOrPrivilegeLeaveUsed = Convert.ToInt32(dt.Rows[0]["EarnedOrPrivilegeLeaveUsed"].ToString());
                    manage_leave.CasualLeave = Convert.ToInt32(dt.Rows[0]["CasualLeave"].ToString());
                    manage_leave.CasualLeaveUsed = Convert.ToInt32(dt.Rows[0]["CasualLeaveUsed"].ToString());
                    manage_leave.SickOrMedicalLeave = Convert.ToInt32(dt.Rows[0]["SickOrMedicalLeave"].ToString());
                    manage_leave.SickOrMedicalLeaveUsed = Convert.ToInt32(dt.Rows[0]["SickOrMedicalLeaveUsed"].ToString());
                    manage_leave.MaternityLeave = Convert.ToInt32(dt.Rows[0]["MaternityLeave"].ToString());
                    manage_leave.MaternityLeaveUsed = Convert.ToInt32(dt.Rows[0]["MaternityLeaveUsed"].ToString());
                    manage_leave.QuarantineLeave = Convert.ToInt32(dt.Rows[0]["QuarantineLeave"].ToString());
                    manage_leave.QuarantineLeaveUsed = Convert.ToInt32(dt.Rows[0]["QuarantineLeaveUsed"].ToString());
                    manage_leave.SessionStartFrom = Convert.ToDateTime(dt.Rows[0]["SessionStartFrom"].ToString());
                    manage_leave.SessionEndTo = Convert.ToDateTime(dt.Rows[0]["SessionEndTo"].ToString());
                    manage_leave.Status = dt.Rows[0]["Status"].ToString();

                }
            }


            return View(manage_leave);

        }

        [HttpPost]
        public ActionResult insert_leave(CloudgenProject.Models.admin.manage_leave manage_leave)
        {
            SqlCommand cmd = new SqlCommand("sp_leavemanage", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", manage_leave.id);
            cmd.Parameters.AddWithValue("@Employeeid", manage_leave.Employeeid);
            cmd.Parameters.AddWithValue("@EmployeeType", manage_leave.EmployeeType);
            cmd.Parameters.AddWithValue("@Department", manage_leave.Department);
            cmd.Parameters.AddWithValue("@EarnedOrPrivilegeLeave", manage_leave.EarnedOrPrivilegeLeave);
            cmd.Parameters.AddWithValue("@EarnedOrPrivilegeLeaveUsed", manage_leave.EarnedOrPrivilegeLeaveUsed);
            cmd.Parameters.AddWithValue("@CasualLeave", manage_leave.CasualLeave);
            cmd.Parameters.AddWithValue("@CasualLeaveUsed", manage_leave.CasualLeaveUsed);
            cmd.Parameters.AddWithValue("@SickOrMedicalLeave", manage_leave.SickOrMedicalLeave);
            cmd.Parameters.AddWithValue("@SickOrMedicalLeaveUsed", manage_leave.SickOrMedicalLeaveUsed);
            cmd.Parameters.AddWithValue("@MaternityLeave", manage_leave.MaternityLeave);
            cmd.Parameters.AddWithValue("@MaternityLeaveUsed", manage_leave.MaternityLeaveUsed);
            cmd.Parameters.AddWithValue("@QuarantineLeave", manage_leave.QuarantineLeave);
            cmd.Parameters.AddWithValue("@QuarantineLeaveUsed", manage_leave.QuarantineLeaveUsed);
            cmd.Parameters.AddWithValue("@SessionStartFrom", manage_leave.SessionStartFrom);
            cmd.Parameters.AddWithValue("@SessionEndTo", manage_leave.SessionEndTo);
            cmd.Parameters.AddWithValue("@Status", manage_leave.Status);

            if (manage_leave.id !=  0)
            {
                cmd.Parameters.AddWithValue("@Action", "update_leave");
            }
            else
            {
                cmd.Parameters.AddWithValue("@Action", "insert_leave");

            }

            if (con.State == ConnectionState.Closed)
                con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
            {
                return RedirectToAction("manage_leave");
            }
            else
            {
                return RedirectToAction("manage_leave");
            }

        }

        public ActionResult view_leave()
        {
            List<CloudgenProject.Models.admin.manage_leave> manage_leave = new List<CloudgenProject.Models.admin.manage_leave>();

            string sql = "select l.*, e.employee_first_name,e.employee_last_name from manage_leave l left outer join employee_management e on l.Employeeid=e.employee_id order by id";

            DataTable dtr = SqlHelper.ExecuteDataset(CommandType.Text, sql).Tables[0];
            if (dtr.Rows.Count > 0)
            {
                foreach (DataRow dr in dtr.Rows)
                {
                    manage_leave.Add(new CloudgenProject.Models.admin.manage_leave()
                    {
                        id = Convert.ToInt32(dr["id"]),
                        Employeeid = dr["employee_first_name"].ToString() + " "+ dr["employee_last_name"].ToString(),
                        EmployeeType = dr["EmployeeType"].ToString(),
                        Department = dr["Department"].ToString(),
                        EarnedOrPrivilegeLeave = Convert.ToInt32(dr["EarnedOrPrivilegeLeave"]),
                        EarnedOrPrivilegeLeaveUsed = Convert.ToInt32(dr["EarnedOrPrivilegeLeaveUsed"]),
                        CasualLeave = Convert.ToInt32(dr["EarnedOrPrivilegeLeave"]),
                        CasualLeaveUsed = Convert.ToInt32(dr["CasualLeave"]),
                        SickOrMedicalLeave = Convert.ToInt32(dr["SickOrMedicalLeave"]),
                        SickOrMedicalLeaveUsed = Convert.ToInt32(dr["SickOrMedicalLeaveUsed"]),
                        MaternityLeave = Convert.ToInt32(dr["MaternityLeave"]),
                        MaternityLeaveUsed = Convert.ToInt32(dr["MaternityLeaveUsed"]),
                        QuarantineLeave = Convert.ToInt32(dr["QuarantineLeave"]),
                        QuarantineLeaveUsed = Convert.ToInt32(dr["QuarantineLeaveUsed"]),
                        SessionStartFrom = Convert.ToDateTime(dr["SessionStartFrom"]),
                        SessionEndTo = Convert.ToDateTime(dr["SessionEndTo"]),
                        Status = dr["Status"].ToString(),

                    });
                }
            }

            return View(manage_leave);
        }



        public ActionResult view_leavebyid(string id)
        {
            //CloudgenProject.App_Code.basic_function basic_function = new CloudgenProject.App_Code.basic_function();
            //if (basic_function.adminssioncheck("") == false)
            //{
            //    string url = Request.Url.PathAndQuery;
            //    return Redirect("/admin/login?url=" + HttpUtility.UrlEncode(url) + "");
            //}

            CloudgenProject.Models.admin.manage_leave manage_leave = new CloudgenProject.Models.admin.manage_leave();


            if (id != null && id.ToString() != "")
            {
                string strr = "select l.*, e.employee_first_name,e.employee_last_name from manage_leave l left outer join employee_management e on l.Employeeid=e.employee_id where l.id='" + id + "'";
                DataTable dt = SqlHelper.ExecuteDataset(CommandType.Text, strr).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    manage_leave.id = Convert.ToInt32(dt.Rows[0]["id"].ToString());
                    manage_leave.Employeeid = dt.Rows[0]["employee_first_name"].ToString() + " "+ dt.Rows[0]["employee_last_name"].ToString();
                    manage_leave.EmployeeType = dt.Rows[0]["EmployeeType"].ToString();
                    manage_leave.Department = dt.Rows[0]["Department"].ToString();
                    manage_leave.EarnedOrPrivilegeLeave = Convert.ToInt32(dt.Rows[0]["EarnedOrPrivilegeLeave"].ToString());
                    manage_leave.EarnedOrPrivilegeLeaveUsed = Convert.ToInt32(dt.Rows[0]["EarnedOrPrivilegeLeaveUsed"].ToString());
                    manage_leave.CasualLeave = Convert.ToInt32(dt.Rows[0]["CasualLeave"].ToString());
                    manage_leave.CasualLeaveUsed = Convert.ToInt32(dt.Rows[0]["CasualLeaveUsed"].ToString());
                    manage_leave.SickOrMedicalLeave = Convert.ToInt32(dt.Rows[0]["SickOrMedicalLeave"].ToString());
                    manage_leave.SickOrMedicalLeaveUsed = Convert.ToInt32(dt.Rows[0]["SickOrMedicalLeaveUsed"].ToString());
                    manage_leave.MaternityLeave = Convert.ToInt32(dt.Rows[0]["MaternityLeave"].ToString());
                    manage_leave.MaternityLeaveUsed = Convert.ToInt32(dt.Rows[0]["MaternityLeaveUsed"].ToString());
                    manage_leave.QuarantineLeave = Convert.ToInt32(dt.Rows[0]["QuarantineLeave"].ToString());
                    manage_leave.QuarantineLeaveUsed = Convert.ToInt32(dt.Rows[0]["QuarantineLeaveUsed"].ToString());
                    manage_leave.SessionStartFrom = Convert.ToDateTime(dt.Rows[0]["SessionStartFrom"].ToString());
                    manage_leave.SessionEndTo = Convert.ToDateTime(dt.Rows[0]["SessionEndTo"].ToString());
                    manage_leave.Status = dt.Rows[0]["Status"].ToString();

                }
            }


            return Json(manage_leave, JsonRequestBehavior.AllowGet);

        }

        public ActionResult delet_leave(string id)
        {
            string str = "delete from manage_branch where id='" + id + "'";

            SqlHelper.ExecuteNonQuery(CommandType.Text, str);

            return RedirectToAction("view_branch");
        }

        public JsonResult getEmployeeDetails(string employeeId)
        {

            string EmployeeType = "";
            string Department = "";
            string query = "select * from employee_management e left outer join department_master d on d.id = e.employee_department where employee_id = " + employeeId;
            con.Open();

            SqlCommand cmd = new SqlCommand(query, con);

            SqlDataReader sdr = cmd.ExecuteReader();


            if (sdr.HasRows)
            {
                sdr.Read();
                EmployeeType = sdr["employee_type"].ToString();
                Department = sdr["department"].ToString();
            }
            con.Close();

            var result = new { employeeType = EmployeeType, department = Department };

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region lead management field
        public ActionResult lead_management(string id)
        {
            //CloudgenProject.App_Code.basic_function basic_function = new CloudgenProject.App_Code.basic_function();
            //if (basic_function.adminssioncheck("") == false)
            //{
            //    string url = Request.Url.PathAndQuery;
            //    return Redirect("/admin/login?url=" + HttpUtility.UrlEncode(url) + "");
            //}

            countrybind();

            CloudgenProject.Models.admin.lead_mangement lead_mangement = new CloudgenProject.Models.admin.lead_mangement();
            List<CloudgenProject.Models.admin.employe_list> employe_list = new List<CloudgenProject.Models.admin.employe_list>();



            string str = " select * from employee_management ";
            DataSet ds = SqlHelper.ExecuteDataset(CommandType.Text, str);


            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    employe_list.Add(new CloudgenProject.Models.admin.employe_list()
                    {
                        name = dr["employee_first_name"].ToString() + dr["employee_last_name"].ToString(),
                        employee_id = dr["employee_id"].ToString()
                    });
                }
            }

            lead_mangement.employe_list = employe_list;


            if (id != null && id.ToString() != "")
            {
                string strr = "select * from LeadTable where LeadID = '" + id + "' ";
                DataTable dt = SqlHelper.ExecuteDataset(CommandType.Text, strr).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    lead_mangement.id = dt.Rows[0]["id"].ToString();
                    lead_mangement.LeadID = dt.Rows[0]["LeadID"].ToString();
                    lead_mangement.LeadSegment = dt.Rows[0]["LeadSegment"].ToString();
                    lead_mangement.Branch = dt.Rows[0]["Branch"].ToString();
                    lead_mangement.Client = dt.Rows[0]["Client"].ToString();
                    lead_mangement.ContactPerson = dt.Rows[0]["ContactPerson"].ToString();
                    lead_mangement.ContactDesignation = dt.Rows[0]["ContactDesignation"].ToString();
                    lead_mangement.ContactEmail = dt.Rows[0]["ContactEmail"].ToString();
                    lead_mangement.ContactMobileNumber = dt.Rows[0]["ContactMobileNumber"].ToString();
                    lead_mangement.Address = dt.Rows[0]["Address"].ToString();
                    lead_mangement.Telephone = dt.Rows[0]["Telephone"].ToString();
                    lead_mangement.City = dt.Rows[0]["City"].ToString();
                    lead_mangement.PIN = dt.Rows[0]["PIN"].ToString();
                    lead_mangement.State = dt.Rows[0]["State"].ToString();
                    lead_mangement.Country = dt.Rows[0]["Country"].ToString();
                    lead_mangement.ClientSegment = dt.Rows[0]["ClientSegment"].ToString();
                    lead_mangement.ClientTurnover = dt.Rows[0]["ClientTurnover"].ToString();
                    lead_mangement.ClientPCsOrNoOfEmployee = dt.Rows[0]["ClientPCsOrNoOfEmployee"].ToString();
                    lead_mangement.ExpectedAnnualBusiness = dt.Rows[0]["ExpectedAnnualBusiness"].ToString();
                    lead_mangement.ProductRequired = dt.Rows[0]["ProductRequired"].ToString();
                    lead_mangement.ProductDescription = dt.Rows[0]["ProductDescription"].ToString();
                    lead_mangement.ProductQuantity = dt.Rows[0]["ProductQuantity"].ToString();
                    lead_mangement.ServiceRequired = dt.Rows[0]["ServiceRequired"].ToString();
                    lead_mangement.ServicesDescription = dt.Rows[0]["ServicesDescription"].ToString();
                    lead_mangement.EstimatedLeadValue = dt.Rows[0]["EstimatedLeadValue"].ToString();
                    lead_mangement.LeadGeneratedBy = dt.Rows[0]["LeadGeneratedBy"].ToString();
                    lead_mangement.LeadGeneratedByType = dt.Rows[0]["LeadGeneratedByType"].ToString();
                    lead_mangement.LeadGenerationDate = dt.Rows[0]["LeadGenerationDate"].ToString();
                    lead_mangement.LeadAssignedTo = dt.Rows[0]["LeadAssignedTo"].ToString();
                    lead_mangement.LeadStatus = dt.Rows[0]["LeadStatus"].ToString();
                    lead_mangement.assignStatus = dt.Rows[0]["assignStatus"].ToString();

                }
            }

            return View(lead_mangement);

        }

        [HttpPost]
        public ActionResult insert_lead(CloudgenProject.Models.admin.lead_mangement lead_mangement)
        {


            SqlParameter[] para = new SqlParameter[32];

            para[0] = new SqlParameter("@LeadID", SqlDbType.NVarChar);
            para[0].Value = lead_mangement.LeadID;

            para[1] = new SqlParameter("@LeadSegment", SqlDbType.NVarChar);
            para[1].Value = lead_mangement.LeadSegment;

            para[2] = new SqlParameter("@Branch", SqlDbType.NVarChar);
            para[2].Value = lead_mangement.Branch;

            para[3] = new SqlParameter("@Client", SqlDbType.NVarChar);
            para[3].Value = lead_mangement.Client;

            para[4] = new SqlParameter("@ContactPerson", SqlDbType.NVarChar);
            para[4].Value = lead_mangement.ContactPerson;

            para[5] = new SqlParameter("@ContactDesignation", SqlDbType.NVarChar);
            para[5].Value = lead_mangement.ContactDesignation;

            para[6] = new SqlParameter("@ContactEmail", SqlDbType.NVarChar);
            para[6].Value = lead_mangement.ContactEmail;

            para[7] = new SqlParameter("@ContactMobileNumber", SqlDbType.NVarChar);
            para[7].Value = lead_mangement.ContactMobileNumber;

            para[8] = new SqlParameter("@Address", SqlDbType.NText);
            para[8].Value = lead_mangement.Address;

            para[9] = new SqlParameter("@Telephone", SqlDbType.NVarChar);
            para[9].Value = lead_mangement.Telephone;

            para[10] = new SqlParameter("@City", SqlDbType.NVarChar);
            para[10].Value = lead_mangement.City;

            para[11] = new SqlParameter("@PIN", SqlDbType.NVarChar);
            para[11].Value = lead_mangement.PIN;

            para[12] = new SqlParameter("@State", SqlDbType.NVarChar);
            para[12].Value = lead_mangement.State;

            para[13] = new SqlParameter("@Country", SqlDbType.NVarChar);
            para[13].Value = lead_mangement.Country;

            para[14] = new SqlParameter("@ClientSegment", SqlDbType.NVarChar);
            para[14].Value = lead_mangement.ClientSegment;

            para[15] = new SqlParameter("@ClientTurnover", SqlDbType.NVarChar);
            para[15].Value = lead_mangement.ClientTurnover;

            para[16] = new SqlParameter("@ClientPCsOrNoOfEmployee", SqlDbType.NVarChar);
            para[16].Value = lead_mangement.ClientPCsOrNoOfEmployee;

            para[17] = new SqlParameter("@ExpectedAnnualBusiness", SqlDbType.NVarChar);
            para[17].Value = lead_mangement.ExpectedAnnualBusiness;

            para[18] = new SqlParameter("@ProductRequired", SqlDbType.NVarChar);
            para[18].Value = lead_mangement.ProductRequired;

            para[19] = new SqlParameter("@ProductDescription", SqlDbType.NText);
            para[19].Value = lead_mangement.ProductDescription;

            para[20] = new SqlParameter("@ProductQuantity", SqlDbType.NVarChar);
            para[20].Value = lead_mangement.ProductQuantity;

            para[21] = new SqlParameter("@ServiceRequired", SqlDbType.NVarChar);
            para[21].Value = lead_mangement.ServiceRequired;

            para[22] = new SqlParameter("@ServicesDescription", SqlDbType.NText);
            para[22].Value = lead_mangement.ServicesDescription;

            para[23] = new SqlParameter("@EstimatedLeadValue", SqlDbType.NVarChar);
            para[23].Value = lead_mangement.EstimatedLeadValue;

            para[24] = new SqlParameter("@LeadGeneratedBy", SqlDbType.NVarChar);
            para[24].Value = lead_mangement.LeadGeneratedBy;

            para[25] = new SqlParameter("@LeadGeneratedByType", SqlDbType.NVarChar);
            para[25].Value = lead_mangement.LeadGeneratedByType;

            para[26] = new SqlParameter("@LeadGenerationDate", SqlDbType.NVarChar);
            para[26].Value = lead_mangement.LeadGenerationDate;

            para[27] = new SqlParameter("@LeadAssignedTo", SqlDbType.NVarChar);
            para[27].Value = lead_mangement.LeadAssignedTo;

            para[28] = new SqlParameter("@LeadStatus", SqlDbType.NVarChar);
            para[28].Value = lead_mangement.LeadStatus;

            para[29] = new SqlParameter("@assignStatus", SqlDbType.NVarChar);
            para[29].Value = lead_mangement.assignStatus;

            para[30] = new SqlParameter("@id", SqlDbType.NVarChar);
            para[30].Value = lead_mangement.id;

            if (lead_mangement.id != null && lead_mangement.id != "")
            {
                para[31] = new SqlParameter("@action", SqlDbType.NVarChar);
                para[31].Value = "UpdateLead";
            }
            else
            {
                para[31] = new SqlParameter("@action", SqlDbType.NVarChar);
                para[31].Value = "insertLead";
            }



            SqlHelper.ExecuteNonQuery(CommandType.StoredProcedure, "sp_InsertOrUpdateLead", para);
            TempData["Message"] = "Your LEAD  InsertedOrUpdated Successfully";
            TempData["para"] = true;


            //}
            //catch
            //{
            //    TempData["para"] = false;
            //    TempData["Message"] = "Error, Call Your Administritive !";
            //}

            return RedirectToAction("viewleadsheetlist");
        }

        public ActionResult view_lead()
        {
            List<CloudgenProject.Models.admin.lead_mangement> lead_mangement = new List<CloudgenProject.Models.admin.lead_mangement>();
            var logintype = Session["usertype"].ToString();
            var loginId = Session["employeeId"].ToString();
            // string sql = "select * from LeadTable order by id";
            string sql = "";
            if (logintype == "admin")
            {
                sql = "select * from LeadTable order by id desc";

            }
            else
            {
                sql = "select * from LeadTable where LeadGeneratedBy =" + loginId;

            }

            DataTable dtr = SqlHelper.ExecuteDataset(CommandType.Text, sql).Tables[0];
            if (dtr.Rows.Count > 0)
            {
                foreach (DataRow dr in dtr.Rows)
                {
                    lead_mangement.Add(new CloudgenProject.Models.admin.lead_mangement()
                    {
                        LeadID = dr["LeadID"].ToString(),
                        LeadSegment = dr["LeadSegment"].ToString(),
                        Branch = dr["Branch"].ToString(),
                        Client = dr["Client"].ToString(),
                        ContactPerson = dr["ContactPerson"].ToString(),
                        ContactDesignation = dr["ContactDesignation"].ToString(),
                        ContactEmail = dr["ContactEmail"].ToString(),
                        ContactMobileNumber = dr["ContactMobileNumber"].ToString(),
                        Address = dr["Address"].ToString(),
                        Telephone = dr["Telephone"].ToString(),
                        City = dr["City"].ToString(),
                        PIN = dr["PIN"].ToString(),
                        State = dr["State"].ToString(),
                        Country = dr["Country"].ToString(),
                        ClientSegment = dr["ClientSegment"].ToString(),
                        ClientTurnover = dr["ClientTurnover"].ToString(),
                        ClientPCsOrNoOfEmployee = dr["ClientPCsOrNoOfEmployee"].ToString(),
                        ExpectedAnnualBusiness = dr["ExpectedAnnualBusiness"].ToString(),
                        ProductRequired = dr["ProductRequired"].ToString(),
                        ProductDescription = dr["ProductDescription"].ToString(),
                        ProductQuantity = dr["ProductQuantity"].ToString(),
                        ServiceRequired = dr["ServiceRequired"].ToString(),
                        ServicesDescription = dr["ServicesDescription"].ToString(),
                        EstimatedLeadValue = dr["EstimatedLeadValue"].ToString(),
                        LeadGeneratedBy = dr["LeadGeneratedBy"].ToString(),
                        LeadGeneratedByType = dr["LeadGeneratedByType"].ToString(),
                        LeadGenerationDate = dr["LeadGenerationDate"].ToString(),
                        LeadAssignedTo = dr["LeadAssignedTo"].ToString(),
                        LeadStatus = dr["LeadStatus"].ToString(),
                        id = dr["id"].ToString(),

                    });
                }
            }

            return View(lead_mangement);
        }

        public ActionResult delet_lead(string id)
        {
            string str = "delete from LeadTable where LeadId='" + id + "'";

            SqlHelper.ExecuteNonQuery(CommandType.Text, str);

            string str2 = "update LeadSheet set generateStatus = '" + 0 + "' where id='" + id + "'";

            SqlHelper.ExecuteNonQuery(CommandType.Text, str2);


            return RedirectToAction("view_lead");
        }


        #endregion

        #region manage leadsheet

        public ActionResult addleadsheet()
        {

            return View();

        }




        [HttpPost]
        public ActionResult addleadsheet(HttpPostedFileBase excelFile)
        {

            // Session["usertype"];
            var leaduploadedby = Session["employeeId"].ToString();
            var leaduploadedbytype = Session["usertype"].ToString();

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            if (excelFile != null && excelFile.ContentLength > 0)
            {
                List<CloudgenProject.Models.admin.manage_leadsheet> leadsheet = new List<CloudgenProject.Models.admin.manage_leadsheet>();

                string fileExtension = Path.GetExtension(excelFile.FileName);

                if (fileExtension == ".xlsx")
                {
                    using (var package = new ExcelPackage(excelFile.InputStream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];

                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            leadsheet.Add(new CloudgenProject.Models.admin.manage_leadsheet
                            {
                                client_name = worksheet.Cells[row, 1].Value.ToString(),
                                contact_no = worksheet.Cells[row, 2].Value.ToString(),
                                eMAIL_ID = worksheet.Cells[row, 3].Value.ToString(),
                                address = worksheet.Cells[row, 4].Value.ToString(),
                                productname = worksheet.Cells[row, 5].Value.ToString(),
                                contactperson = worksheet.Cells[row, 6].Value.ToString(),
                            });
                        }
                    }
                }


                manage_leadsheet leadStatus = new manage_leadsheet();



                switch (leaduploadedbytype)
                {
                    case "admin":
                        leadStatus.assignStatus = "1";
                        break;
                    case "sales":
                        leadStatus.assignStatus = "2";
                        break;
                    case "agent":
                        leadStatus.assignStatus = "3";
                        break;
                    default:
                        return RedirectToAction("viewleadsheetlist");

                }




                con.Open();
                foreach (var lead in leadsheet)
                {

                    SqlCommand cmd = new SqlCommand("sp_LeadSheet", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@client_name", lead.client_name);
                    cmd.Parameters.AddWithValue("@contact_no", lead.contact_no);
                    cmd.Parameters.AddWithValue("@eMAIL_ID", lead.eMAIL_ID);
                    cmd.Parameters.AddWithValue("@address", lead.address);
                    cmd.Parameters.AddWithValue("@contactperson", lead.contactperson);
                    cmd.Parameters.AddWithValue("@productname", lead.productname);
                    cmd.Parameters.AddWithValue("@uploadedby", leaduploadedby);
                    cmd.Parameters.AddWithValue("@uploadedbytype", leaduploadedbytype);
                    cmd.Parameters.AddWithValue("@assignStatus", leadStatus.assignStatus);
                    cmd.Parameters.AddWithValue("@generateStatus", 0);

                    cmd.Parameters.AddWithValue("@Action", "insert_leadsheet");



                    int i = cmd.ExecuteNonQuery();
                }

                con.Close();


                return RedirectToAction("addleadsheet"); // Redirect to a success page
            }

            return View();

        }



        public ActionResult viewleadsheetlist()
        {

            var loginId = Session["employeeId"].ToString();


            var logintype = Session["usertype"].ToString();

            var repmanager = db.getreportedManager(Convert.ToInt32(loginId));



            var rpmId = repmanager.report_manager_Id;



            List<CloudgenProject.Models.admin.employe_list> employe_list = new List<CloudgenProject.Models.admin.employe_list>();

            string str = "";
            // string str = "select * from employee_management where employee_id NOT IN (" + loginId + ")";

            str = "select * from employee_management where id = '" + rpmId + "'";

            //if (logintype == "sales")
            //{
            //    str = "select * from employee_management where reporting_manager = '" + loginId + "'";
            //}


            DataSet ds = SqlHelper.ExecuteDataset(CommandType.Text, str);


            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    employe_list.Add(new CloudgenProject.Models.admin.employe_list()
                    {
                        name = dr["employee_first_name"].ToString() + dr["employee_last_name"].ToString(),
                        employee_id = dr["employee_id"].ToString()
                    });
                }
            }

            ViewBag.ReAssigemploye_list = employe_list;



            //--------------------------------------


            //List<SelectListItem> agentlist = new List<SelectListItem>();
            //agentlist = db.getAgentList(loginId);
            //ViewBag.AgentList = agentlist;


            //List<SelectListItem> saleslist = new List<SelectListItem>();
            //saleslist = db.getSalesList();

            //ViewBag.SalesList = saleslist;



            var usertype = Session["usertype"].ToString();
            List<manage_leadsheet> leadsheetlist = new List<manage_leadsheet>();
            leadsheetlist = db.getleadsheetlist(loginId, usertype);




            return View(leadsheetlist);
        }


        //add lead manualy
        public ActionResult addLeadManualyBySalesMan(string id)
        {

            manage_leadsheet lead = new manage_leadsheet();



            if (id != null && id != "")
            {
                lead = db.getleadsheetById(id).FirstOrDefault();
            }


            return View(lead);
        }

        [HttpPost]

        public ActionResult insertLeadManualyBySalesMan(CloudgenProject.Models.admin.manage_leadsheet leadObj)
        {

            //leadObj.uploadedby = Session["employeeId"].ToString();
            // leadObj.uploadedbytype = Session["usertype"].ToString();

            var uploadedBy = Session["employeeId"].ToString();
            var uploadedbytype = Session["usertype"].ToString();


            switch (uploadedbytype)
            {
                case "admin":
                    leadObj.assignStatus = "1";
                    break;
                case "sales":
                    leadObj.assignStatus = "2";
                    break;
                case "agent":
                    leadObj.assignStatus = "3";
                    break;
                default:
                    return RedirectToAction("viewleadsheetlist");

            }


            //if(uploadedbytype == "admin")
            //{
            //    leadObj.assignStatus = "1";
            //}

            //if (uploadedbytype == "sales")
            //{
            //    leadObj.assignStatus = "2";
            //}

            //if (uploadedbytype == "agent")
            //{
            //    leadObj.assignStatus = "3";
            //}




            leadObj.uploadedby = uploadedBy;
            leadObj.uploadedbytype = uploadedbytype;


            db.addmanualyleadBySalesman(leadObj);
            return RedirectToAction("viewleadsheetlist");
        }


        public ActionResult generateLeadBySalesman(string id)
        {

            var newlead = db.getleadsheetById(id).FirstOrDefault();
            newlead.uploadedby = Session["employeeId"].ToString();
            newlead.uploadedbytype = Session["usertype"].ToString();
            // newlead.LeadGenerationDate= DateTime.Now.ToString("dd/MM/yyyy"));
            //  Console.WriteLine(Date.ToString("dd/MM/yyyy"));

            db.generatenewLeadBySalesman(newlead);
            var leadId = db.getgeneratedleadById(id).FirstOrDefault();

            return RedirectToAction("lead_management", "Admin", new { id = leadId.id });
        }





        public ActionResult delet_leadBySalesman(string id)
        {
            db.delete_leadBySalesman(id);
            return RedirectToAction("viewleadsheetlist");
        }




        public ActionResult AssignLead()
        {
            var rpm = Session["employeeId"].ToString();
            List<SelectListItem> agentlist = new List<SelectListItem>();
            agentlist = db.getAgentList(rpm);
            ViewBag.AgentList = agentlist;


            List<SelectListItem> saleslist = new List<SelectListItem>();
            saleslist = db.getSalesList();

            ViewBag.SalesList = saleslist;


            return View();
        }


        public JsonResult AllLeadJson()
        {
            var loginId = Session["employeeId"].ToString();
            var loginType = Session["usertype"].ToString();

            List<manage_leadsheet> leadlist = new List<manage_leadsheet>();
            leadlist = db.getallLeadForAssign(loginId, loginType);
            IDictionary<string, object> alldata = new Dictionary<string, object>();
            alldata.Add(new KeyValuePair<string, object>("leadlistData", leadlist));
            return Json(alldata, JsonRequestBehavior.AllowGet);
        }
        //testing 
        public ActionResult testlead()
        {

            return View();
        }

        [HttpPost]
        public ActionResult AssignLeadTo(string agentId, string selectRows)
        {
            var uploadedbytype = Session["usertype"].ToString();
            var assignLead = new assignLead();

            switch (uploadedbytype)
            {
                case "admin":
                    assignLead.assignStatus = "2";
                    break;
                case "sales":
                    assignLead.assignStatus = "3";
                    break;
                default:
                    return RedirectToAction("AssignLead");

            }





            assignLead.agentId = agentId;
            assignLead.assignBy_Id = Session["employeeId"].ToString();
            //assignLead.assignBy_Id = Session["Id"].ToString();
            //     string s = "a,b, b, c";

            string[] values = selectRows.Split(',');
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = values[i].Trim();
            }

            foreach (var item in values)
            {
                assignLead.AssignLead_Id = item.ToString();

                db.assignLeadToAgent(assignLead);

            }


            return RedirectToAction("AssignLead");

        }

        //assigned lead by sales team
        public ActionResult showAssignedLeadBySales()
        {
            var loginId = Session["employeeId"].ToString();

            List<SelectListItem> agentlist = new List<SelectListItem>();
            agentlist = db.getAgentList(loginId);

            ViewBag.AgentList = agentlist;


            List<lead_mangement> assingedleadlist = new List<lead_mangement>();
            assingedleadlist = db.GetAllAssignedLeadList(loginId);

            return View(assingedleadlist);
        }
        //edit by salesteam
        [HttpPost]
        public ActionResult reAssignLead(string agentId, string LeadId, string userType)
        {
            var assignLead = new assignLead();

            var uploadedbytype = userType;

            switch (uploadedbytype)
            {
                case "admin":
                    assignLead.assignStatus = "1";
                    break;
                case "sales":
                    assignLead.assignStatus = "2";
                    break;
                case "agent":
                    assignLead.assignStatus = "3";
                    break;
                default:
                    return RedirectToAction("AssignLead");

            }



            var loginId = Session["employeeId"].ToString();
            assignLead.assignBy_Id = loginId;


            assignLead.agentId = agentId;
            assignLead.AssignLead_Id = LeadId;
         //   assignLead.assignStatus = userType;
            db.assignLeadToAgent(assignLead);

            return Json(assignLead, JsonRequestBehavior.AllowGet);
        }
        //view assign lead details
        public ActionResult viewAssignedLeadByLeadId(string leadId)
        {
            // var assignedLead = db.getassingedLeadByLeadId(leadId);
            var assignedLead = db.getLeadDetailbyleadId(leadId);
            return Json(assignedLead, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AgentPannel()
        {

            return View();

        }
        //lead details by leadId

        public ActionResult viewleaddeatailById(string leadId)
        {
            var deatils = db.getLeadDetailbyleadId(leadId);
            return Json(deatils, JsonRequestBehavior.AllowGet);
        }

        public ActionResult viewLeadsheetstatusById(string leadId)
        {

            var leadsheetstatus = db.getleadsheetDetailsById(leadId);


            return Json(leadsheetstatus, JsonRequestBehavior.AllowGet);

        }

        public ActionResult view_assignedlead()
        {
            int loginId = Convert.ToInt32(Session["employeeId"]);
            var logintype = Session["usertype"].ToString();
            //  Session["AgentId"] = id;
            //get reporting manager
            //getreportedManager
            var repmanager = db.getreportedManager(loginId);

            //Session["managerId"] = repmanager.report_manager_Id;
            //Session["managername"] = repmanager.report_manager;
            //Session["empname"] = repmanager.employee_name;
            //Session["empId" ]= repmanager.employee_id;

            var rpmId = repmanager.report_manager_Id;

            //List<SelectListItem> reAssignList = new List<SelectListItem>();
            //reAssignList = db.getAgentList();




            List<CloudgenProject.Models.admin.employe_list> employe_list = new List<CloudgenProject.Models.admin.employe_list>();

            string str = "";
            // string str = "select * from employee_management where employee_id NOT IN (" + loginId + ")";

            str = "select * from employee_management where id = '" + rpmId + "'";

            //if (logintype == "sales")
            //{
            //    str = "select * from employee_management where reporting_manager = '" + loginId + "'";
            //}


            DataSet ds = SqlHelper.ExecuteDataset(CommandType.Text, str);


            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    employe_list.Add(new CloudgenProject.Models.admin.employe_list()
                    {
                        name = dr["employee_first_name"].ToString() + dr["employee_last_name"].ToString(),
                        employee_id = dr["employee_id"].ToString()
                    });
                }
            }

            ViewBag.ReAssigemploye_list = employe_list;
            // lead_mangement.employe_list = employe_list;




            List<lead_mangement> assignLead = new List<lead_mangement>();

            assignLead = db.getAllLeadAssignToAgent(loginId);



            return View(assignLead);

        }


        public ActionResult FollowUpLeadResponse(string leadId, string resId)
        {

            var AgentId = Session["employeeId"].ToString();

            //var LeadId=id.ToString();

            var leaddetails = db.getleadsheetById(leadId).FirstOrDefault();
            //var obj = db.getfollowUpLeadResponse(LeadId);
            ViewBag.clientName = leaddetails.client_name;
            ViewBag.contactperson = leaddetails.contactperson;
            ViewBag.contact = leaddetails.contact_no;
            ViewBag.address = leaddetails.address;
            ViewBag.email = leaddetails.eMAIL_ID;
            ViewBag.LeadId = leaddetails.id;
            ViewBag.AgentId = AgentId;


            List<manage_folloupLeadResponse> responseList = new List<manage_folloupLeadResponse>();

            responseList = db.getfollowUpLeadResponse(leadId);


            return View(responseList);
        }


        [HttpPost]

        public ActionResult insertFollowUpLeadResponse(manage_folloupLeadResponse obj)
        {

            db.insertfolloupLeadResponse(obj);

            //List<lead_mangement> assignLead = new List<lead_mangement>();
            var LeadId = obj.LeadId;
            var response = db.getfollowUpLeadResponse(LeadId);


            return Json(response, JsonRequestBehavior.AllowGet);



        }



        public ActionResult viewleadFollowUpresponse(string id)
        {
            var obj = db.getfollowUpLeadResponse(id);

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        //today follow up list view manage make for agent
        public ActionResult Todayfollowuplist()
        {
         
             DateTime   todayfollowUpDate = DateTime.Now;
            
            var loginid = Session["employeeId"].ToString();

            List<todayfollowupmodel> todayfollow = new List<todayfollowupmodel>();
            todayfollow = db.todayfollowuplist(loginid, todayfollowUpDate);
            return View(todayfollow);
        }

        public ActionResult next_followup_responseNotification()
        {
            var loginid = Session["employeeId"].ToString();
            List<manage_folloupLeadResponse> todayfollowup = new List<manage_folloupLeadResponse>();

            todayfollowup = db.getnextfollowUpLeadResponseNotifiaction(loginid);

            var notificationCount = todayfollowup.Count();

            IDictionary<string, object> alldata = new Dictionary<string, object>();
            alldata.Add(new KeyValuePair<string, object>("todayfollowup", todayfollowup));
            alldata.Add(new KeyValuePair<string, object>("notificationCount", notificationCount));


            //  var a = "abc";
            return Json(alldata, JsonRequestBehavior.AllowGet); 
        }

        //transfer generated lead 
        public ActionResult TransferAssignLead(string employee_id, string LeadId, string userType)
        {

            var assignLead = new assignLead();


            //var uploadedbytype = Session["usertype"].ToString();
            var uploadedbytype = userType;

            switch (uploadedbytype)
            {
                case "admin":
                    assignLead.assignStatus = "1";
                    break;
                case "sales":
                    assignLead.assignStatus = "2";
                    break;
                case "agent":
                    assignLead.assignStatus = "3";
                    break;
                default:
                    return RedirectToAction("AssignLead");

            }



            var loginId = Session["employeeId"].ToString();
            assignLead.assignBy_Id = loginId;

            assignLead.reAssignTo = employee_id;
            assignLead.AssignLead_Id = LeadId;

            db.transferAssignLeadTo_rpm(assignLead);

            return Json(assignLead, JsonRequestBehavior.AllowGet);

        }

        public ActionResult viewleadstatus()
        {
            List<leadstatusmodel> statuslist = new List<leadstatusmodel>();
            statuslist = db.viewleadstatus();
            return View(statuslist);
        }



        public ActionResult viewresponsebyleadId(string LeadId)
        {

            var response = db.getfollowUpLeadResponse(LeadId);


            return Json(response, JsonRequestBehavior.AllowGet);

        }

        public JsonResult getUserType(string employeeId)
        {

            string userType = "";
            string query = "select * from employee_management where employee_id = " + employeeId;
            con.Open();

            SqlCommand cmd = new SqlCommand(query,con);

            SqlDataReader sdr = cmd.ExecuteReader();


            if (sdr.HasRows)
            {
                sdr.Read();
                userType = sdr["employee_type"].ToString();
            }

            //if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            //{
            //    foreach (DataRow dr in ds.Tables[0].Rows)
            //    {
            //        emp.Add(new employee_manage
            //        {
            //            employee_type = dr["employee_type"].ToString(),
            //            // Add other properties as needed
            //        });
            //    }
            //}

            con.Close();


            return Json(userType, JsonRequestBehavior.AllowGet);

        }


        #endregion

        #region manage subadmin

        public ActionResult add_subadmin(string id)
        {
            manage_subadmin subadmin = new manage_subadmin();


            if (id != null && id != "")
            {
                subadmin = db.getsubadminById(id);
            }
            return View(subadmin);
        }
        [HttpPost]
        public ActionResult insert_subadmin(manage_subadmin subadmin)
        {
            var path = System.IO.Path.Combine(Server.MapPath("~/tempimage/"));


            HttpPostedFileBase file1 = Request.Files["fileupload1"];

            string uploadpayslipss1;

            if (file1 != null && file1.FileName.ToString() != "")
            {
                uploadpayslipss1 = DateTime.Now.ToString("ddMMyy") + System.Guid.NewGuid() + "." + file1.FileName.Split('.')[1];
                file1.SaveAs(path + uploadpayslipss1);
                subadmin.File_Path = "/tempimage/" + uploadpayslipss1;

            }

            var dd = db.insert_subadmin(subadmin);

            if (dd == true)
            {
                TempData["Message"] = "subadmin Submitted Successfully";
                TempData["para"] = "true";
            }
            else
            {
                TempData["Message"] = "Please Review Your Input Details!!";
                TempData["para"] = "false";
            }


            return RedirectToAction("add_subadmin");
        }
        public ActionResult view_subadmin()
        {
            List<manage_subadmin> subadminlist = new List<manage_subadmin>();
            subadminlist = db.getsubadminlist();
            return View(subadminlist);
        }


        public ActionResult delet_subadmin(string id)
        {

            db.delete_subadmin(id);


            return RedirectToAction("view_subadmin");
        }

        #endregion

        #region manage sequence No


        public ActionResult add_sequence(string id)
        {

            manage_sequence sequence = new manage_sequence();


            if (id != null && id != "")
            {
                sequence = db.getsequenceById(id);
            }

            return View(sequence);
        }
        [HttpPost]
        public ActionResult insert_sequence(manage_sequence sequence)
        {

            var dd = db.insert_sequence(sequence);

            if (dd == true)
            {
                TempData["Message"] = "Sequence Submitted Successfully";
                TempData["para"] = "true";
            }
            else
            {
                TempData["Message"] = "Please Review Your Input Details!!";
                TempData["para"] = "false";
            }

            return RedirectToAction("add_sequence");
        }

        public ActionResult view_sequence()
        {
            List<manage_sequence> sequencelist = new List<manage_sequence>();
            sequencelist = db.get_sequence_list();
            return View(sequencelist);
        }


        public ActionResult delet_sequence(string id)
        {

            db.delete_sequence(id);


            return RedirectToAction("view_sequence");
        }

        #endregion

        #region manage product master

        public ActionResult add_product(string id)
        {
            manage_product product = new manage_product();

            if (id != null && id != "")
            {
                product = db.getproductbyId(id);
            }
            return View(product);
        }

        [HttpPost]
        public ActionResult insert_products(manage_product products)
        {


            var path = System.IO.Path.Combine(Server.MapPath("~/tempimage/"));


            HttpPostedFileBase file1 = Request.Files["fileupload1"];

            string uploadpayslipss1;

            if (file1 != null && file1.FileName.ToString() != "")
            {
                uploadpayslipss1 = DateTime.Now.ToString("ddMMyy") + System.Guid.NewGuid() + "." + file1.FileName.Split('.')[1];
                file1.SaveAs(path + uploadpayslipss1);
                products.File_Path = "/tempimage/" + uploadpayslipss1;

            }

            var dd = db.insert_product(products);

            if (dd == true)
            {
                TempData["Message"] = "product Submitted Successfully";
                TempData["para"] = "true";
            }
            else
            {
                TempData["Message"] = "Please Review Your Input Details!!";
                TempData["para"] = "false";
            }


            return RedirectToAction("add_product");
        }
        public ActionResult view_product()
        {
            List<manage_product> products = new List<manage_product>();
            products = db.getproductlist();
            return View(products);
        }


        public ActionResult delet_products(string id)
        {

            db.delete_product(id);


            return RedirectToAction("view_product");
        }

        #endregion

        #region manage vendor master

        public ActionResult add_vendor(string id)
        {
            manage_vendor vendor = new manage_vendor();
            if (id != null && id != "")
            {
                vendor = db.getvendorById(id);
            }

            return View(vendor);
        }
        [HttpPost]
        public ActionResult insert_vendor(manage_vendor vendor)
        {


            var dd = db.insert_vendor(vendor);

            if (dd == true)
            {
                TempData["Message"] = "subadmin Submitted Successfully";
                TempData["para"] = "true";
            }
            else
            {
                TempData["Message"] = "Please Review Your Input Details!!";
                TempData["para"] = "false";
            }

            return RedirectToAction("add_vendor");
        }

        public ActionResult view_vendor()
        {
            List<manage_vendor> vendor = new List<manage_vendor>();
            vendor = db.getvendorlist();
            return View(vendor);
        }


        public ActionResult delet_vendor(string id)
        {

            db.delete_vendor(id);


            return RedirectToAction("view_vendor");
        }

        #endregion

        #region manage sale book

        public ActionResult add_salebook(string id)
        {
            manage_salebook salebook = new manage_salebook();
            if (id != null && id != "")
            {
                salebook = db.getsalebookbyId(id);
            }
            return View(salebook);

        }

        [HttpPost]
        public ActionResult insert_salebook(manage_salebook salebook)
        {


            var dd = db.insert_salebook(salebook);

            if (dd == true)
            {
                TempData["Message"] = "salebook Submitted Successfully";
                TempData["para"] = "true";
            }
            else
            {
                TempData["Message"] = "Please Review Your Input Details!!";
                TempData["para"] = "false";
            }

            return RedirectToAction("add_salebook");
        }

        public ActionResult view_salebook()
        {
            List<manage_salebook> salebook = new List<manage_salebook>();
            salebook = db.getsalebooklist();
            return View(salebook);
        }


        public ActionResult delet_salebook(string id)
        {

            db.delete_salebook(id);


            return RedirectToAction("view_salebook");
        }


        #endregion

        #region manage PI

        public ActionResult add_pi(string id)
        {

            manage_pi pi = new manage_pi();
            if (id != null && id != "")
            {
                pi = db.getPIbyId(id);
            }
            return View(pi);
        }
        [HttpPost]
        public ActionResult insert_pi(manage_pi pi)
        {

            var dd = db.insert_PI(pi);

            if (dd == true)
            {
                TempData["Message"] = "PI Submitted Successfully";
                TempData["para"] = "true";
            }
            else
            {
                TempData["Message"] = "Please Review Your Input Details!!";
                TempData["para"] = "false";
            }

            return RedirectToAction("add_pi");
        }

        public ActionResult view_pi()
        {
            List<manage_pi> pi = new List<manage_pi>();
            pi = db.getPIlist();
            return View(pi);
        }


        public ActionResult delet_pi(string id)
        {

            db.delete_PI(id);


            return RedirectToAction("view_pi");
        }

        #endregion

        #region manage PO

        public ActionResult add_po(string id)
        {
            manage_po po = new manage_po();


            if (id != null && id != "")
            {
                po = db.getPObyId(id);
            }


            return View(po);
        }
        [HttpPost]
        public ActionResult insert_po(manage_po po)
        {

            var dd = db.insert_PO(po);

            if (dd == true)
            {
                TempData["Message"] = "PO Submitted Successfully";
                TempData["para"] = "true";
            }
            else
            {
                TempData["Message"] = "Please Review Your Input Details!!";
                TempData["para"] = "false";
            }

            return RedirectToAction("add_po");
        }

        public ActionResult view_po()
        {
            List<manage_po> polist = new List<manage_po>();
            polist = db.getPOlist();
            return View(polist);
        }

        public ActionResult delet_po(string id)
        {
            db.delete_PO(id);
            return RedirectToAction("view_po");
        }


        #endregion

        #region manage Quotation

        public ActionResult add_quotation(string LeadId, string id)
        {


            ViewBag.LeadId = LeadId;

            manage_quotation quot = new manage_quotation();

            lead_mangement leadForQuot = new lead_mangement();

            //  var leadforQuotes=;

            if (LeadId != "" && LeadId != null)
            {
                leadForQuot = db.getassingedLeadByLeadId(LeadId);

                quot.Contact_Person_No = leadForQuot.ContactMobileNumber;
                quot.Client_Name = leadForQuot.ContactPerson;
                quot.Email = leadForQuot.ContactEmail;
                quot.Lead_Reference = leadForQuot.LeadID;
                quot.Address = leadForQuot.Address;
                quot.Date = leadForQuot.LeadGenerationDate;

            }

            if (id != null && id != "")
            {
                quot = db.getQuotationbyId(id);
            }


            return View(quot);
        }


        //public ActionResult leadforQuot(string LeadId)
        //{



        //     lead_mangement leadForQuot = new lead_mangement();

        //  //  var leadforQuotes=;

        //   if(LeadId!="" && LeadId!=null)
        //    {
        //        leadForQuot = db.getassingedLeadByLeadId(LeadId);


        //    }

        //   // var leadforQuotes = db.getassingedLeadByLeadId(LeadId);


        //    return Json(leadForQuot, JsonRequestBehavior.AllowGet);


        //}

        [HttpPost]
        public ActionResult insert_quotation(manage_quotation quot)
        {
            var loginId = Session["employeeId"];
            var sendermail = Session["loginuseremail"].ToString();
            var recipientmail = quot.Email.ToString();
            quot.GeneratedBy = loginId.ToString();
            var dd = db.insert_Quotation(quot);


            // Create SmtpClient and configure it
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("rashmiguptarg121@gmail.com", "cgkq kdvz zgwx sxum"),
                EnableSsl = true,
            };

            // Create MailMessage and configure it
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("rashmiguptarg121@gmail.com"),
                Subject = "Quotation Generated",
                Body = "Hello " + quot.Client_Name + ", your quotation has been generated. This is your quotation number: " + quot.Quotation_No + ". Quotation created by: " + sendermail
            };

            // Add recipient email address
            mailMessage.To.Add(new MailAddress(recipientmail));

            // Send the email
            smtpClient.Send(mailMessage);

            //MailMessage mail = new MailMessage();

            //// Set the sender address
            //// mail.From = new MailAddress(sendermail);
            //// mail.From = new MailAddress("udayprakash2895@gmail.com");

            //// Set the recipient address
            ////  mail.To.Add(new MailAddress(recipientmail));
            //mail.To.Add(new MailAddress("udayprakash2895@gmail.com"));

            //// Set the subject and body of the email
            //mail.Subject = "Quotation Generated";
            //mail.Body = "hello " + quot.Client_Name + " your quotation  generated and this is your quotation number " + quot.Quotation_No + "quotation created by :-" + sendermail;

            //// Create SmtpClient and send the email
            //using (SmtpClient smtpClient = new SmtpClient())
            //{
            //    smtpClient.Send(mail);
            //}





            if (dd == true)
            {
                TempData["Message"] = "Quotation Submitted Successfully";
                TempData["para"] = "true";
            }
            else
            {
                TempData["Message"] = "Please Review Your Input Details!!";
                TempData["para"] = "false";
            }
            var leadId = quot.Lead_Reference;

            List<manage_quotation> quotlist = new List<manage_quotation>();
            quotlist = db.getQuotationlistwithversion(leadId);
            return Json(quotlist, JsonRequestBehavior.AllowGet);
            //  return RedirectToAction("add_quotation");
        }



        [HttpGet]
        public ActionResult allquotationbyLeadRefernce(string leadId)
        {

            List<manage_quotation> quotlist = new List<manage_quotation>();
            quotlist = db.getQuotationlistwithversion(leadId);
            return Json(quotlist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult view_quotation()
        {
            var loginId = Session["employeeId"].ToString();
            var logintype = Session["usertype"].ToString();
            List<manage_quotation> quotlist = new List<manage_quotation>();
            quotlist = db.getQuotationlistGeneratedBy(loginId, logintype);
            return View(quotlist);
        }
        [HttpGet]
        public ActionResult view_quotationdeatailsbyId(string id)
        {
            var quot = db.getQuotationbyId(id);
            return Json(quot, JsonRequestBehavior.AllowGet);
        }
        public ActionResult delet_quotation(string id)
        {
            db.delete_Quotation(id);
            return RedirectToAction("view_quotation");
        }


        #endregion

        #region countrystatecitybind
        public ActionResult StateBind(string countryId)
        {
            List<SelectListItem> stateList = new List<SelectListItem>();
            string query = "SELECT * FROM tbl_state WHERE country_id = " + countryId;
            DataSet ds = SqlHelper.ExecuteDataset(CommandType.Text, query);

            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    stateList.Add(new SelectListItem
                    {
                        Text = dr["state_name"].ToString(),
                        Value = dr["state_id"].ToString()
                    });
                }
            }

            return Json(stateList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CityBind(string stateId)
        {
            List<SelectListItem> cityList = new List<SelectListItem>();
            string query = "SELECT * FROM tbl_city WHERE state_id = " + stateId;
            DataSet ds = SqlHelper.ExecuteDataset(CommandType.Text, query);

            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    cityList.Add(new SelectListItem
                    {
                        Text = dr["city_name"].ToString(),
                        Value = dr["city_id"].ToString()
                    });
                }
            }

            return Json(cityList, JsonRequestBehavior.AllowGet);
        }

        public void countrybind()
        {
            List<SelectListItem> countrylist = new List<SelectListItem>();

            string str = "select * from tbl_countrybind ";
            DataSet ds = SqlHelper.ExecuteDataset(CommandType.Text, str);


            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    countrylist.Add(new SelectListItem
                    {
                        Text = dr["country_name"].ToString(),
                        Value = dr["country_id"].ToString()
                    });
                }
            }
            ViewBag.Country = countrylist;
            //return Json(countrylist, JsonRequestBehavior.AllowGet);
        }
       
        // transfer leadsheet lead before generated

        public ActionResult transferLead(string agentId, string LeadId, string userType)
        {


            var assignLead = new assignLead();

            var loginId = Session["employeeId"].ToString();

            //var uploadedbytype = Session["usertype"].ToString();
            var uploadedbytype = userType;


            switch (uploadedbytype)
            {
                case "admin":
                    assignLead.assignStatus = "1";
                    break;
                case "sales":
                    assignLead.assignStatus = "2";
                    break;
                case "agent":
                    assignLead.assignStatus = "3";
                    break;
                default:
                    return RedirectToAction("viewleadsheetlist");

            }



            assignLead.assignBy_Id = loginId;
            assignLead.agentId = agentId;
            assignLead.AssignLead_Id = LeadId;

            db.transferLead(assignLead);

            return Json(assignLead, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}
