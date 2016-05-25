using Inliner.Tests.Utils;
using System.Web;
using System.Web.Optimization;
using Xunit;

namespace Inliner.Tests
{
    public class InlineStylesTests
    {
        public InlineStylesTests()
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
            var res = Styles.Render(null);

            Assert.Equal(string.Empty, res.ToHtmlString());
        }

        [Fact]
        public void Render_WhenEmptyPaths_ShouldReturnEmpty()
        {
            var res = Styles.Render(new string[] { });

            Assert.Equal(string.Empty, res.ToHtmlString());
        }

        [Fact]
        public void WhenNoOptimizations_TagShouldNotBeMinified()
        {
            //Setup the vpp to contain the files/directories
            var directory = new TestVirtualPathProvider.TestVirtualDirectory("/dir/");
            directory.DirectoryFiles.Add(new TestVirtualPathProvider.TestVirtualFile("/dir/style1.css", "/* demo file1 helpers*/ body {    color: #454545; }"));
            directory.DirectoryFiles.Add(new TestVirtualPathProvider.TestVirtualFile("/dir/style2.css", "/* demo file2 helpers*/ body {    color: #454545; }"));
            testVirtualPathProvider.AddDirectory(directory);

            BundleTable.EnableOptimizations = false;
            BundleTable.Bundles.Clear();

            var res = Styles.Render("~/dir/style1.css", "~/dir/style2.css");

            var ashtml = res.ToHtmlString();
            Assert.True(ashtml.Length > 0);
            Assert.Equal("<style>/* demo file1 helpers*/ body {    color: #454545; }\r\n/* demo file2 helpers*/ body {    color: #454545; }\r\n</style>", ashtml);
        }

        [Fact]
        public void WhenOptimizations_TagShouldBeMinified()
        {
            //Setup the vpp to contain the files/directories
            var directory = new TestVirtualPathProvider.TestVirtualDirectory("/dir/");
            directory.DirectoryFiles.Add(new TestVirtualPathProvider.TestVirtualFile("/dir/style1.css", "/* demo file1 helpers*/ body {    color: #454545; }"));
            directory.DirectoryFiles.Add(new TestVirtualPathProvider.TestVirtualFile("/dir/style2.css", "/* demo file2 helpers*/ body {    color: #454545; }"));
            testVirtualPathProvider.AddDirectory(directory);

            BundleTable.EnableOptimizations = true;
            BundleTable.Bundles.Clear();

            var res = Styles.Render("~/dir/style1.css", "~/dir/style2.css");

            var ashtml = res.ToHtmlString();
            Assert.True(ashtml.Length > 0);
            Assert.Equal("<style>body{color:#454545}body{color:#454545}</style>", ashtml);
        }
    }
}
