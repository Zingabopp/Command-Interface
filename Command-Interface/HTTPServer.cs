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
    public class CIHTTPServer : MonoBehaviour
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

        private void Start()
        {
            Console.WriteLine("Starting CIHTTPServer...");
            if (_instance != null)
            {
                Console.WriteLine("CIHTTPServer already exists, destroying...");
                Destroy(_instance);
            }
            _instance = this;
            InitServer();
        }

        private void OnDestroy()
        {
            Console.WriteLine("Destroying CIHTTPServer...");
            StopServer();
        }
        public void InitServer()
        {
            Console.WriteLine("Initializing server...");
            _server = new WebSocketServer(_serverPort);
            _server.AddWebSocketService<CommandBehavior>("/socket", behavior => behavior.Setup(_server));
            _server.Log.Output = (_, __) => { }; // Disable error output
            _server.Start();
        }

        public void StopServer()
        {
            Console.WriteLine("Stopping server...");
            _server.Stop();
        }
    }

    public class CommandBehavior : WebSocketBehavior
    {
        public void Setup(WebSocketServer server)
        {
            Console.WriteLine("CommandBehavior Setup()");
            SceneManager.activeSceneChanged += OnSceneChange;
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("Received message...");
            var msg = e.Data.ToString() == "BALUS"
                ? "I've been balused already..."
                : "I'm not available now.";
            Console.WriteLine($"Sending: {e.Data}\n{msg}");
            Send($"{e.Data}\n{msg}");

        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine($"Connection closed because: {e.Reason}");
            SceneManager.activeSceneChanged -= OnSceneChange;
            base.OnClose(e);
        }

        public void OnSceneChange(Scene oldScene, Scene newScene)
        {
            try
            {
                if (newScene.name == "Menu")
                {
                    //Code to execute when entering The Menu
                    var testMessage = new MessageData("OBSControl", "OBSControl", "", "STOPREC");
                    Logger.Trace($"In menu, sending message:\n{testMessage.ToString()}");
                    Send(testMessage.ToJSON());

                }

                if (newScene.name == "GameCore")
                {
                    //Code to execute when entering actual gameplay
                    var testMessage = new MessageData("OBSControl", "OBSControl", "", "STARTREC");
                    Logger.Trace($"In GameCore, sending message:\n{testMessage.ToString()}");
                    Send(testMessage.ToJSON());

                }
            } catch (Exception ex)
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
