using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Text2Buxfer.Models;
using Text2Buxfer.Services;
using Microsoft.AspNet.Identity;

namespace Text2Buxfer.Controllers
{
    public class TransactionController : Controller
    {
        
        [Authorize]
        public ActionResult New()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> New(Transaction model)
        {
            Buxfer api = new Buxfer();
            var userId = Crypto.Decrypt(User.Identity.GetUserId(), "t2b20130708").Split('|');
            
            if (await api.Login(userId[0], userId[1]))
            {
                bool success = await api.AddTransaction(model.Text);
                if (success)
                    return RedirectToAction("Success");    
            }

            return RedirectToAction("Fail");
        }

        public ActionResult Success()
        {
            return View();
        }

        public ActionResult Fail()
        {
            return View();
        }
	}
}