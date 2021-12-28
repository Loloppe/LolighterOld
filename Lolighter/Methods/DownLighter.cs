using Lolighter.Items;
using System.Collections.Generic;
using System.Linq;
using EventLightValue = Lolighter.Items.Enum.EventLightValue;
using Utils = Lolighter.Items.Utils;

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
            Dictionary<int, List<_Events>> mapEvents = new Dictionary<int, List<_Events>>(9);
            foreach (var type in Utils.EnvironmentEvent.AllEventType)
            {
                mapEvents.Add(type, new List<_Events>(light.Where(x => x._type == type)));
            }

            // Send them to the algorithm
            foreach (var type in Utils.EnvironmentEvent.LightEventType)
            {
                mapEvents[type] = Mod(mapEvents[type], speed);
            }

            // Spin/Zoom, we want to remove spam
            foreach (var type in Utils.EnvironmentEvent.TrackRingEventType)
            {
                mapEvents[type] = Spam(mapEvents[type], spamSpeed);
            }

            // Put back together the list
            light = new List<_Events>();
            foreach (var type in Utils.EnvironmentEvent.AllEventType)
            {
                light.AddRange(mapEvents[type]);
            }

            // Turn On an Event if no light for a while.
            light = On(light, onSpeed);

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
                        now._value = Utils.Swap(now._value);
                    }
                }
            }

            return light;
        }
    }
}
