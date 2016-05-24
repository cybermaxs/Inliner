using Inliner.Tests.Utils;
using Xunit;
using System.Linq;

namespace Inliner.Tests
{
    public class VirtualPathHelperTests
    {
        public VirtualPathHelperTests()
        {
            TestVirtualPathProvider = new TestVirtualPathProvider();
            VirtualPathHelper = new VirtualPathHelper(TestVirtualPathProvider);
        }

        public TestVirtualPathProvider TestVirtualPathProvider { get; private set; }
        public VirtualPathHelper VirtualPathHelper { get; private set; }

        [Theory]
        [InlineData("", false)]
        [InlineData("/test", false)]
        [InlineData("~/test", true)]
        [InlineData("c:/test", false)]
        public void IsVirtualPath(string path, bool expected)
        {
            Assert.Equal(expected, VirtualPathHelper.IsVirtualPath(path));
        }

        [Fact]
        public void IsVirtualFile()
        {
            TestVirtualPathProvider.AddFile(new TestVirtualPathProvider.TestVirtualFile("/file1", "foobar"));
            
            Assert.True(VirtualPathHelper.IsVirtualFile("~/file1"));
            Assert.False(VirtualPathHelper.IsVirtualDirectory("~/file1"));
        }

        [Fact]
        public void IsVirtualDirectory()
        {
            var directory = new TestVirtualPathProvider.TestVirtualDirectory("/dir/");
            directory.DirectoryFiles.Add(new TestVirtualPathProvider.TestVirtualFile("/dir/file1.css", "css"));
            directory.DirectoryFiles.Add(new TestVirtualPathProvider.TestVirtualFile("/dir/file1.js", "js"));
            TestVirtualPathProvider.AddDirectory(directory);

            Assert.True(VirtualPathHelper.IsVirtualDirectory("~/dir"));
            Assert.False(VirtualPathHelper.IsVirtualFile("~/dir"));
        }

        [Fact]
        public void GetResourcesFiles()
        {
            var directory = new TestVirtualPathProvider.TestVirtualDirectory("/dir/");
            directory.DirectoryFiles.Add(new TestVirtualPathProvider.TestVirtualFile("/dir/file1.css", "css"));
            directory.DirectoryFiles.Add(new TestVirtualPathProvider.TestVirtualFile("/dir/file1.js", "js"));
            directory.DirectoryFiles.Add(new TestVirtualPathProvider.TestVirtualFile("/dir/file1.map", "map"));
            directory.DirectoryFiles.Add(new TestVirtualPathProvider.TestVirtualFile("/dir/file1.woff", "woff"));
            directory.DirectoryFiles.Add(new TestVirtualPathProvider.TestVirtualFile("/dir/_references.js", "_references"));
            TestVirtualPathProvider.AddDirectory(directory);

            Assert.Equal(2, VirtualPathHelper.GetResourcesFiles("~/dir").Count());
        }

    }
}
