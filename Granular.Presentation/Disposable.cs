using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows
{
    public class Disposable : IDisposable
    {
        private class EmptyDisposable : IDisposable
        {
            public void Dispose()
            {
                //
            }
        }

        public static readonly IDisposable Empty = new EmptyDisposable();

        private Action dispose;

        public Disposable(Action dispose)
        {
            this.dispose = dispose;
        }

        public void Dispose()
        {
            dispose();
        }

        public static IDisposable Combine(IDisposable disposable1, IDisposable disposable2)
        {
            return new Disposable(() =>
            {
                disposable1.Dispose();
                disposable2.Dispose();
            });
        }
    }
}
