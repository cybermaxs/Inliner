using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Inliner
{
    /// <summary>
    /// Tools for bundles/files resolution.
    /// </summary>
    internal class AssetResolver : IAssetResolver
    {
        public AssetResolver(IBundleManager bundleManager)
        {
            BundleManager = bundleManager;
            VirtualPathUtils = new VirtualPathHelper(bundleManager.VirtualPathProvider);
        }

        private readonly VirtualPathHelper VirtualPathUtils;
        private readonly IBundleManager BundleManager;

        public IEnumerable<Asset> ResolveUrls(string[] vpaths)
        {
            if (vpaths == null || vpaths.Length == 0)
                return Enumerable.Empty<Asset>();

            var assets = new List<Asset>(vpaths.Length);

            foreach (var vpath in vpaths)
            {
                if (!VirtualPathHelper.IsVirtualPath(vpath))
                    continue;
                if (BundleManager.IsBundle(vpath))
                {
                    assets.Add(new Asset(vpath, AssetType.Bundle));
                }
                // case single virtual file
                else if (VirtualPathUtils.IsVirtualFile(vpath))
                {
                    assets.Add(new Asset(vpath, GetAssetType(vpath)));
                }
                // case virtual directory
                else if (VirtualPathUtils.IsVirtualDirectory(vpath))
                {
                    foreach (var vfilepath in VirtualPathUtils.GetResourcesFiles(vpath))
                    {
                        assets.Add(new Asset("~" + vfilepath, GetAssetType(vfilepath)));
                    }
                }
            }
            return assets;
        }

        private static AssetType GetAssetType(string path)
        {
            switch (Path.GetExtension(path))
            {
                case Constants.FileExts.Js:
                    return AssetType.Script;
                case Constants.FileExts.Css:
                    return AssetType.Stylesheet;
                default:
                    return AssetType.Undefined;
            }
        }
    }
}