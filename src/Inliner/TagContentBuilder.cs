using System.Web;
using System.Web.Optimization;

namespace Inliner
{
    internal class TagContentBuilder
    {
        private static IAssetResolver _assetResolver;
        internal static IAssetResolver AssetResolver
        {
            get
            {
                return _assetResolver ?? new AssetResolver(BundleManager);
            }
            set
            {
                _assetResolver = value;
            }
        }

        private static IBundleManager _bundleManager;
        public static IBundleManager BundleManager
        {
            get
            {
                return _bundleManager ?? new DefaultBundleManager();
            }
            set
            {
                _bundleManager = value;
            }
        }



        public static TagContent Merge(string[] paths)
        {
            var response = new TagContent();
            var assets = AssetResolver.ResolveUrls(paths);

            foreach (var asset in assets)
            {
                var bundleResponse = BundleManager.GetBundleResponse(asset);
                response.Append(asset.VirtualPath, bundleResponse.Content);
            }

            return response;
        }
    }
}
