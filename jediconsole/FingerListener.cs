using Leap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jediconsole
{
    public class FingerListener
    {
        private long time;
        private long lastTime;
        private long aggregate;
        private Stopwatch stopwatch;
        private GestureService gestureService;

        public FingerListener()
        {
            stopwatch = new Stopwatch();
            gestureService = new GestureService();
        }

        public void OnInit(Controller controller)
        {
            Console.WriteLine("Initialized");
        }

        public void OnConnect(object sender, DeviceEventArgs args)
        {
            Console.WriteLine("Connected");
        }

        public void OnDisconnect(object sender, DeviceEventArgs args)
        {
            Console.WriteLine("Disconnected");
        }

        public void OnFrame(object sender, FrameEventArgs args)
        {
            if (!stopwatch.IsRunning)
            {
                stopwatch.Start();
            }
            time = stopwatch.ElapsedMilliseconds;
            long dt = time - lastTime;
            lastTime = stopwatch.ElapsedMilliseconds;
            aggregate += dt;
            if(aggregate > 1000)
            {
                aggregate -= 1000;
                Frame frame = args.frame;
                CheckFingers(frame);
            }
        }

        private void CheckFingers(Frame frame)
        {
            if (frame.Hands.Count != 0)
            {
                Hand hand = frame.Hands.First();
                bool pointsIndex = PointsIndex(hand);
                if (pointsIndex)
                {
                    Console.WriteLine("INDEX POINT DETECTED!");
                }
                else Console.WriteLine("Hands detected!");
            } else Console.WriteLine("No hands detected!");
        }

        private bool PointsIndex(Hand hand)
        {
            bool output = true;
            foreach(Finger finger in hand.Fingers)
            {
                if(finger.Type == Finger.FingerType.TYPE_INDEX)
                {
                    if (!finger.IsExtended) output = false;
                }
                else
                {
                    if (finger.IsExtended) output = false;
                }
            }
            return output;
        }

        public void OnServiceConnect(object sender, ConnectionEventArgs args)
        {
            Console.WriteLine("Service Connected");
        }

        public void OnServiceDisconnect(object sender, ConnectionLostEventArgs args)
        {
            Console.WriteLine("Service Disconnected");
        }

        public void OnServiceChange(Controller controller)
        {
            Console.WriteLine("Service Changed");
        }

        public void OnDeviceFailure(object sender, DeviceFailureEventArgs args)
        {
            Console.WriteLine("Device Error");
            Console.WriteLine("  PNP ID:" + args.DeviceSerialNumber);
            Console.WriteLine("  Failure message:" + args.ErrorMessage);
        }

        public void OnLogMessage(object sender, LogEventArgs args)
        {
            switch (args.severity)
            {
                case Leap.MessageSeverity.MESSAGE_CRITICAL:
                    Console.WriteLine("[Critical]");
                    break;
                case Leap.MessageSeverity.MESSAGE_WARNING:
                    Console.WriteLine("[Warning]");
                    break;
                case Leap.MessageSeverity.MESSAGE_INFORMATION:
                    Console.WriteLine("[Info]");
                    break;
                case Leap.MessageSeverity.MESSAGE_UNKNOWN:
                    Console.WriteLine("[Unknown]");
                    break;
            }
            Console.WriteLine("[{0}] {1}", args.timestamp, args.message);
        }
    }
}
