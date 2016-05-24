using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;

namespace Inliner
{
    public class VirtualPathHelper
    {
        private readonly VirtualPathProvider virtualPathProvider;

        public VirtualPathHelper(VirtualPathProvider virtualPathProvider)
        {
            this.virtualPathProvider = virtualPathProvider;
        }
        public bool IsVirtualFile(string vpath)
        {
            return virtualPathProvider.FileExists(vpath);
        }
        public bool IsVirtualDirectory(string vpath)
        {
            return virtualPathProvider.DirectoryExists(vpath);
        }

        public IEnumerable<string> GetResourcesFiles(string vpath)
        {
            var vdir = virtualPathProvider.GetDirectory(vpath);

            return vdir.Files.Cast<VirtualFile>().Where(IsResourceFile).Select(v=>v.VirtualPath);
        }

        private static bool IsResourceFile(VirtualFile virtualFile)
        {
            var vpath = virtualFile.VirtualPath;
            if (vpath.Contains("_references.js"))// VS
                return false;
            if (!vpath.Contains(Constants.FileExts.Js) && !vpath.Contains(Constants.FileExts.Css))
                return false;
            return true;
        }

        public static bool IsVirtualPath(string virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath))
            {
                return false;
            }
            if (!virtualPath.StartsWith("~/", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            return true;
        }
    }
}
