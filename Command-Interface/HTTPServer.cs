using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;
using CommandPluginLib;

namespace Command_Interface
{
    public class CIHTTPServer : MonoBehaviour, ICommandPlugin
    {
        private static CIHTTPServer _instance;
        public static CIHTTPServer Instance
        {
            get
            {
                return _instance;
            }
        }

        private int _serverPort = 6558;
        private WebSocketServer _server;
        private CommandBehavior _comService;
        private Dictionary<string, ICommandPlugin> _plugins;

        public event Action<MessageData> MessageReady;

        public Dictionary<string, ICommandPlugin> Plugins
        {
            get
            {
                var plugins = this.GetComponents<ICommandPlugin>().Where(p => !(p is CIHTTPServer)).ToList();
                var dict = new Dictionary<string, ICommandPlugin>();
                foreach (var plug in plugins)
                {
                    Logger.Trace($"Adding component {plug.PluginName} to dictionary");
                    dict.AddSafe(plug.PluginName, plug);
                    _comService.RegisterPlugin(plug);
                }
                return dict;
            }
        }

        public string PluginName => Plugin.PluginName;

        public Dictionary<string, Action<string>> Commands => throw new NotImplementedException();

        private void Start()
        {
            Logger.Debug("Starting CIHTTPServer...");
            if (_instance != null)
            {
                Logger.Warning("CIHTTPServer already exists, destroying...");
                Destroy(_instance);
            }
            _instance = this;
            InitServer();
            CheckPlugins();
            
        }

        public void CheckPlugins()
        {
            /*
            var plugins = this.GetComponents<ICommandPlugin>().Where(p => !(p is CIHTTPServer)).ToList();
            var dict = new Dictionary<string, ICommandPlugin>();
            plugins.Where(p => Plugins.ContainsKey(p.PluginName)).ToList().ForEach(plug => {
                Logger.Trace($"Adding component {plug.PluginName} to dictionary");
                dict.AddSafe(plug.PluginName, plug);
                _comService.RegisterPlugin(plug);
            });
            */
            var thing = Plugins.Values;
        }

        private void OnDestroy()
        {
            Logger.Debug("Destroying CIHTTPServer...");
            StopServer();
        }
        public void InitServer()
        {
            Logger.Info("Initializing server...");
            _server = new WebSocketServer(_serverPort);
            _server.AddWebSocketService<CommandBehavior>("/socket", behavior => {
                _comService = behavior;
                _comService.Setup(Instance);
            });
            _server.Log.Output = (_, __) => { }; // Disable error output
            _server.Start();
            
        }

        public void StopServer()
        {
            Logger.Debug("Stopping server...");
            _server.Stop();
        }


        /// <summary>
        /// Received message from plugin.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnMessage(object sender, MessageData e)
        {
            ICommandPlugin s = (ICommandPlugin) sender;
            if (e.Destination == "Command-Interface" && e.Data == "REGISTER")
                _comService.RegisterPlugin(s);
            else
                MessageReady(e);
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }

    
}
