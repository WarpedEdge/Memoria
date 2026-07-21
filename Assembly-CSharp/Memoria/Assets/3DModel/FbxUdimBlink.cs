using System;
using UnityEngine;

namespace Memoria.Assets
{
    internal sealed class FbxUdimBlink : MonoBehaviour
    {
        private const Single MinimumBlinkDelay = 2.5f;
        private const Single MaximumBlinkDelay = 6f;
        private const Single ClosedEyeDuration = 0.12f;

        private Mesh[] _meshes;
        private Vector2[][] _openUVs;
        private Vector2[][] _closedUVs;
        private Single _nextBlinkTime;
        private Single _openEyeTime;
        private Boolean _eyesClosed;

        public void Initialize(Mesh[] meshes, Vector2[][] openUVs, Vector2[][] closedUVs)
        {
            _meshes = meshes;
            _openUVs = openUVs;
            _closedUVs = closedUVs;
            ApplyUVs(_openUVs);
            _eyesClosed = false;
            ScheduleNextBlink();
        }

        private void Update()
        {
            if (_meshes == null)
                return;

            Single currentTime = Time.realtimeSinceStartup;
            if (_eyesClosed)
            {
                if (currentTime < _openEyeTime)
                    return;

                // Keep the open UVs until the next blink.
                ApplyUVs(_openUVs);
                _eyesClosed = false;
                ScheduleNextBlink();
            }
            else if (currentTime >= _nextBlinkTime)
            {
                // Show the closed eyes for a moment.
                ApplyUVs(_closedUVs);
                _eyesClosed = true;
                _openEyeTime = currentTime + ClosedEyeDuration;
            }
        }

        private void OnEnable()
        {
            if (_meshes != null)
                ScheduleNextBlink();
        }

        private void OnDisable()
        {
            if (_meshes == null)
                return;

            ApplyUVs(_openUVs);
            _eyesClosed = false;
        }

        private void OnDestroy()
        {
            _meshes = null;
            _openUVs = null;
            _closedUVs = null;
        }

        private void ScheduleNextBlink()
        {
            _nextBlinkTime = Time.realtimeSinceStartup + UnityEngine.Random.Range(MinimumBlinkDelay, MaximumBlinkDelay);
        }

        private void ApplyUVs(Vector2[][] uvs)
        {
            for (Int32 i = 0; i < _meshes.Length; i++)
            {
                if (_meshes[i] != null && uvs[i] != null)
                    _meshes[i].uv = uvs[i];
            }
        }
    }
}
