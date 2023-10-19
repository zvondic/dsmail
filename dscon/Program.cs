using System;
using System.Text.Json;
using Serilog;
using DSmail;
using System.Timers;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Object = System.Object;

namespace DSmail
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            Log.Logger.Information("DSmail strated");
            string settingFn = Path.Combine(Environment.CurrentDirectory,"appconfig.json");
            Log.Logger.Information(settingFn);
            string jsonString = File.ReadAllText(settingFn);
            Log.Logger.Information(jsonString);
            Config?  config= JsonSerializer.Deserialize<Config>(jsonString);
            if ( config != null && config.Pop3 !=null)
            {
                DSmail.Api.ReceiveMessages(config.Pop3.Items);
            }
            Console.ReadKey();
        }

    }

}