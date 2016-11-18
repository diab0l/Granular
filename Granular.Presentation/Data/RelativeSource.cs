using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media;

namespace System.Windows.Data
{
    public enum RelativeSourceMode
    {
        //PreviousData,
        TemplatedParent,
        Self,
        FindAncestor,
    }

    [MarkupExtensionParameter("Mode")]
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class RelativeSource : IMarkupExtension
    {
        public RelativeSourceMode Mode { get; set; }
        public int AncestorLevel { get; set; }
        public Type AncestorType { get; set; }

        public object ProvideValue(InitializeContext context)
        {
            return this;
        }

        public IObservableValue CreateSourceObserver(DependencyObject target)
        {
            switch (Mode)
            {
                case RelativeSourceMode.TemplatedParent: return new TemplatedParentSourceObserver(target);
                case RelativeSourceMode.Self: return new StaticObservableValue(target);
                case RelativeSourceMode.FindAncestor: return new FindAncestorSourceObserver(target, AncestorType, AncestorLevel);
            }

            throw new Granular.Exception("Unexpected RelativeSourceMode \"{0}\"", Mode);
        }
    }
}
