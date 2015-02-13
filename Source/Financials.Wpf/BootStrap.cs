﻿using System;
using System.Windows;
using System.Windows.Threading;
using StructureMap;
using Financials.Wpf.Infrastucture;
using Financials.Common.Services;

namespace Financials.Wpf
{
    public class BootStrap
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var app = new App { ShutdownMode = ShutdownMode.OnLastWindowClose };
            app.InitializeComponent();

           var container =  new Container(x=> x.AddRegistry<AppRegistry>());


           var factory = container.GetInstance<TraderWindowFactory>();
           var window = factory.Create(true);
           container.Configure(x => x.For<Dispatcher>().Add(window.Dispatcher));

            //run start up jobs
            var priceUpdater = container.GetInstance<TradePriceUpdateJob>();


            window.Show();

            app.Resources.Add(SystemParameters.ClientAreaAnimationKey, null);
            app.Resources.Add(SystemParameters.MinimizeAnimationKey, null);
            app.Resources.Add(SystemParameters.UIEffectsKey, null);


            app.Run();
        }
    }
}
