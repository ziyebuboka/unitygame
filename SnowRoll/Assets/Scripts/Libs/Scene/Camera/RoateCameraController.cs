﻿using UnityEngine;

namespace SDK.Lib
{
    /**
     * @brief 旋转相机控制器
     */
    public class RoateCameraController : CameraController
    {
        private float eulerAngles_x;
        private float eulerAngles_y;        

        //水平滚动相关    
        public float xSpeed = 70.0f;//主相机水平方向旋转速度  

        //垂直滚动相关  
        public int yMaxLimit = 90;//最大y（单位是角度） 
        public int yMinLimit = 10;//最小y（单位是角度） 
        public float ySpeed = 70.0f;//主相机纵向旋转速度

        private float critical_value = 0.0f;

        public LayerMask CollisionLayerMask;

        private Transform transform;

        public RoateCameraController(Camera camera, GameObject go, SceneEntityBase actor)
            : base(camera, go)
        {
            this.transform = camera.gameObject.GetComponent<Transform>();
            (actor as PlayerMain).addChildChangedHandle(null, onTargetOrientPosChanged);
        }

        public void init()
        {
            //Ctx.mInstance.mTickMgr.addTick(this, TickPriority.eTPCamController);

            Ctx.mInstance.mInputMgr.addMouseListener(MMouse.MouseLeftButton, EventId.MOUSEMOVE_EVENT, onTouchMove);
            Ctx.mInstance.mInputMgr.addTouchListener(EventId.TOUCHMOVED_EVENT, onTouchMove);

            Vector3 eulerAngles = this.transform.eulerAngles;//当前物体的欧拉角  
            this.eulerAngles_x = eulerAngles.y;
            this.eulerAngles_y = eulerAngles.x;

            //critical_value = Mathf.Pow(limit_radius_value, MoveSensitivity);
            //critical_value2 = critical_value - Mathf.Pow(limit_radius_value2, Mathf.Abs(MoveSensitivity - MoveSensitivity2));
            critical_value = Ctx.mInstance.mSnowBallCfg.mCameraChangeFactor_Z * Mathf.Pow(Ctx.mInstance.mSnowBallCfg.mLimitRadius, 0.5f);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;//设置屏幕永远亮着
        }

        public void dispose()
        {

        }

        public void onTouchMove(IDispatchObject dispObj)
        {
            MMouseOrTouch touch = dispObj as MMouseOrTouch;

            float xOffset = 0;
            xOffset = touch.getXOffset();
            this.eulerAngles_x += ((xOffset * this.xSpeed) * 10.0f) * 0.02f;
            float yOffset = 0;
            yOffset = touch.getYOffset();
            this.eulerAngles_y -= (yOffset * this.ySpeed) * 0.02f;

            //Ctx.mInstance.mLogSys.log(string.Format("xOffset is {0}, yOffset is {1}", xOffset, yOffset), LogTypeId.eLogCommon);

            this.LateUpdate();
        }

        public void onTargetOrientPosChanged(IDispatchObject dispObj)
        {
            this.LateUpdate();
        }

        //public void onTick(float delta)
        //{
        //    this.Update();
        //}

        //public void Update()
        //{
        //    //if (Input.mousePosition.x >= fward_force_Op_x_min && Input.mousePosition.x <= fward_force_Op_x_max &&
        //    //   Input.mousePosition.y >= fward_force_Op_y_min && Input.mousePosition.y <= fward_force_Op_y_max)
        //    //{
        //    //Debug.Log("触摸在ui上 " + " x: " + Input.mousePosition.x + " y: " + Input.mousePosition.y + "  x_min: " + fward_force_Op_x_min + "  x_max: " + fward_force_Op_x_max + "  y_min: " + fward_force_Op_y_min + "  y_max: " + fward_force_Op_y_max);
        //    //}
        //    //else
        //    //{
        //    float xOffset = 0;
        //    xOffset = Input.GetAxis("Mouse X");
        //    this.eulerAngles_x += ((xOffset * this.xSpeed) * this.distance_Z) * 0.02f;
        //    float yOffset = Input.GetAxis("Mouse Y");
        //    this.eulerAngles_y -= (yOffset * this.ySpeed) * 0.02f;

