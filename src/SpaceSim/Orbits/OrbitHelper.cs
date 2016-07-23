using System;
using SpaceSim.Proxies;
using SpaceSim.SolarSystem;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Orbits
{
    static class OrbitHelper
    {
        /// <summary>
        /// Gets the position of an object in periapsis form.
        /// </summary>
        public static DVector2 GetPosition(double periapsis, double longitudeOfPeriapsis, DVector2 offset)
        {
            return new DVector2(Math.Cos(longitudeOfPeriapsis), Math.Sin(longitudeOfPeriapsis)) * periapsis + offset;
        }

        /// <summary>
        /// Gets the velocity of an object in periapsis form.
        /// </summary>
        public static DVector2 GetVelocity(double periapsis, double longitudeOfPeriapsis, double speed, DVector2 offset)
        {
            return new DVector2(Math.Cos(longitudeOfPeriapsis + Math.PI*0.5), Math.Sin(longitudeOfPeriapsis + Math.PI*0.5)) * speed + offset;
        }

        /// <summary>
        /// Traces a massive body orbit by re-centering the world around the parent.
        /// </summary>
        public static OrbitTrace TraceMassiveBody(MassiveBodyBase body)
        {
            IMassiveBody parent = body.GravitationalParent;

            DVector2 initialPosition = body.Position - parent.Position;

            var proxyParent = new MassiveBodyProxy(DVector2.Zero, DVector2.Zero, parent);
            var proxySatellite = new MassiveBodyProxy(initialPosition, body.Velocity - parent.Velocity, body);

            double orbitalTerminationRadius;
            double altitude = body.GetRelativeAltitude();

            double orbitalDt = GetOrbitalDt(initialPosition, proxySatellite.Velocity, out orbitalTerminationRadius);

            var trace = new OrbitTrace(body.Position, altitude);

            for (int i=0; i < 300; i++)
            {
                proxySatellite.ResetAccelerations();

                proxySatellite.ResolveGravitation(proxyParent);

                proxySatellite.Update(orbitalDt);

                altitude = proxyParent.GetRelativeHeight(proxySatellite.Position);

                // Check expensive termination conditions after half of the iterations
                if (i > 150)
                {
                    DVector2 offsetVector = proxySatellite.Position - initialPosition;

                    double distanceFromStart = offsetVector.Length();

                    // Terminate and add the end point
                    if (distanceFromStart < orbitalTerminationRadius)
                    {
                        trace.AddPoint(proxySatellite.Position + parent.Position, altitude);
                        break;
                    }
                }

                trace.AddPoint(proxySatellite.Position + parent.Position, altitude);
            }

            return trace;
        }

        /// <summary>
        /// Traces a space craft orbit by re-centering the world around the parent.
        /// </summary>
        public static OrbitTrace TraceSpaceCraft(SpaceCraftBase satellite)
        {
            IMassiveBody parent = satellite.GravitationalParent;

            DVector2 initialPosition = satellite.Position - parent.Position;

            var shipOffset = new DVector2(Math.Cos(satellite.Rotation) * (satellite.TotalWidth - satellite.Width),
                                          Math.Sin(satellite.Rotation) * (satellite.TotalHeight - satellite.Height)) * 0.5;

            initialPosition -= shipOffset;

            var proxyParent = new MassiveBodyProxy(DVector2.Zero, DVector2.Zero, parent);
            var proxySatellite = new SpaceCraftProxy(initialPosition, satellite.Velocity - parent.Velocity, satellite);

            proxySatellite.SetGravitationalParent(proxyParent);

            int stepCount;

            double targetDt;
            double proximityDt;
            double orbitalDt = 0;

            bool isOrbiting;
            double orbitalTerminationRadius = 0;

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

                orbitalDt = GetOrbitalDt(initialPosition, proxySatellite.Velocity, out orbitalTerminationRadius);

                targetDt = orbitalDt;
            }

            var trace = new OrbitTrace(satellite.Position - shipOffset, altitude);

            // Simulate 300 orbital steps, more for proximity
            for (int step = 0; step < stepCount; step++)
            {
                proxySatellite.ResetAccelerations();

                proxySatellite.ResolveGravitation(proxyParent);
                proxySatellite.ResolveAtmopsherics(proxyParent);

                proxySatellite.Update(targetDt);

                altitude = proxyParent.GetRelativeHeight(proxySatellite.Position);

                // Check if reference frame shifting needs to occur in atmosphere
                if (altitude < proxyParent.AtmosphereHeight)
                {
                    double offsetFactor = 1.0 - (altitude / proxyParent.AtmosphereHeight);

                    DVector2 difference = proxyParent.Position - proxySatellite.Position;
                    difference.Normalize();

                    var surfaceNormal = new DVector2(-difference.Y, difference.X);

                    double altitudeFromCenter = altitude + proxyParent.SurfaceRadius;

                    // Distance of circumference at this altitude ( c= 2r * pi )
                    double pathCirumference = 2 * Math.PI * altitudeFromCenter;

                    double rotationalSpeed = pathCirumference / parent.RotationPeriod;

                    DVector2 atmopshereVelocity = surfaceNormal * rotationalSpeed;

                    proxySatellite.ApplyFrameOffset(atmopshereVelocity * offsetFactor * targetDt);

                    // Return early if the trace goes into a planet
                    if (altitude <= 0)
                    {
                        trace.AddPoint(proxySatellite.Position + parent.Position, altitude);
                        break;
                    }
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
                        orbitalDt = GetOrbitalDt(proxySatellite.Position, proxySatellite.Velocity, out orbitalTerminationRadius);

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

                // Check expensive termination conditions after half of the iterations
                if (isOrbiting && step > 100)
                {
                    DVector2 offsetVector = proxySatellite.Position - initialPosition;

                    double distanceFromStart = offsetVector.Length();

                    // Terminate and add the end point
                    if (distanceFromStart < orbitalTerminationRadius)
                    {
                        trace.AddPoint(proxySatellite.Position + parent.Position, altitude);
                        break;
                    }
                }

                trace.AddPoint(proxySatellite.Position + parent.Position, altitude);
            }

            return trace;
        }

        // Finds the orbtial delta time step by assuming 200 points along the oribtal cirumference
        private static double GetOrbitalDt(DVector2 positon, DVector2 velocity, out double terminationRadius)
        {
            double orbitalAltitude = positon.Length();

            double approximateOrbitDiameter = orbitalAltitude * 2 * Math.PI;

            // Terminate the trace if it comes within 1/100 of starting point
            terminationRadius = approximateOrbitDiameter * 0.01;

            double approximateOrbitPeriod = approximateOrbitDiameter / velocity.Length();

            return approximateOrbitPeriod * 0.005;
        }

        private static double GetProximityDt(double altitude, double proxityAltitude)
        {
            double altitudeRatio = altitude / proxityAltitude;

            return Math.Max(altitudeRatio * 15, 1);
        }
    }
}
