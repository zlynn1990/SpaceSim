using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Cloo;
using OpenCLWrapper;
using SpaceSim.Contracts;
using SpaceSim.Controllers;
using SpaceSim.Drawing;
using SpaceSim.Gauges;
using SpaceSim.Orbits;
using SpaceSim.Physics;
using SpaceSim.Properties;
using SpaceSim.SolarSystem;
using SpaceSim.SolarSystem.Moons;
using SpaceSim.SolarSystem.Planets;
using SpaceSim.SolarSystem.Stars;
using SpaceSim.Spacecrafts;
using SpaceSim.Structures;
using VectorMath;
using Color = System.Drawing.Color;
using EventManager = SpaceSim.Drawing.EventManager;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace SpaceSim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<string> ProfilePaths;

        private RenderingType _renderingType = RenderingType.OpenCLHardware;

        private bool _isActive;
        private Thread _updateThread;

        private int _timeStepIndex;
        private List<TimeStep> _timeSteps;
        private bool _userUpdatedTimesteps;

        private OpenCLProxy _clProxy;
        private GpuClear _gpuClear;

        private Bitmap _imageBitmap;
        private WriteableBitmap _backBuffer;

        private float _scrollRate;
        private float _targetScrollRate;

        private Camera _camera;
        private int _targetIndex;
        private bool _targetInOrbit;

        private Sun _sun;

        private List<IGauge> _gauges;
        private ProgradeButton _progradeButton;
        private RetrogradeButton _retrogradeButton;
        private EventManager _eventManager;

        private SpaceCraftManager _spaceCraftManager;
        private List<IMassiveBody> _massiveBodies;
        private List<StructureBase> _structures; 
        private List<IGravitationalBody> _gravitationalBodies;

        private DateTime _originTime;

        private bool _isPaused;
        private double _clockDelay;
        private double _totalElapsedSeconds;

        private TextDisplay _textDisplay;

        int _bmIndex = Settings.Default.FirstBitmapIndex;
        DateTime now = DateTime.Now;

        public MainWindow()
        {
            InitializeComponent();
            InitializeScreen();

            LoadGui();
            LoadSolarSystem();
            LoadKernels();

            _camera = new Camera(_gravitationalBodies[_targetIndex], 0.3);
            //_camera = new Camera(_gravitationalBodies[_targetIndex], 5000);

            _timeStepIndex = TimeStep.RealTimeIndex;
            _timeSteps = TimeStep.Defaults();

            _isPaused = true;
            _isActive = true;
            _updateThread = new Thread(GameLoop);
            _updateThread.Start();
        }

        private void InitializeScreen()
        {
            Mouse.OverrideCursor = Cursors.None;

            if (Settings.Default.FullScreen)
            {
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;

                RenderUtils.ScreenWidth = (int)SystemParameters.PrimaryScreenWidth;
                RenderUtils.ScreenHeight = (int)SystemParameters.PrimaryScreenHeight;
            }
            else
            {
                RenderUtils.ScreenWidth = Settings.Default.ScreenSize.Width;
                RenderUtils.ScreenHeight = Settings.Default.ScreenSize.Height;
            }

            RenderUtils.ScreenArea = RenderUtils.ScreenWidth * RenderUtils.ScreenHeight;

            Application.Current.MainWindow.Width = RenderUtils.ScreenWidth;
            Application.Current.MainWindow.Height = RenderUtils.ScreenHeight;

            _imageBitmap = new Bitmap(RenderUtils.ScreenWidth, RenderUtils.ScreenHeight, PixelFormat.Format32bppArgb);
            _backBuffer = new WriteableBitmap(RenderUtils.ScreenWidth, RenderUtils.ScreenHeight, 96, 96, PixelFormats.Bgra32, null);

            BackBuffer.Source = _backBuffer;
        }

        private IMassiveBody LocatePlanet(string planetName)
        {
            foreach (IMassiveBody body in _massiveBodies)
            {
                if (body.ToString().Equals(planetName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return body;
                }
            }

            throw new Exception("Could not find planet: " + planetName);
        }

        private void LoadGui()
        {
            _eventManager = new EventManager(new Point(RenderUtils.ScreenWidth / 2, 60), 5, 0.25);

            _progradeButton = new ProgradeButton(new Point(RenderUtils.ScreenWidth - 160, RenderUtils.ScreenHeight - 105));
            _retrogradeButton = new RetrogradeButton(new Point(RenderUtils.ScreenWidth - 160, RenderUtils.ScreenHeight - 45));

            _gauges = new List<IGauge>
            {
                new NavBall(new Point(RenderUtils.ScreenWidth - 75, RenderUtils.ScreenHeight - 75)),
                _progradeButton,
                _retrogradeButton,
                new ThrustGauge(new Point(RenderUtils.ScreenWidth - 195, RenderUtils.ScreenHeight - 75)),
                new Scale(new Point(75, RenderUtils.ScreenHeight - 25))
            };

            _textDisplay = new TextDisplay();
        }

        private void LoadSolarSystem()
        {
            _sun = new Sun();

            var mercury = new Mercury();
            var venus = new Venus();
            var earth = new Earth();
            var moon = new Moon(earth.Position, earth.Velocity);
            var mars = new Mars();
            var jupiter = new Jupiter();
            var europa = new Europa(jupiter.Position, jupiter.Velocity);
            var saturn = new Saturn();

            _massiveBodies = new List<IMassiveBody>
            {
                _sun, mercury, venus, earth, moon, mars, jupiter, europa, saturn
            };

            ResolveMassiveBodyParents();

            _gravitationalBodies = new List<IGravitationalBody>
            {
                _sun, mercury, venus, earth, moon, mars, jupiter, europa, saturn
            };

            _spaceCraftManager = new SpaceCraftManager(_gravitationalBodies);
            _structures = new List<StructureBase>();

            MissionConfig primaryMission = MissionConfig.Load(ProfilePaths[0]);

            _originTime = primaryMission.GetLaunchDate();

            OrbitHelper.SimulateToTime(_massiveBodies, _originTime, 300);

            // Load missions
            for (int i = 0; i < ProfilePaths.Count; i++)
            {
                MissionConfig missionConfig = MissionConfig.Load(ProfilePaths[i]);

                if (missionConfig.ClockDelay > _clockDelay)
                {
                    _clockDelay = missionConfig.ClockDelay;
                }

                IMassiveBody targetPlanet = LocatePlanet(missionConfig.ParentPlanet);

                // Get the launch angle relative to the sun for the given time at the origin
                // and offset each vehicle by a certain amount on the surface
                double launchAngle = targetPlanet.GetSurfaceAngle(_originTime, _sun) + i * 0.00002;

                _spaceCraftManager.Add(SpacecraftFactory.BuildSpaceCraft(targetPlanet, launchAngle, missionConfig, ProfilePaths[i]));

                _structures.AddRange(StructureFactory.Load(targetPlanet, launchAngle, ProfilePaths[i]));
            }

            _spaceCraftManager.Initialize(_eventManager, _clockDelay);

            // Target the spacecraft
            _targetIndex = _gravitationalBodies.IndexOf(_spaceCraftManager.First);

            _spaceCraftManager.ResolveGravitionalParents(_massiveBodies);
        }

        /// <summary>
        /// Load GPU kernels for planet rendering
        /// </summary>
        private void LoadKernels()
        {
            try
            {
                bool useSoftware = (_renderingType == RenderingType.OpenCLSoftware);

                _gpuClear = new GpuClear();

                _clProxy = new OpenCLProxy(useSoftware);

                KernelManager.GenerateKernels(Directory.Exists("Kernels") ? "Kernels" : "../../Kernels");

                _clProxy.CreateIntArgument("resX", RenderUtils.ScreenWidth);
                _clProxy.CreateIntArgument("resY", RenderUtils.ScreenHeight);

                _clProxy.CreateDoubleArgument("cX", 0);
                _clProxy.CreateDoubleArgument("cY", 0);

                _clProxy.CreateDoubleArgument("cWidth", 0);
                _clProxy.CreateDoubleArgument("cHeight", 0);

                _clProxy.CreateDoubleArgument("cRot", 0);

                _clProxy.CreateDoubleArgument("sunNormalX", 0);
                _clProxy.CreateDoubleArgument("sunNormalY", 0);

                _clProxy.CreateDoubleArgument("bodyX", 0);
                _clProxy.CreateDoubleArgument("bodyY", 0);
                _clProxy.CreateDoubleArgument("bodyRot", 0);

                _clProxy.CreateIntBuffer("image", new int[RenderUtils.ScreenArea], ComputeMemoryFlags.UseHostPointer);

                _gpuClear.Load(_clProxy);

                foreach (IGpuRenderable renderable in _massiveBodies)
                {
                    renderable.Load(_clProxy);
                }
            }
            catch (Exception)
            {
                _renderingType = RenderingType.GDIPlus;
            }
        }

        private void OnScroll(object sender, MouseWheelEventArgs e)
        {
            _targetScrollRate -= e.Delta * 0.0001f;
        }

        public void SetRate(int index)
        {
            _timeStepIndex = index;
            _userUpdatedTimesteps = true;
        }

        public void SetTarget(bool next)
        {
            if (next)
                _targetIndex = GravitationalBodyIterator.Next(_targetIndex, _gravitationalBodies, _camera);
            else
                _targetIndex = GravitationalBodyIterator.Prev(_targetIndex, _gravitationalBodies, _camera);

            _camera.UpdateTarget(_gravitationalBodies[_targetIndex]);           
        }

        public void SetZoom(float delta)
        {
            _targetScrollRate += delta;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _isPaused = !_isPaused;
            }

            if (e.Key == Key.Escape)
            {
                Close();
            }

            var spaceCraft = _gravitationalBodies[_targetIndex] as ISpaceCraft;

            if (spaceCraft != null)
            {
                spaceCraft.Controller.KeyUp(e.Key);
            }

            if (e.Key == Key.X)
            {
                _spaceCraftManager.ToggleDisplayVectors();
            }

            if (e.Key == Key.OemComma && _timeStepIndex > 0)
            {
                _timeStepIndex--;
                _userUpdatedTimesteps = true;
            }

            if (e.Key == Key.OemPeriod && _timeStepIndex < _timeSteps.Count - 1)
            {
                _timeStepIndex++;
                _userUpdatedTimesteps = true;
            }

            if (e.Key == Key.OemCloseBrackets && !_isPaused)
            {
                _targetIndex = GravitationalBodyIterator.Next(_targetIndex, _gravitationalBodies, _camera);

                _camera.UpdateTarget(_gravitationalBodies[_targetIndex]);
            }

            if (e.Key == Key.OemOpenBrackets && !_isPaused)
            {
                _targetIndex = GravitationalBodyIterator.Prev(_targetIndex, _gravitationalBodies, _camera);

                _camera.UpdateTarget(_gravitationalBodies[_targetIndex]);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            var spaceCraft = _gravitationalBodies[_targetIndex] as ISpaceCraft;

            if (spaceCraft != null)
            {
                spaceCraft.Controller.KeyDown(e.Key);
            }
        }

        // Find the parent body for each massive body
        private void ResolveMassiveBodyParents()
        {
            foreach (IMassiveBody bodyA in _massiveBodies)
            {
                double bestMassDistanceRatio = double.MaxValue;

                foreach (IMassiveBody bodyB in _massiveBodies)
                {
                    if (bodyA == bodyB) continue;

                    double massRatio = Math.Pow(bodyA.Mass / bodyB.Mass, 0.45);

                    DVector2 difference = bodyA.Position - bodyB.Position;

                    double massDistanceRatio = massRatio * difference.Length();

                    // New parent
                    if (massDistanceRatio < bestMassDistanceRatio)
                    {
                        bodyA.SetGravitationalParent(bodyB);

                        bestMassDistanceRatio = massDistanceRatio;
                    }
                }
            }
        }

        // Adjusts the timestep to ensure burns aren't missed at normal speed
        private void AdjustSpeedForBurns(TimeStep timeStep)
        {
            // Nothing to do already realtime
            if (_timeStepIndex <= TimeStep.RealTimeIndex) return;

            // Don't override the user for steps < real time
            if (_timeStepIndex <= TimeStep.MaxRealTimeIndex && _userUpdatedTimesteps) return;

            // Future timestep accounting for 5 iterations + padding to be safe
            double stepEnd = timeStep.Dt * timeStep.UpdateLoops * 5 + _totalElapsedSeconds - _clockDelay + 2;

            double nextBurn = _spaceCraftManager.GetNextBurnTime();

            if (nextBurn > 0 && nextBurn < stepEnd)
            {
                _timeStepIndex--;
                _userUpdatedTimesteps = false;
            }
        }

        private void SetCameraRotation()
        {
            IGravitationalBody target = _gravitationalBodies[_targetIndex];

            if (target is ISpaceCraft)
            {
                if (target.InOrbit)
                {
                    if (!_targetInOrbit)
                    {
                        _camera.SetRotation(0, true);

                        _targetInOrbit = true;
                    }
                    else
                    {
                        _camera.SetRotation(0);
                    }
                }
                else
                {
                    DVector2 craftOffset = target.GravitationalParent.Position - target.Position;
                    craftOffset.Normalize();

                    if (_targetInOrbit)
                    {
                        _camera.SetRotation(Constants.PiOverTwo - craftOffset.Angle(), true);

                        _targetInOrbit = false;
                    }
                    else
                    {
                        _camera.SetRotation(Constants.PiOverTwo - craftOffset.Angle());
                    }
                }
            }
            else
            {
                _camera.SetRotation(0);
            }
        }

        /// <summary>
        /// Main game loop - update - draw - sleep
        /// </summary>
        private void GameLoop()
        {
            var frameTimer = new FpsManager(60);

            while (_isActive)
            {
                frameTimer.StartFrame();

                TimeStep timeStep = _timeSteps[_timeStepIndex];

                Update(timeStep);
                DrawFrame(timeStep, frameTimer);
                WriteFrameToScreen();

                frameTimer.FinishFrame();
            }
        }

        /// <summary>
        /// Updates the physics bodies and resolves all forces.
        /// </summary>
        private void Update(TimeStep timeStep)
        {
            ResolveMassiveBodyParents();
            _spaceCraftManager.ResolveGravitionalParents(_massiveBodies);
            AdjustSpeedForBurns(timeStep);

            double targetDt = _isPaused ? 0 : timeStep.Dt;

            // Update all bodies according to the timestep
            for (int i = 0; i < timeStep.UpdateLoops; i++)
            {
                // Resolve n body massive body forces
                foreach (IMassiveBody bodyA in _massiveBodies)
                {
                    bodyA.ResetAccelerations();

                    foreach (IMassiveBody bodyB in _massiveBodies)
                    {
                        if (bodyA == bodyB) continue;

                        bodyA.ResolveGravitation(bodyB);
                    }
                }

                _spaceCraftManager.ResolveForces(_massiveBodies);
                _spaceCraftManager.Update(timeStep, targetDt);

                // Update bodies
                foreach (IGravitationalBody gravitationalBody in _gravitationalBodies)
                {
                    gravitationalBody.Update(targetDt);
                }

                _totalElapsedSeconds += targetDt;
            }

            _camera.Update(TimeStep.RealTimeDt);
            _eventManager.Update(TimeStep.RealTimeDt);

            // Fixed update all gravitational bodies
            foreach (IGravitationalBody body in _gravitationalBodies)
            {
                body.FixedUpdate(timeStep);
            }

            var targetSpaceCraft = _gravitationalBodies[_targetIndex] as ISpaceCraft;

            if (targetSpaceCraft != null)
            {
                if (targetSpaceCraft.Controller.IsPrograde)
                {
                    _progradeButton.Enable();
                }
                else
                {
                    _progradeButton.Disable();
                }

                if (targetSpaceCraft.Controller.IsRetrograde)
                {
                    _retrogradeButton.Enable();
                }
                else
                {
                    _retrogradeButton.Disable();
                }
            }

            _scrollRate = MathHelper.Lerp(_scrollRate, _targetScrollRate, 0.1f);

            _targetScrollRate = MathHelper.Lerp(_targetScrollRate, 0, 0.1f);

            if (_camera.Zoom > 1)
            {
                double scroll = Math.Pow(_camera.Zoom, 1.05f)*_scrollRate;

                _camera.ChangeZoom(scroll);
            }
            else
            {
                _camera.ChangeZoom(_scrollRate);
            }

            SetCameraRotation();
        }

        /// <summary>
        /// Draws all the physics bodies and UI elements.
        /// </summary>
        private unsafe void DrawFrame(TimeStep timeStep, FpsManager frameTimer)
        {
            _textDisplay.Clear();

            // check for global events
            _eventManager.CheckForGlobalEvents(this);

            RectangleD cameraBounds = _camera.Bounds;

            IGravitationalBody target = _gravitationalBodies[_targetIndex];
            var targetSpaceCraft = target as SpaceCraftBase;

            // If openCL is supported render all cl bodies
            if (_renderingType == RenderingType.OpenCLHardware ||
                _renderingType == RenderingType.OpenCLSoftware)
            {
                _gpuClear.RenderCl(_clProxy);

                foreach (MassiveBodyBase renderable in _massiveBodies)
                {
                    if (renderable.Visibility(cameraBounds) > 0)
                    {
                        renderable.RenderCl(_clProxy, _camera, _sun);
                    }
                }

                int[] frameData = _clProxy.ReadIntBuffer("image", RenderUtils.ScreenArea);

                var rect = new Rectangle(0, 0, _imageBitmap.Width, _imageBitmap.Height);

                BitmapData bmpData = _imageBitmap.LockBits(rect, ImageLockMode.WriteOnly,
                                                            PixelFormat.Format32bppArgb);

                Marshal.Copy(frameData, 0, bmpData.Scan0, RenderUtils.ScreenArea);

                var ptr = (byte*)bmpData.Scan0;

                // Hack to force full alpha for now
                for (int i = 0; i < RenderUtils.ScreenArea; i++)
                {
                    ptr[i * 4 + 3] = 255;
                }

                _imageBitmap.UnlockBits(bmpData);
            }
            else
            {
                // Fall back to gdi for cl renderables
                using (var graphics = Graphics.FromImage(_imageBitmap))
                {
                    graphics.Clear(Color.Black);

                    _camera.ApplyScreenRotation(graphics);

                    foreach (MassiveBodyBase renderable in _massiveBodies)
                    {
                        if (renderable.Visibility(cameraBounds) > 0)
                        {
                            renderable.RenderGdiFallback(graphics, _camera, _sun);
                        }
                    }
                }
            }

            // Draw all orbit traces, spacecrafts, and GDI objects
            using (Graphics graphics = RenderUtils.GetContext(false, _imageBitmap))
            {
                _camera.ApplyScreenRotation(graphics);

                //RenderUtils.DrawLine(graphics, cameraBounds, new DVector2(0, -10e12), new DVector2(0, 10e12), Color.FromArgb(40, 255, 255, 255));
                //RenderUtils.DrawLine(graphics, cameraBounds, new DVector2(-10e12, 0), new DVector2(10e12, 0), Color.FromArgb(40, 255, 255, 255));

                // Draw all massive body orbit traces
                foreach (MassiveBodyBase massiveBody in _massiveBodies)
                {
                    if (massiveBody is Sun) continue;

                    massiveBody.RenderGdi(graphics, _camera);
                }

                graphics.ResetTransform();

                // Draw structures
                foreach (StructureBase structure in _structures)
                {
                    structure.RenderGdi(graphics, _camera);
                }

                _spaceCraftManager.Render(graphics, _camera);
            }

            // Draw all GUI elements (higher quality)
            using (Graphics graphics = RenderUtils.GetContext(true, _imageBitmap))
            {
                double throttle = 0;

                if (targetSpaceCraft != null)
                {
                    throttle = targetSpaceCraft.Throttle;
                }

                foreach (IGauge gauge in _gauges)
                {
                    if (targetSpaceCraft != null)
                    {
                        gauge.Update(_gravitationalBodies[_targetIndex].GetRelativePitch(), throttle / 100.0);
                    }

                    gauge.Render(graphics, cameraBounds);
                }

                _eventManager.Render(graphics);

                var elapsedTime = TimeSpan.FromSeconds(_totalElapsedSeconds - _clockDelay);

                int elapsedYears = elapsedTime.Days / 365;
                int elapsedDays = elapsedTime.Days % 365;

                DateTime localTime = _originTime + elapsedTime;

                // Main timing display
                _textDisplay.AddTextBlock(StringAlignment.Near, new List<string>
                {
                    $"Origin Time: {localTime.ToShortDateString()} {localTime.ToShortTimeString()}",
                    //$"Elapsed Time: Y:{elapsedYears} D:{elapsedDays} H:{elapsedTime.Hours} M:{elapsedTime.Minutes} S:{Math.Round(elapsedTime.TotalSeconds % 60)}",
                    $"Elapsed Time: Y:{elapsedYears} D:{elapsedDays} H:{elapsedTime.Hours} M:{elapsedTime.Minutes} S:{elapsedTime.Seconds}",
                    $"Update Speed: {timeStep.Multiplier}"
                });

                // Target display
                _textDisplay.AddTextBlock(StringAlignment.Center, string.Format("Target: {0}", target));

                // FPS
                _textDisplay.AddTextBlock(StringAlignment.Far, "FPS: " + frameTimer.CurrentFps);

                double targetVelocity = target.GetRelativeVelocity().Length();

                // Info for altitude
                var altitudeInfo = new List<string> { "Altitude: " + UnitDisplay.Distance(target.GetRelativeAltitude()) };

                // Add downrange if spacecraft exists
                if (targetSpaceCraft != null)
                {
                    double downrangeDistance = targetSpaceCraft.GetDownrangeDistance(_structures[0].Position);

                    altitudeInfo.Add("Downrange: " + UnitDisplay.Distance(downrangeDistance));
                }

                _textDisplay.AddTextBlock(StringAlignment.Near, altitudeInfo);

                // Info for speed / acceleration
                var movementInfo = new List<string>
                {
                    "Relative Speed: " + UnitDisplay.Speed(targetVelocity, false),
                    "Relative Acceleration: " + UnitDisplay.Acceleration(target.GetRelativeAcceleration().Length()),
                };

                // Add angle of attack if it exists
                if (targetSpaceCraft != null)
                {
                    movementInfo.Add("Angle of Attack: " + UnitDisplay.Degrees(targetSpaceCraft.GetAlpha()));
                }

                _textDisplay.AddTextBlock(StringAlignment.Near, movementInfo);

                var forceInfo = new List<string> { "Mass: " + UnitDisplay.Mass(target.Mass) };

                // Add additional forces
                if (targetSpaceCraft != null)
                {
                    DVector2 dragForce = targetSpaceCraft.AccelerationD * targetSpaceCraft.Mass;
                    DVector2 liftForce = targetSpaceCraft.AccelerationL * targetSpaceCraft.Mass;
                    if (Settings.Default.UseTheTurnForce)
                        liftForce *= Math.Cos(targetSpaceCraft.Roll);

                    forceInfo.Add("Thrust: " + UnitDisplay.Force(targetSpaceCraft.Thrust));
                    forceInfo.Add("Drag: " + UnitDisplay.Force(dragForce.Length()));
                    forceInfo.Add("Lift: " + UnitDisplay.Force(liftForce.Length()));
                }

                _textDisplay.AddTextBlock(StringAlignment.Near, forceInfo);

                // Don't show Apoapsis/Periapsis info for the sun
                if (!(target is Sun))
                {
                    _textDisplay.AddTextBlock(StringAlignment.Near, new List<string>
                    {
                        $"{target.GravitationalParent.ApoapsisName}: {UnitDisplay.Distance(target.Apoapsis)}",
                        $"{target.GravitationalParent.PeriapsisName}: {UnitDisplay.Distance(target.Periapsis)}",
                    });
                }

                // Add atmospheric info if the spaceship is the target
                if (targetSpaceCraft != null)
                {
                    double density = targetSpaceCraft.GravitationalParent.GetAtmosphericDensity(target.GetRelativeAltitude());
                    double dynamicPressure = 0.5 * density * targetVelocity * targetVelocity;

                    _textDisplay.AddTextBlock(StringAlignment.Near, new List<string>
                    {
                        "Air Density: " + UnitDisplay.Density(density),
                        "Dynamic Pressure: " + UnitDisplay.Pressure(dynamicPressure),
                        "Heating Rate: " + UnitDisplay.Heat(targetSpaceCraft.HeatingRate)
                    });
                }

                _textDisplay.Draw(graphics);
            }
        }

        private void WriteFrameToScreen()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                var rect = new Rectangle(0, 0, _imageBitmap.Width, _imageBitmap.Height);
                BitmapData bmpData = _imageBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                var source = new Int32Rect(0, 0, RenderUtils.ScreenWidth, RenderUtils.ScreenHeight);

                _backBuffer.WritePixels(source, bmpData.Scan0, RenderUtils.ScreenArea * 4, RenderUtils.ScreenWidth * 4);

                if (Settings.Default.WriteBitmaps)
                {
                    TimeSpan elapsed = DateTime.Now - now;
                    if (elapsed.Milliseconds >= 40)
                    {
                        now = DateTime.Now;
                        if (!Directory.Exists(".\\Bitmaps"))
                            Directory.CreateDirectory(".\\Bitmaps");
                        string bitmap = Path.Combine(".\\Bitmaps", string.Format("{0:D6}.png", _bmIndex++));
                        _imageBitmap.Save(bitmap, ImageFormat.Png);
                    }
                }

                _imageBitmap.UnlockBits(bmpData);


            }), DispatcherPriority.Render, null);
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            _isActive = false;
            _updateThread.Join(1000);
        }
    }
}
