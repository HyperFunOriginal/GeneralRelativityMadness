using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Curve
{
    [System.Serializable]
    public struct CubBéziSpline
    {
        public Vector3 pointA;
        public Vector3 pointB;
        public Vector3 pointC;
        public Vector3 pointD;

        float[] LUT;

        public Vector3 getPoint(float t)
        {
            return pointA * (-4.5f * t * t * t + 9f * t * t - 5.5f * t + 1f) + pointB * (13.5f * t * t * t - 22.5f * t * t + 9f * t) + pointC * (-13.5f * t * t * t + 18f * t * t - 4.5f * t) + pointD * (4.5f * t * t * t - 4.5f * t * t + t);
        }
        public Vector3 getDerivative(float t)
        {
            return (getPoint(t + 0.001f) - getPoint(t)) * 1000f;
        }
        public Vector3 getSecDerivative(float t)
        {
            return (2f * getPoint(t) - getPoint(t + 0.005f) - getPoint(t - 0.005f)) * 40000f;
        }
        public Vector3 GetCurvature(float t, bool normal)
        {
            Vector3 deriv = getDerivative(t);
            Vector3 secDeriv = getSecDerivative(t) / deriv.sqrMagnitude;
            deriv /= deriv.magnitude;
            if (!normal)
                return Vector3.Cross(deriv, secDeriv);
            return Vector3.Cross(Vector3.Cross(deriv, secDeriv), deriv);
        }
        public void GenerateDistanceLUT(int samples)
        {
            LUT = new float[samples];
            float t = 0, d = 0;
            Vector3 a = Vector3.zero, b = getPoint(0);

            for (int i = 1; i < LUT.Length; i++)
            {
                a = b;
                t += 1f / (samples - 1f);
                b = getPoint(t);
                d += (b - a).magnitude;
                LUT[i] = d;
            }
            LUT[0] = 0;
        }
        public float DistanceToT(float distance)
        {
            if (LUT == null)
                GenerateDistanceLUT(20);
            int p = -1;
            for (int i = 0; i < LUT.Length; i++)
                if (LUT[i] - 0.001f > distance)
                {
                    p = i;
                    i = 1000;
                    continue;
                }

            if (p == -1)
                p = LUT.Length - 1;

            distance -= LUT[Mathf.Max(p - 1, 0)];
            distance /= LUT[Mathf.Max(p, 1)] - LUT[Mathf.Max(p - 1, 0)];
            return (p + distance - 1f) / LUT.Length;
        }
        public float GetLength(int samples)
        {
            float l = 0;
            for (float i = 0; i < samples; i++)
                l += (getPoint((i + 1) / samples) - getPoint(i / samples)).magnitude;
            return l;
        }
    }
    [System.Serializable]
    public struct CubBézier
    {
        public Vector3 pointA;
        public Vector3 pointB;
        public Vector3 pointC;
        public Vector3 pointD;

        float[] LUT;

        public Vector3 getPoint(float t)
        {
            return pointA * (1 - t) * (1 - t) * (1 - t) + pointB * 3 * t * (1 - t) * (1 - t) + pointC * 3 * t * t * (1 - t) + pointD * t * t * t;
        }
        public Vector3 getDerivative(float t)
        {
            return pointA * (-3 * t * t + 6 * t - 3) + pointB * 3 * (3 * t * t - 4 * t + 1) + pointC * 3 * (-3 * t * t + 2 * t) + pointD * 3 * t * t;
        }
        public Vector3 getSecDerivative(float t)
        {
            return pointA * (-6 * t + 6) + pointB * 3 * (6 * t - 4) + pointC * 3 * (-6 * t + 2) + pointD * 6 * t;
        }
        public Vector3 GetCurvature(float t, bool normal)
        {
            Vector3 deriv = getDerivative(t);
            Vector3 secDeriv = getSecDerivative(t) / deriv.sqrMagnitude;
            deriv /= deriv.magnitude;
            if (!normal)
                return Vector3.Cross(deriv, secDeriv);
            return Vector3.Cross(Vector3.Cross(deriv, secDeriv), deriv);
        }
        public void GenerateDistanceLUT(int samples)
        {
            LUT = new float[samples];
            float t = 0, d = 0;
            Vector3 a = Vector3.zero, b = getPoint(0);

            for (int i = 1; i < LUT.Length; i++)
            {
                a = b;
                t += 1f / (samples - 1f);
                b = getPoint(t);
                d += (b - a).magnitude;
                LUT[i] = d;
            }
            LUT[0] = 0;
        }
        public float DistanceToT(float distance)
        {
            if (LUT == null)
                GenerateDistanceLUT(20);
            int p = -1;
            for (int i = 0; i < LUT.Length; i++)
                if (LUT[i] - 0.001f > distance)
                {
                    p = i;
                    i = 1000;
                    continue;
                }

            if (p == -1)
                p = LUT.Length - 1;

            distance -= LUT[Mathf.Max(p - 1, 0)];
            distance /= LUT[Mathf.Max(p, 1)] - LUT[Mathf.Max(p - 1, 0)];
            return (p + distance - 1f) / LUT.Length;
        }
        public float GetLength(int samples)
        {
            float l = 0;
            for (float i = 0; i < samples; i++)
                l += (getPoint((i + 1) / samples) - getPoint(i / samples)).magnitude;
            return l;
        }
    }
    [System.Serializable]
    public struct QuadBézier
    {
        public Vector3 pointA;
        public Vector3 pointB;
        public Vector3 pointC;

        float[] LUT;
        public Vector3 getPoint(float t)
        {
            return pointA * (1 - t) * (1 - t) + pointB * 2 * t * (1 - t) + pointC * t * t;
        }
        public Vector3 getDerivative(float t)
        {
            return pointA * 2 * (t - 1) + pointB * 2 * (1 - 2 * t) + pointC * 2 * t;
        }
        public Vector3 getSecDerivative(float t)
        {
            return pointA * 2 - pointB * 4 + pointC * 2;
        }
        public Vector3 GetCurvature(float t, bool normal)
        {
            Vector3 deriv = getDerivative(t);
            Vector3 secDeriv = getSecDerivative(t) / deriv.sqrMagnitude;
            deriv /= deriv.magnitude;
            if (!normal)
                return Vector3.Cross(deriv, secDeriv);
            return Vector3.Cross(Vector3.Cross(deriv, secDeriv), deriv);
        }
        public void GenerateDistanceLUT(int samples)
        {
            LUT = new float[samples];
            float t = 0, d = 0;
            Vector3 a = Vector3.zero, b = getPoint(0);

            for (int i = 1; i < LUT.Length; i++)
            {
                a = b;
                t += 1f / (samples - 1f);
                b = getPoint(t);
                d += (b - a).magnitude;
                LUT[i] = d;
            }
            LUT[0] = 0;
        }
        public float DistanceToT(float distance)
        {
            if (LUT == null)
                GenerateDistanceLUT(20);
            int p = -1;
            for (int i = 0; i < LUT.Length; i++)
                if (LUT[i] - 0.001f > distance)
                {
                    p = i;
                    i = 1000;
                    continue;
                }

            if (p == -1)
                p = LUT.Length - 1;

            distance -= LUT[Mathf.Max(p - 1, 0)];
            distance /= LUT[Mathf.Max(p, 1)] - LUT[Mathf.Max(p - 1, 0)];
            return (p + distance - 1f) / LUT.Length;
        }
        public float GetLength(int samples)
        {
            float l = 0;
            for (float i = 0; i < samples; i++)
                l += (getPoint((i + 1) / samples) - getPoint(i / samples)).magnitude;
            return l;
        }
    }
}

