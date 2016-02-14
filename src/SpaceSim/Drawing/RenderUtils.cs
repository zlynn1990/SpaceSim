using System.Collections.Generic;
using System.Drawing;
using VectorMath;

namespace SpaceSim.Drawing
{
    static class RenderUtils
    {
        public static int ScreenWidth;
        public static int ScreenHeight;

        public static int ScreenArea;

        public static PointF WorldToScreen(DVector2 point, RectangleD cameraBounds)
        {
            var normalizedPoint = new DVector2((point.X - cameraBounds.Left) / cameraBounds.Width,
                                               (point.Y - cameraBounds.Top) / cameraBounds.Height);

            return new PointF((float)(normalizedPoint.X * ScreenWidth),
                              (float)(normalizedPoint.Y * ScreenHeight));
        }

        public static void DrawRectangles(Graphics graphics, List<RectangleF> rectangles, Color color)
        {
            if (rectangles.Count > 0)
            {
                var brush = new SolidBrush(color);

                graphics.FillRectangles(brush, rectangles.ToArray());
            }
        }

        public static void DrawLine(Graphics graphics, RectangleD cameraBounds, DVector2 start, DVector2 end, Color color)
        {
            PointF localStart = WorldToScreen(start, cameraBounds);
            PointF localEnd = WorldToScreen(end, cameraBounds);

            localStart.X = MathHelper.Clamp(localStart.X, 0, ScreenWidth);
            localStart.Y = MathHelper.Clamp(localStart.Y, 0, ScreenHeight);

            localEnd.X = MathHelper.Clamp(localEnd.X, 0, ScreenWidth);
            localEnd.Y = MathHelper.Clamp(localEnd.Y, 0, ScreenHeight);

            graphics.DrawLine(new Pen(color, 2), localStart, localEnd);
        }

        public static RectangleF ComputeEllipseSize(DVector2 position, RectangleD cameraBounds, double radius)
        {
            double screenRadius = (radius / cameraBounds.Width) * ScreenWidth;

            DVector2 screenPosition = position - new DVector2(cameraBounds.Left, cameraBounds.Top);

            double screenU = screenPosition.X / cameraBounds.Width;
            double screenV = screenPosition.Y / cameraBounds.Height;

            double screenX = screenU * ScreenWidth;
            double screenY = screenV * ScreenHeight;

            return new RectangleF((float) (screenX - screenRadius),
                                  (float) (screenY - screenRadius),
                                  (float) (screenRadius*2.0), (float) (screenRadius*2.0));
        }

        public static RectangleF ComputeBoundingBox(DVector2 position, RectangleD cameraBounds, double width, double height)
        {
            double screenWidth = (width / cameraBounds.Width) * ScreenWidth;
            double screenHeight = (height / cameraBounds.Height) * ScreenHeight;

            DVector2 screenPosition = position - new DVector2(cameraBounds.Left, cameraBounds.Top);

            double screenU = screenPosition.X / cameraBounds.Width;
            double screenV = screenPosition.Y / cameraBounds.Height;

            double screenX = screenU * ScreenWidth;
            double screenY = screenV * ScreenHeight;

            return new RectangleF((float) (screenX - screenWidth*0.5),
                                  (float) (screenY - screenHeight*0.5),
                                  (float) screenWidth, (float) screenHeight);
        }
    }
}
