using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using static Lolighter.Items.Enum;

namespace Lolighter.Items
{
    internal static class Utils
    {
        public static class EnvironmentEvent
        {
            public static List<int> LightEventType = new List<int>() { 0, 1, 2, 3, 4 };
            public static List<int> TrackRingEventType = new List<int>() { 8, 9 };
            public static List<int> LaserRotationEventType = new List<int>() { 12, 13 };
            public static List<int> AllEventType = new List<int>() { 0, 1, 2, 3, 4, 8, 9, 12, 13 };
        }

        public static int Swap(int temp) //Fade -> On, On -> Fade
        {
            switch (temp)
            {
                case EventLightValue.BlueFlashFade: return EventLightValue.BlueOn;
                case EventLightValue.RedFlashFade: return EventLightValue.RedOn;
                case EventLightValue.BlueOn: return EventLightValue.BlueFlashFade;
                case EventLightValue.RedOn: return EventLightValue.RedFlashFade;
                default: return 0;
            }
        }

        public static int Inverse(int temp) //Red -> Blue, Blue -> Red
        {
            if (temp > EventLightValue.BlueFlashFade)
                return temp - 4; //Turn to blue
            else
                return temp + 4; //Turn to red
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
