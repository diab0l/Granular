using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel
{
    public class PropertyChangedEventArgs : EventArgs
    {
        public string PropertyName { get; private set; }

        public PropertyChangedEventArgs(string propertyName)
        {
            this.PropertyName = propertyName;
        }
    }

    public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

    public interface INotifyPropertyChanged
    {
        event PropertyChangedEventHandler PropertyChanged;
    }
}
