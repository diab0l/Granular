using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Granular.Extensions;

namespace Granular.Presentation.Tests.Media
{
    public class TestRenderImageSource : IRenderImageSource
    {
        public event EventHandler StateChanged;

        private RenderImageState state;
        public RenderImageState State
        {
            get { return state; }
            set
            {
                if (state == value)
                {
                    return;
                }

                state = value;
                StateChanged.Raise(this);
            }
        }

        public Size Size { get; set; }
    }
}