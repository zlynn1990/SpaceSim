using System;
using System.Collections.Generic;
using System.Drawing;
using SpaceSim.Spacecrafts;
using SpaceSim.Physics;

namespace SpaceSim.Drawing
{
    class EventMessage
    {
        public string Message { get; set; }

        public double StartTime { get; set; }

        public ISpaceCraft SpaceCraft { get; set; }
    }

    class EventRate
    {
        public int Delta { get; set; }

        public double StartTime { get; set; }

        public ISpaceCraft SpaceCraft { get; set; }
    }

    class EventTarget
    {
        public bool Next { get; set; }

        public double StartTime { get; set; }

        public ISpaceCraft SpaceCraft { get; set; }
    }

    class EventZoom
    {
        public float Scale { get; set; }

        public double StartTime { get; set; }

        public ISpaceCraft SpaceCraft { get; set; }
    }

    class EventManager
    {
        public double DisplayTime { get; private set; }

        public double FadeTime { get; private set; }

        private Point _position;
        private double _elapsedTime;
        private List<EventMessage> _eventMessages;
        private List<EventRate> _eventRates;
        private List<EventTarget> _eventTargets;
        private List<EventZoom> _eventZooms;

        public EventManager(Point position, double displayTime, double fadeTime)
        {
            _position = position;

            DisplayTime = displayTime;
            FadeTime = fadeTime;

            _eventMessages = new List<EventMessage>();
            _eventRates = new List<EventRate>();
            _eventTargets = new List<EventTarget>();
            _eventZooms = new List<EventZoom>();
        }

        public void AddMessage(string message, ISpaceCraft spaceCraft)
        {
            var eventMessage = new EventMessage
            {
                Message = message,
                StartTime = _elapsedTime,
                SpaceCraft = spaceCraft
            };

            // "Enqueue" new messages
            _eventMessages.Insert(0, eventMessage);
        }

        public void AddRate(int delta, ISpaceCraft spaceCraft)
        {
            var eventRate = new EventRate
            {
                Delta = delta,
                StartTime = _elapsedTime,
                SpaceCraft = spaceCraft
            };

            // "Enqueue" new messages
            _eventRates.Insert(0, eventRate);
        }

        public void AddTarget(bool next, ISpaceCraft spaceCraft)
        {
            var eventTarget = new EventTarget
            {
                Next = next,
                StartTime = _elapsedTime,
                SpaceCraft = spaceCraft
            };

            // "Enqueue" new messages
            _eventTargets.Insert(0, eventTarget);
        }

        public void AddZoom(float scale, ISpaceCraft spaceCraft)
        {
            var eventZoom = new EventZoom
            {
                Scale = scale * 0.001f,
                StartTime = _elapsedTime,
                SpaceCraft = spaceCraft
            };

            // "Enqueue" new messages
            _eventZooms.Insert(0, eventZoom);
        }

        public void Update(double dt)
        {
            // Remove old messages
            if (_eventMessages.Count > 0)
            {
                int lastIndex = _eventMessages.Count - 1;

                if (_eventMessages.Count > 5)
                {
                    _eventMessages.RemoveAt(lastIndex);
                }
                else if (_eventMessages[lastIndex].StartTime + DisplayTime < _elapsedTime)
                {
                    _eventMessages.RemoveAt(lastIndex);
                }
            }

            _elapsedTime += dt;
        }

        public void CheckForGlobalEvents(MainWindow main)
        {
            foreach (EventRate rate in _eventRates)
            {
                TimeStep step = main.SetRate(rate.Delta);
                if(rate.Delta > 0)
                    AddMessage(string.Format("Update Speed increased to {0}x", step.Multiplier), null);
                else
                    AddMessage(string.Format("Update Speed decreased to {0}x", step.Multiplier), null);
            }

            foreach (EventTarget target in _eventTargets)
            {
                main.SetTarget(target.Next);
            }

            foreach (EventZoom zoom in _eventZooms)
            {
                main.SetZoom(zoom.Scale);
            }

            _eventRates.Clear();
            _eventTargets.Clear();
            _eventZooms.Clear();
        }

        public void Render(Graphics graphics)
        {
            var font = new Font("Verdana Bold", 14);

            int messageY = _position.Y;

            // Display each message
            foreach (EventMessage message in _eventMessages)
            {
                string messageOutput = string.Format("{0} - {1}", message.SpaceCraft, message.Message);
                if (message.SpaceCraft == null)
                    messageOutput = message.Message;

                int alpha = 255;

                // Fade in messages when 1 exists
                if (_eventMessages.Count == 1)
                {
                    if (message.StartTime + FadeTime > _elapsedTime)
                    {
                        alpha = (int)(255.0 * (_elapsedTime - message.StartTime) / FadeTime);
                    }
                }

                // Fade out
                if (message.StartTime + DisplayTime - FadeTime < _elapsedTime)
                {
                    double fadeOut = Math.Min(_elapsedTime - (message.StartTime + DisplayTime - FadeTime), FadeTime);

                    alpha = (int)(255.0 -  255.0 * (fadeOut / FadeTime));
                }

                graphics.DrawString(messageOutput, font, new SolidBrush(Color.FromArgb(alpha, 255, 255, 255)), _position.X, messageY, new StringFormat {Alignment = StringAlignment.Center});

                messageY += 30;
            }
        }
    }
}
