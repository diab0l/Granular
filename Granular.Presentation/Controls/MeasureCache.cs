using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Extensions;

namespace System.Windows.Controls
{
    internal class MeasureCache
    {
        private int capacity;

        private Size[] availableSizes;
        private Size[] measuredSizes;
        private int nextIndex;
        private int count;

        public MeasureCache(int capacity)
        {
            this.capacity = capacity;

            availableSizes = new Size[capacity];
            measuredSizes = new Size[capacity];
        }

        public void SetMeasure(Size availableSize, Size measuredSize)
        {
            availableSizes[nextIndex] = availableSize;
            measuredSizes[nextIndex] = measuredSize;
            nextIndex = (nextIndex + 1) % capacity;
            count = (count + 1).Min(capacity);
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        public bool TryGetMeasure(Size availableSize, out Size measuredSize)
        {
            for (int i = 0; i < count; i++)
            {
                if (availableSizes[i].IsClose(availableSize))
                {
                    measuredSize = measuredSizes[i];
                    return true;
                }
            }

            measuredSize = Size.Empty;
            return false;
        }

        public void Clear()
        {
            count = 0;
            nextIndex = 0;
        }
    }
}
