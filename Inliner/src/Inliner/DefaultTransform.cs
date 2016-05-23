using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Optimization;

namespace Inliner
{
    internal sealed class DefaultTransform
    {
        public TransformOutput Process(string fileName, string source)
        {
            var extension = Path.GetExtension(fileName);
            if (BundleTable.EnableOptimizations)
            {
                if (string.Equals(extension, ".js", StringComparison.OrdinalIgnoreCase))
                {
                    return JsMinify(source);
                }
                if (string.Equals(extension, ".css", StringComparison.OrdinalIgnoreCase))
                {
                    return CssMinify(source);
                }
            }
            // no-op
            return new TransformOutput { Content = source };
        }

        private static TransformOutput CssMinify(string source)
        {
            var minifier = new Minifier();
            var cssSettings = new CssSettings
            {
                CommentMode = CssComment.None
            };
            var content = minifier.MinifyStyleSheet(source, cssSettings);
            if (minifier.ErrorList.Count > 0)
            {
                content = GenerateErrorResponse(minifier.ErrorList);
            }
            return new TransformOutput { Content = content, ContentType ="text/javascript", ConcatenationToken = ";" };
        }

        private static TransformOutput JsMinify(string source)
        {
            var minifier = new Minifier();
            var codeSettings = new CodeSettings
            {
                EvalTreatment = EvalTreatment.MakeImmediateSafe,
                PreserveImportantComments = false
            };
            var content = minifier.MinifyJavaScript(source, codeSettings);
            if (minifier.ErrorList.Count > 0)
            {
                content = GenerateErrorResponse(minifier.ErrorList);
            }
            return new TransformOutput { Content = content, ContentType = "text/css"};
        }

        private static string GenerateErrorResponse(IEnumerable<object> errors)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("/* ");
            stringBuilder.Append("MinifyError").Append("\r\n");
            foreach (object current in errors)
            {
                stringBuilder.Append(current.ToString()).Append("\r\n");
            }
            stringBuilder.Append(" */\r\n");
            return stringBuilder.ToString();
        }

        internal static readonly DefaultTransform Instance = new DefaultTransform();
    }
}

