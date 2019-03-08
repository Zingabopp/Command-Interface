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
    public class CommandBehavior : WebSocketBehavior
    {
        private CIHTTPServer _server;
        private List<ICommandPlugin> _registeredPlugins;
        private List<ICommandPlugin> RegisteredPlugins
        {
            get
            {
                if (_registeredPlugins == null)
                    return _registeredPlugins = new List<ICommandPlugin>();
                return _registeredPlugins;
            }
        }

        public void Setup(CIHTTPServer server)
        {
            _server = server;
            _server.MessageReady += onMessageReady;
            Console.WriteLine("CommandBehavior Setup()");
            SceneManager.activeSceneChanged += OnSceneChange;
        }

        public void RegisterPlugin(ICommandPlugin newPlug)
        {
            if (!RegisteredPlugins.Contains(newPlug))
            {
                RegisteredPlugins.Add(newPlug);
                newPlug.MessageReady += onMessageReady;
            }
            else
            {
                Logger.Debug($"Unable to register plugin: {newPlug.PluginName} is already registered");
            }
        }

        public void DeRegisterPlugin(ICommandPlugin plug)
        {
            RegisteredPlugins.Remove(plug);
            plug.MessageReady -= onMessageReady;
        }

        /// <summary>
        /// Received a message from the server.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMessage(MessageEventArgs e)
        {
            var msg = e.Data.ToMessageData();
            if (_server.Plugins.ContainsKey(msg.Destination))
                _server.Plugins[msg.Destination].OnMessage(this, msg);
            else
                Logger.Debug($"Message destination not found: {msg.Destination},\n{msg.ToString(3)}");

        }

        /// <summary>
        /// Message ready to send to server.
        /// </summary>
        /// <param name="msg"></param>
        public void onMessageReady(MessageData msg)
        {
            Logger.Debug($"Message ready:\n{msg.ToString(3)}");
            Send(msg.ToJSON());
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine($"Connection closed because: {e.Reason}");
            SceneManager.activeSceneChanged -= OnSceneChange;
            _server.MessageReady -= onMessageReady;
            RegisteredPlugins.ForEach(p => DeRegisterPlugin(p));
            base.OnClose(e);
        }

        public void OnSceneChange(Scene oldScene, Scene newScene)
        {
            try
            {
                if (newScene.name == "Menu")
                {
                    _server.CheckPlugins();
                    //Code to execute when entering The Menu
                    //var testMessage = new MessageData("OBSControl", "OBSControl", "", "STOPREC");
                    //Logger.Trace($"In menu, sending message:\n{testMessage.ToString()}");
                    //Send(testMessage.ToJSON());

                }

                if (newScene.name == "GameCore")
                {
                    //Code to execute when entering actual gameplay
                    //var testMessage = new MessageData("OBSControl", "OBSControl", "", "STARTREC");
                    //Logger.Trace($"In GameCore, sending message:\n{testMessage.ToString()}");
                    //Send(testMessage.ToJSON());

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\n{ex.StackTrace}");
            }

        }

        public void OnSendComplete(bool finished)
        {
            Console.WriteLine($"Completed? {finished}");
        }
    }
}
