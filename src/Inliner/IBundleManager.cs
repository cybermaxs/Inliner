using System.Web.Hosting;
using System.Web.Optimization;

namespace Inliner
{
    interface IBundleManager
    {
        VirtualPathProvider VirtualPathProvider { get; }
        bool IsBundle(string virtualPath);
        bool EnableOptimization { get; }
        Bundle GetBundle(Asset asset);
        BundleResponse GetBundleResponse(Asset asset);
        void RegisterNewBundle(Bundle bundle);
    }
}
