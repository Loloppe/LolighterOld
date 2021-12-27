using Lolighter.Items;
using System.Collections.Generic;
using System.Linq;
using EventLightValue = Lolighter.Items.Enum.EventLightValue;
using EventType = Lolighter.Items.Enum.EventType;

namespace Lolighter.Methods
{
    class DownLighter
    {
        static public List<_Events> Down(List<_Events> light, double speed, double spamSpeed, double onSpeed)
        {
            // Turns all long strobes into pulse (alternate between fade and on)
            // Remove fast off
            // Automatically set on Backlight during long period of nothing
            // Remove spin/zoom spam

            // Sort the list
            light.Sort((x, y) => x._time.CompareTo(y._time));

            // Sort each of them per type
            List<_Events> Back = new List<_Events>(light.Where(x => x._type == EventType.LightBackTopLasers));
            List<_Events> Neon = new List<_Events>(light.Where(x => x._type == EventType.LightTrackRingNeons));
            List<_Events> Side = new List<_Events>(light.Where(x => x._type == EventType.LightBottomBackSideLasers));
            List<_Events> Left = new List<_Events>(light.Where(x => x._type == EventType.LightLeftLasers));
            List<_Events> Right = new List<_Events>(light.Where(x => x._type == EventType.LightRightLasers));
            List<_Events> LeftSpeed = new List<_Events>(light.Where(x => x._type == EventType.RotatingLeftLasers));
            List<_Events> RightSpeed = new List<_Events>(light.Where(x => x._type == EventType.RotatingRightLasers));
            List<_Events> Spin = new List<_Events>(light.Where(x => x._type == EventType.RotationAllTrackRings));
            List<_Events> Zoom = new List<_Events>(light.Where(x => x._type == EventType.RotationSmallTrackRings));

            // Send them to the algorithm
            Back = Mod(Back, speed);
            Neon = Mod(Neon, speed);
            Side = Mod(Side, speed);
            Left = Mod(Left, speed);
            Right = Mod(Right, speed);

            // Spin/Zoom, we want to remove spam
            Spin = Spam(Spin, spamSpeed);
            Zoom = Spam(Zoom, spamSpeed);

            // Put back together the list
            light = new List<_Events>();
            light.AddRange(Back);
            light.AddRange(Neon);
            light.AddRange(Side);
            light.AddRange(Left);
            light.AddRange(Right);
            light.AddRange(LeftSpeed);
            light.AddRange(RightSpeed);

            // Turn On an Event if no light for a while.
            light = On(light, onSpeed);

            // Put back together the list
            light.AddRange(Spin);
            light.AddRange(Zoom);

            // Sort the list
            light.Sort((x, y) => x._time.CompareTo(y._time));

            return light;
        }

        static List<_Events> On(List<_Events> light, double onSpeed)
        {
            for (int i = light.Count() - 1; i > 0; i--)
            {
                _Events previous = light[i - 1];
                _Events now = light[i];

                // If no light for a long duration, we turn on something.
                if (now._time - previous._time >= onSpeed)
                {
                    if(previous._value < 4)
                    {
                        previous._value = EventLightValue.BlueOn;
                    }
                    else
                    {
                        previous._value = EventLightValue.RedOn;
                    }
                }
            }

            return light;
        }

        static List<_Events> Spam(List<_Events> light, double spamSpeed)
        {
            for (int i = light.Count() - 1; i > 0; i--)
            {
                _Events previous = light[i - 1];
                _Events now = light[i];

                // We remove spam under that speed
                if (now._time - previous._time <= spamSpeed)
                {
                    light.Remove(now);
                }
            }

            return light;
        }

        static List<_Events> Mod(List<_Events> light, double speed)
        {
            for (int i = light.Count() - 1; i > 0; i--)
            {
                _Events previous = light[i - 1];
                _Events now = light[i];

                // The light are pretty close
                if (now._time - previous._time <= speed)
                {
                    // One of them is an Off event
                    if (now._value == 4 || now._value == 0)
                    {
                        light.Remove(now);
                    }
                    else if (previous._value == 4 || previous._value == 0)
                    {
                        light.Remove(previous);
                    }
                }
            }

            // Now with fast stuff removed.
            for (int i = 1; i < light.Count(); i++)
            {
                _Events previous = light[i - 1];
                _Events now = light[i];

                // Swap light between Fade and On if they are close.
                if (now._time - previous._time <= speed && now._value == previous._value)
                {
                    if (now._value == EventLightValue.BlueFlashFade || now._value == EventLightValue.RedFlashFade || now._value == EventLightValue.BlueOn || now._value == EventLightValue.RedOn)
                    {
                        now._value = Swap(now._value);
                    }
                }
            }

            return light;
        }

        static int Inverse(int temp) //Red -> Blue, Blue -> Red
        {
            if (temp > EventLightValue.BlueFlashFade)
                return temp - 4; //Turn to blue
            else
                return temp + 4; //Turn to red
        }

        static int Swap(int temp)
        {
            if (temp == EventLightValue.BlueFlashFade)
                return EventLightValue.BlueOn;
            else if (temp == EventLightValue.RedFlashFade)
                return EventLightValue.RedOn;
            else if (temp == EventLightValue.BlueOn)
                return EventLightValue.BlueFlashFade;
            else if (temp == EventLightValue.RedOn)
                return EventLightValue.RedFlashFade;

            return 0;
        }
    }
}
