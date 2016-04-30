using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var assets = new List<Asset>()
            {
                new Asset("1","asset 1"),
                new Asset("2","asset 2"),
                new Asset("3","asset 3"),
            };
            var trades = new List<Trade>()
            {
                new Trade("1","1",100),
                new Trade("2","2",200),
                new Trade("3","3",300),
            };

            var assetProcessor = new AssetProcessor();
            var tradeProcessor = new TradeProcessor();

            var dataReceiver = new DataReceiver();
            dataReceiver.Register(assetProcessor);
            dataReceiver.Register(tradeProcessor);

            dataReceiver.GetData(assets);
            dataReceiver.GetData(trades);
        }
    }

    interface IDataProcessor
    {
    }

    interface IDataProcessor<T> : IDataProcessor
    {
        void ProcessData(T[] data);
    }

    class AssetProcessor : IDataProcessor<IAsset>
    {
        public void ProcessData(IAsset[] data)
        {

        }
    }

    class TradeProcessor : IDataProcessor<ITrade>
    {
        public void ProcessData(ITrade[] data)
        {

        }
    }

    interface ITrade
    {
        string Id { get; set; }
        string AssetId { get; set; }
        decimal Price { get; set; }
    }

    class Trade : ITrade
    {
        public string AssetId
        {
            get;
            set;
        }

        public string Id
        {
            get;
            set;
        }

        public decimal Price
        {
            get;
            set;
        }

        public Trade(string id, string assetId, decimal price)
        {
            Id = id;
            AssetId = assetId;
            Price = price;
        }

        public Trade()
        {

        }
    }

    interface IAsset
    {
        string Id { get; set; }
        string Name { get; set; }
    }

    class Asset : IAsset
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Asset(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public Asset()
        {
        }
    }

    class DataReceiver
    {
        class DataProcessorInfo
        {
            public IDataProcessor DataProcessor { get; set; }
            public Type DataType { get; set; }
            public MethodInfo ProcessMethodInfo { get; set; }
        }

        List<DataProcessorInfo> dataProcessors = new List<DataProcessorInfo>();

        public void Register<T>(IDataProcessor<T> processor)
        {
            dataProcessors.Add(new DataProcessorInfo
            {
                DataProcessor = processor,
                DataType = typeof(T),
                ProcessMethodInfo = processor.GetType().GetMethod("ProcessData")
            });
        }

        public void GetData<T>(List<T> data)
        {
            var dataType = typeof(T);
            foreach (var item in dataProcessors.Where(t => t.DataType.IsAssignableFrom(dataType)))
            {
                item.ProcessMethodInfo.Invoke(item.DataProcessor, new object[] { data.ToArray() });
            }
        }
    }
}
