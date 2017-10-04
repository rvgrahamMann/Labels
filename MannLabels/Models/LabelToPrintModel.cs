using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MannLabels.Models
{
    public class LabelToPrintModel
    {
        public int Id { get; set; }
        public int CooId { get; set; }
        public string UsebyLang { get; set; }
        public bool NoDate { get; set; }
        //values below for print time only
        public int Qty { get; set; }
        public DateTime? SellOrUseBy { get; set; }
        public string Shift { get; set; }
        public string CrewNum { get; set; }
        public string SrcAddress { get; set; }
        public string PrinterName { get; set; }
        public bool JulianPlusOne { get; set; }
        public bool IsWm { get; set; }
        public int UseByDays { get; set; }
        public string WalmartCode { get; set; }
    }
}