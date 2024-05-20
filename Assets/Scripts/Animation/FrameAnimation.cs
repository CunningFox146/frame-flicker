using System.Collections.Generic;
using UnityEngine;

namespace CunningFox146.Animation.Animation
{
    [CreateAssetMenu(menuName = "Create FrameAnimation", fileName = "FrameAnimation", order = 0)]
    public class FrameAnimation : ScriptableObject
    {
        [field: SerializeField] public string AnimationName { get; private set; }
        [field: SerializeField] public List<Sprite> Sprites { get; private set; }
    }
}