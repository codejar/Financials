using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TradeExample.Annotations;

namespace Financials.Common.Infrastucture
{
    public abstract class NotifyPropertyChangedBase: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void SetAndRaise<T>(ref T backingField,  T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, newValue)) return;
            backingField = newValue;
           // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(propertyName);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
