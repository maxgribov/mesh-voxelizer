using System.Linq;
using UnityEngine;

namespace MathStructs
{
    public readonly struct XZQuad
    {
        #region properties

        public Vector3 LeftBottom { get; }
        public float XLength { get; }
        public float ZLength { get; }

        #endregion

        #region construction

        public XZQuad(Vector3[] _CornerPoints)
        {
            var center = 0.25f * _CornerPoints.Aggregate(Vector3.zero, (total, next) => total + next);
            var fromCenterToCorner = _CornerPoints.First() - center;

            XLength = Mathf.Abs(fromCenterToCorner.x * 2);
            ZLength = Mathf.Abs(fromCenterToCorner.z * 2);

            LeftBottom = center - (XLength * Vector3.right + ZLength * Vector3.forward) * 0.5f;
        }

        #endregion

        #region public methods

        public bool ContainsVerticalProjection(Vector3 _Point)
        {
            if (_Point.y > LeftBottom.y)
                return false;
            
            float t1 = (_Point.x - LeftBottom.x) / XLength;
            float t2 = (_Point.z - LeftBottom.z) / ZLength;
            return t1 is >= 0 and <= 1 && 
                   t2 is >= 0 and <= 1;
        }

        #endregion

        #region object methods

        // Can be overrided in cases of bad perfomance
        public override int GetHashCode()
        {
            return 0;
        }

        public override bool Equals(object _Other)
        {
            if (_Other is not XZQuad quad)
                return false;

            return MathUtility.MathUtility.Approximately(LeftBottom, quad.LeftBottom) &&
                   MathUtility.MathUtility.Approximately(XLength, quad.XLength) &&
                   MathUtility.MathUtility.Approximately(ZLength, quad.ZLength);
        }

        #endregion
    }
}