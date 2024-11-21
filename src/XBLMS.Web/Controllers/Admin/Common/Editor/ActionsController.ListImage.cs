using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XBLMS.Configuration;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Common.Editor
{
    public partial class ActionsController
    {
        [HttpGet, Route(RouteActionsListImage)]
        public ActionResult<ListImageResult> ListImage([FromQuery] ListImageRequest request)
        {
            if (_settingsManager.IsSafeMode)
            {
                return new ListImageResult
                {
                    State = Constants.ErrorSafe,
                    Size = 0,
                    Start = 0,
                    Total = 0,
                    List = null
                };
            }

            var directoryPath =  _pathManager.GetEditUploadFilesPath();

            var files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories).Where(x =>
                _pathManager.IsImageExtensionAllowed(PathUtils.GetExtension(x))).OrderByDescending(x => x);

            var list = new List<ImageResult>();
            foreach (var x in files.Skip(request.Start).Take(request.Size))
            {
                list.Add(new ImageResult
                {
                    Url =  _pathManager.GetRootUrlByPath(x)
                });
            }

            return new ListImageResult
            {
                State = "SUCCESS",
                Size = request.Size,
                Start = request.Start,
                Total = files.Count(),
                List = list
            };
        }
    }
}
