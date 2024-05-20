using System;
using UnityEngine;

namespace CunningFox146.Animation.Util
{
    public class FPS : MonoBehaviour
    {
        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 100, 20), (1f/Time.deltaTime).ToString());
        }
    }
}
