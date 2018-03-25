using System;
using System.Collections.Generic;
using SpaceSim.Drawing;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Physics
{
    static class GravitationalBodyIterator
    {
        public static int Next(int currentIndex, IList<IGravitationalBody> bodies, Camera camera)
        {
            DVector2 cameraNormal = camera.Rotation == 0 ? new DVector2(1, 0) : DVector2.FromAngle(-camera.Rotation);

            return Iterate(currentIndex, bodies, cameraNormal);
        }

        public static int Prev(int currentIndex, IList<IGravitationalBody> bodies, Camera camera)
        {
            DVector2 cameraNormal = camera.Rotation == 0 ? new DVector2(-1, 0) : DVector2.FromAngle(camera.Rotation);

            return Iterate(currentIndex, bodies, cameraNormal);
        }

        private static int Iterate(int currentIndex, IList<IGravitationalBody> bodies, DVector2 iterateNormal)
        {
            HashSet<int> connectedBodies = GetConnectedBodies(currentIndex, bodies);

            List<Tuple<double, int>> bodiesByDistance = GetSortedBodyDistances(currentIndex, bodies, iterateNormal);

            foreach (Tuple<double, int> bodyDistancePair in bodiesByDistance)
            {
                int targetId = bodyDistancePair.Item2;

                // The closest object is still part of the same craft
                if (connectedBodies.Contains(targetId))
                {
                    continue;
                }

                return GetParentIndex(targetId, bodies);
            }

            return currentIndex;
        }

        private static int GetParentIndex(int targetIndex, IList<IGravitationalBody> bodies)
        {
            while (bodies[targetIndex] is ISpaceCraft)
            {
                var targetBody = bodies[targetIndex] as ISpaceCraft;

                if (targetBody.Parent == null)
                {
                    break;
                }

                targetIndex--;
            }

            return targetIndex;
        }

        // Gets bodies sorted by distance that point the direction of the camera normal
        private static List<Tuple<double, int>> GetSortedBodyDistances(int currentIndex, IList<IGravitationalBody> bodies, DVector2 cameraNormal)
        {
            DVector2 targetCenter = bodies[currentIndex].Position;

            var bodiesByDistance = new List<Tuple<double, int>>();

            for (int i = 0; i < bodies.Count; i++)
            {
                if (i == currentIndex) continue;

                DVector2 difference = bodies[i].Position - targetCenter;

                double distance = difference.LengthSquared();

                difference.Normalize();

                // Only add bodies in the same direction as the camera normal
                if (difference.Dot(cameraNormal) > 0)
                {
                    bodiesByDistance.Add(new Tuple<double, int>(distance, i));
                }
            }

            bodiesByDistance.Sort(Compare);

            return bodiesByDistance;
        }

        private static HashSet<int> GetConnectedBodies(int currentIndex, IList<IGravitationalBody> bodies)
        {
            var connectedBodies = new HashSet<int>();

            // Only bother checking for connected bodies if the target is a spacecraft
            if (bodies[currentIndex] is ISpaceCraft)
            {
                for (int i = currentIndex + 1; i < bodies.Count; i++)
                {
                    var craft = bodies[i] as ISpaceCraft;

                    // Next object not a craft, exit early
                    if (craft == null)
                    {
                        break;
                    }

                    // Next object is craft without a parent, exit early
                    if (craft.Parent == null)
                    {
                        break;
                    }

                    connectedBodies.Add(i);
                }
            }

            return connectedBodies;
        }

        private static int Compare(Tuple<double, int> one,
                                   Tuple<double, int> two)
        {
            return one.Item1.CompareTo(two.Item1);
        }
    }
}
