using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace Kinel.VideoPlayer.Udon.Module
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class VideoSpeedChangeModule : UdonSharpBehaviour
    {
        [SerializeField] private KinelVideoPlayer videoPlayer;

        [SerializeField] private Animator animator;
        [SerializeField] private Slider speedChangerSlider;
        [SerializeField] private Text text;
        [SerializeField] private float min, max, animationParameterMax;
        [SerializeField] private AudioSource source;
        [SerializeField] private bool pitchChange;
        [SerializeField] private float increaseSpeed;

        [SerializeField, FieldChangeCallback(nameof(Speed))]
        private float speed = 1f;

        private bool isEdit = false;

        public float Speed
        {
            get => speed;
            set
            {
                speed = value;
                SetSpeed(speed);
            }
        }

        public void Start()
        {
            videoPlayer.RegisterListener(this);
            animator.Rebind();
            speedChangerSlider.maxValue = max;
            speedChangerSlider.minValue = min;
            speedChangerSlider.value = 1;
        }

        public void OnExMenuEnable()
        {
            
        }
        
        public void OnExMenuDisable()
        {
            
        }

        public void OnExMenuActive()
        {
            
        }

        public void OnExMenuInactive()
        {
            
        }

        public void OnExMenuReset()
        {
            
        }

        public void OnKinelVideoStart()
        {
            VideoTimeRecalculation();
        }

        public void OnChangeSlider()
        {
            var speed = speedChangerSlider.value;
            TakeOwnership();
            SetSpeed(speed);
            this.speed = speed;
            isEdit = true;
            RequestSerialization();
        }

        public void ResetSpeed()
        {
            isEdit = false;
            speedChangerSlider.value = 1;
            SetSpeed(1);
        }

        public void SetSpeed(float speed)
        {
            var animationRation = ConverToAnimationRation(speed,max,min,2);
            animator.SetFloat("Speed", animationRation);
            text.text = $"{speed:F2}";
            if (pitchChange)
            {
                if (source != null)
                {
                    source.pitch = speed;
                }
            }

            SendCustomEventDelayedFrames(nameof(VideoTimeRecalculation), 5);
        }

        public void VideoTimeRecalculation()
        {
            var nowVideoTime = videoPlayer.VideoTime;
            float normalSpeedVideoTime = 0f;// ((float)Networking.GetServerTimeInSeconds()) - videoPlayer.VideoStartGlobalTime;

            if (videoPlayer.IsPause)
            {
                normalSpeedVideoTime = (videoPlayer.PausedTime) - videoPlayer.VideoStartGlobalTime;
            }
            else
            {
                normalSpeedVideoTime = ((float)Networking.GetServerTimeInSeconds()) - videoPlayer.VideoStartGlobalTime;
            }
            
            Debug.Log($"now {nowVideoTime}, Normal Video Time {normalSpeedVideoTime}, speed {Speed}");
            Debug.Log($"Global Video Time {videoPlayer.VideoStartGlobalTime}, now GV {Networking.GetServerTimeInSeconds() }");
            videoPlayer.VideoStartGlobalTime -= normalSpeedVideoTime - nowVideoTime;
        }

        public float SliderValueToVideoSpeed()
        {
            var value = speedChangerSlider.value;

            var rawSpeed = Mathf.Clamp((value), 0.25f, 2);

            return rawSpeed;
        }

        public void SpeedDown()
        {
            var changeSpeed = speed - increaseSpeed;
            speedChangerSlider.value = changeSpeed;
            TakeOwnership();
            SetSpeed(changeSpeed);
            this.speed = changeSpeed;
            isEdit = true;
            RequestSerialization();
        }

        public void SpeedUp()
        {
            var changeSpeed = speed + increaseSpeed;
            speedChangerSlider.value = changeSpeed;
            TakeOwnership();
            SetSpeed(changeSpeed);
            this.speed = changeSpeed;
            isEdit = true;
            RequestSerialization();
        }

        /// <summary>
        /// 再生速度をアニメーション内のパラメータ値に変換します
        /// </summary>
        /// <param name="speed">再生速度(適用したい再生速度)</param>
        /// <param name="max">最大速度(Animaton max value)</param>
        /// <param name="min">最小速度(Animaton min value)</param>
        /// <param name="maxParameterValue">アニメーションパラメーター最大値</param>
        /// <returns></returns>
        public float ConverToAnimationRation(float speed, float max, float min, float maxParameterValue)
        {
            var diff = max - min; 
            var ration = maxParameterValue / diff;
            return speed * ration;
        }
        
        private void TakeOwnership()
        {

            if (Networking.IsOwner(Networking.LocalPlayer, gameObject))
            {

                return;
            }
            
            Networking.SetOwner(Networking.LocalPlayer, gameObject);


        }


    }
}