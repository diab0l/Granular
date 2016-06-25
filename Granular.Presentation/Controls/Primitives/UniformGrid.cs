using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Granular.Extensions;

namespace System.Windows.Controls.Primitives
{
    public class UniformGrid : Panel
    {
        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(int), typeof(UniformGrid), new FrameworkPropertyMetadata(affectsMeasure: true));
        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), typeof(UniformGrid), new FrameworkPropertyMetadata(affectsMeasure: true));
        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public static readonly DependencyProperty FirstColumnProperty = DependencyProperty.Register("FirstColumn", typeof(int), typeof(UniformGrid), new FrameworkPropertyMetadata(affectsMeasure: true));
        public int FirstColumn
        {
            get { return (int)GetValue(FirstColumnProperty); }
            set { SetValue(FirstColumnProperty, value); }
        }

        private int actualRows;
        private int actualColumns;

        protected override Size MeasureOverride(Size availableSize)
        {
            int cellsCount = FirstColumn + Children.Where(child => child.Visibility != Visibility.Collapsed).Count();

            if (cellsCount == 0)
            {
                return Size.Zero;
            }

            actualRows = Rows > 0 ? Rows : (int)Math.Ceiling(Columns > 0 ? (double)cellsCount / Columns : Math.Sqrt(cellsCount)).Max(1);
            actualColumns = Columns > 0 ? Columns : (int)Math.Ceiling((double)cellsCount / actualRows).Max(1);

            Size availableCellSize = new Size(availableSize.Width / actualColumns, availableSize.Height / actualRows);
            Size desiredCellSize = Size.Zero;

            foreach (UIElement child in Children)
            {
                child.Measure(availableCellSize);
                desiredCellSize = desiredCellSize.Max(child.DesiredSize);
            }

            return new Size(desiredCellSize.Width * actualColumns, desiredCellSize.Height * actualRows);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double cellWidth = finalSize.Width / actualColumns;
            double cellHeight = finalSize.Height / actualRows;

            int cellIndex = FirstColumn;

            foreach (UIElement child in Children)
            {
                int columnIndex = cellIndex % actualColumns;
                int rowIndex = cellIndex / actualColumns;

                child.Arrange(new Rect(columnIndex * cellWidth, rowIndex * cellHeight, cellWidth, cellHeight));

                if (child.Visibility != Visibility.Collapsed)
                {
                    cellIndex++;
                }
            }

            return finalSize;
        }
    }
}
