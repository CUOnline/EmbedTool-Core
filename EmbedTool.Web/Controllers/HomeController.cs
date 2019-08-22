using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using EmbedTool.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;

namespace EmbedTool.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment environment;
        private readonly AppSettings settings;

        public HomeController(IHostingEnvironment environment, IOptions<AppSettings> options)
        {
            this.environment = environment;
            this.settings = options.Value;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel();
            model.BaseUrl = settings.BaseUrl;
            return View(model);
        }

        [FormatFilter]
        public XmlDocument Config(string url, string title, bool? accountNavigation, bool? courseNavigation, bool? userNavigation)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load($"{environment.WebRootPath}/content/lti_config.xml");

            doc.GetElementsByTagName("blti:title")[0].InnerText = string.IsNullOrWhiteSpace(title) ? "Embed Tool" : title;
            doc.GetElementsByTagName("blti:launch_url")[0].InnerText = $"{settings.BaseUrl}Home/Config";
            GetNodeByName(doc.ChildNodes, "url").InnerText = url + "?embedded=true";
            GetNodeByName(doc.ChildNodes, "domain").InnerText = $"{settings.BaseUrl}";

            if (accountNavigation != true)
            {
                var node = GetNodeByName(doc.ChildNodes, "account_navigation");
                node.ParentNode.RemoveChild(node);
            }

            if (courseNavigation != true)
            {
                var node = GetNodeByName(doc.ChildNodes, "course_navigation");
                node.ParentNode.RemoveChild(node);
            }

            if (userNavigation != true)
            {
                var node = GetNodeByName(doc.ChildNodes, "user_navigation");
                node.ParentNode.RemoveChild(node);
            }

            HttpContext.Response.ContentType = "application/xml";
            return doc;
        }

        [HttpPost]
        public IActionResult Config()
        {
            var ltiParams = ParseLtiParams(HttpContext.Request.Form);
            
            // This is required for the LTI app to respond without a 403 error.
            Response.Headers.Add("X-Frame-Options", $"ALLOW-FROM {settings.CanvasBaseUrl}");
            return Redirect(ltiParams["custom_url"]);
        }

        private XmlNode GetNodeByName(XmlNodeList nodes, string name)
        {
            foreach (XmlNode node in nodes)
            {
                if (node.HasChildNodes)
                {
                    var xmlNode = GetNodeByName(node.ChildNodes, name);
                    if (xmlNode != null)
                    {
                        return xmlNode;
                    }
                }

                if (node.Attributes != null)
                {
                    XmlAttributeCollection attrs = node.Attributes;
                    foreach (XmlAttribute attr in attrs)
                    {
                        if (attr.Name == "name" && attr.Value == name)
                        {
                            return node;
                        }
                    }
                }
            }

            return null;
        }

        private bool IsValidLtiRequest(HttpRequest request, Dictionary<string, string> ltiParams)
        {
            // Write later
            return true;
        }

        private Dictionary<string, string> ParseLtiParams(IFormCollection form)
        {
            Dictionary<string, string> LtiParams = new Dictionary<string, string>();

            if (form != null && form.Count > 0)
            {
                foreach (var pair in form)
                {
                    LtiParams.Add(pair.Key, pair.Value);
                }
            }

            return LtiParams;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
