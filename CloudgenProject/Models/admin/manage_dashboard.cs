using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudgenProject.Models.admin
{
    public class manage_dashboard
    {
        public int total_employee { get; set; }
        public int total_branch { get; set;}
        public int total_lead { get; set;}
        public int total_sales_book { get; set;}
        public int total_product { get; set;}
        public int total_vendor { get; set;}
        public int total_pi { get; set;}
        public int total_po { get; set; }
        public int TotalFollowUp { get; set; }
        public int TotalTodayFollowUp { get; set; }

    }
}