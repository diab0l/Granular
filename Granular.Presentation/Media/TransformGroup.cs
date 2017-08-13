using Granular.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace System.Windows.Media
{
    [ContentProperty("Children")]
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class TransformGroup : Transform
    {
        private Matrix matrix;
        public override Matrix Value { get { return matrix; } }

        public FreezableCollection<Transform> Children { get; private set; }

        public TransformGroup()
        {
            this.matrix = Matrix.Identity;

            this.Children = new FreezableCollection<Transform>();
            this.Children.TrySetContextParent(this);
            this.Children.Changed += OnChildChanged;
        }

        private void OnChildChanged(object sender, EventArgs e)
        {
            this.matrix = Children.Select(child => child.Value).DefaultIfEmpty(Matrix.Identity).Aggregate((matrix1, matrix2) => matrix1 * matrix2);
            InvalidateRenderResource();
            RaiseChanged();
        }
    }
}
