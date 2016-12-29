using Kendo.DynamicLinq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Text;
using System.Security.Principal;
using System.Threading;
using SimpleImpersonation;
using MannLabels.Models;
using System.Diagnostics;

namespace MannLabels.Controllers
{
    public class LabelsController : ApiController
    {

        [Route("api/Labels/POSTLogin")]
        public IHttpActionResult POSTLogin(UserLoginModel login)
        {
            using (LabelPrintModels db = new LabelPrintModels())
            {
                UserList ul = db.UserLists.FirstOrDefault(p => p.Logon == login.LogonName);
                if (ul != null)
                {
                    if (ul.Password == login.Passworrd)
                    {
                        return Ok(ul.Printer1.Printer1);
                    }
                    else
                    {
                        return BadRequest("User Name or Password incorrect");
                    }
                }
                else
                {
                    return BadRequest("User Name or Password incorrect");
                }
            }
        }

        ////GET api/Labels/GetItemsList
        //[Route("api/Labels/GetItemsList")]
        //public IHttpActionResult GetItemsList(HttpRequestMessage requestMessage)
        //{
        //    DataSourceRequest request = JsonConvert.DeserializeObject<DataSourceRequest>(
        //    requestMessage.RequestUri.ParseQueryString().GetKey(0));
        //    using (LabelPrintModels db = new LabelPrintModels())
        //    {
        //        DataSourceResult dr = db.ItemsMasters.OrderBy(p => p.ItemNumer)
        //            .Select(p => new ItemsMasterModel
        //            {
        //                BrandAbbrv = p.BrandAbbrv,
        //                BrandFull = p.BrandFull,
        //                GTIN = p.GTIN,
        //                ItemDesc = p.ItemDesc,
        //                ItemFull = p.ItemFull,
        //                WalmartCode = p.WalmartCode
        //            }).ToDataSourceResult(request.Take, request.Skip, request.Sort, request.Filter);

        //        return Ok(dr);
        //    }
        //}

        //GET api/Labels/GetItemsList
        [Route("api/Labels/GetJdeItemsList")]
        public IHttpActionResult GetJdeItemsList(HttpRequestMessage requestMessage)
        {
            DataSourceRequest request = JsonConvert.DeserializeObject<DataSourceRequest>(
            requestMessage.RequestUri.ParseQueryString().GetKey(0));
            if (request.Filter != null)
            {
                foreach (Filter f in request.Filter.Filters)
                {
                    if (f.Field == "ItemFull")
                    {
                        f.Value = f.Value.ToString().PadLeft(5, '0');
                    }
                }
            }

            using (JDE_PRODUCTIONEntities db = new JDE_PRODUCTIONEntities())
            {
                //db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
                DataSourceResult dr = (from x in db.F4101
                    where x.IMLITM.Length == 5
                    && x.IMSRP4 != ""
                    && x.IMSRP8.ToLower() == "fgd"
                    && x.IMAITM.Length >= 14
                    orderby x.IMLITM
                    select new ItemsMasterModel
                    {
                        ItemFull = x.IMLITM.Trim(),
                        BrandAbbrv = x.IMSRP4.Trim(),
                        BrandFull = (from z in db.F0005 where z.DRSY == "41" && z.DRRT.ToUpper() == "S4" && z.DRKY.Trim() == x.IMSRP4.Trim() select z.DRDL01.Trim()).FirstOrDefault(),
                        ItemDesc = x.IMSRTX.Trim(),
                        GTIN = x.IMAITM.Trim().Substring(0, 14),
                        WalmartCode = (from y in db.F4104
                            where y.IVLITM.Trim() == x.IMLITM && y.IVXRT.ToUpper() == "UC"
                            select y.IVCITM.Trim()).FirstOrDefault() != null ? (from y in db.F4104
                                where y.IVLITM.Trim() == x.IMLITM && y.IVXRT.ToUpper() == "UC"
                                select y.IVCITM.Trim()).FirstOrDefault() : ""
                        //WalmartCode = x.IMSRP4.Trim() == "WM" ? ((from y in db.F4104 //BG removed Wal-Mart filter
                        //    where y.IVLITM.Trim() == x.IMLITM && y.IVXRT.ToUpper() == "UC"
                        //    select y.IVCITM.Trim()).FirstOrDefault() != null ? (from y in db.F4104
                        //        where y.IVLITM.Trim() == x.IMLITM && y.IVXRT.ToUpper() == "UC"
                        //        select y.IVCITM.Trim()).FirstOrDefault() : "") : ""
                    }).ToDataSourceResult(request.Take, request.Skip, request.Sort, request.Filter);

                return Ok(dr);
            }
        }

