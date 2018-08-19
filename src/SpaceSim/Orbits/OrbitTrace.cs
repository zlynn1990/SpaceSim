using System.Collections.Generic;
using System.Drawing;
using SpaceSim.Drawing;
using VectorMath;

namespace SpaceSim.Orbits
{
    class OrbitTrace
    {
        public double Apogee { get; private set; }
        public double Perigee { get; private set; }

        private List<DVector2> _points;

        private int _apogeeIndex;
        private int _perigeeIndex;

        public OrbitTrace()
        {
            _points = new List<DVector2>();
        }

        public void Reset(DVector2 start)
        {
            Apogee = 0;
            Perigee = double.MaxValue;

            _apogeeIndex = 0;
            _perigeeIndex = 0;

            _points.Clear();
            _points.Add(start);
        }

        public void AddPoint(DVector2 point, double altitude)
        {
            if (altitude > Apogee)
            {
                Apogee = altitude;
                _apogeeIndex = _points.Count;
            }
            else if (altitude < Perigee)
            {
                Perigee = altitude;

                _perigeeIndex = _points.Count;
            }

            _points.Add(point);
        }

        public void Draw(Graphics graphics, Camera camera, IMapRenderable orbitingBody)
        {
            var traceBounds = new List<RectangleF>();

            for (int i = 1; i < _points.Count; i++)
            {
                DVector2 orbitPoint = _points[i];

                if (camera.Contains(orbitPoint))
                {
                    PointF localPoint = RenderUtils.WorldToScreen(orbitPoint, camera.Bounds);

                    if (i == _apogeeIndex && i > 1)
                    {
                        RenderApogee(graphics, localPoint);
                    }
                    else if (i == _perigeeIndex)
                    {
                        RenderPerigee(graphics, localPoint);
                    }
                    else
                    {
                        traceBounds.Add(new RectangleF(localPoint.X, localPoint.Y, 2, 2));
                    }
                }
            }

            if (_points.Count > 0 && camera.Contains(_points[0]))
            {
                RenderStart(graphics, camera.Bounds, orbitingBody, _points[0]);
            }

            RenderUtils.DrawRectangles(graphics, traceBounds, orbitingBody.IconColor);
        }

        private static void RenderStart(Graphics graphics, RectangleD cameraBounds, IMapRenderable orbitingBody, DVector2 start)
        {
            double visibility = orbitingBody.Visibility(cameraBounds);

            if (visibility < 1)
            {
                PointF iconPoint = RenderUtils.WorldToScreen(start, cameraBounds);

                var iconBounds = new RectangleF(iconPoint.X - 5, iconPoint.Y - 5, 10, 10);

                var iconColor = Color.FromArgb((int)((1 - visibility) * 255),
                                               orbitingBody.IconColor.R,
                                               orbitingBody.IconColor.G,
                                               orbitingBody.IconColor.B);

                graphics.FillEllipse(new SolidBrush(iconColor), iconBounds);
            }
        }

        private static void RenderApogee(Graphics graphics, PointF localPoint)
        {
            var bounds = new RectangleF(localPoint.X - 2.5f, localPoint.Y - 2.5f, 5, 5);

            graphics.FillEllipse(new SolidBrush(Color.Red), bounds);
        }

        private static void RenderPerigee(Graphics graphics, PointF localPoint)
        {
            var bounds = new RectangleF(localPoint.X - 2.5f, localPoint.Y - 2.5f, 5, 5);

            graphics.FillEllipse(new SolidBrush(Color.Yellow), bounds);
        }
    }
}