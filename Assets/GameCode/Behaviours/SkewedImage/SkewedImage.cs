using UnityEngine;
using UnityEngine.UI;

namespace UIExtensions
{
    public class SkewedImage : Image
    {
        public SkewMethod SkewMethod;
        public Vector2 SkewAngles;
        public Vector2 SkewVector;
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            if(SkewMethod == SkewMethod.Angles)
            {
                UpdateAngles(vh);
            }
            else
            {
                UpdatePixels(vh);
            }
        }

        private void UpdateAngles(VertexHelper vh)
        {
            var r = GetPixelAdjustedRect();
            var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);
            Color32 color32 = color;

            Vector2 modifiedSkewVector = Vector2.zero;

            var rt = (RectTransform)transform;

            modifiedSkewVector.x = Mathf.Tan(SkewAngles.x * Mathf.Deg2Rad) * r.height;
            modifiedSkewVector.y = Mathf.Tan(SkewAngles.y * Mathf.Deg2Rad) * r.width;

            vh.Clear();
            vh.AddVert(
                new Vector3(
                    v.x - ((modifiedSkewVector.x < 0) ? 0 : modifiedSkewVector.x),
                    v.y - ((modifiedSkewVector.y < 0) ? 0 : modifiedSkewVector.y)),
                color32, new Vector2(0f, 0f));
            vh.AddVert(
                new Vector3(
                    v.x + ((modifiedSkewVector.x >= 0) ? 0 : modifiedSkewVector.x),
                    v.w - ((modifiedSkewVector.y >= 0) ? 0 : modifiedSkewVector.y)),
                color32, new Vector2(0f, 1f));
            vh.AddVert(
                new Vector3(
                    v.z + ((modifiedSkewVector.x < 0) ? 0 : modifiedSkewVector.x),
                    v.w + ((modifiedSkewVector.y < 0) ? 0 : modifiedSkewVector.y)),
                color32, new Vector2(1f, 1f));
            vh.AddVert(
                new Vector3(
                    v.z - ((modifiedSkewVector.x >= 0) ? 0 : modifiedSkewVector.x),
                    v.y + ((modifiedSkewVector.y >= 0) ? 0 : modifiedSkewVector.y)),
                color32, new Vector2(1f, 0f));
            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }

        private void UpdatePixels(VertexHelper vh)
        {
            var r = GetPixelAdjustedRect();
            var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);
            Vector2 modifiedSkewVector = SkewVector;
            Color32 color32 = color;
            vh.Clear();
            vh.AddVert(
                new Vector3(
                    v.x - ((modifiedSkewVector.x < 0) ? 0 : modifiedSkewVector.x),
                    v.y - ((modifiedSkewVector.y < 0) ? 0 : modifiedSkewVector.y)),
                color32, new Vector2(0f, 0f));
            vh.AddVert(
                new Vector3(
                    v.x + ((modifiedSkewVector.x >= 0) ? 0 : modifiedSkewVector.x),
                    v.w - ((modifiedSkewVector.y >= 0) ? 0 : modifiedSkewVector.y)),
                color32, new Vector2(0f, 1f));
            vh.AddVert(
                new Vector3(
                    v.z + ((modifiedSkewVector.x < 0) ? 0 : modifiedSkewVector.x),
                    v.w + ((modifiedSkewVector.y < 0) ? 0 : modifiedSkewVector.y)),
                color32, new Vector2(1f, 1f));
            vh.AddVert(
                new Vector3(
                    v.z - ((modifiedSkewVector.x >= 0) ? 0 : modifiedSkewVector.x),
                    v.y + ((modifiedSkewVector.y >= 0) ? 0 : modifiedSkewVector.y)),
                color32, new Vector2(1f, 0f));
            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }
    }

    public enum SkewMethod
    {
        Pixels,
        Angles
    }
}