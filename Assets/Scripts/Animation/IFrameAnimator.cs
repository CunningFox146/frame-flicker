using System;

namespace CunningFox146.Animation.Animation
{
    public interface IFrameAnimator
    {
        event Action AnimationDone;
        float SpeedMultiplier { get; set; }
        void PushAnimation(FrameAnimation anim);
        void PlayAnimation(FrameAnimation anim);
        void Pause();
        void Resume();
    }
}