        //    Ctx.mInstance.mLogSys.log(string.Format("xOffset is {0}, yOffset is {1}", xOffset, yOffset) , LogTypeId.eLogCommon);

        //    //}

        //    this.LateUpdate();
        //}

        // Update is called once per frame 
        protected void LateUpdate()
        {
            PlayerMain playerMain = Ctx.mInstance.mPlayerMgr.getHero();
            SetCameraPosition();
        }

        private void SetCameraPosition()
        {
            PlayerMain playerMain = Ctx.mInstance.mPlayerMgr.getHero();
            if(null != playerMain)
            {
                this.eulerAngles_y = ClampAngle(this.eulerAngles_y, (float)this.yMinLimit, (float)this.yMaxLimit);
                Quaternion quaternion = Quaternion.Euler(this.eulerAngles_y, this.eulerAngles_x, (float)0);
                //中心位置
                Vector3 centerPos = playerMain.getPos();
                //缩放参照距离
                float radius = playerMain.mPlayerSplitMerge.getMaxCameraLength();

                //缩放相机距离
                //float cur_distance_Z = this.distance_Z * radius;
                float cur_distance_Z = Ctx.mInstance.mSnowBallCfg.mCameraDistance_Z + Ctx.mInstance.mSnowBallCfg.mCameraChangeFactor_Z * Mathf.Pow(radius, 0.5f) + radius;
                if (radius > Ctx.mInstance.mSnowBallCfg.mLimitRadius)
                {
                    cur_distance_Z = Ctx.mInstance.mSnowBallCfg.mCameraDistance_Z + critical_value + (radius - Ctx.mInstance.mSnowBallCfg.mLimitRadius) + radius;
                }

                float cur_distance_Y = Ctx.mInstance.mSnowBallCfg.mCameraDistance_Y * radius * Ctx.mInstance.mSnowBallCfg.mCameraChangeFactor_Y;

                //Ctx.mInstance.mLogSys.log("radius: " + radius + "      Z: " + cur_distance_Z + "       Y: " + cur_distance_Y + "   log: " + Mathf.Log(Ctx.mInstance.mSnowBallCfg.mCameraChangeFactor_Z, radius));

                //从目标物体处，到当前脚本所依附的对象（主相机）发射一个射线，如果中间有物体阻隔，则更改this.distance（这样做的目的是为了不被挡住）  
                /*RaycastHit hitInfo = new RaycastHit();
                if (Physics.Linecast(this.target.position, this.transform.position, out hitInfo, this.CollisionLayerMask))
                {
                    this.distance = hitInfo.distance - 0.05f;
                }*/

                Vector3 vector = ((Vector3)(quaternion * new Vector3(0.0f, cur_distance_Y, -cur_distance_Z))) + centerPos;
                //Ctx.mInstance.mLogSys.error("centerPos: " + centerPos + "   vector: " + vector);
                //更改主相机的旋转角度和位置
                this.transform.rotation = quaternion;
                this.transform.position = vector;
                //旋转玩家角度，x轴不变
                //Vector3 eulerAngles_cam = this.transform.rotation.eulerAngles;
                //Vector3 eulerAngles = new Vector3(0, eulerAngles_cam.y, eulerAngles_cam.z);
                //playerMain.setDestRotate(eulerAngles, true);

                Ctx.mInstance.mGlobalDelegate.mCameraOrientChangedDispatch.dispatchEvent(null);
            }
        }

        //把角度限制到给定范围内 
        private float ClampAngle(float angle, float min, float max)
        {
            while (angle < -360)
            {
                angle += 360;
            }

            while (angle > 360)
            {
                angle -= 360;
            }

            return Mathf.Clamp(angle, min, max);
        }
    }
}