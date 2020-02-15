using System.Collections.Generic;
using UnityEngine.VFX;

namespace UnityEngine.VFX.Utility
{
    [AddComponentMenu("VFX/Property Binders/Random Position Binder")]
    [VFXBinder("Point Cache/Random Position Binder")]
    class VFXRandomPositionBinder : VFXBinderBase
    {
        [VFXPropertyBinding("UnityEngine.Texture2D"), UnityEngine.Serialization.FormerlySerializedAs("PositionMapParameter")]
        public ExposedProperty PositionMapProperty = "PositionMap";
        [VFXPropertyBinding("System.Int32"), UnityEngine.Serialization.FormerlySerializedAs("PositionCountParameter")]
        public ExposedProperty PositionCountProperty = "PositionCount";

        public bool EveryFrame = false;
        [Range(1,1024)] public int pointDensity = 32;
        public float scale = 0.02f;
        public float amplitude = 0.5f;
        public float frequency = 2f;

        private Texture2D positionMap;
        private int count = 0;

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateTexture();
        }

        public override bool IsValid(VisualEffect component)
        {
            return
                component.HasTexture(PositionMapProperty) &&
                component.HasInt(PositionCountProperty);
        }

        public override void UpdateBinding(VisualEffect component)
        {
            if (EveryFrame || Application.isEditor)
                UpdateTexture();

            component.SetTexture(PositionMapProperty, positionMap);
            component.SetInt(PositionCountProperty, count);
        }

        void UpdateTexture()
        {
            var candidates = new List<Vector3>();

            for (int i=0; i < pointDensity; i++)
            {
                Vector3 randomPosition = Random.insideUnitSphere;
                float s = Mathf.Sin(Time.time * frequency) * amplitude;
                randomPosition *= s;
                candidates.Add(randomPosition + new Vector3(-1f * scale, -1f * scale, 0f));
                candidates.Add(randomPosition + new Vector3(0f, 1.5f * scale, 0f));
                candidates.Add(randomPosition + new Vector3(1f * scale, -1f * scale, 0f));
            }

            count = pointDensity * 3; // 3 points per triangle

            if (positionMap == null || positionMap.width != count)
            {

                positionMap = new Texture2D(count, 1, TextureFormat.RGBAFloat, false);

            }

            List<Color> colors = new List<Color>();
            for (int i=0; i < candidates.Count; i++)
            {
                colors.Add(new Color(candidates[i].x, candidates[i].y, candidates[i].z));
            }
            positionMap.name = gameObject.name + "_PositionMap";
            positionMap.filterMode = FilterMode.Point;
            positionMap.wrapMode = TextureWrapMode.Repeat;
            positionMap.SetPixels(colors.ToArray(), 0);
            positionMap.Apply();
        }

        public override string ToString()
        {
            return string.Format("Random Position Binder ({0} positions)", count);
        }
    }
}

