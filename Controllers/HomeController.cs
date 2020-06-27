using Akeem.Web.Tools.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Akeem.Web.Tools.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {

        private readonly IOptions<ShortUrlSetting> options;

        public ToolsContext ToolsContext { get; }

        public HomeController(ToolsContext toolsContext, IOptions<ShortUrlSetting> options)
        {
            this.ToolsContext = toolsContext;
            this.options = options;
        }

        [HttpGet("/Home/Compress")]
        public IActionResult Compress()
        {
            return View();
        }

        [HttpPost("/Home/Compress")]
        public async Task<IActionResult> Compress(ToolShortUrl urlModel)
        {
            if (string.IsNullOrEmpty(urlModel.Url))
            {
                return BadRequest();
            }

            ToolShortUrl firstModel = ToolsContext.ToolShortUrl.FirstOrDefault(item => urlModel.Url.Equals(item.Url));
            if (firstModel == null)
            {
                string hash = MurmurHashUtil.ToHash(urlModel.Url);
                firstModel = new ToolShortUrl()
                {
                    Compress = hash,
                    ExpiredTime = DateTime.Now.AddYears(1),
                    Url = urlModel.Url,
                    CreTime = DateTime.Now
                };
                var result = await ToolsContext.AddAsync(firstModel);
                if (await ToolsContext.SaveChangesAsync() == 0)
                {
                    return BadRequest();
                }
            }

            return Json(new
            {
                firstModel.Compress,
                firstModel.ExpiredTime,
                options.Value.BaseUrl,
                complete = options.Value.BaseUrl + firstModel.Compress
            });
        }

        [HttpGet("/{id}")]
        public async Task<IActionResult> GetAsync(string id)
        {
            ToolShortUrl urlModel = ToolsContext.ToolShortUrl.FirstOrDefault(item => id.Equals(item.Compress));
            if (urlModel != null)
            {
                try
                {
                    ToolShortUrlReport urlReport = ToolsContext.ToolShortUrlReport.FirstOrDefault(item => item.Id == urlModel.Id);
                    if (urlReport == null)
                    {
                        urlReport = new ToolShortUrlReport()
                        {
                            Id = urlModel.Id,
                            WatchNum = 1
                        };
                        _ = await ToolsContext.ToolShortUrlReport.AddAsync(urlReport);
                    }
                    else
                    {
                        urlReport.WatchNum++;
                    }
                    _ = await ToolsContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                }

                return Redirect(urlModel.Url);
            }
            return RedirectToAction("Error_404", "Home");
        }


        [HttpGet("/")]
        public IActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View();
            }
            return View();
        }
        [HttpGet("/Home/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet("/404")]
        public IActionResult Error_404()
        {
            return View();
        }
    }
}
