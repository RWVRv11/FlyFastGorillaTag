﻿using System;
using System.Collections.Generic;
using HarmonyLib;
using BepInEx;
using UnityEngine;
using System.Reflection;
using UnityEngine.XR;
using Photon.Pun;
using System.IO;
using System.Net;
using Photon.Realtime;
using UnityEngine.Rendering;

namespace FlyFastMod
{
    [BepInPlugin("org.Crafterbot.monkeytag.Fly", "FlyFast", "1.0")]
    public class MyPatcher : BaseUnityPlugin
    {
        public void Awake()
        {
            var harmony = new Harmony("com.Crafterbot.monkeytag.FlyFast");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(GorillaLocomotion.Player))]
    [HarmonyPatch("Update", MethodType.Normal)]

    public class code
    {
        static bool fly = false;
        static bool stop = false;
        static bool grip1 = false;
        static bool grip2 = false;
        static bool trigger1 = false;
        static bool trigger2 = false;
        static bool active = false;
        static void Postfix(GorillaLocomotion.Player __instance)
        {
            if (!PhotonNetwork.CurrentRoom.IsVisible || !PhotonNetwork.InRoom)
            {
                List<InputDevice> list = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller, list);
                list[0].TryGetFeatureValue(CommonUsages.primaryButton, out fly);
                InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller, list);
                list[0].TryGetFeatureValue(CommonUsages.secondaryButton, out stop);
                InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller, list);
                list[0].TryGetFeatureValue(CommonUsages.gripButton, out grip1);
                InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Left | UnityEngine.XR.InputDeviceCharacteristics.Controller, list);
                list[0].TryGetFeatureValue(CommonUsages.gripButton, out grip2);
                InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Left | UnityEngine.XR.InputDeviceCharacteristics.Controller, list);
                list[0].TryGetFeatureValue(CommonUsages.triggerButton, out trigger1);
                InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller, list);
                list[0].TryGetFeatureValue(CommonUsages.triggerButton, out trigger2);

                //ghost mode idea hit left secondary for move through walls

                //config stuff
                string[] array = File.ReadAllLines("BepInEx\\plugins\\FlyFast\\config.txt");
                string Line2 = array[0];
                string Line3 = array[1];
                string Line4 = array[2];
                float gripMode1 = int.Parse(Line2);
                float gripMode2 = int.Parse(Line3);
                float gripMode3 = int.Parse(Line4);
                if (gripMode1 < 8001)
                {
                    if (gripMode2 < 8001)
                    {
                        if (gripMode3 < 8001)
                        {
                            active = true;
                        }
                    }
                }

                //flying stuff
                if (active)
                {
                    float speed = 1500;
                    if (fly & !grip1)
                    {
                        speed = gripMode1;
                    }
                    //put photon stuff back in
                    if (grip1 & grip2 & fly)
                    {
                        speed = gripMode2;
                    }

                    if (trigger1 & trigger2 & grip1 & grip2 & fly)
                    {
                        speed = gripMode3;
                    }

                    if (stop)
                    {
                        if (fly)
                        {
                            fly = false;
                        }
                        __instance.bodyCollider.attachedRigidbody.velocity = __instance.headCollider.transform.forward * Time.deltaTime * 0;
                    }




                    if (fly)
                    {
                        //if you grip down then you fly in that direction but if you dont then you fly normal - idea
                        __instance.bodyCollider.attachedRigidbody.velocity = __instance.headCollider.transform.forward * Time.deltaTime * speed;
                        __instance.bodyCollider.attachedRigidbody.useConeFriction = false;
                        __instance.bodyCollider.attachedRigidbody.useGravity = false;

                    }
                    else
                    {
                        __instance.bodyCollider.attachedRigidbody.useConeFriction = true;
                        __instance.bodyCollider.attachedRigidbody.useGravity = true;
                    }


                }
            }
        }
    }
}

