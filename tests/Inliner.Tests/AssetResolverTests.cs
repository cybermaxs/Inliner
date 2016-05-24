using Inliner.Tests.Utils;
using System.Web.Optimization;
using Xunit;
using System.Linq;

namespace Inliner.Tests
{
    public class AssetResolverTests
    {
        public AssetResolverTests()
        {
            TestVirtualPathProvider = new TestVirtualPathProvider();
            BundleManager = new DefaultBundleManager();
            BundleManager.VirtualPathProvider = TestVirtualPathProvider;
            Resolver = new AssetResolver(BundleManager);
        }

        private TestVirtualPathProvider TestVirtualPathProvider { get; set; }
        private AssetResolver Resolver { get; set; }
        internal DefaultBundleManager BundleManager { get; private set; }

        [Fact]
        public void Files()
        {
            TestVirtualPathProvider.AddFile(new TestVirtualPathProvider.TestVirtualFile("/file1.js", string.Empty));
            TestVirtualPathProvider.AddFile(new TestVirtualPathProvider.TestVirtualFile("/file2.css", string.Empty));

            var paths = new string[] { "~/file1.js", "~/file2.css" };
            var assets = Resolver.ResolveUrls(paths);

            Assert.Equal(2, assets.Count());
            Assert.Contains(new Asset("~/file1.js", AssetType.Script), assets);
            Assert.Contains(new Asset("~/file2.css", AssetType.Stylesheet), assets);
        }

        [Fact]
        public void Directory()
        {
            var vdir = new TestVirtualPathProvider.TestVirtualDirectory("/dir");
            vdir.DirectoryFiles.Add(new TestVirtualPathProvider.TestVirtualFile("/dir/file1.js", string.Empty));
            vdir.DirectoryFiles.Add(new TestVirtualPathProvider.TestVirtualFile("/dir/file2.css", string.Empty));
            TestVirtualPathProvider.AddDirectory(vdir);
            var paths = new string[] { "~/dir" };
            var assets = Resolver.ResolveUrls(paths);

            Assert.Equal(2, assets.Count());
            Assert.Contains(new Asset("~/dir/file1.js", AssetType.Script), assets);
            Assert.Contains(new Asset("~/dir/file2.css", AssetType.Stylesheet), assets);
        }

        [Fact]
        public void MissingFiles_ShouldReturn_EmptyResults()
        {
            var paths = new string[] { "~/file1.js", "~/file2.js" };
            var assets = Resolver.ResolveUrls(paths);

            Assert.Equal(0, assets.Count());
        }

        [Fact]
        public void Bundles()
        {
            BundleManager.RegisterNewBundle(new ScriptBundle("~/scripts").Include("~/file1.js", "~/file2.js"));
            BundleManager.RegisterNewBundle(new StyleBundle("~/styles").Include("~/style1.css", "~/style2.css"));
            var paths = new string[] { "~/scripts", "~/styles" };
            var assets = Resolver.ResolveUrls(paths);

            Assert.Equal(2, assets.Count());
            Assert.Contains(new Asset("~/scripts", AssetType.Bundle), assets);
            Assert.Contains(new Asset("~/styles", AssetType.Bundle), assets);
        }
    }
}
