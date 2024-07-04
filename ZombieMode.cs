﻿using RedLoader;
using RedLoader.Utils;
using SonsSdk;
using UnityEngine;
using ZombieMode.Core;
using ZombieMode.Gameplay;
using ZombieMode.Helpers;
using ZombieMode.UI;

namespace ZombieMode;

public class ZombieMode : SonsMod
{
    public static HarmonyLib.Harmony HarmonyInst;
    private static bool _isSdkOK;

    public ZombieMode()
    {
        OnUpdateCallback = Update;
    }

    protected override void OnInitializeMod()
    {
        Config.Init();
        Installer.GetDownloadSize();     
    }

    protected override void OnSdkInitialized()
    {
        HarmonyInst = HarmonyInstance;
        #if DEBUG
        GlobalEvents.OnGUI.Subscribe(Debugging.DebugUI);
        #endif
        SoundTools.LoadBank(Path.Combine(LoaderEnvironment.ModsDirectory, "ZombieMode\\ZombieMode.bank"));
        SoundManager.Init();
        UiManager.CreateAllUi();
        _isSdkOK = true;
    }

    public static void Update()
    {
#if DEBUG
        if (Input.GetKeyDown(KeyCode.H))
        {

        }  
#endif
    }

    protected override void OnSonsSceneInitialized(ESonsScene sonsScene)
    {
        if (sonsScene == ESonsScene.Title)
        {         
            if (_isSdkOK)
            {
                ZMainMenu.SonsPanel.Active(true);
            }
            HarmonyInstance.UnpatchSelf();
            Game.GameState = Game.GameStates.Menu;
        }
        else
        {
            InstallerUi.InstallerPanel.Active(false);
            ZMainMenu.SonsPanel.Active(false);
        }
    }
}