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

            Left = x;
            Right = x + width;

            Top = y;
            Bottom = y + height;
        }

        public bool Contains(DVector2 other)
        {
            return (other.X > Left && other.X < Right && other.Y > Top && other.Y < Bottom);
        }

        public bool IntersectsWith(RectangleD other)
        {
            return (Left < other.Right && Right > other.Left && Top < other.Bottom && Bottom > other.Top);
        }
    }
}
