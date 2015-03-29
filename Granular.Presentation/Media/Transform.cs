using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media
{
    public abstract class Transform : DependencyObject
    {
        private class IdentityTransfrom : Transform
        {
            public override Matrix Value { get { return Matrix.Identity; } }
        }

        public static readonly Transform Identity = new IdentityTransfrom();

        public abstract Matrix Value { get; }
    }
}
