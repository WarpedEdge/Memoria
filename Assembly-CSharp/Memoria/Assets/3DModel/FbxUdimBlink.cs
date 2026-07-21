using System;
using Memoria.Prime;
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
        private Boolean _loggedFirstUpdate;
        private Boolean _loggedMeshReplacement;
        private Boolean _loggedTargetFailure;

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
            Int32 blinkVertexCount = 0;
            for (Int32 i = 0; i < _targets.Length; i++)
                blinkVertexCount += CountChangedUVs(_targets[i].OpenUVs, _targets[i].ClosedUVs);

            Int32 appliedTargetCount = ApplyUVs(false);
            _eyesClosed = false;
            ScheduleNextBlink();
            Log.Message($"[FbxUdimBlink] Initialize model='{gameObject.name}' targets={_targets.Length} blinkVertices={blinkVertexCount} openApplied={appliedTargetCount} nextDelay={_nextBlinkTime - Time.realtimeSinceStartup:F2}s");
        }

        private void Start()
        {
            Log.Message($"[FbxUdimBlink] Start model='{gameObject.name}' targets={(_targets != null ? _targets.Length : 0)} enabled={enabled} active={gameObject.activeInHierarchy}");
        }

        private void Update()
        {
            if (_targets == null)
                return;

            Single currentTime = Time.realtimeSinceStartup;
            if (!_loggedFirstUpdate)
            {
                _loggedFirstUpdate = true;
                Log.Message($"[FbxUdimBlink] First Update model='{gameObject.name}' targets={_targets.Length} nextIn={_nextBlinkTime - currentTime:F2}s");
            }
            if (_eyesClosed)
            {
                if (currentTime < _openEyeTime)
                    return;

                // Keep the open UVs until the next blink.
                Int32 appliedTargetCount = ApplyUVs(false);
                _eyesClosed = false;
                ScheduleNextBlink();
                Log.Message($"[FbxUdimBlink] Open model='{gameObject.name}' applied={appliedTargetCount} nextDelay={_nextBlinkTime - Time.realtimeSinceStartup:F2}s");
            }
            else if (currentTime >= _nextBlinkTime)
            {
                // Show the closed eyes for a moment.
                Int32 appliedTargetCount = ApplyUVs(true);
                _eyesClosed = true;
                _openEyeTime = currentTime + ClosedEyeDuration;
                Log.Message($"[FbxUdimBlink] Close model='{gameObject.name}' applied={appliedTargetCount} duration={ClosedEyeDuration:F2}s");
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

        private Int32 ApplyUVs(Boolean useClosedUVs)
        {
            Int32 appliedTargetCount = 0;
            for (Int32 i = 0; i < _targets.Length; i++)
            {
                BlinkTarget target = _targets[i];
                if (target == null)
                {
                    LogTargetFailureOnce($"target {i} is missing");
                    continue;
                }

                Mesh mesh = target.Renderer != null && target.Renderer.sharedMesh != null ? target.Renderer.sharedMesh : target.ImportedMesh;
                Vector2[] uvs = useClosedUVs ? target.ClosedUVs : target.OpenUVs;
                if (mesh == null)
                {
                    LogTargetFailureOnce($"target {i} has no current mesh");
                    continue;
                }
                if (uvs == null || mesh.vertexCount != uvs.Length)
                {
                    LogTargetFailureOnce($"target {i} has mesh vertices={mesh.vertexCount} but UVs={(uvs != null ? uvs.Length : 0)}");
                    continue;
                }
                if (!_loggedMeshReplacement && target.ImportedMesh != null && mesh != target.ImportedMesh)
                {
                    _loggedMeshReplacement = true;
                    Log.Message($"[FbxUdimBlink] Current mesh replaced model='{gameObject.name}' target={i} imported='{target.ImportedMesh.name}' current='{mesh.name}'");
                }

                mesh.uv = uvs;
                appliedTargetCount++;
            }
            return appliedTargetCount;
        }

        private void LogTargetFailureOnce(String reason)
        {
            if (_loggedTargetFailure)
                return;

            _loggedTargetFailure = true;
            Log.Warning($"[FbxUdimBlink] Cannot apply blink UVs for model '{gameObject.name}': {reason}");
        }

        private static Int32 CountChangedUVs(Vector2[] openUVs, Vector2[] closedUVs)
        {
            if (openUVs == null || closedUVs == null || openUVs.Length != closedUVs.Length)
                return 0;

            Int32 changedCount = 0;
            for (Int32 i = 0; i < openUVs.Length; i++)
                if (openUVs[i].x != closedUVs[i].x || openUVs[i].y != closedUVs[i].y)
                    changedCount++;
            return changedCount;
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
