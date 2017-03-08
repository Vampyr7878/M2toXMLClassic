using System.Windows.Media.Media3D;

namespace M2_to_XML
{
    class PositionVector : Matrix
    {
        public PositionVector() : base(4, 1)
        {
        }

        public PositionVector(Vector3D vector) : base(4, 1)
        {
            fields[0][0] = (float)vector.X;
            fields[1][0] = (float)vector.Y;
            fields[2][0] = (float)vector.Z;
            fields[3][0] = 1f;
        }

        public static PositionVector operator +(PositionVector v1, PositionVector v2)
        {
            PositionVector matrix = new PositionVector();
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    matrix[i, j] = v1[i, j] + v2[i, j];
                }
            }
            return matrix;
        }

        public static PositionVector operator *(TransformationMatrix m, PositionVector v)
        {
            PositionVector vector = new PositionVector();
            vector.Zero();
            for(int i = 0; i < vector.Rows; i++)
            {
                for(int j = 0; j < m.Columns; j++)
                {
                    vector[i, 0] += m[i, j] * v[j, 0];
                }
            }
            return vector;
        }

        public static PositionVector operator *(PositionVector v, float n)
        {
            PositionVector matrix = new PositionVector();
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    matrix[i, j] = v[i, j] * n;
                }
            }
            return matrix;
        }
    }
}
