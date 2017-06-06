using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GraphFilesWeb.Services
{
    public class GraphService
    {
        public async Task<IDriveItemChildrenCollectionPage> GetMyFiles(GraphServiceClient graphClient, int? pageSize, string nextLink)
        {
            pageSize = pageSize ?? 25;
            var request = graphClient.Me.Drive.Root.Children.Request().Top(pageSize.Value);
            if (nextLink != null){
                request = new DriveItemChildrenCollectionRequest(nextLink, graphClient, null);
            }
            var results = await request.GetAsync();
            return results;
        }
        public async Task DeleteFile(GraphServiceClient graphClient, string itemId, string etag)
        {

            var request = graphClient.Me.Drive.Items[itemId].Request(new List<Option> { new HeaderOption("If-Match", etag) });
            await request.DeleteAsync();

            return;
        }
        public async Task UploadFile(GraphServiceClient graphClient, string filename, Stream content)
        {
            var request = graphClient.Me.Drive.Root.Children[filename].Content.Request();
            var uploadedFile = await request.PutAsync<DriveItem>(content);
            return;
        }
    }
}