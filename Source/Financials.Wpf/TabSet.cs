using System.Collections.ObjectModel;
using Dragablz;

namespace Financials.Wpf
{
    public class TabSet
    {
        public ObservableCollection<HeaderedItemViewModel> Contents { get; } = new ObservableCollection<HeaderedItemViewModel>();
    }
}