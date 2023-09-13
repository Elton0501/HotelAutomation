using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelAutomation.Web.Controllers
{
    public class ImageController : Controller
    {
        public JsonResult MultipleUploadImage()
        {
            var json = "";
            try
            {
                var file = Request.Files;
                for (int i = 0; i < file.Count; i++)
                {
                    var files = file[i];
                    var fileName = Guid.NewGuid() + Path.GetExtension(files.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                    files.SaveAs(path);
                    var imgurl = string.Format("/Content/Images/" + fileName);
                    json += imgurl + ",";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(json);
        }

        public JsonResult UploadImage()
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            try
            {
                var file = Request.Files[0];
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                file.SaveAs(path);
                result.Data = new { Success = true, ImageURL = string.Format("/Content/Images/{0}", fileName) };

            }
            catch (Exception ex)
            {
                result.Data = new { Success = false, Message = ex.Message };
            }
            return result;
        }

        public JsonResult UploadMenuImage()
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            try
            {
                var file = Request.Files[0];
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/MenuImg/"), fileName);
                file.SaveAs(path);
                result.Data = new { Success = true, ImageURL = string.Format("/Content/MenuImg/{0}", fileName) };

            }
            catch (Exception ex)
            {
                result.Data = new { Success = false, Message = ex.Message };
            }
            return result;
        }
        public JsonResult UploadPromoImage()
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            var weblink = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
            try
            {
                var file = Request.Files[0];
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/PromImg/"), fileName);
                file.SaveAs(path);
                result.Data = new { Success = true, ImageURL = string.Format(weblink + "/Content/PromImg/{0}", fileName) };

            }
            catch (Exception ex)
            {
                result.Data = new { Success = false, Message = ex.Message };
            }
            return result;
        }
    }
}