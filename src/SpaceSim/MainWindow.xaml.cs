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
using SpaceSim.Drawing;
using SpaceSim.Gauges;
using SpaceSim.Orbits;
using SpaceSim.Physics;
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
        public static string ProfileDirectory;
        public static bool FullScreen;

        private RenderingType _renderingType = RenderingType.OpenCLHardware;

        private bool _isActive;
        private Thread _updateThread;

        private int _timeStepIndex;
        private List<TimeStep> _timeSteps;

        private OpenCLProxy _clProxy;
        private GpuClear _gpuClear;

        private Bitmap _imageBitmap;
        private WriteableBitmap _backBuffer;

        private float _scrollRate;
        private float _targetScrollRate;

        private Camera _camera;
        private int _targetIndex;

        private Sun _sun;
        private Strongback _strongback;

        private List<IGauge> _gauges;
        private ProgradeButton _progradeButton;
        private RetrogradeButton _retrogradeButton;
        private EventManager _eventManager;

        private List<ISpaceCraft> _spaceCrafts; 
        private List<IMassiveBody> _massiveBodies;
        private List<StructureBase> _structures; 
        private List<IGravitationalBody> _gravitationalBodies;

        private bool _isPaused;
        private double _totalElapsedSeconds;

        public MainWindow()
        {
            InitializeComponent();
            InitializeScreen();

            LoadGui();
            LoadSolarSystem();
            LoadKernels();

            _camera = new Camera(_gravitationalBodies[_targetIndex], 0.1);

            _timeStepIndex = 4;

            _timeSteps = new List<TimeStep>
            {
                new TimeStep {Multiplier = 0.01, UpdateLoops = 1, Dt = 0.000166666667},
                new TimeStep {Multiplier = 0.1, UpdateLoops = 1, Dt = 0.001666667},
                new TimeStep {Multiplier = 0.25, UpdateLoops = 2, Dt = 0.00208333333333333333333333333333},
                new TimeStep {Multiplier = 0.5, UpdateLoops = 2, Dt = 0.00416666666666666666666666666667},
                new TimeStep {Multiplier = 1, UpdateLoops = 4, Dt = 0.00416666666666666666666666666667},
                new TimeStep {Multiplier = 2, UpdateLoops = 8, Dt = 0.00416666666666666666666666666667},
                new TimeStep {Multiplier = 4, UpdateLoops = 16, Dt = 0.00416666666666666666666666666667},
                new TimeStep {Multiplier = 8, UpdateLoops = 32, Dt = 0.00416666666666666666666666666667},
                new TimeStep {Multiplier = 16, UpdateLoops = 64, Dt = 0.00416666666666666666666666666667},
                new TimeStep {Multiplier = 32, UpdateLoops = 128, Dt = 0.00416666666666666666666666666667},
                new TimeStep {Multiplier = 100, UpdateLoops = 256, Dt = 0.00416666666666666666666666666667},
                new TimeStep {Multiplier = 500, UpdateLoops = 100, Dt = 0.0833333333},
                new TimeStep {Multiplier = 1000, UpdateLoops = 200, Dt = 0.0833333333},
                new TimeStep {Multiplier = 2000, UpdateLoops = 200, Dt = 0.1666666666},
                new TimeStep {Multiplier = 5000, UpdateLoops = 500, Dt = 0.1666666666},
                new TimeStep {Multiplier = 10000, UpdateLoops = 1000, Dt = 0.1666666666},
                new TimeStep {Multiplier = 50000, UpdateLoops = 1000, Dt = 0.8333333333333},
                new TimeStep {Multiplier = 100000, UpdateLoops = 2000, Dt = 0.8333333333333},
                new TimeStep {Multiplier = 1000000, UpdateLoops = 2000, Dt = 8.333333333333},
                new TimeStep {Multiplier = 5000000, UpdateLoops = 2000, Dt = 41.66666666666},
                new TimeStep {Multiplier = 20000000, UpdateLoops = 2000, Dt = 166.66666666664},
            };

            _isPaused = true;
            _isActive = true;
            _updateThread = new Thread(GameLoop);
            _updateThread.Start();
        }

        private void InitializeScreen()
        {
            Mouse.OverrideCursor = Cursors.None;

            if (FullScreen)
            {
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;

                //RenderUtils.ScreenWidth = 1688;
                //RenderUtils.ScreenHeight = 950;

                RenderUtils.ScreenWidth = (int)SystemParameters.PrimaryScreenWidth;
                RenderUtils.ScreenHeight = (int)SystemParameters.PrimaryScreenHeight;
            }
            else
            {
                RenderUtils.ScreenWidth = (int)SystemParameters.PrimaryScreenWidth - 200;
                RenderUtils.ScreenHeight = (int)SystemParameters.PrimaryScreenHeight - 100;
            }

            RenderUtils.ScreenArea = RenderUtils.ScreenWidth * RenderUtils.ScreenHeight;

            Application.Current.MainWindow.Width = RenderUtils.ScreenWidth;
            Application.Current.MainWindow.Height = RenderUtils.ScreenHeight;

            _imageBitmap = new Bitmap(RenderUtils.ScreenWidth, RenderUtils.ScreenHeight, PixelFormat.Format32bppArgb);
            _backBuffer = new WriteableBitmap(RenderUtils.ScreenWidth, RenderUtils.ScreenHeight, 96, 96, PixelFormats.Bgra32, null);

            BackBuffer.Source = _backBuffer;
        }

        private void LoadGui()
        {
            _eventManager = new EventManager(new Point(RenderUtils.ScreenWidth / 2, 50), 5, 0.25);

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
        }

        private void LoadSolarSystem()
        {
            _sun = new Sun(DVector2.Zero, DVector2.Zero);

            var mercury = new Mercury();
            var venus = new Venus();
            var earth = new Earth();
            var moon = new Moon(earth.Position, earth.Velocity);
            var mars = new Mars();
            var jupiter = new Jupiter();
            var europa = new Europa(jupiter.Position + new DVector2(6.64862e8, 0), jupiter.Velocity + new DVector2(0, -13740));
            var saturn = new Saturn();

            _massiveBodies = new List<IMassiveBody>
            {
                _sun, mercury, venus, earth, moon, mars, jupiter, europa, saturn
            };

            //_spaceCrafts = SpacecraftFactory.BuildFalconHeavy(earth, ProfileDirectory);
            _spaceCrafts = SpacecraftFactory.BuildF9SSTO(earth, ProfileDirectory);
            //_spaceCrafts = SpacecraftFactory.BuildF9(earth, ProfileDirectory);
            //_spaceCrafts = SpacecraftFactory.BuildF9Dragon(earth, ProfileDirectory);

            // Initialize the spacecraft controllers
            foreach (ISpaceCraft spaceCraft in _spaceCrafts)
            {
                spaceCraft.InitializeController(ProfileDirectory, _eventManager);
            }

            // Start at nearly -Math.Pi / 2
            _strongback = new Strongback(-1.5707947, _spaceCrafts[0].TotalHeight * 0.05, earth);

            // Start downrange at ~300km
            var asds = new ASDS(-1.62026, 20, earth);

            _gravitationalBodies = new List<IGravitationalBody>
            {
                _sun, mercury, venus, earth
            };

            foreach (ISpaceCraft spaceCraft in _spaceCrafts)
            {
                _gravitationalBodies.Add(spaceCraft);
            }

            _structures = new List<StructureBase>
            {
                _strongback, //asds
            };

            _gravitationalBodies.Add(moon);
            _gravitationalBodies.Add(mars);
            _gravitationalBodies.Add(jupiter);
            _gravitationalBodies.Add(europa);
            _gravitationalBodies.Add(saturn);

            // Target the spacecraft
            _targetIndex = _gravitationalBodies.IndexOf(_spaceCrafts.FirstOrDefault());
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

                if (Directory.Exists("Kernels"))
                {
                    KernelManager.GenerateKernels("Kernels");
                }
                else
                {
                    KernelManager.GenerateKernels("../../Kernels");
                }

                _clProxy.CreateIntArgument("resX", RenderUtils.ScreenWidth);
                _clProxy.CreateIntArgument("resY", RenderUtils.ScreenHeight);

                _clProxy.CreateDoubleArgument("cameraLeft", 0);
                _clProxy.CreateDoubleArgument("cameraTop", 0);

                _clProxy.CreateDoubleArgument("cameraWidth", 0);
                _clProxy.CreateDoubleArgument("cameraHeight", 0);

                _clProxy.CreateDoubleArgument("sunNormalX", 0);
                _clProxy.CreateDoubleArgument("sunNormalY", 0);

                _clProxy.CreateDoubleArgument("rotation", 0);

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

            if (e.Key == Key.OemComma && _timeStepIndex > 0)
            {
                _timeStepIndex--;
            }

            if (e.Key == Key.OemPeriod && _timeStepIndex < _timeSteps.Count - 1)
            {
                _timeStepIndex++;
            }

            if (e.Key == Key.OemCloseBrackets && !_isPaused)
            {
                _targetIndex++;

                if (_targetIndex == _gravitationalBodies.Count)
                {
                    _targetIndex = 0;
                }

                while (_gravitationalBodies[_targetIndex] is ISpaceCraft)
                {
                    var nextCraft = _gravitationalBodies[_targetIndex] as ISpaceCraft;

                    if (nextCraft != null && nextCraft.Parent == null) break;

                    _targetIndex++;
                }

                _camera.UpdateTarget(_gravitationalBodies[_targetIndex]);
            }

            if (e.Key == Key.OemOpenBrackets && !_isPaused)
            {
                _targetIndex--;

                if (_targetIndex < 0)
                {
                    _targetIndex = _gravitationalBodies.Count - 1;
                }

                while (_gravitationalBodies[_targetIndex] is ISpaceCraft)
                {
                    var nextCraft = _gravitationalBodies[_targetIndex] as ISpaceCraft;

                    if (nextCraft != null && nextCraft.Parent == null) break;

                    _targetIndex--;
                }

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

        private void ResolveGravitionalParents()
        {
            // Find the parent body for each massive body
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

            // Find the parent for each space craft
            foreach (ISpaceCraft spaceCraft in _spaceCrafts)
            {
                double bestMassDistanceRatio = double.MaxValue;

                foreach (IMassiveBody bodyB in _massiveBodies)
                {
                    double massRatio = Math.Pow(spaceCraft.Mass / bodyB.Mass, 0.45);

                    DVector2 difference = spaceCraft.Position - bodyB.Position;

                    double massDistanceRatio = massRatio * difference.Length();

                    // New parent
                    if (massDistanceRatio < bestMassDistanceRatio)
                    {
                        spaceCraft.SetGravitationalParent(bodyB);

                        bestMassDistanceRatio = massDistanceRatio;
                    }
                }
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
            ResolveGravitionalParents();

            double targetDt = (_isPaused) ? 0 : timeStep.Dt;

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

                // Reslove spacecraft forces
                foreach (SpaceCraftBase spaceCraft in _spaceCrafts)
                {
                    spaceCraft.ResetAccelerations();

                    foreach (MassiveBodyBase massiveBody in _massiveBodies)
                    {
                        spaceCraft.ResolveGravitation(massiveBody);

                        if (spaceCraft.GravitationalParent == massiveBody)
                        {
                            spaceCraft.ResolveAtmopsherics(massiveBody);
                        }
                    }
                }

                // Update spacecraft animations
                foreach (ISpaceCraft spaceCraft in _spaceCrafts)
                {
                    spaceCraft.UpdateAnimations(timeStep);
                }

                // Update oribitng bodies
                foreach (IGravitationalBody gravitationalBody in _gravitationalBodies)
                {
                    gravitationalBody.Update(targetDt);
                }

                _camera.Update(targetDt);
                _eventManager.Update(targetDt);

                _totalElapsedSeconds += targetDt;
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
                double scroll = Math.Pow(_camera.Zoom, 1.05f) * _scrollRate;

                _camera.ChangeZoom(scroll);
            }
            else
            {
                _camera.ChangeZoom(_scrollRate);
            }
        }

        /// <summary>
        /// Draws all the physics bodies and UI elements.
        /// </summary>
        private unsafe void DrawFrame(TimeStep timeStep, FpsManager frameTimer)
        {
            var font = new Font("Verdana Bold", 14);
            var brush = new SolidBrush(Color.White);

            RectangleD cameraBounds = _camera.GetBounds();

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
                        renderable.RenderCl(_clProxy, cameraBounds, _sun);
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

                    foreach (MassiveBodyBase renderable in _massiveBodies)
                    {
                        if (renderable.Visibility(cameraBounds) > 0)
                        {
                            renderable.RenderGdiFallback(graphics, cameraBounds, _sun);
                        }
                    }
                }
            }

            // Draw all orbit traces, spacecrafts, and GDI objects
            using (var graphics = Graphics.FromImage(_imageBitmap))
            {
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

                RenderUtils.DrawLine(graphics, cameraBounds, new DVector2(0, -10e12), new DVector2(0, 10e12), Color.FromArgb(40, 255, 255, 255));
                RenderUtils.DrawLine(graphics, cameraBounds, new DVector2(-10e12, 0), new DVector2(10e12, 0), Color.FromArgb(40, 255, 255, 255));

                double apogee = 0;
                double perigee = 0;

                // Draw orbit traces
                foreach (MassiveBodyBase massiveBody in _massiveBodies)
                {
                    if (massiveBody is Sun) continue;

                    OrbitTrace trace = OrbitHelper.TraceMassiveBody(massiveBody);

                    if (target == massiveBody)
                    {
                        apogee = trace.Apogee;
                        perigee = trace.Perigee;
                    }

                    trace.Draw(graphics, cameraBounds, massiveBody);
                }

                // Draw structures
                foreach (StructureBase structure in _structures)
                {
                    structure.RenderGdi(graphics, cameraBounds);
                }

                // Draw spacecraft
                foreach (SpaceCraftBase spaceCraft in _spaceCrafts)
                {
                    if (spaceCraft.Visibility(cameraBounds) > 0)
                    {
                        RectangleD bounds = spaceCraft.ComputeBoundingBox();

                        // In range for render
                        if (cameraBounds.IntersectsWith(bounds))
                        {
                            spaceCraft.RenderGdi(graphics, cameraBounds);
                        }
                    }

                    if (spaceCraft.Parent != null) continue;

                    OrbitTrace trace = OrbitHelper.TraceSpaceCraft(spaceCraft);

                    if (target == spaceCraft)
                    {
                        apogee = trace.Apogee;
                        perigee = trace.Perigee;
                    }

                    trace.Draw(graphics, cameraBounds, spaceCraft);
                }

                var elapsedTime = TimeSpan.FromSeconds(_totalElapsedSeconds);

                int elapsedYears = elapsedTime.Days / 365;
                int elapsedDays = elapsedTime.Days % 365;

                graphics.DrawString("Elapsed Time: " + string.Format("Y: {0} D: {1} H: {2} M: {3} S: {4}", elapsedYears, elapsedDays, elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds), font, brush, 5, 5);
                graphics.DrawString("Update Speed: " + timeStep.Multiplier + " X", font, brush, 5, 35);

                double altitude = target.GetRelativeAltitude();

                graphics.DrawString("Altitude: " + UnitDisplay.Distance(altitude), font, brush, 5, 90);

                graphics.DrawString(string.Format("Target: {0}", target), font, brush, RenderUtils.ScreenWidth / 2.0f, 5, new StringFormat {Alignment = StringAlignment.Center});

                double targetVelocity = target.GetRelativeVelocity().Length();

                graphics.DrawString("Relative Speed: " + UnitDisplay.Speed(targetVelocity, false), font, brush, 5, 175);
                graphics.DrawString("Relative Acceleration: " + UnitDisplay.Acceleration(target.GetRelativeAcceleration().Length()), font, brush, 5, 205);

                graphics.DrawString("Apogee: " + UnitDisplay.Distance(apogee), font, brush, 5, 345);
                graphics.DrawString("Perigee: " + UnitDisplay.Distance(perigee), font, brush, 5, 375);

                graphics.DrawString("Mass: " + UnitDisplay.Mass(target.Mass), font, brush, 5, 260);

                if (targetSpaceCraft != null)
                {
                    double downrangeDistance = targetSpaceCraft.GetDownrangeDistance(_strongback.Position);

                    graphics.DrawString("Downrange: " + UnitDisplay.Distance(downrangeDistance), font, brush, 5, 120);

                    graphics.DrawString("Thrust: " + UnitDisplay.Force(targetSpaceCraft.Thrust), font, brush, 5, 290);

                    double density = targetSpaceCraft.GravitationalParent.GetAtmosphericDensity(altitude);

                    graphics.DrawString("Air Density: " + UnitDisplay.Density(density), font, brush, 5, 430);

                    double dynamicPressure = 0.5 * density * targetVelocity * targetVelocity;

                    graphics.DrawString("Dynamic Pressure: " + UnitDisplay.Pressure(dynamicPressure), font, brush, 5, 460);
                }

                graphics.DrawString("FPS: " + frameTimer.CurrentFps, font, brush, RenderUtils.ScreenWidth - 80, 5);
            }

            // Draw all GUI elements (higher quality)
            using (var graphics = Graphics.FromImage(_imageBitmap))
            {
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                double throttle = 0;

                if (targetSpaceCraft != null)
                {
                    throttle = targetSpaceCraft.Throttle;
                }

                foreach (IGauge gauge in _gauges)
                {
                    if (targetSpaceCraft != null)
                    {
                        gauge.Update(_gravitationalBodies[_targetIndex].Rotation, throttle / 100.0);
                    }

                    gauge.Render(graphics, cameraBounds);
                }

                _eventManager.Render(graphics);
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
