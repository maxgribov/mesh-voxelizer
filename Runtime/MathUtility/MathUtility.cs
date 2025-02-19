using System.Collections.Generic;
using MathStructs;
using UnityEngine;

namespace MathUtility
{
    public static class MathUtility
    {
        #region constants

        public const double EPSILON = 1e-6;
        public const double EPSILONSQR = EPSILON * EPSILON;

        #endregion

        #region public methods

        public static bool Approximately(float _A, float _B)
        {
            return Mathf.Abs(_A - _B) < EPSILON;
        }

        public static bool AreCollinear(Vector3 _A, Vector3 _B)
        {
            return Vector3.Cross(_A, _B).sqrMagnitude < EPSILONSQR;
        }

        public static bool Approximately(Vector3 _A, Vector3 _B)
        {
            return (_A - _B).sqrMagnitude < EPSILONSQR;
        }

        public static Vector3 GetQuadCorner(Vector3 _A, Vector3 _B, Vector3 _C)
        {
            if (IsRightAngle(_A, _B, _C))
                return _B + _C - _A;

            if (IsRightAngle(_B, _A, _C))
                return _A + _C - _B;

            return _A + _B - _C;
        }

        public static bool IsXZQuad(Vector3 _A, Vector3 _B, Vector3 _C)
        {
            Vector3 v1 = _B - _A;
            Vector3 v2 = _C - _A;
            Vector3 axisVector = Vector3.Cross(v1, v2);

            return AreCollinear(axisVector, Vector3.up);
        }

        //Check if BAC/CAB is right angle 
        public static bool IsRightAngle(Vector3 _A, Vector3 _B, Vector3 _C)
        {
            Vector3 v1 = _B - _A;
            Vector3 v2 = _C - _A;

            return Approximately(Vector3.Dot(v1, v2), 0);
        }

        #endregion
    }
}