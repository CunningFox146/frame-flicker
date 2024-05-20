using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CunningFox146.Animation.Animation
{
    public class FrameAnimator : MonoBehaviour, IFrameAnimator
    {
        public event Action AnimationDone;
        public event Action AnimationQueueDone;

        [SerializeField] private int _framesPerSecond = 12;
        [SerializeField] private FrameAnimation _currentAnimation;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        private float _frameChangeTimer;
        private int _currentFrame;
        private readonly Queue<FrameAnimation> _queuedAnimations = new();

        public float SpeedMultiplier { get; set; } = 1f;
        private float ChangeFrameTime => 1 / (float)_framesPerSecond;
        private int FrameCount => _currentAnimation ? _currentAnimation.Sprites.Count : 0;
        private bool ShouldChangeFrame => _frameChangeTimer > ChangeFrameTime;

        private void OnValidate()
        {
            if (_currentAnimation)
                _spriteRenderer.sprite = _currentAnimation.Sprites.FirstOrDefault();
        }

        private void Update()
        {
            _frameChangeTimer += Time.deltaTime * SpeedMultiplier;
            if (ShouldChangeFrame)
            {
                ChangeFrame();
                _frameChangeTimer = 0f;
            }
        }

        public void PushAnimation(FrameAnimation anim)
        {
            _queuedAnimations.Enqueue(anim);
        }

        public void PlayAnimation(FrameAnimation anim)
        {
            _queuedAnimations.Clear();
            _currentAnimation = anim;
            ClearState();
        }

        public void Pause() 
            => enabled = false;

        public void Resume()
            => enabled = true;
        
        private void ChangeFrame()
        {
            if (++_currentFrame >= FrameCount) 
                OnAnimationDone();

            _spriteRenderer.sprite = _currentAnimation.Sprites[_currentFrame];
        }

        private void OnAnimationDone()
        {
            Debug.Log("OnAnimationDone", this);
            AnimationDone?.Invoke();
            ClearState();
            if (_queuedAnimations.Any())
                _currentAnimation = _queuedAnimations.Dequeue();
            else
                AnimationQueueDone?.Invoke();
        }

        private void ClearState()
        {
            _frameChangeTimer = 0f;
            _currentFrame = 0;
        }
    }
}