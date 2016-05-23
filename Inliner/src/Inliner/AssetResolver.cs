using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;

namespace Inliner
{
    /// <summary>
    /// Tools for bundles/files resolution.
    /// </summary>
    internal class AssetResolver : IAssetResolver
    {
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

        private bool EnableOptimizations
        {
            get { return BundleTable.EnableOptimizations; }
        }
        public IEnumerable<Asset> ResolveUrls(string[] vpaths)
        {
            var assets = new List<Asset>();

            foreach (var vpath in vpaths)
            {
                //bundle
                if (IsBundle(vpath))
                {
                    if(EnableOptimizations)
                        assets.Add(new Asset { VirtualPath = vpath, Type = AssetType.Bundle });
                    else
                    {
                        var bundleFor = GetBundle(vpath);
                        var bundleContext = new BundleContext(Context, BundleTable.Bundles, vpath);
                        foreach (var bundle in bundleFor.EnumerateFiles(bundleContext))
                        {
                            assets.Add(new Asset { VirtualPath = bundle.VirtualFile.VirtualPath, Type = GetAssetType(bundle.VirtualFile.VirtualPath) });
                        }
                    }
                }

                //vfile
                else if (IsFile(vpath))
                {
                    assets.Add(new Asset { VirtualPath = vpath, Type = GetAssetType(vpath) });
                }

                //vdir
                else if (IsDirectory(vpath))
                {
                    foreach (var fpath in GetFiles(vpath))
                    {
                        assets.Add(new Asset { VirtualPath = RelativePath(fpath), Type = GetAssetType(vpath) });
                    }
                }
            }
            return assets;
        }

        public Bundle GetBundle(string vpath)
        {
            return BundleTable.Bundles.GetBundleFor(vpath);
        }

        public VirtualFile GetVirtualFile(string vpath)
        {
            return HostingEnvironment.VirtualPathProvider.GetFile(vpath);
        }
        private static AssetType GetAssetType(string path)
        {
            switch (Path.GetExtension(path))
            {
                case ".js":
                    return AssetType.Script;
                case ".css":
                    return AssetType.Stylesheet;
                default:
                    return AssetType.Undefined;
            }
        }

        public static string RelativePath(string path)
        {
            return path.Replace(Context.Request.ServerVariables["APPL_PHYSICAL_PATH"], "~/").Replace(@"\", "/");
        }

        #region Privates
        private static bool IsBundle(string vpath)
        {
            return BundleTable.Bundles.GetBundleFor(vpath) != null;
        }

        private static bool IsFile(string vpath)
        {
            var fpath = HostingEnvironment.MapPath(vpath);

            return File.Exists(fpath);
        }

        private static bool IsDirectory(string vpath)
        {
            var dpath = HostingEnvironment.MapPath(vpath);

            return Directory.Exists(dpath);
        }

        private static IEnumerable<string> GetFiles(string vpath)
        {
            var dpath = HostingEnvironment.MapPath(vpath);

            return Directory.EnumerateFiles(dpath);
        }
        #endregion
    }
}