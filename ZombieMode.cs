﻿using RedLoader;
using RedLoader.Utils;
using Sons.Ai.Vail;
using SonsSdk;
using TheForest.Utils;
using UnityEngine;
using ZombieMode.Core;
using ZombieMode.Gameplay;
using ZombieMode.Helpers;
using ZombieMode.UI;

namespace ZombieMode;

public class ZombieMode : SonsMod
{
    public static HarmonyLib.Harmony HarmonyInst;

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
        HarmonyInst.PatchAll(typeof(WeaponsUpgrade));
        HarmonyInst.PatchAll(typeof(ActorsManager));
        HarmonyInst.PatchAll(typeof(Player));
        #if DEBUG
        GlobalEvents.OnGUI.Subscribe(Debugging.DebugUI);
        #endif
        SoundTools.LoadBank(Path.Combine(LoaderEnvironment.ModsDirectory, "ZombieMode\\ZombieMode.bank"));
        SoundManager.Init();
        UiManager.CreateAllUi();
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
            if (Installer.CheckInstallation() == false)
            {
                InstallerUi.InstallerPanel.Active(true);
            }
            ZMainMenu.SonsPanel.Active(true);
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