using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Graph;
using GraphFilesWeb.Helpers;
using GraphFilesWeb.Services;

namespace GraphFilesWeb.Controllers
{
    public class FilesController : Controller
    {
        GraphService graphService = new GraphService();

        [Authorize]
        public async Task<ActionResult> Index(int? pageSize, string nextLink)
        {
            try
            {
                GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();
                var results = await graphService.GetMyFiles(graphClient, pageSize, nextLink);
                if (null != results.NextPageRequest)
                {
                    ViewBag.NextLink = results.NextPageRequest.GetHttpRequestMessage().RequestUri;
                }
                else
                {
                    ViewBag.NextLink = null;
                }
                return View(results);
            }
            catch (ServiceException se)
            {
                if (se.Error.Message == "Error_AuthChallengeNeeded") return new EmptyResult();
                return RedirectToAction("Index", "Error", new { message = se.Error.Message + Request.RawUrl + ": " + se.Error.Message });
            }
        }
        [Authorize]
        public async Task<ActionResult> Delete(string itemId, string etag)
        {
            // Initialize the GraphServiceClient.
            GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();
            await graphService.DeleteFile(graphClient, itemId, etag);
            return Redirect("/Files");
        }

        [Authorize]
        public async Task<ActionResult> Upload()
        {
            GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();

            foreach (string key in Request.Files)
            {
                var fileInRequest = Request.Files[key];
                if (fileInRequest != null && fileInRequest.ContentLength > 0)
                {
                    var filename = System.IO.Path.GetFileName(fileInRequest.FileName);
                    await graphService.UploadFile(graphClient, filename, fileInRequest.InputStream);
                }
            }
            return Redirect("/Files");
        }
    }
}