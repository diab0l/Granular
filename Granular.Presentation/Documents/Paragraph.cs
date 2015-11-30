using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using Granular.Collections;

namespace System.Windows.Documents
{
    [ContentProperty("Inlines")]
    public class Paragraph : Block
    {
        public ObservableCollection<Inline> Inlines { get; private set; }
    }
}
