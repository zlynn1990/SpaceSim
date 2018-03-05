namespace VectorMath
{
    public class RectangleD
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public double Width { get; private set; }
        public double Height { get; private set; }

        public double Left { get; private set; }
        public double Right { get; private set; }

        public double Top { get; private set; }
        public double Bottom { get; private set; }

        public RectangleD(double x, double y, double width, double height)
        {
            X = x;
            Y = y;

            Width = width;
            Height = height;

            ComputeProperties();
        }

        public bool Contains(DVector2 other)
        {
            return (other.X > Left && other.X < Right && other.Y > Top && other.Y < Bottom);
        }

        public bool IntersectsWith(RectangleD other)
        {
            return (Left < other.Right && Right > other.Left && Top < other.Bottom && Bottom > other.Top);
        }

        public void Inflate(double x, double y)
        {
            X -= x * 0.5;
            Y -= y * 0.5;

            Width += x;
            Height += y;

            ComputeProperties();
        }

        public RectangleD Clone()
        {
            return new RectangleD(X, Y, Width, Height);
        }

        private void ComputeProperties()
        {
            Left = X;
            Right = X + Width;

            Top = Y;
            Bottom = Y + Height;
        }
    }
}
