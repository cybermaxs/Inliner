using System.Collections.Generic;
using System.Web.Hosting;
using System.Web.Optimization;

namespace Inliner
{
    internal interface IAssetResolver
    {
        IEnumerable<Asset> ResolveUrls(string[] paths);
        Bundle GetBundle(string vpath);
        VirtualFile GetVirtualFile(string vpath);
    }
}
