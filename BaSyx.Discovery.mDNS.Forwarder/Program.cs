using BaSyx.Registry.Client.Http;
using CommandLine;
using NLog;
using System;
using System.Linq;
using System.Threading;

namespace BaSyx.Discovery.mDNS.Forwarder
{
    class Program
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private static RegistryClientSettings registryClientSettings;
        private static ManualResetEvent _quitApplicationEvent = new ManualResetEvent(false);

        public class Options
        {
            [Option('s', "settings", Required = false, HelpText = "Path to the RegistryClientSettings.xml")]
            public string SettingsFilePath { get; set; }

            [Option('u', "url", Required = false, HelpText = "Target registry URL, e.g. http://myServerRegistry.com:4999")]
            public string Url { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       if (!string.IsNullOrEmpty(o.SettingsFilePath))
                           registryClientSettings = RegistryClientSettings.LoadSettingsFromFile(o.SettingsFilePath);
                       else
                           registryClientSettings = RegistryClientSettings.LoadSettings();

                       if (!string.IsNullOrEmpty(o.Url))
                           registryClientSettings.RegistryConfig.RegistryUrl = o.Url;
                   });

            if (args.Contains("--help") || args.Contains("--version"))
                return;

            Console.CancelKeyPress += (sender, eArgs) => {
                _quitApplicationEvent.Set();
                eArgs.Cancel = true;
            };

            RegistryHttpClient client = new RegistryHttpClient(registryClientSettings);
            client.StartDiscovery();

            logger.Info($"mDNS-Forwarder started with target registry {registryClientSettings.RegistryConfig.RegistryUrl}");

            _quitApplicationEvent.WaitOne();

            client.StopDiscovery();
        }
    }
}
