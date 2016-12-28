using UnityEngine;

namespace SDK.Lib
{
    public enum SoundPlayState
    {
        eSS_None,           // Ĭ��״̬
        eSS_Play,           // ����״̬
        eSS_Stop,           // ��ͣ״̬
        eSS_Pause,           // ֹͣ״̬
    }

    public enum SoundResType
    {
        eSRT_Prefab,
        eSRT_Clip,
    }

    /**
     * @brief ���ֺ���Ч���������
     */
    public class SoundItem
    {
        public string mPath;           // ��ԴĿ¼
        public SoundResType mSoundResType = SoundResType.eSRT_Prefab;
        protected SoundPlayState mPlayState = SoundPlayState.eSS_None;      // ������Ч����״̬
        public Transform mTrans;       // λ����Ϣ
        public GameObject mGo;         // audio ��� GameObject ����
        public AudioSource mAudio;             // ��Դ
        public bool mPlayOnStart = true;

        public ulong mDelay = 0;
        public bool mBypassEffects = false;        // �Ƿ�����Ƶ��Ч
        public bool mMute = false;         // �Ƿ���
        public bool mIsLoop = false;        // �Ƿ�ѭ������
        public float mVolume = 1.0f;
        public float mPitch = 1.0f;
        public bool mScaleOutputVolume = true;

        public SoundItem()
        {

        }

        public bool bInCurState(SoundPlayState state)
        {
            return mPlayState == state;
        }

        public virtual void setResObj(UnityEngine.Object go_)
        {

        }

        public void initParam(SoundParam soundParam)
        {
            mTrans = soundParam.mTrans;
            mIsLoop = soundParam.mIsLoop;
            mPath = soundParam.mPath;
        }

        protected void updateParam()
        {
            if (mTrans != null)
            {
                mGo.transform.position = mTrans.position;
            }
            mAudio = mGo.GetComponent<AudioSource>();
            //mAudio.rolloffMode = AudioRolloffMode.Logarithmic;
            mAudio.loop = mIsLoop;
            //mAudio.dopplerLevel = 0f;
            //mAudio.spatialBlend = 0f;
            volume = mVolume;

            //mAudio.minDistance = 1.0f;
            //mAudio.maxDistance = 50;
        }

        public float volume
        {
            get 
            { 
                return mVolume; 
            }
            set
            {
                if (mScaleOutputVolume)
                {
                    mAudio.volume = ScaleVolume(value);
                }
                else
                {
                    mAudio.volume = value;
                }
                mVolume = value;
            }
        }

        public float pitch
        {
            get 
            {
                return mPitch; 
            }
            set
            {
                mAudio.pitch = value;
                mPitch = value;
            }
        }

        void Start()
        {
            if (mPlayOnStart)
            {
                Play();
            }
        }

        public void Pause()
        {
            mPlayState = SoundPlayState.eSS_Pause;
            mAudio.Pause();
        }

        public void Play()
        {
            if (SoundPlayState.eSS_Pause == mPlayState)
            {
                mAudio.UnPause();
            }
            else
            {
                mAudio.Play(mDelay);
            }

            mPlayState = SoundPlayState.eSS_Play;
        }

        public void Stop()
        {
            mPlayState = SoundPlayState.eSS_Stop;
            mAudio.Stop();
        }

        public void SetPitch(float p)
        {
            pitch = p;
        }

        // TODO: we should consider using this dB scale as an option when porting these changes
        // over to unity-bowerbird: http://wiki.unity3d.com/index.php?title=Loudness
        /*
        * Quadratic scaling of actual volume used by AudioSource. Approximates the proper exponential.
        */
        public float ScaleVolume(float v)
        {
            v = Mathf.Pow(v, 4);
            return Mathf.Clamp(v, 0f, 1f);
        }

        // ж��
        public virtual void unload()
        {
            
        }

        public bool isEnd()
        {
            if (SoundPlayState.eSS_Play == mPlayState)     // ������ڲ���״̬��
            {
                return !mAudio.isPlaying;
            }

            return false;
        }
    }
}