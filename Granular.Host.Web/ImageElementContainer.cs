using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge.Html5;

namespace Granular.Host
{
    public class ImageElementContainer
    {
        public HTMLElement HtmlElement { get; private set; }

        public ImageElementContainer()
        {
            HtmlElement = Document.CreateElement("div");
            HtmlElement.Style.SetProperty("visibility", "hidden");
            HtmlElement.Style.SetProperty("overflow", "hidden");
            HtmlElement.Style.Width = "0px";
            HtmlElement.Style.Height = "0px";
        }

        public void Add(HTMLElement element)
        {
            HtmlElement.AppendChild(element);
        }

        public void Remove(HTMLElement element)
        {
            HtmlElement.RemoveChild(element);
        }
    }
}
