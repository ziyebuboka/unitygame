﻿using SDK.Lib;
using UnityEngine;

namespace UnitTest
{
    public class TestCameraMan
    {
        public void run()
        {
            test();
        }

        public void test()
        {
            Ctx.m_instance.m_sceneSys.loadScene("TestCameraControl.unity", onResLoadScene);
        }

        public void onResLoadScene(IDispatchObject dispObj)
        {
            Scene scene = dispObj as Scene;
            GameObject camera = UtilApi.GoFindChildByName("MainCamera");
            GameObject man = UtilApi.GoFindChildByName("Cube");
            Ctx.m_instance.m_camSys.setMainCamera(camera.GetComponent<Camera>());
            Ctx.m_instance.m_camSys.setCameraActor(man);
        }
    }
}