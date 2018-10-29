using System;
using System.Drawing;
using SpaceSim.Common;
using SpaceSim.Drawing;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts
{
    abstract class SpaceCraftPart : IPhysicsBody, IGdiRenderable
    {
        public DVector2 Position { get; protected set; }
        public DVector2 Velocity { get; protected set; }
        public double Mass { get; protected set; }
        public double Pitch { get; protected set; }

        public abstract double Width { get; protected set; }
        public abstract double Height { get; protected set; }
        protected abstract double DrawingOffset { get; }

        protected ISpaceCraft _parent;

        private double _sootRatio;
        private SootRenderer _sootRenderer;

        protected SpaceCraftPart(ISpaceCraft parent, string baseTexturePath)
        {
            var baseTexture = new Bitmap(baseTexturePath);
            _sootRenderer = new SootRenderer(baseTexture, baseTexturePath);

            _parent = parent;
        }

        public abstract void Update(double dt);

        public void UpdateSootRatio(double ratio)
        {
            _sootRatio = ratio;
        }

        public virtual void RenderGdi(Graphics graphics, Camera camera)
        {
            Bitmap sootedTexture = _sootRenderer.GenerateTexture(_sootRatio);

            double drawingRotation = _parent.Pitch + Pitch;

            DVector2 drawingOffset = new DVector2(Math.Cos(drawingRotation), Math.Sin(drawingRotation)) * DrawingOffset;

            RectangleF screenBounds = RenderUtils.ComputeBoundingBox(Position - drawingOffset, camera.Bounds, Width, Height);

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f, screenBounds.Y + screenBounds.Height * 0.5f);

            camera.ApplyScreenRotation(graphics);
            camera.ApplyRotationMatrix(graphics, offset, drawingRotation + Constants.PiOverTwo);

            int rollAngle = (int)(_parent.Roll * MathHelper.RadiansToDegrees) % 360;
            if (rollAngle <= 90)
            {
                graphics.DrawImage(sootedTexture, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);
            }
            else
            {
                graphics.DrawImage(sootedTexture, screenBounds.X + screenBounds.Width * 2.0f, screenBounds.Y, -screenBounds.Width, screenBounds.Height);
            }

            graphics.ResetTransform();
        }
    }
}
