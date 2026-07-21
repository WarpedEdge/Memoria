using System;
using UnityEngine;

namespace Memoria.Assets
{
    internal sealed class FbxUdimBlink : MonoBehaviour
    {
        private const Single MinimumBlinkDelay = 2.5f;
        private const Single MaximumBlinkDelay = 6f;
        private const Single ClosedEyeDuration = 0.12f;

        [SerializeField]
        private BlinkTarget[] _targets;
        private Single _nextBlinkTime;
        private Single _openEyeTime;
        private Boolean _eyesClosed;

        public void Initialize(SkinnedMeshRenderer[] renderers, Mesh[] meshes, Vector2[][] openUVs, Vector2[][] closedUVs)
        {
            Int32 targetCount = 0;
            for (Int32 i = 0; i < meshes.Length; i++)
                if (meshes[i] != null)
                    targetCount++;

            _targets = new BlinkTarget[targetCount];
            Int32 targetIndex = 0;
            for (Int32 i = 0; i < meshes.Length; i++)
            {
                if (meshes[i] == null)
                    continue;

                _targets[targetIndex++] = new BlinkTarget(renderers[i], meshes[i], openUVs[i], closedUVs[i]);
            }
            ApplyUVs(false);
            _eyesClosed = false;
            ScheduleNextBlink();
        }

        private void Update()
        {
            if (_targets == null)
                return;

            Single currentTime = Time.realtimeSinceStartup;
            if (_eyesClosed)
            {
                if (currentTime < _openEyeTime)
                    return;

                // Keep the open UVs until the next blink.
                ApplyUVs(false);
                _eyesClosed = false;
                ScheduleNextBlink();
            }
            else if (currentTime >= _nextBlinkTime)
            {
                // Show the closed eyes for a moment.
                ApplyUVs(true);
                _eyesClosed = true;
                _openEyeTime = currentTime + ClosedEyeDuration;
            }
        }

        private void OnEnable()
        {
            if (_targets == null)
                return;

            ApplyUVs(false);
            _eyesClosed = false;
            ScheduleNextBlink();
        }

        private void OnDisable()
        {
            if (_targets == null)
                return;

            ApplyUVs(false);
            _eyesClosed = false;
        }

        private void OnDestroy()
        {
            _targets = null;
        }

        private void ScheduleNextBlink()
        {
            _nextBlinkTime = Time.realtimeSinceStartup + UnityEngine.Random.Range(MinimumBlinkDelay, MaximumBlinkDelay);
        }

        private void ApplyUVs(Boolean useClosedUVs)
        {
            for (Int32 i = 0; i < _targets.Length; i++)
            {
                BlinkTarget target = _targets[i];
                Mesh mesh = target.Renderer != null && target.Renderer.sharedMesh != null ? target.Renderer.sharedMesh : target.ImportedMesh;
                Vector2[] uvs = useClosedUVs ? target.ClosedUVs : target.OpenUVs;
                if (mesh != null && uvs != null && mesh.vertexCount == uvs.Length)
                    mesh.uv = uvs;
            }
        }

        [Serializable]
        private sealed class BlinkTarget
        {
            public SkinnedMeshRenderer Renderer;
            public Mesh ImportedMesh;
            public Vector2[] OpenUVs;
            public Vector2[] ClosedUVs;

            public BlinkTarget(SkinnedMeshRenderer renderer, Mesh importedMesh, Vector2[] openUVs, Vector2[] closedUVs)
            {
                Renderer = renderer;
                ImportedMesh = importedMesh;
                OpenUVs = openUVs;
                ClosedUVs = closedUVs;
            }
        }
    }
}
