using System.Web;

namespace Inliner
{
    public static class InlineStyles
    {
        private const string defaultTagFormat = "<style>{0}</style>";

        public static IHtmlString Render(params string[] paths)
        {
            var response = ResponseBuilder.Assemble(paths);

            if (response.HasContent)
                return new HtmlString(string.Format(defaultTagFormat, response.Content));
            else
                return new HtmlString(string.Empty);
        }
    }
}
