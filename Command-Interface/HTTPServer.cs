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

        /// <summary>
        /// Event fired when there is a message to send to the Command-Server.
        /// </summary>
        public event Action<object, MessageData> MessageReady;

        /// <summary>
        /// Dictionary of currently registered plugins.
        /// </summary>
        public Dictionary<string, ICommandPlugin> Plugins
        {
            get
            {
                if (_plugins == null)
                    _plugins = new Dictionary<string, ICommandPlugin>();
                else
                {
                    // Check for plugins that don't exist and remove them
                    foreach (var pair in _plugins)
                    {
                        if (pair.Value == null)
                            _plugins.Remove(pair.Key);
                    }
                }
                return _plugins;
            }
        }

        public string PluginName => Plugin.PluginName;

        private Dictionary<string, Action<object, string>> _commands;

        /// <summary>
        /// Dictionary of commands that can be handled by the server.
        /// </summary>
        public Dictionary<string, Action<object, string>> Commands
        {
            get
            {
                if (_commands == null)
                    _commands = new Dictionary<string, Action<object, string>>();
                return _commands;
            }
        }

        private void BuildCommands()
        {

            Commands.Add("REGISTER", RegisterPlugin);
            Commands.Add("DEREGISTER", DeRegisterPlugin);

        }

        /// <summary>
        /// Register the plugin with the server.
        /// TODO: If key is already registered, check if plugin reference is the same and reply if they aren't. (Resolve two plugins trying to use same name).
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="name"></param>
        public void RegisterPlugin(object plugin, string name)
        {
            Logger.Debug($"Registering plugin {name}");

            if (plugin == null)
            {
                Logger.Error("Plugin is null, unable to register");
                return;
            }
            ICommandPlugin plug = plugin as ICommandPlugin;
            if (plug != null)
            {
                try
                {
                    Plugins.AddSafe(plug.PluginName, plug);
                    plug.MessageReady += OnMessage;
                }
                catch (Exception ex)
                {
                    Logger.Exception("Error registering plugin", ex);
                }

            }
            else
                Logger.Error("Tried to register null plugin");

        }

        /// <summary>
        /// Unregisters the plugin. The plugin will no longer be sent messages, and the server won't receive messages from the plugin.
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="name"></param>
        public void DeRegisterPlugin(object plugin, string name)
        {
            Logger.Debug($"Registering plugin {name}");
            var plug = plugin as ICommandPlugin;
            if (plug != null)
            {
                Plugins.Remove(plug.PluginName);
                plug.MessageReady -= OnMessage;
            }
            else
                Logger.Error("Tried to register null plugin");
        }

        /// <summary>
        /// Initialize the server.
        /// </summary>
        private void Awake()
        {
            Logger.Debug("Starting CIHTTPServer...");
            if (_instance != null)
            {
                Logger.Warning("CIHTTPServer already exists, destroying...");
                Destroy(_instance);
            }
            _instance = this;
            InitServer();
            BuildCommands();
        }

        private void Start()
        {
            // Not currently used.
        }

        /// <summary>
        /// Stops the WebSocketServer when the GameObject is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Logger.Debug("Destroying CIHTTPServer...");
            StopServer();
        }

        /// <summary>
        /// Creates and starts a new WebSocketServer with the WebSocketBehavior that receives the messages from the server. 
        /// </summary>
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
            Logger.Info("Server started");
            foreach (var plug in Plugins.Values)
            {
                //_comService.RegisterPlugin(plug);
            }

        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void StopServer()
        {
            Logger.Debug("Stopping server...");
            _server.Stop();
        }


        /// <summary>
        /// Received message from plugin.
        /// </summary>
        /// <param name="sender">ICommandPlugin that is sending the message</param>
        /// <param name="e">MessageData being sent</param>
        public void OnMessage(object sender, MessageData e)
        {
            Logger.Trace($"Received a message from plugin:\n{e.ToString(3)}");
            try
            {
                if (e.Destination != "Command-Interface")
                {
                    // Message is not for Command-Interface, pass it on
                    Logger.Trace("Passing on message, Command-Interface is not the destination");
                    if(MessageReady != null)
                        MessageReady(sender, e);
                    else
                    {
                        Logger.Error("CommandBehavior is not listening");
                        //TODO: Reply system to notify plugins we're not connected to Command-Server.
                    }
                    return;
                }
                Logger.Trace($"Message is for Command-Interface:\n{e.ToString(3)}");
                ICommandPlugin s = sender as ICommandPlugin;
                if (s != null)
                {
                    // Check if the command is valid
                    if (Commands.ContainsKey(e.Flag))
                    {
                        Commands[e.Flag](s, e.Data);
                    }
                    else
                        Logger.Warning($"Invalid command: {e.Flag}");
                }
            }
            catch (Exception ex)
            {
                Logger.Exception("Exception in OnMessage", ex);
            }
        }

        public void Initialize()
        {
            // Not currently used.
        }
    }


}
