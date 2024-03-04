using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudgenProject.Models
{
    public class common_response
    {
        public string message { get; set; }
        public string parameter { get; set; }
        public Boolean success { get; set; }
        public string username { get; set; }
        public string useremail { get; set; }
        public string usertype { get; set; }
        public string employee_branch { get; set; }
        public string employeeId { get; set; }
        public string Id { get; set; }
    }

}