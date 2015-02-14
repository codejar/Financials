﻿using System;
using System.IO;
using StructureMap.Configuration.DSL;
using Financials.Common.Infrastucture;
using Financials.Common.Services;

namespace Financials.Wpf.Infrastucture
{
    internal class AppRegistry : Registry
    {
        public AppRegistry()
        {

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            if (!File.Exists(path))
                throw new FileNotFoundException("The log4net.config file was not found" + path);

            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(path));

            For<ILogger>().Use<Log4NetLogger>().Ctor<Type>("type").Is(x => x.RootType);
            For<ISchedulerProvider>().Singleton().Use<SchedulerProvider>();
            For<IObjectProvider>().Singleton().Use<ObjectProvider>();
            For<ITradeService>().Singleton().Use<TradeService>();
            For<IStaticData>().Singleton().Use<StaticData>();
            For<IMarketPriceService>().Singleton().Use<MarketPriceService>();
            For<INearToMarketService>().Singleton().Use<NearToMarketService>();
            

           
            For<TradePriceUpdateJob>().Singleton();

            Scan(scanner => scanner.LookForRegistries());
        }
    }
}

