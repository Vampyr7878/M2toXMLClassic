using System;
using System.Windows.Media.Media3D;

namespace M2_to_XML
{
    class TransformationMatrix : Matrix
    {
        public TransformationMatrix() : base(4, 4)
        {
        }

        public static TransformationMatrix operator *(TransformationMatrix m1, TransformationMatrix m2)
        {
            TransformationMatrix matrix = new TransformationMatrix();
            matrix.Zero();
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    for (int k = 0; k < m1.Columns; k++)
                    {
                        matrix[i, j] += m1[i, k] * m2[k, j];
                    }
                }
            }
            return matrix;
        }

        public void Inverse()
        {
            fields[0][3] = -fields[0][3];
            fields[1][3] = -fields[1][3];
            fields[2][3] = -fields[2][3];
        }

        public void Translate(Vector3D translation)
        {
            Unit();
            fields[0][3] = (float)translation.X;
            fields[1][3] = (float)translation.Y;
            fields[2][3] = (float)translation.Z;
        }

        public static TransformationMatrix Translation(Vector3D translation)
        {
            TransformationMatrix matrix = new TransformationMatrix();
            matrix.Translate(translation);
            return matrix;
        }

        public void QuaterionRotate(Quaternion rotation)
        {
            Zero();
            fields[3][3] = 1;
            fields[0][0] = 1 - 2 * (float)rotation.Y * (float)rotation.Y - 2 * (float)rotation.Z * (float)rotation.Z;
            fields[0][1] = 2 * (float)rotation.X * (float)rotation.Y - 2 * (float)rotation.Z * (float)rotation.W;
            fields[0][2] = 2 * (float)rotation.X * (float)rotation.Z + 2 * (float)rotation.Y * (float)rotation.W;
            fields[1][0] = 2 * (float)rotation.X * (float)rotation.Y + 2 * (float)rotation.Z * (float)rotation.W;
            fields[1][1] = 1 - 2 * (float)rotation.X * (float)rotation.X - 2 * (float)rotation.Z * (float)rotation.Z;
            fields[1][2] = 2 * (float)rotation.Y * (float)rotation.Z - 2 * (float)rotation.X * (float)rotation.W;
            fields[2][0] = 2 * (float)rotation.X * (float)rotation.Z - 2 * (float)rotation.Y * (float)rotation.W;
            fields[2][1] = 2 * (float)rotation.Y * (float)rotation.Z + 2 * (float)rotation.X * (float)rotation.W;
            fields[2][2] = 1 - 2 * (float)rotation.X * (float)rotation.X - 2 * (float)rotation.Y * (float)rotation.Y;
        }

        public Quaternion ToQuaterion()
        {
            float w = (float)Math.Sqrt(1 + fields[0][0] + fields[1][1] + fields[2][2]) / 2;
            float x = (fields[2][1] - fields[1][2]) / (4 * w);
            float y = (fields[0][2] - fields[2][0]) / (4 * w);
            float z = (fields[1][0] - fields[0][1]) / (4 * w);
            return new Quaternion(x, y, z, w);
        }

        public static TransformationMatrix QuaterionRotation(Quaternion rotation)
        {
            TransformationMatrix matrix = new TransformationMatrix();
            matrix.QuaterionRotate(rotation);
            return matrix;
        }
    }
}
