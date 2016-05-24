using System.Text;

namespace Inliner
{
    internal class TagContent
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

        public TagContent()
        {
            builder = new StringBuilder();
        }

        public void Append(string path, string content)
        {
            builder.AppendFormat("/* content for : {0} */\r\n",path);
            builder.Append(content);
        }
    }
}