public static class Matrix {
    [System.Serializable()]
    public struct TwoDMatrix
    {
        public Vector2 newXAxis, newYAxis;
        public float det
        {
            get
            {
                return newXAxis.x*newYAxis.y - newXAxis.y*newYAxis.x;
            }
        }
        public static TwoDMatrix Lerp(TwoDMatrix a, TwoDMatrix b, float t)
        {
            return new TwoDMatrix(a.newXAxis * (1f - t) + b.newXAxis * t, a.newYAxis * (1f - t) + b.newYAxis * t);
        }

        public static TwoDMatrix GetStandardRotationMatrix(float theta)
        {
            return new TwoDMatrix(new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)), new Vector2(-Mathf.Sin(theta), Mathf.Cos(theta)));
        }
        public override string ToString()
        {
            return "    [" + newXAxis + ", " + newYAxis + "]"; 
        }
        public TwoDMatrix(Vector2 newXAxis, Vector2 newYAxis)
        {
            this.newXAxis = newXAxis;
            this.newYAxis = newYAxis;
        }
        public void Multiply(Vector2 a)
        {
            newXAxis *= a.x;
            newYAxis *= a.y;
        }
        public static TwoDMatrix operator *(TwoDMatrix second, TwoDMatrix first)
        {
            return new TwoDMatrix(second.Transform(first.newXAxis), second.Transform(first.newYAxis));
        }
        public static TwoDMatrix operator *(TwoDMatrix m, float a)
        {
            return new TwoDMatrix(m.newXAxis * a, m.newYAxis * a);
        }
        public static TwoDMatrix operator /(TwoDMatrix m, float a)
        {
            return new TwoDMatrix(m.newXAxis / a, m.newYAxis / a);
        }
        public TwoDMatrix Inverse()
        {
            return new TwoDMatrix(new Vector2(newYAxis.y, -newXAxis.y), new Vector2(-newYAxis.x, newXAxis.x))/det;
        }
        public Vector2 Transform(Vector2 a)
        {
            return a.x * newXAxis + a.y * newYAxis;
        }

        public void Normalize()
        {
            if (det == 0f)
                return;
            float scale = Mathf.Pow(Mathf.Abs(det), -0.5f);
            this *= scale;
        }
        /// <summary>
        /// Returns the Eigenvectors for this 2-D matrix, vectors formatted like: normalized (x, 1) * eigenvalue
        /// </summary>
        /// <returns></returns>
        public List<Vector2> GetEigenVectors()
        {
            List<Vector2> eigenvectors = new List<Vector2>();
            float meanEigenValue = (newXAxis.x + newYAxis.y) * 0.5f;
            float discriminant = meanEigenValue * meanEigenValue - det;
            if (discriminant < 0f) // No eigenvalue
                return eigenvectors;
            float eigen1 = meanEigenValue + Mathf.Sqrt(discriminant);
            if (eigen1 == 0f) // Eigenvalue = 0
                return eigenvectors;
            float eigen2 = meanEigenValue - Mathf.Sqrt(discriminant);
            Vector2 vec1 = new Vector2(-(newYAxis.x - newYAxis.y + eigen1) / (newXAxis.x - newXAxis.y - eigen1), 1f); // Linear equation solve for first component (2nd component = eigenvalue)
            if (!float.IsInfinity(vec1.x))
                eigenvectors.Add(vec1 * eigen1);
            else
                eigenvectors.Add(Vector2.right * eigen1); // Eigenvector has 0 slope (is entirely on x-axis)
            Vector2 vec2 = new Vector2(-(newYAxis.x - newYAxis.y + eigen2) / (newXAxis.x - newXAxis.y - eigen2), 1f); // Linear equation solve for first component (2nd component = eigenvalue)
            if (!float.IsInfinity(vec2.x))
                eigenvectors.Add(vec2 * eigen2);
            else
                eigenvectors.Add(Vector2.right * eigen2); // Eigenvector has 0 slope (is entirely on x-axis)
            return eigenvectors;
        }
    }

    [System.Serializable()]
    public struct ThreeDMatrix
    {
        public Vector3 newXAxis, newYAxis, newZAxis;
        public float det
        {
            get
            {
                return newXAxis.x * (newYAxis.y * newZAxis.z - newZAxis.y * newYAxis.z) - newYAxis.x * (newXAxis.y * newZAxis.z - newZAxis.y * newXAxis.z) + newZAxis.x * (newXAxis.y * newYAxis.z - newYAxis.y * newXAxis.z);
            }
        }

        public static ThreeDMatrix Lerp(ThreeDMatrix a, ThreeDMatrix b, float t)
        {
            return new ThreeDMatrix(a.newXAxis * (1f - t) + b.newXAxis * t, a.newYAxis * (1f - t) + b.newYAxis * t, a.newZAxis * (1f - t) + b.newZAxis * t);
        }

        public override string ToString()
        {
            return "    [" + newXAxis + ", " + newYAxis + ", " + newZAxis + "]";
        }
        public ThreeDMatrix(Vector3 newXAxis, Vector3 newYAxis, Vector3 newZAxis)
        {
            this.newXAxis = newXAxis;
            this.newYAxis = newYAxis;
            this.newZAxis = newZAxis;
        }
        public void Multiply(Vector3 a)
        {
            newXAxis *= a.x;
            newYAxis *= a.y;
            newZAxis *= a.z;
        }
        public static ThreeDMatrix operator *(ThreeDMatrix second, ThreeDMatrix first)
        {
            return new ThreeDMatrix(second.Transform(first.newXAxis), second.Transform(first.newYAxis), second.Transform(first.newZAxis));
        }
        public static ThreeDMatrix operator *(ThreeDMatrix m, float a)
        {
            return new ThreeDMatrix(m.newXAxis * a, m.newYAxis * a, m.newZAxis * a);
        }
        public static ThreeDMatrix operator +(ThreeDMatrix m, ThreeDMatrix a)
        {
            return new ThreeDMatrix(m.newXAxis + a.newXAxis, m.newYAxis + a.newYAxis, m.newZAxis + a.newZAxis);
        }
        public static ThreeDMatrix operator /(ThreeDMatrix m, float a)
        {
            return new ThreeDMatrix(m.newXAxis / a, m.newYAxis / a, m.newZAxis / a);
        }
        public ThreeDMatrix Inverse()
        {
            return Adjugate() / det;
        }
        public ThreeDMatrix Adjugate()
        {
            ThreeDMatrix result = new ThreeDMatrix();
            result.newXAxis.x = new TwoDMatrix(new Vector2(newYAxis.y, newYAxis.z), new Vector2(newZAxis.y, newZAxis.z)).det;
            result.newYAxis.x = -new TwoDMatrix(new Vector2(newYAxis.x, newYAxis.z), new Vector2(newZAxis.x, newZAxis.z)).det;
            result.newZAxis.x = new TwoDMatrix(new Vector2(newYAxis.x, newYAxis.y), new Vector2(newZAxis.x, newZAxis.y)).det;
            result.newXAxis.y = -new TwoDMatrix(new Vector2(newXAxis.y, newXAxis.z), new Vector2(newZAxis.y, newZAxis.z)).det;
            result.newYAxis.y = new TwoDMatrix(new Vector2(newXAxis.x, newXAxis.z), new Vector2(newZAxis.x, newZAxis.z)).det;
            result.newZAxis.y = -new TwoDMatrix(new Vector2(newXAxis.x, newXAxis.y), new Vector2(newZAxis.x, newZAxis.y)).det;
            result.newXAxis.z = new TwoDMatrix(new Vector2(newXAxis.y, newXAxis.z), new Vector2(newYAxis.y, newYAxis.z)).det;
            result.newYAxis.z = -new TwoDMatrix(new Vector2(newXAxis.x, newXAxis.z), new Vector2(newYAxis.x, newYAxis.z)).det;
            result.newZAxis.z = new TwoDMatrix(new Vector2(newXAxis.x, newXAxis.y), new Vector2(newYAxis.x, newYAxis.y)).det;
            return result;
        }
        public Vector3 Transform(Vector3 a)
        {
            return a.x * newXAxis + a.y * newYAxis + a.z * newZAxis;
        }

        public void Normalize()
        {
            if (det == 0f)
                return;
            float scale = Mathf.Pow(Mathf.Abs(det), -0.33333333333333333333333333333f);
            this *= scale;
        }

        public static ThreeDMatrix Exp(ThreeDMatrix m)
        {
            ThreeDMatrix x = new ThreeDMatrix(Vector3.right, Vector3.up, Vector3.forward);
            ThreeDMatrix output = new ThreeDMatrix(Vector3.right, Vector3.up, Vector3.forward);
            int iterations = (int)(Mathf.Abs(m.det) + 5);
            float factorial = 1f;
            for (int i = 0; i < iterations; i++)
            {
                factorial *= i + 1;
                x *= m;
                output += x / factorial;
            }
            return output;
        }
        /// <summary>
        /// Returns all eigenvalues of this matrix.
        /// </summary>
        /// <returns></returns>
        public List<float> GetEigenvalues()
        {
            float polyParamB = newXAxis.x + newYAxis.y + newZAxis.z;
            float polyParamC = newYAxis.z * newZAxis.y + newXAxis.y * newYAxis.x + newZAxis.x * newXAxis.z - newXAxis.x * newYAxis.y - newXAxis.x * newZAxis.z - newYAxis.y * newZAxis.z;
            float polyParamD = newXAxis.x * newYAxis.y * newZAxis.z - newXAxis.x * newYAxis.z * newZAxis.y - newYAxis.x * newXAxis.y * newZAxis.z + newYAxis.x * newZAxis.y * newXAxis.z + newZAxis.x * newXAxis.y * newYAxis.z - newZAxis.x * newYAxis.y * newXAxis.z;
            return SolveCubic(-1f, polyParamB, polyParamC, polyParamD);
        }

        /// <summary>
        /// Returns the Eigenvectors for this 3-D matrix, vectors formatted like: normalized (x, 1, z) * eigenvalue
        /// </summary>
        /// <returns></returns>
        public List<Vector3> GetEigenVectors()
        {
            List<Vector3> eVec = new List<Vector3>();
            List<float> eVal = GetEigenvalues();
            foreach(float f in eVal)
            {
                eVec.Add(GetEigenVector(f));
            }
            return eVec;
        }

        /// <summary>
        /// Returns the Eigenvector for this 3-D matrix of the corresponding eigenvalue, vectors formatted like: normalized (x, 1, z) * eigenvalue
        /// </summary>
        /// <returns></returns>
        public Vector3 GetEigenVector(float eigenvalue)
        {
            float x = (newZAxis.x * eigenvalue - newZAxis.x * newYAxis.y + newZAxis.y * newYAxis.x) / (newZAxis.y * eigenvalue - newZAxis.y * newXAxis.x + newXAxis.y * newZAxis.x);
            float z = (newXAxis.z * x + newYAxis.z) / (eigenvalue - newZAxis.z);
            return new Vector3(x, 1f, z) * eigenvalue;
        }
    }
    struct ComplexNumber
    {
        public float r, i;
        public static implicit operator ComplexNumber(float a)
        {
            return new ComplexNumber() { r = a, i = 0 };
        }
        public static ComplexNumber operator +(ComplexNumber a, ComplexNumber b)
        {
            return new ComplexNumber() { r = a.r + b.r, i = a.i + b.i };
        }
        public static ComplexNumber operator -(ComplexNumber a, ComplexNumber b)
        {
            return new ComplexNumber() { r = a.r - b.r, i = a.i - b.i };
        }
        public static ComplexNumber operator *(ComplexNumber a, ComplexNumber b)
        {
            return new ComplexNumber() { r = a.r * b.r - a.i * b.i, i = a.i * b.r + a.r * b.i };
        }
        public static ComplexNumber operator /(ComplexNumber a, ComplexNumber b)
        {
            return new ComplexNumber() { r = (a.r * b.r + a.i * b.i) / (b.r * b.r + b.i * b.i), i = (a.i * b.r - a.r * b.i) / (b.r * b.r + b.i * b.i) };
        }
        public static ComplexNumber Sqrt(ComplexNumber a)
        {
            return new ComplexNumber() { r = Mathf.Sqrt((Mathf.Sqrt(a.r * a.r + a.i * a.i) + a.r) / 2f), i = Mathf.Sqrt((Mathf.Sqrt(a.r * a.r + a.i * a.i) - a.r) / 2f) };
        }
        public static ComplexNumber Cbrt(ComplexNumber a)
        {
            if (a.i * a.i + a.r * a.r < 1E-10f)
                return new ComplexNumber() { r = 0, i = 0 };
            return Exp(Log(a) / 3f);
        }
        public static ComplexNumber Exp(ComplexNumber a)
        {
            return new ComplexNumber() { r = Mathf.Exp(a.r) * Mathf.Cos(a.i), i = Mathf.Exp(a.r) * Mathf.Sin(a.i) };
        }
        public static ComplexNumber Log(ComplexNumber a)
        {
            return new ComplexNumber() { r = Mathf.Log(new Vector2(a.r, a.i).magnitude, 2.718281828459045f), i = Mathf.Atan2(a.i, a.r) };
        }
    }
    public static List<float> SolveCubic(float a, float b, float c, float d)
    {
        List<float> solutions = new List<float>();
        float t0 = b * b - 3 * a * c;
        float t1 = 2 * b * b * b - 9 * a * b * c + 27 * a * a * d;
        ComplexNumber C = t1 + ComplexNumber.Sqrt(t1 * t1 - 4f * t0 * t0 * t0);

        C = ComplexNumber.Cbrt(C / 2f);
        for (int i = 0; i <= 2; i++)
        {
            if (C.r * C.r + C.i * C.i < 1E-10f)
                solutions.Add(0f);
            else
            {
                ComplexNumber sol = -0.333333333333333333333333333f * (b + C + (t0 / C)) / a;
                if (Mathf.Abs(sol.i) < 4E-6f)
                    solutions.Add(sol.r);
            }
            C *= new ComplexNumber() { r = -0.5f, i = 0.86602540378f };
        }
        return solutions;
    }

    public struct NxNMatrix
    {
        public float[,] entries;

        public override string ToString()
        {
            string s = "[";
            int size = entries.GetLength(0);
            for (int i = 0; i < size; i++)
            {
                s += "(";
                for (int j = 0; j < size; j++)
                    s += entries[i, j] + (j < size - 1 ? ", " : "");
                s += ")" + (i < size - 1 ? ", " : "");
            }
            return s + "]";
        }

        public NxNMatrix(float[,] entries)
        {
            if (entries.GetLength(0) != entries.GetLength(1))
                throw new System.Exception("Error: Matrix is not square.");
            this.entries = entries;
        }
        public NxNMatrix(int size)
        {
            entries = new float[size, size];
            for (int i = 0; i < size; i++)
                entries[i, i] = 1f;
        }
        public static NxNMatrix GetIdentityMatrix(int dimensions)
        {
            NxNMatrix matrix = new NxNMatrix(new float[dimensions, dimensions]);
            for (int i = 0; i < dimensions; i++)
                matrix.entries[i, i] = 1f;
            return matrix;
        }
        public static float[] Add(float[] a, float[] b)
        {
            if (a.Length != b.Length)
                throw new System.Exception("Vectors of different dimension!");
            for (int i = 0; i < a.Length; i++)
                a[i] += b[i];
            return a;
        }
        public static float[] Sub(float[] a, float[] b)
        {
            if (a.Length != b.Length)
                throw new System.Exception("Vectors of different dimension!");
            for (int i = 0; i < a.Length; i++)
                a[i] -= b[i];
            return a;
        }
        public static float[] Div(float[] a, float b)
        {
            for (int i = 0; i < a.Length; i++)
                a[i] /= b;
            return a;
        }
        public static float[] Mul(float[] a, float b)
        {
            for (int i = 0; i < a.Length; i++)
                a[i] *= b;
            return a;
        }
        public static float SqLen(float[] a)
        {
            float m = 0f;
            for (int i = 0; i < a.Length; i++)
                m += a[i] * a[i];
            return m;
        }
        public static float Dot(float[] a, float[] b)
        {
            if (a.Length != b.Length)
                throw new System.Exception("Vectors of different dimension!");
            float m = 0f;
            for (int i = 0; i < a.Length; i++)
                m += a[i] * b[i];
            return m;
        }
        public static string toStringVec(float[] a)
        {
            string s = "[";
            for (int i = 0; i < a.Length; i++)
                s += a[i] + (i < a.Length - 1 ? ", " : "");
            return s + "]";
        }
        public static NxNMatrix operator *(NxNMatrix a, NxNMatrix b)
        {
            if (a.entries.GetLength(0) != b.entries.GetLength(0))
                throw new System.Exception("Error: Attempting to multiply matrices of different size!");
            int size = a.entries.GetLength(0);
            NxNMatrix matrix = new NxNMatrix(new float[size, size]);
            for (int i = 0; i < size; i++)
            {
                float[] vector = new float[size];
                for (int j = 0; j < size; j++)
                    vector[j] = b.entries[i, j];
                vector = a.Transform(vector);
                for (int j = 0; j < size; j++)
                    matrix.entries[i, j] = vector[j];
            }
            return matrix;
        }

        public float GetDeterminant()
        {
            if (entries.GetLength(0) == 2)
                return entries[0, 0] * entries[1, 1] - entries[1, 0] * entries[0, 1];
            float det = 0f;
            for (int i = 0; i < entries.GetLength(0); i++)
                if (entries[i, 0] != 0)
                    det += (i % 2 == 0 ? 1 : -1) * entries[i, 0] * GetMinor(new Vector2Int(i, 0)).GetDeterminant();
            return det;
        }

        public NxNMatrix GetMatrixOfMinors()
        {
            int size = entries.GetLength(0);
            NxNMatrix m = new NxNMatrix(new float[size, size]);
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    m.entries[i, j] = GetMinor(new Vector2Int(i, j)).GetDeterminant();
            return m;
        }
        
        public NxNMatrix GetMatrixOfCofactors()
        {
            int size = entries.GetLength(0);
            NxNMatrix m = new NxNMatrix(new float[size, size]);
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    m.entries[i, j] = entries[i,j] * ((i + j) % 2 == 0 ? 1 : -1);
            return m;
        }
        public NxNMatrix GetTranspose()
        {
            int size = entries.GetLength(0);
            NxNMatrix m = new NxNMatrix(new float[size, size]);
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    m.entries[i, j] = entries[j, i];
            return m;
        }
        
        public static NxNMatrix operator *(NxNMatrix a, float b)
        {
            for (int i = 0; i < a.entries.GetLength(0); i++)
                for (int j = 0; j < a.entries.GetLength(0); j++)
                    a.entries[i, j] *= b;
            return a;
        }
        public static NxNMatrix operator /(NxNMatrix a, float b)
        {
            for (int i = 0; i < a.entries.GetLength(0); i++)
                for (int j = 0; j < a.entries.GetLength(0); j++)
                    a.entries[i, j] /= b;
            return a;
        }

        public NxNMatrix GetInverse()
        {
            return GetAdjoint() / GetDeterminant();
        }
        public NxNMatrix GetAdjoint()
        {
            return GetMatrixOfMinors().GetMatrixOfCofactors().GetTranspose();
        }

        public NxNMatrix GetMinor(Vector2Int pos)
        {
            int size = entries.GetLength(0);
            NxNMatrix m = new NxNMatrix(new float[size - 1, size - 1]);
            int currI = 0;
            int currJ = 0;
            for (int i = 0; i < size; i++)
            {
                if (i == pos.x)
                    continue;

                for (int j = 0; j < size; j++)
                {
                    if (j == pos.y)
                        continue;

                    m.entries[currI, currJ] = entries[i, j];
                    currJ++;
                }
                currI++;
                currJ = 0;
            }
            return m;
        }

        public float[] Transform(float[] inputVector)
        {
            if (inputVector.Length != entries.GetLength(0))
                throw new System.Exception("Error: Wrong number of dimensions in matrix multiplication!");
            float[] output = new float[inputVector.Length];
            for (int i = 0; i < inputVector.Length; i++)
                for (int j = 0; j < inputVector.Length; j++)
                {
                    output[i] += entries[j, i] * inputVector[j];
                }
            return output;
        }
    }
}
