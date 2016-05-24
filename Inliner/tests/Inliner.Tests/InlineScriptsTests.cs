using Inliner.Tests.Utils;
using Xunit;

namespace Inliner.Tests
{
    public class InlineScriptsTests
    {
        [Fact]
        public void Render_WhenNullPaths_ShouldReturnEmpty()
        {
            var res = InlineScripts.Render(null);

            Assert.Equal(string.Empty, res.ToHtmlString());

        }

        [Fact]
        public void Render_WhenEmptyPaths_ShouldReturnEmpty()
        {
            var res = InlineScripts.Render(new string[] { });

            Assert.Equal(string.Empty, res.ToHtmlString());

        }


    }
}
