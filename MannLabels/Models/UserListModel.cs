using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MannLabels.Models
{
    public class UserListModel
    {
        public int idx { get; set; }
        public string Logon { get; set; }
        public string Password { get; set; }
        public int Printer { get; set; }
        public string Email { get; set; }
    }
}