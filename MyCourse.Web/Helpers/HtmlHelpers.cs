using HtmlAgilityPack;
using System.Text;

namespace MyCourse.Web.Helpers
{
    public static class HtmlHelpers
    {
        public static string TruncateHtml(string html, int maxLength)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var sb = new StringBuilder();
            int currentLength = 0;

            void TraverseNodes(HtmlNode node)
            {
                foreach (var child in node.ChildNodes)
                {
                    if (currentLength >= maxLength)
                        return;

                    if (child.NodeType == HtmlNodeType.Text)
                    {
                        var text = child.InnerText;
                        if (currentLength + text.Length > maxLength)
                        {
                            sb.Append(HtmlEntity.Entitize(text.Substring(0, maxLength - currentLength)));
                            sb.Append("...");
                            currentLength = maxLength;
                            return;
                        }
                        else
                        {
                            sb.Append(HtmlEntity.Entitize(text));
                            currentLength += text.Length;
                        }
                    }
                    else if (child.NodeType == HtmlNodeType.Element)
                    {
                        sb.Append($"<{child.Name}");

                        foreach (var attr in child.Attributes)
                        {
                            sb.Append($" {attr.Name}=\"{HtmlEntity.Entitize(attr.Value)}\"");
                        }
                        sb.Append(">");

                        TraverseNodes(child);

                        sb.Append($"</{child.Name}>");
                    }
                }
            }

            TraverseNodes(doc.DocumentNode);

            return sb.ToString();
        }
    }
}
