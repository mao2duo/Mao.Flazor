using Mao.Flazor.Features.Services;
using Mao.Flazor.Views.Home;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;

namespace Mao.Flazor.Controllers
{
    public class HomeController : Controller
    {
        private readonly DemoService _demoService;

        public HomeController(DemoService demoService)
        {
            _demoService = demoService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Demo01()
        {
            return View();
        }
        public ActionResult Demo02()
        {
            return View();
        }
        public ActionResult Demo03()
        {
            return View();
        }
        public ActionResult Demo04()
        {
            return View();
        }
        public ActionResult Demo05(string primaryValue)
        {
            var viewModel = new Demo05_ViewModel();
            viewModel.PrimaryValue = primaryValue;
            viewModel.SecondaryOptions = _demoService.GetSecondaryOptions(primaryValue);
            return View(viewModel);
        }
    }
}