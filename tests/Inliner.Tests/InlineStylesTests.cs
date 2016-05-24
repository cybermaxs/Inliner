using Inliner.Tests.Utils;
using System.Web;
using System.Web.Optimization;
using Xunit;

namespace Inliner.Tests
{
    public class InlineStylesTests
    {
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
        public void StyleBundleCustomVPPIncludeVersionSelectsTest()
        {
            //Setup the vpp to contain the files/directories
            var vpp = new TestVirtualPathProvider();
            var directory = new TestVirtualPathProvider.TestVirtualDirectory("/dir/");
            directory.DirectoryFiles.Add(new TestVirtualPathProvider.TestVirtualFile("/dir/style.css", "/* demo file helpers*/ body {    color: #454545; }"));
            vpp.AddDirectory(directory);

            BundleTable.VirtualPathProvider = vpp;
            BundleTable.EnableOptimizations = true;
            DefaultBundleManager.Context = new HttpContextWrapper( new HttpContext(
            new HttpRequest(null, "http://tempuri.org", null),
            new HttpResponse(null)));


            var res = Styles.Render("~/dir/style.css");

            var ashtml = res.ToHtmlString();
            Assert.True(ashtml.Length > 0);
            //Assert.Equal("<style>body{color:#454545;}</style>", ashtml);
        }
    }
}
