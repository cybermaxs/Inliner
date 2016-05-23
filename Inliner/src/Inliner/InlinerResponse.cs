using System.Text;

namespace Inliner
{
    internal class InlinerResponse
    {
        public bool HasContent
        {
            get { return Content.Length > 0; }
        }
        public string Content
        {
            get { return builder.ToString(); }
        }

        private StringBuilder builder;

        public InlinerResponse()
        {
            builder = new StringBuilder();
        }

        public void Append(string path, string content, string concatenationToken)
        {
            //builder.AppendFormat("/* file : {0} */\r\n",path);
            builder.Append(content);
            builder.Append(concatenationToken);
        }
    }
}
