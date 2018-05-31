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
            DVector2 cameraNormal = DVector2.FromAngle(-camera.Rotation);

            return Iterate(currentIndex, bodies, cameraNormal);
        }

        public static int Prev(int currentIndex, IList<IGravitationalBody> bodies, Camera camera)
        {
            DVector2 cameraNormal = DVector2.FromAngle(Math.PI - camera.Rotation);

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
            // Keep iterating until the parent index is found so that the focus becomes the parent
            while (bodies[targetIndex] is ISpaceCraft)
            {
                var targetBody = bodies[targetIndex] as ISpaceCraft;

                if (targetBody.Parent == null)
                {
                    break;
                }

                // If the target has a parent try that parent and continue to go up the chain
                for (int i = 0; i < bodies.Count; i++)
                {
                    if (targetBody.Parent == bodies[i])
                    {
                        targetIndex = i;
                    }
                }
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

                var spaceCraft = bodies[i] as ISpaceCraft;

                // Skip terminated bodies
                if (spaceCraft != null && spaceCraft.Terminated)
                {
                    continue;
                }

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
            var bodyHash = new HashSet<int>();

            var targetCraft = bodies[currentIndex] as ISpaceCraft;

            // Only bother checking for connected bodies if the target is a spacecraft
            if (targetCraft != null)
            {
                List<int> connectedBodies = ConnectedBodyHelper(targetCraft, bodies);

                foreach (int bodyIndex in connectedBodies)
                {
                    bodyHash.Add(bodyIndex);
                }
            }

            return bodyHash;
        }

        private static List<int> ConnectedBodyHelper(IGravitationalBody target, IList<IGravitationalBody> bodies)
        {
            var connections = new List<int>();

            // Find all bodies connected to the target
            for (int i = 0; i < bodies.Count; i++)
            {
                var craft = bodies[i] as ISpaceCraft;

                if (craft != null && craft.Parent != null)
                {
                    if (craft.Parent == target)
                    {
                        connections.Add(i);

                        // Recursively add all the bodies connected to this one
                        connections.AddRange(ConnectedBodyHelper(craft, bodies));
                    }
                }
            }

            return connections;
        }

        private static int Compare(Tuple<double, int> one,
                                   Tuple<double, int> two)
        {
            return one.Item1.CompareTo(two.Item1);
        }
    }
}
