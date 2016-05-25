using Inliner.Tests.Utils;
using System.Web;
using System.Web.Optimization;
using Xunit;

namespace Inliner.Tests
{
    public class InlineScriptsTests
    {
        public InlineScriptsTests()
        {
            testVirtualPathProvider = new TestVirtualPathProvider();
            bundleManager = new DefaultBundleManager();
            bundleManager.VirtualPathProvider = testVirtualPathProvider;
            var context = new HttpContext(new HttpRequest(null, "http://tempuri.org", null), new HttpResponse(null));
            //clear cache
            foreach (System.Collections.DictionaryEntry entry in context.Cache)
            {
                context.Cache.Remove((string)entry.Key);
            }
            bundleManager.Context = new HttpContextWrapper(context);
            resolver = new AssetResolver(bundleManager);
            TagContentBuilder.BundleManager = bundleManager;
        }

        private readonly TestVirtualPathProvider testVirtualPathProvider;
        private readonly AssetResolver resolver;
        private readonly DefaultBundleManager bundleManager;
        [Fact]
        public void Render_WhenNullPaths_ShouldReturnEmpty()
        {
            var res = Scripts.Render(null);

            Assert.Equal(string.Empty, res.ToHtmlString());
        }

        [Fact]
        public void Render_WhenEmptyPaths_ShouldReturnEmpty()
        {
            var res = Scripts.Render(new string[] { });

            Assert.Equal(string.Empty, res.ToHtmlString());
        }

        [Fact]
        public void WhenNoOptimizations_TagShouldNotBeMinified()
        {
            //Setup the vpp to contain the files/directories
            var directory = new TestVirtualPathProvider.TestVirtualDirectory("/dir/");
            directory.DirectoryFiles.Add(new TestVirtualPathProvider.TestVirtualFile("/dir/file1.js", "/* demo file1 helpers*/ function test(a) { return 'test'; }"));
            directory.DirectoryFiles.Add(new TestVirtualPathProvider.TestVirtualFile("/dir/file2.js", "/* demo file2 helpers*/ function test(a) { return 'test'; }"));
            testVirtualPathProvider.AddDirectory(directory);

            BundleTable.EnableOptimizations = false;
            BundleTable.Bundles.Clear();

            var res = Scripts.Render("~/dir/file1.js", "~/dir/file2.js");

            var ashtml = res.ToHtmlString();
            Assert.True(ashtml.Length > 0);
            Assert.Equal("<script type=\"text/javascript\">/* demo file1 helpers*/ function test(a) { return 'test'; };\r\n;/* demo file2 helpers*/ function test(a) { return 'test'; };\r\n;</script>", ashtml);
        }

        [Fact]
        public void WhenOptimizations_TagShouldBeMinified()
        {
            //Setup the vpp to contain the files/directories
            var directory = new TestVirtualPathProvider.TestVirtualDirectory("/dir/");
            directory.DirectoryFiles.Add(new TestVirtualPathProvider.TestVirtualFile("/dir/file1.js", "/* demo file1 helpers*/ function test(a) { return 'test'; }"));
            directory.DirectoryFiles.Add(new TestVirtualPathProvider.TestVirtualFile("/dir/file2.js", "/* demo file2 helpers*/ function test(a) { return 'test'; }"));
            testVirtualPathProvider.AddDirectory(directory);

            BundleTable.EnableOptimizations = true;
            BundleTable.Bundles.Clear();

            var res = Scripts.Render("~/dir/file1.js", "~/dir/file2.js");

            var ashtml = res.ToHtmlString();
            Assert.True(ashtml.Length > 0);
            Assert.Equal("<script type=\"text/javascript\">function test(){return\"test\"};function test(){return\"test\"};</script>", ashtml);
        }
    }
}
