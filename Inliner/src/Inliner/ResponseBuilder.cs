using System.IO;
using System.Web;
using System.Web.Optimization;

namespace Inliner
{
    internal class ResponseBuilder
    {
        private static IAssetResolver _resolver;
        internal static IAssetResolver Resolver
        {
            get
            {
                return _resolver ?? new AssetResolver();
            }
            set
            {
                _resolver = value;
            }
        }

        private static HttpContextBase _context;
        internal static HttpContextBase Context
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

        public static InlinerResponse Assemble(string[] paths)
        {
            var response = new InlinerResponse();
            var assets = Resolver.ResolveUrls(paths);

            foreach (var asset in assets)
            {
                if (asset.Type== AssetType.Bundle)
                {
                    var bundle = Resolver.GetBundle(asset.VirtualPath);
                    var bundleContext = new BundleContext(Context, BundleTable.Bundles, asset.VirtualPath);
                    var bundleResponse = GetBundleResponse(bundle, bundleContext);
                    response.Append(asset.VirtualPath, bundleResponse.Content, bundle.ConcatenationToken);
                }
                else
                {
                    var vfile = Resolver.GetVirtualFile(asset.VirtualPath);
                    string fileContents;
                    using (StreamReader streamReader = new StreamReader(vfile.Open()))
                    {
                        fileContents = streamReader.ReadToEnd();
                    }
                    var output = DefaultTransform.Instance.Process(asset.VirtualPath, fileContents);
                    response.Append(asset.VirtualPath, output.Content, output.ConcatenationToken);
                }
            }

            return response;
        }

        internal static BundleResponse GetBundleResponse(Bundle bundle, BundleContext context)
        {
            var bundleResponse = bundle.CacheLookup(context);
            if (bundleResponse == null)
            {
                bundleResponse = bundle.GenerateBundleResponse(context);
                bundle.UpdateCache(context, bundleResponse);
            }
            return bundleResponse;
        }
    }
}
