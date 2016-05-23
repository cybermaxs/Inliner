using System.Web;

namespace Inliner
{
    public static class InlineScripts
    {
        private const string defaultTagFormat = "<script type=\"text/javascript\">{0}</script>";

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