        [Route("api/Labels/GetCountryList")]
        public IHttpActionResult GetCountryList()
        {
            List<object> retVals = new List<object>();
            using (LabelPrintModels db = new LabelPrintModels())
            {
                var cntris = db.COO_List.Select(p =>
                new CountriesModel
                {
                    idx = p.idx,
                    Abbrv = p.Abbrv,
                    LongName = p.LongName
                }).ToDataSourceResult(0, 0, null, null);
                retVals.Add(cntris);

                //OBSOLETED
                //var items = db.ItemsMasters.OrderBy(p => p.ItemNumer)
                //    .Select(p => new ItemsMasterModel
                //    {
                //        BrandAbbrv = p.BrandAbbrv,
                //        GTIN = p.GTIN,
                //        BrandFull = p.BrandFull,
                //        ItemDesc = p.ItemDesc,
                //        ItemFull = p.ItemFull,
                //    }).ToDataSourceResult(0, 0, null, null);
                //retVals.Add(items);

                var srcAddresses = db.SrcAddresses.OrderBy(p => p.Address)
                    .Select(p => new
                    {
                        idx = p.idx,
                        Addrs = p.Address
                    }).ToDataSourceResult(0, 0, null, null);
                retVals.Add(srcAddresses);
            }

            using (JDE_PRODUCTIONEntities db = new JDE_PRODUCTIONEntities())
            {
                var brands = db.F4101
                    .OrderBy(f=>f.IMSRP4)
                    .Where(f=>f.IMLITM.Length == 5
                    && f.IMSRP4 != ""
                    && f.IMSRP8.ToLower() == "fgd"
                    && f.IMAITM.Length >= 14)
                    .Select(p => p.IMSRP4.Trim())
                    .Distinct().ToList();
                retVals.Add(brands);
            }

            return Ok(retVals);
        }

        [Route("api/Labels/GetPrinterList")]
        public IHttpActionResult GetPrinterList(PrinterModel prn)
        {
            LabelPrintModels db = new LabelPrintModels();

            try
            {
                var prins = db.Printers.OrderBy(p => p.idx)
                    .Where(p => p.idx > 1) //1 is reserved for developer test device
                    .Select(p => new PrinterModel
                    {
                        Idx = p.idx,
                        Printer = p.Printer1
                    }).ToList();
                return Ok(prins);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                db.Dispose();
            }
        }

        [Route("api/Labels/PostRegisterUser")]
        public IHttpActionResult PostRegisterUser(UserListModel dat)
        {
            LabelPrintModels db = new LabelPrintModels();

            var testLogon = db.UserLists.FirstOrDefault(p => p.Logon == dat.Logon);
            if (testLogon != null)
            {
                return BadRequest("Logon name is already in use, please try a different one");
            }

            var testLogon2 = db.UserLists.FirstOrDefault(p => p.Email == dat.Email);
            if (testLogon2 != null)
            {
                return BadRequest("Account with this email already exists, call 2628 if you need to recover user name and password");
            }

            if (dat.Email.ToLower().IndexOf("@mannpacking.com") == -1)
            {
                if (dat.Email.ToLower().IndexOf("@freshleaffarms.com") == -1)
                {
                    return BadRequest("Email must be a Mann Packing Or FreshLeafFarms email address");
                }
            }

            UserList ul = new UserList();
            ul.Email = dat.Email;
            ul.Logon = dat.Logon;
            ul.Password = dat.Password;
            ul.Printer = dat.Printer;

            Printer pr = db.Printers.FirstOrDefault(p => p.idx == dat.Printer);

            db.UserLists.Add(ul);
            try
            {
                db.SaveChanges();
                return Ok(pr.Printer1);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                db.Dispose();
            }
        }

