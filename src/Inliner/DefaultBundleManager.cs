using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;

namespace Inliner
{
    internal class DefaultBundleManager : IBundleManager
    {
        #region HttpContext
        private static HttpContextBase _context;
        public static HttpContextBase Context
        {
            get
            {
                return _context ?? new HttpContextWrapper(HttpContext.Current);
            }
            set
            {
                _context = value;
            }
        }
        #endregion

        public bool EnableOptimization
        {
            get { return BundleTable.EnableOptimizations; }
        }

        private VirtualPathProvider _virtualPathProvider;
        public VirtualPathProvider VirtualPathProvider
        {
            get { return _virtualPathProvider ?? BundleTable.VirtualPathProvider; }
            set { _virtualPathProvider = value; }
        }

        /// <summary>
        /// Get bundle for a virtual path.
        /// </summary>
        /// <param name="asset">Virtual Path of the bundle.</param>
        /// <returns>Bundle</returns>
        public Bundle GetBundle(Asset asset)
        {
            var vpath = asset.VirtualPath;
            var bundleFor = BundleTable.Bundles.GetBundleFor(vpath);
            if (bundleFor == null)
            {
                switch (asset.Type)
                {
                    case AssetType.Script:
                        bundleFor = new ScriptBundle(vpath).Include(vpath);
                        break;
                    case AssetType.Stylesheet:
                        bundleFor = new StyleBundle(vpath).Include(vpath);
                        break;
                }
                RegisterNewBundle(bundleFor);
            }

            // when optimizations are disabled we remove all transforms
            if (bundleFor != null && !EnableOptimization && bundleFor.Transforms.Count > 0)
                bundleFor.Transforms.Clear();

            return bundleFor;
        }

        public BundleResponse GetBundleResponse(Asset asset)
        {
            var bundle = GetBundle(asset);
            var bundleContext = new BundleContext(Context, BundleTable.Bundles, asset.VirtualPath);
            var bundleResponse = bundle.CacheLookup(bundleContext);
            if (bundleResponse == null)
            {
                bundleResponse = bundle.GenerateBundleResponse(bundleContext);
                bundle.UpdateCache(bundleContext, bundleResponse);
            }
            return bundleResponse;
        }

        public bool IsBundle(string virtualPath)
        {
            return BundleTable.Bundles.GetBundleFor(virtualPath) != null;
        }

        public void RegisterNewBundle(Bundle bundle)
        {
            BundleTable.Bundles.Add(bundle);
        }
    }
}
