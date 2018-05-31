using System;
using System.Collections.Generic;
using SpaceSim.Physics;
using SpaceSim.Proxies;
using SpaceSim.SolarSystem;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Orbits
{
    static class OrbitHelper
    {
        private static double AngularCutoff = Constants.TwoPi - 0.01;

        /// <summary>
        /// Converts orbital data from JPL Emphemeris data.
        /// </summary>
        public static DVector2 FromJplEphemeris(double x, double y)
        {
            return new DVector2(x, -y) * 1000;
        }

        public static void SimulateToTime(List<IMassiveBody> bodies, DateTime targetDate, double timeStep)
        {
            if (targetDate < Constants.Epoch)
            {
                throw new Exception("Starting date must be greater than the epoch: " + Constants.Epoch.ToLongDateString());
            }

            TimeSpan timeToSimulate = targetDate - Constants.Epoch;

            int iterations = (int)(timeToSimulate.TotalSeconds / timeStep);

            for (int i = 0; i < iterations; i++)
            {
                // Resolve n body massive body forces
                foreach (IMassiveBody bodyA in bodies)
                {
                    bodyA.ResetAccelerations();

                    foreach (IMassiveBody bodyB in bodies)
                    {
                        if (bodyA == bodyB) continue;

                        bodyA.ResolveGravitation(bodyB);
                    }
                }

                // Update bodies
                foreach (IMassiveBody body in bodies)
                {
                    body.Update(timeStep);
                }
            }

            // Reset bodies to their starting rotations so spacecraft behave correctly
            foreach (IMassiveBody body in bodies)
            {
                body.ResetOrientation();
            }
        }

        /// <summary>
        /// Traces a massive body orbit by re-centering the world around the parent.
        /// </summary>
        public static void TraceMassiveBody(MassiveBodyBase body, OrbitTrace trace)
        {
            IMassiveBody parent = body.GravitationalParent;

            DVector2 initialPosition = body.Position - parent.Position;

            var proxyParent = new MassiveBodyProxy(DVector2.Zero, DVector2.Zero, parent);
            var proxySatellite = new MassiveBodyProxy(initialPosition, body.Velocity - parent.Velocity, body);

            double orbitalDt = GetOrbitalDt(initialPosition.Length(), parent.Mass, proxySatellite.Velocity.Length());

            trace.Reset(body.Position);

            // Assumes that the parent is (0,0) which will be true in this case
            double previousAngle = proxySatellite.Position.Angle();
            double totalAngularDisplacement = 0;

            // Max of 1000 steps, in practice it's 150-200
            for (int i = 0; i < 1000; i++)
            {
                proxySatellite.ResetAccelerations();

                proxySatellite.ResolveGravitation(proxyParent);

                proxySatellite.Update(orbitalDt);

                double currentAngle = proxySatellite.Position.Angle();

                totalAngularDisplacement += DeltaAngle(currentAngle, previousAngle);

                // Made a full orbit
                if (totalAngularDisplacement > AngularCutoff)
                {
                    break;
                }

                previousAngle = currentAngle;

                double altitude = proxyParent.GetRelativeHeight(proxySatellite.Position);

                trace.AddPoint(proxySatellite.Position + parent.Position, altitude);
            }
        }

        /// <summary>
        /// Traces a space craft orbit by re-centering the world around the parent.
        /// </summary>
        public static void TraceSpaceCraft(SpaceCraftBase satellite, OrbitTrace trace)
        {
            IMassiveBody parent = satellite.GravitationalParent;

            DVector2 initialPosition = satellite.Position - parent.Position;

            var proxyParent = new MassiveBodyProxy(DVector2.Zero, DVector2.Zero, parent);
            var proxySatellite = new SpaceCraftProxy(initialPosition, satellite.Velocity - parent.Velocity, satellite);

            proxySatellite.SetGravitationalParent(proxyParent);

            int stepCount;

            double targetDt;
            double proximityDt;
            double orbitalDt = 0;

            bool isOrbiting;

            double altitude = proxyParent.GetRelativeHeight(proxySatellite.Position);
            double proximityAltitude = proxyParent.SurfaceRadius * 0.15;

            if (altitude < proximityAltitude)
            {
                stepCount = 1100;

                isOrbiting = false;

                proximityDt = GetProximityDt(altitude, proximityAltitude);

                targetDt = proximityDt;
            }
            else
            {
                stepCount = 600;

                isOrbiting = true;

                orbitalDt = GetOrbitalDt(initialPosition.Length(), parent.Mass, proxySatellite.Velocity.Length());

                targetDt = orbitalDt;
            }

            // Assumes that the parent is (0,0) which will be true in this case
            double previousAngle = proxySatellite.Position.Angle();
            double totalAngularDisplacement = 0;

            trace.Reset(satellite.Position);

            // Update steps based on orbit vs atmosphere
            for (int step = 0; step < stepCount; step++)
            {
                proxySatellite.ResetAccelerations();

                proxySatellite.ResolveGravitation(proxyParent);
                proxySatellite.ResolveAtmopsherics(proxyParent);

                proxySatellite.Update(targetDt);

                double currentAngle = proxySatellite.Position.Angle();

                totalAngularDisplacement += DeltaAngle(currentAngle, previousAngle);

                // Made a full orbit
                if (totalAngularDisplacement > AngularCutoff)
                {
                    break;
                }

                previousAngle = currentAngle;

                altitude = proxyParent.GetRelativeHeight(proxySatellite.Position);

                double velocity = proxySatellite.GetRelativeVelocity().Length();

                double offsetFactor = 0.0;

                //if (altitude < proxyParent.AtmosphereHeight*2)
                if (altitude < proxyParent.AtmosphereHeight * 4)
                {
                    if (velocity < 3000)
                    {
                        offsetFactor = 1.0;
                    }
                    else if (velocity < 4000)
                    {
                        offsetFactor = 1.0 - velocity / 3000.0;
                    }   
                }

                // Check if reference frame shifting needs to occur in atmosphere
                if (offsetFactor > 0.0001)
                {
                    DVector2 difference = proxyParent.Position - proxySatellite.Position;
                    difference.Normalize();

                    var surfaceNormal = new DVector2(-difference.Y, difference.X);

                    double altitudeFromCenter = altitude + proxyParent.SurfaceRadius;

                    // Distance of circumference at this altitude ( c= r * 2pi )
                    double pathCirumference = Constants.TwoPi * altitudeFromCenter;

                    double rotationalSpeed = pathCirumference / parent.RotationPeriod;

                    DVector2 atmopshereVelocity = surfaceNormal * rotationalSpeed;

                    proxySatellite.ApplyFrameOffset(atmopshereVelocity * offsetFactor * targetDt);
                }

                // Return early if the trace goes into a planet
                if (altitude <= 0)
                {
                    trace.AddPoint(proxySatellite.Position + parent.Position, altitude);
                    break;
                }

                // Determine the correct change overs from orbital to surface proximity
                if (isOrbiting)
                {
                    if (altitude < proximityAltitude)
                    {
                        proximityDt = GetProximityDt(altitude, proximityAltitude);

                        targetDt = MathHelper.Lerp(targetDt, proximityDt, 0.75);

                        isOrbiting = false;

                        stepCount = 1000;
                    }
                    else
                    {
                        targetDt = MathHelper.Lerp(targetDt, orbitalDt, 0.1);
                    }
                }
                else
                {
                    if (altitude > proximityAltitude)
                    {
                        orbitalDt = GetOrbitalDt(proxySatellite.Position.Length(), parent.Mass, proxySatellite.Velocity.Length());

                        targetDt = MathHelper.Lerp(targetDt, orbitalDt, 0.1);

                        isOrbiting = true;

                        stepCount = 600;
                    }
                    else
                    {
                        proximityDt = GetProximityDt(altitude, proximityAltitude);

                        targetDt = MathHelper.Lerp(targetDt, proximityDt, 0.75);
                    }
                }

                trace.AddPoint(proxySatellite.Position + parent.Position, altitude);
            }
        }

        // Finds the orbtial delta time step by assuming 300 points along the oribtal cirumference
        private static double GetOrbitalDt(double distance, double parentMass, double velocity)
        {
            double circularVelocity = Math.Sqrt((Constants.GravitationConstant * parentMass) / distance);

            double velocityRatio = velocity / circularVelocity;

            double approximateOrbitDiameter = distance * Constants.TwoPi;

            double approximateOrbitPeriod = approximateOrbitDiameter / velocity;

            return Math.Max(approximateOrbitPeriod * 0.0033 * velocityRatio, 125);
        }

        private static double GetProximityDt(double altitude, double proxityAltitude)
        {
            double altitudeRatio = altitude / proxityAltitude;

            return Math.Max(altitudeRatio * 15, 1);
        }

        private static double DeltaAngle(double angle1, double angle2)
        {
            double difference = angle2 - angle1;

            while (difference < -Constants.PiOverTwo)
            {
                difference += Math.PI;
            }

            while (difference > Constants.PiOverTwo)
            {
                difference -= Math.PI;
            }

            return Math.Abs(difference);
        }
    }
}
