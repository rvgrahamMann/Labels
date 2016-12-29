using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MannLabels.Models
{
    public class ItemsMasterModel
    {
        public string ItemFull { get; set; }
        public string ItemDesc { get; set; }
        public string BrandAbbrv { get; set; }
        public string BrandFull { get; set; }
        public string GTIN { get; set; }
        public string WalmartCode { get; set; }

    }
}