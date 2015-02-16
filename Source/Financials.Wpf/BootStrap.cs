using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Dragablz;
using StructureMap;
using Financials.Wpf.Infrastucture;
using Financials.Common.Services;
using Financials.Wpf.Views;

namespace Financials.Wpf
{
    public class BootStrap
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var app = new App { ShutdownMode = ShutdownMode.OnLastWindowClose };
            app.InitializeComponent();

            var container = new Container(x => x.AddRegistry<AppRegistry>());

            var tabSet = new TabSet();
            var mainWindow = new MainWindow
            {
                DataContext = tabSet
            };
            container.Configure(x => x.For<Dispatcher>().Add(mainWindow.Dispatcher));                   
            tabSet.Contents.Add(new HeaderedItemViewModel
            {
                Header = "TRADES",
                Content = container.GetInstance<LiveTradesViewer>()
            });            
            tabSet.Contents.Add(new HeaderedItemViewModel
            {
                Header = "MARKET",
                Content = container.GetInstance<MarketDataViewer>()
            });
            tabSet.Contents.Add(new HeaderedItemViewModel
            {
                Header = "ABOUT",
                Content = container.GetInstance<About>()
            });
            mainWindow.Show();

            //run start up jobs
            var priceUpdater = container.GetInstance<TradePriceUpdateJob>();

            app.Run();

            priceUpdater.Dispose();
        }
    }
}
