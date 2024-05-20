using System;
using System.Collections.Generic;
using CunningFox146.Animation.Util;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Profiling;

namespace CunningFox146.Animation.Billboard
{
    public class BillboardSystem : MonoBehaviour
    {
        private TransformAccessList _list;
        
        private void Start()
        {
            _list = new TransformAccessList(32);
        }

        private void FixedUpdate()
        {
            var job = new RotationJob
            {
                Time = Time.time,
                Count = _list.Count,
            };
            job.Schedule(_list.Array).Complete();
        }

        private void OnDestroy()
        {
            _list.Dispose();
        }

        [BurstCompile]
        private struct RotationJob : IJobParallelForTransform
        {
            public float Time { get; set; }
            public int Count { get; set; }

            [BurstCompile]
            public void Execute(int index, TransformAccess transform)
            {
                var angleDelta = math.PI / 2f * Time;
                var angle = angleDelta + math.PI2 * (index / (float)Count);
                var radius = math.sin(Time) * 20f;
                transform.localPosition = new Vector3(math.sin(angle) * radius, 0f, math.cos(angle) * radius);
            }
        }
    }
}