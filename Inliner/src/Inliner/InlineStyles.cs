using System.Web;

namespace Inliner
{
    public static class InlineStyles
    {
        private const string defaultTagFormat = "<style>{0}</style>";

        /// <summary>
        /// Render all paths as inline styles.
        /// </summary>
        /// <param name="paths">List of virtual paths (single file, directory or bundle).</param>
        /// <returns>Style tag</returns>
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
