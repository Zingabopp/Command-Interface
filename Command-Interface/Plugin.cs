﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using IllusionPlugin;


namespace Command_Interface
{
    public class Plugin : IPlugin
    {
        public static string PluginName => "Command-Interface";
        public string Name => PluginName;
        public string Version => "0.0.1";

        bool doesPluginExist;

        public void OnApplicationStart()
        {
            Logger.LogLevel = LogLevel.Error;
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;

            //Checks if a IPlugin with the name in quotes exists, in case you want to verify a plugin exists before trying to reference it, or change how you do things based on if a plugin is present
            doesPluginExist = IllusionInjector.PluginManager.Plugins.Any(x => x.Name == "Saber Mod");
            var CI_obj = new GameObject("CIHTTPServer").AddComponent<CIHTTPServer>();
            GameObject.DontDestroyOnLoad(CI_obj);
            Logger.Debug($"Created CIHTTPServer GameObject: {CI_obj.name}");

        }

        private void SceneManagerOnActiveSceneChanged(Scene oldScene, Scene newScene)
        {

            if (newScene.name == "Menu")
            {
                //Code to execute when entering The Menu
                

            }

            if (newScene.name == "GameCore")
            {
                //Code to execute when entering actual gameplay


            }


        }

        private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            //Create GameplayOptions/SettingsUI if using either
            if (scene.name == "Menu")
                UI.BasicUI.CreateUI();

        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        public void OnLevelWasLoaded(int level)
        {

        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnUpdate()
        {


        }

        public void OnFixedUpdate()
        {
        }
    }
}
