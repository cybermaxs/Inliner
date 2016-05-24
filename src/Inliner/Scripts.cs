using System.Web;

namespace Inliner
{
    public static class Scripts
    {
        private const string defaultTagFormat = "<script type=\"text/javascript\">{0}</script>";

        /// <summary>
        /// Render all paths as inline scripts.
        /// </summary>
        /// <param name="paths">List of virtual paths (single file, directory or bundle).</param>
        /// <returns>Script tag</returns>
        public static IHtmlString Render(params string[] paths)
        {
            var response = TagContentBuilder.Merge(paths);

            if (response.HasContent)
                return new HtmlString(string.Format(defaultTagFormat, response.Content));
            else
                return new HtmlString(string.Empty);
        }
    }
}