        [Route("api/Labels/PostPrintLabel")]
        public IHttpActionResult PostPrintLabel(LabelToPrintModel dat)
        {
            LabelPrintModels db = new LabelPrintModels();
            
            DateTime dt = (DateTime)dat.SellOrUseBy;
            string shift = dat.Shift;
            string fullItem = dat.Id.ToString().PadLeft(5, '0');
            bool incrementJulian = dat.JulianPlusOne; //this is new
            string fakeJulian = incrementJulian ? (DateTime.Today.DayOfYear + 1).ToString() : DateTime.Today.DayOfYear.ToString();

            using (JDE_PRODUCTIONEntities dbJDcn = new JDE_PRODUCTIONEntities())
            {
                ItemsMasterModel im = dbJDcn.F4101
                    .Where(p => p.IMLITM == fullItem)
                    .Select(p => new ItemsMasterModel
                    {
                        ItemFull = p.IMLITM.Trim(),
                        ItemDesc = p.IMDSC1.Trim(),
                        BrandAbbrv = p.IMSRP4.Trim(),
                        BrandFull = (from c in dbJDcn.F0005
                                     where c.DRSY == "41"
                                     && c.DRRT.ToUpper() == "S4"
                                     && c.DRKY.Trim() == p.IMSRP4.Trim()
                                     select c.DRDL01.Trim()).FirstOrDefault(),
                        GTIN = p.IMAITM.Trim(),
                        WalmartCode = (from y in dbJDcn.F4104
                                        where y.IVLITM.Trim() == p.IMLITM && y.IVXRT.ToUpper() == "UC"
                                        select y.IVCITM.Trim()).FirstOrDefault() != null ? (from y in dbJDcn.F4104
                                                                                            where y.IVLITM.Trim() == p.IMLITM && y.IVXRT.ToUpper() == "UC"
                                                                                            select y.IVCITM.Trim()).FirstOrDefault() : ""
                        //WalmartCode = p.IMSRP4.Trim() == "WM" ? ((from y in dbJDcn.F4104 //BG removed WM condition
                        //                                          where y.IVLITM.Trim() == p.IMLITM && y.IVXRT.ToUpper() == "UC"
                        //                                          select y.IVCITM.Trim()).FirstOrDefault() != null ? (from y in dbJDcn.F4104
                        //                                                                                              where y.IVLITM.Trim() == p.IMLITM && y.IVXRT.ToUpper() == "UC"
                        //                                                                                              select y.IVCITM.Trim()).FirstOrDefault() : "") : ""
                    }).SingleOrDefault();

                if (im != null)
                {
                    string secondBcString = shift + im.ItemFull + fakeJulian + DateTime.Now.Year.ToString().Substring(2, 2) + dat.CooId;

                    //See if we need to use alternate label
                    AltLabelItem useAltTemplate = db.AltLabelItems.FirstOrDefault(p => p.ItemNum == fullItem);

                    string voicePickCode = VoiceCode.Compute(im.GTIN, secondBcString, null);
                    string lilDigits = voicePickCode.Substring(0, 2);
                    string bigDigits = voicePickCode.Substring(2, 2);
                    string PrintDate = useAltTemplate != null && useAltTemplate.ShowJulianNoSellby == true ? fakeJulian : dt.ToString("MMM dd");
                    bool item = im.WalmartCode != "";
                    string btwFile = item ? "Base4by2WalMartdesign" : "Base4by2design";
                    string cooo = db.COO_List.FirstOrDefault(p => p.idx == dat.CooId).LongName;
                    using (Impersonation.LogonUser("MANN", "bartender", "vodkagimlet", LogonType.Interactive))
                    {
                        StringBuilder sb = new StringBuilder();
                        //Fork if using alternate template
                        if (useAltTemplate == null)
                        {
                            sb.AppendFormat(@"%BTW% /AF=""C:\Bottomline Technologies\BarTender\Forms\{0}.btw"" /D=""%Trigger File Name%"" /PRN=""{1}"" /R=3 /P /C={2}", btwFile, dat.PrinterName, dat.Qty.ToString());
                            sb.AppendLine();
                            sb.AppendLine("%END%");
                            sb.AppendFormat("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\"",
                                im.ItemFull, im.GTIN.Substring(0, 13), cooo, PrintDate, im.ItemDesc, dat.SrcAddress, secondBcString, im.BrandFull,
                                dat.Shift, lilDigits, bigDigits, dat.UsebyLang, im.WalmartCode, dat.CrewNum);
                        }
                        else
                        {
                            if (useAltTemplate.AlterLabel == "CVF_Opt_Format")
                            {
                                sb.AppendFormat(@"%BTW% /AF=""C:\Bottomline Technologies\BarTender\Forms\{0}.btw"" /D=""%Trigger File Name%"" /PRN=""{1}"" /R=3 /P /C={2}", useAltTemplate.AlterLabel, dat.PrinterName, dat.Qty.ToString());
                                sb.AppendLine();
                                sb.AppendLine("%END%");
                                sb.AppendFormat("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\"",
                                    im.ItemFull, im.GTIN.Substring(0, 13), cooo, PrintDate, im.ItemDesc, useAltTemplate.CustProdId, secondBcString,
                                    lilDigits, bigDigits, useAltTemplate.ShowJulianNoSellby == true ? "" : dat.UsebyLang, dat.CrewNum);
                            }
                            else
                            {
                                sb.AppendFormat(@"%BTW% /AF=""C:\Bottomline Technologies\BarTender\Forms\{0}.btw"" /D=""%Trigger File Name%"" /PRN=""{1}"" /R=3 /P /C={2}", btwFile, dat.PrinterName, dat.Qty.ToString());
                                sb.AppendLine();
                                sb.AppendLine("%END%");
                                sb.AppendFormat("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\"",
                                    im.ItemFull, im.GTIN.Substring(0, 13), cooo, fakeJulian, im.ItemDesc, dat.SrcAddress, secondBcString, im.BrandFull,
                                    dat.Shift, lilDigits, bigDigits, "", im.WalmartCode, dat.CrewNum);
                            }
                        }

                        sb.AppendLine();
                        try
                        {
                            File.WriteAllText(@"\\mann-forms\Data\FileFromMannLabels.csv", sb.ToString());
                            return Ok();
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex.Message);
                        }
                        finally
                        {
                            db.Dispose();
                        }
                    }
                }
                else
                {
                    return BadRequest("Item Not found");
                }
            }
        }
    }
}
