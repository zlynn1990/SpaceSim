using System;
using System.Collections.Generic;
using System.Drawing;
using SpaceSim.Spacecrafts;

namespace SpaceSim.Drawing
{
    class EventMessage
    {
        public string Message { get; set; }

        public double StartTime { get; set; }

        public ISpaceCraft SpaceCraft { get; set; }
    }

    class EventManager
    {
        public double DisplayTime { get; }
        public double FadeTime { get; }

        private Point _position;
        private double _elapsedTime;
        private List<EventMessage> _eventMessages;

        public EventManager(Point position, double displayTime, double fadeTime)
        {
            _position = position;

            DisplayTime = displayTime;
            FadeTime = fadeTime;

            _eventMessages = new List<EventMessage>();
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
