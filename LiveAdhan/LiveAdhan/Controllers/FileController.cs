using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiveAthan.Controllers
{
    public class FileController : Controller
    {
        //
        // GET: /File/

        static Random r = new Random();
        public ActionResult BackgroundImage()
        {
            int whichPicture = r.Next(2);
            if (whichPicture == 1)
            {
                return File(Server.MapPath("~/content/img/bgs/waseef_ramadan.jpg"), "image/jpeg");
            }
            else
            {
                return File(Server.MapPath("~/content/img/bgs/waseef_ramadan2.jpg"), "image/jpeg");
            }
        }

    }
}
