using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudgenProject.Models.admin
{
    public class manage_folloupLeadResponse
    {
        public string id { get; set; }
        public string AgentId  { get; set; }
        public string LeadId { get; set; }
        public string response { get; set; }
        public string leadType { get; set; }
        public string current_date { get; set; }
        public string nextfollow_up_date { get; set; }
        public string status { get; set; }

    }

    public class todayfollowupmodel
    {
        public string id { get; set; }
        public string leadId { get; set; }
        public string AgentId { get; set; }
        public string Client { get; set; }
        public string contactemail { get; set; }
        public string contac_no { get; set; }
        public string product { get; set; }
        public string response { get; set; }
        public string leadType { get; set; }
        public string assignToName { get; set; }
        public string assignByName { get; set; }
        public string nextfollow_up_date { get; set; }
        public string status { get; set; }
        public string assignstatus { get; set; }
        public string ContactPerson { get; set; }
        public string address { get; set; }
       
       

    }


}