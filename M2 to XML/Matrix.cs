namespace M2_to_XML
{
    class Matrix
    {
        protected float[][] fields;

        public Matrix(int rows, int columns)
        {
            fields = new float[rows][];
            for(int i = 0; i < rows; i++)
            {
                fields[i] = new float[columns];
            }
        }

        public int Rows
        {
            get { return fields.Length; }
        }

        public int Columns
        {
            get { return fields[0].Length; }
        }

        public float this[int row, int column]
        {
            get { return fields[row][column]; }
            set { fields[row][column] = value; }
        }

        public void Zero()
        {
            for (int i = 0; i < this.Rows; i++)
            {
                for(int j = 0; j < this.Columns; j++)
                {
                    fields[i][j] = 0f;
                }
            }
        }

        public void Unit()
        {
            Zero();
            for(int i = 0; i < this.Rows; i++)
            {
                fields[i][i] = 1f;
            }
        }
    }
}
