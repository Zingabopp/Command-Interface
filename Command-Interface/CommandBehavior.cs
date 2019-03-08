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

        public void Setup(CIHTTPServer server)
        {
            _server = server;
            _server.MessageReady += onMessageReady;
            
            Console.WriteLine("CommandBehavior Setup()");
            //SceneManager.activeSceneChanged += OnSceneChange;
        }

        /// <summary>
        /// Received a message from the server.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMessage(MessageEventArgs e)
        {
            Logger.Trace($"Recieved a message: {e.Data}");
            var msg = e.Data.ToMessageData();

            Logger.Trace($"Received a message:\n{msg.ToString(3)}");
            if (_server.Plugins.ContainsKey(msg.Destination))
                _server.Plugins[msg.Destination].OnMessage(this, msg);
            else
                Logger.Debug($"Message destination not found: {msg.Destination},\n{msg.ToString(3)}");

        }

        /// <summary>
        /// Message ready to send to server.
        /// </summary>
        /// <param name="msg"></param>
        public void onMessageReady(object sender, MessageData msg)
        {
            Logger.Debug($"Message ready:\n{msg.ToString(3)}");
            Send(msg.ToJSON());
        }

        /// <summary>
        /// Connection is closing, unsubscribe from the server's MessageReady event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine($"Connection closed because: {e.Reason}");
            //SceneManager.activeSceneChanged -= OnSceneChange;
            _server.MessageReady -= onMessageReady;
            //_server.Plugins.Values.ToList().ForEach(p => DeRegisterPlugin(p));
            base.OnClose(e);
        }

        /*
        public void OnSceneChange(Scene oldScene, Scene newScene)
        {
            try
            {
                if (newScene.name == "Menu")
                {


                }

                if (newScene.name == "GameCore")
                {


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\n{ex.StackTrace}");
            }

        }
        */
    }
}
