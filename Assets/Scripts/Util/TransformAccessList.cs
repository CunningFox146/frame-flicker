using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

namespace CunningFox146.Animation.Util
{
    public class TransformAccessList : IDisposable
    {
        private TransformAccessArray _array;

        public ref TransformAccessArray Array => ref _array;
        public int Count => _array.length;

        public TransformAccessList(int startCapacity)
        {
            _array = new TransformAccessArray(startCapacity);
        }

        public void Add(Transform item)
        {
            if (_array.length > _array.capacity)
                Resize(ref _array, _array.capacity * 2);
            _array.Add(item);
        }

        public void Remove(Transform item)
        {
            var itemIdx = GetIndex(item);
            _array.RemoveAtSwapBack(itemIdx);
        }

        public void SetTransforms(Transform[] transforms)
            => _array.SetTransforms(transforms);

        private void Resize(ref TransformAccessArray array, int capacity)
        {
            var newArray = new TransformAccessArray(capacity);
            if (array.isCreated)
            {
                for (var i = 0; i < array.length; ++i)
                    newArray.Add(array[i]);

                array.Dispose();
            }
            array = newArray;
        }

        private int GetIndex(Transform item)
        {
            for (var i = 0; i < _array.length; i++)
            {
                if (_array[i] == item)
                    return i;
            }

            throw new KeyNotFoundException();
        }

        public void Dispose()
        {
            _array.Dispose();
        }
    }
}