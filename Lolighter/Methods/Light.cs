using Lolighter.Items;
using System;
using System.Collections.Generic;
using static Lolighter.Items.Enum;

namespace Lolighter.Methods
{
    static class Light
    {
        static public List<_Events> CreateLight(List<_Notes> noteTempo, double ColorOffset, double ColorSwap, bool AllowBackStrobe, bool AllowNeonStrobe, bool AllowSideStrobe, bool AllowFade, bool AllowSpinZoom, int SlowMinSpinSpeed, int SlowMaxSpinSpeed, int FastMinSpinSpeed, int FastMaxSpinSpeed)
        {
            //This massive mess is based on my old AutoLighter that I created in Osu2Saber.
            //It work pretty well and is pretty simple to modify, but could definitely be better.
            //This could use a cleanup.

            Random rnd = new Random();
            int lastSpeed = 0;
            double last = new double(); //Var to stop spin-stack and also used as time check.
            double[] time = new double[4]; //Now, before, before-before, before-before-before, in this order.
            //0.0D = Default value for double, similar to NULL for int.
            int[] light = new int[3]; //Now, before, before-before.
            int lastLight = 0;
            double offset;
            double firstNote = 0;
            double timer = 0; //Timer start on the first note.
            int count; //Light counter, stop at maximum.
            int maximum = 2; //Maximum number of light per same time.
            int color; //Set color start value.
            int lastCut = 0;
            _Events ev;
            List<_Events> eventTempo = new List<_Events>();

            void ResetTimer() //Pretty much reset everything necessary.
            {
                if (AllowFade)
                {
                    color = EventLightValue.BlueFlashFade; //Blue Fade
                }
                else
                {
                    color = EventLightValue.BlueOn; //Blue On
                }
                foreach (_Notes note in noteTempo) //For each note
                {
                    if (note._type == NoteType.Red || note._type == NoteType.Blue) //Find the first note to know where to start the timer
                    {
                        firstNote = note._time;
                        break;
                    }
                }
                offset = firstNote;
                count = 0;
                for (int i = 0; i < 2; i++)
                {
                    time[i] = 0.0D;
                    light[i] = 0;
                }
                time[2] = 0.0D;
                time[3] = 0.0D;
            }

            int Inverse(int temp) //Red -> Blue, Blue -> Red
            {
                if (temp > EventLightValue.BlueFlashFade)
                    return temp - 4; //Turn to blue
                else
                    return temp + 4; //Turn to red
            }

            void TimerDuration() //Check the checkpoint
            {
                timer = time[0];
                if (timer >= ColorOffset + ColorSwap + offset) //If the timer is above offset + ColorOffset + ColorSwap (From the interface), then it's time to change color.
                {
                    int swapTime = (int)((time[0] - time[1]) / ColorSwap) + 1; //We get the number of "beat" since the last time it entered here this way.
                    for (int i = 0; i < swapTime; i++) //For each time that it need to swap. (Dumb fix for a dumb method)
                    {
                        color = Inverse(color); //Swap color
                        offset += ColorSwap; //Offset incremented
                    }
                }
            }

            void CreateGenericLight(int speed) //Receive laser speed as var.
            {
                if (time[0] == time[1]) //Same beat
                {
                    if (count < maximum) //Maximum laser is 2
                    {
                        count++;
                    }
                }
                else
                {
                    count = 0; //Reset the count, we are moving forward (in time).
                    for (int i = 0; i < 2; i++)
                    {
                        if (light[i] != 0 && time[0] - time[1] <= 2.5) //TODO: Re-add as an option left/right laser strobe.
                        {
                            //ev = new _Events((time[0] - (time[0] - time[1]) / 2), light[i], 0);
                            //eventTempo.Add(ev);
                        }
                        light[i] = 0;
                    }
                }

                if (count == maximum) //If count reach the maximum, we skip this.
                {
                    return;
                }

                if (light[0] != 0)
                {
                    light[1] = light[0];
                }

                if (lastLight == 2) //We swap between laser
                {
                    light[0] = 3;
                }
                else
                {
                    light[0] = 2;
                }

                switch (light[0]) //Add laser + speed
                {
                    case 2:
                        ev = new _Events(time[0], EventType.LightLeftLasers, color);
                        eventTempo.Add(ev);
                        ev = new _Events(time[0], EventType.RotatingLeftLasers, speed);
                        eventTempo.Add(ev);
                        break;
                    case 3:
                        ev = new _Events(time[0], EventType.LightRightLasers, color);
                        eventTempo.Add(ev);
                        ev = new _Events(time[0], EventType.RotatingRightLasers, speed);
                        eventTempo.Add(ev);
                        break;
                }

                lastLight = light[0];
            }

            ResetTimer();

            foreach (_Notes note in noteTempo) //Process specific light (Side/Neon) using time.
            {
                time[0] = note._time;

                //Here we process Spin and Zoom
                if (time[0] == firstNote && time[1] == 0.0D && AllowSpinZoom) //If we are processing the first note, add spin + zoom to it.
                {
                    ev = new _Events(time[0], EventType.RotationAllTrackRings, 0);
                    eventTempo.Add(ev);
                    ev = new _Events(time[0], EventType.RotationSmallTrackRings, 0);
                    eventTempo.Add(ev);
                }
                else if (time[0] >= ColorOffset + ColorSwap + offset && time[0] > firstNote && AllowSpinZoom) //If we are reaching the next threshold of the timer
                {
                    ev = new _Events(offset, EventType.RotationAllTrackRings, 0); //Add a spin at timer.
                    eventTempo.Add(ev);
                    if (count == 0) //Only add zoom every 2 spin.
                    {
                        ev = new _Events(offset, EventType.RotationSmallTrackRings, 0);
                        eventTempo.Add(ev);
                        count = 1;
                    }
                    else
                    {
                        count--;
                    }
                }
                //If there's a quarter between two double parallel notes and timer didn't pass the check.
                else if (time[1] - time[2] == 0.25 && time[3] == time[2] && time[1] == time[0] && timer < offset && AllowSpinZoom)
                {
                    ev = new _Events(time[0], EventType.RotationAllTrackRings, 0);
                    eventTempo.Add(ev);
                }

                TimerDuration();

                if ((time[0] == time[1] || (time[0] - time[1] <= 0.05 && time[1] != time[2])) && (time[1] != 0.0D && time[0] != last)) //If not same note, same beat, apply once.
                {
                    if (last != 0.0D && time[0] - last <= 2.5) //Off event
                    {
                        if (AllowBackStrobe) //Back Top Laser
                        {
                            ev = new _Events(time[0] - (time[0] - last) / 2, EventType.LightBackTopLasers, 0);
                            eventTempo.Add(ev);
                        }
                        if (AllowNeonStrobe) //Neon Light
                        {
                            ev = new _Events(time[0] - (time[0] - last) / 2, EventType.LightTrackRingNeons, 0);
                            eventTempo.Add(ev);
                        }
                        if (AllowSideStrobe) //Side Light
                        {
                            ev = new _Events(time[0] - (time[0] - last) / 2, EventType.LightBottomBackSideLasers, 0);
                            eventTempo.Add(ev);
                        }
                    }

                    ev = new _Events(time[0], EventType.LightBackTopLasers, color); //Back Top Laser
                    eventTempo.Add(ev);

                    ev = new _Events(time[0], EventType.LightBottomBackSideLasers, color); //Side Light
                    eventTempo.Add(ev);

                    ev = new _Events(time[0], EventType.LightTrackRingNeons, color); //Track Ring Neons
                    eventTempo.Add(ev);

                    last = time[0];
                }

                for (int i = 3; i > 0; i--) //Keep the timing of up to three notes before.
                {
                    time[i] = time[i - 1];
                }
            }

            ResetTimer();

            foreach (_Notes note in noteTempo) //Process all note using time.
            {
                time[0] = note._time;

                TimerDuration();

                if (time[2] == 0.0D) //No third note processed yet.
                {
                    if (time[1] == 0.0D) //No second note processed yet.
                    {
                        time[1] = time[0]; //Skip first note.
                        continue;
                    }
                    else //The second note is processed a very specific way.
                    {
                        ev = new _Events(time[0], EventType.LightRightLasers, color);
                        eventTempo.Add(ev);
                        ev = new _Events(0, EventType.RotatingRightLasers, rnd.Next(SlowMinSpinSpeed, SlowMaxSpinSpeed));
                        eventTempo.Add(ev);
                        ev = new _Events(time[1], EventType.LightLeftLasers, color);
                        eventTempo.Add(ev);
                        ev = new _Events(0, EventType.RotatingLeftLasers, rnd.Next(SlowMinSpinSpeed, SlowMaxSpinSpeed));
                        eventTempo.Add(ev);
                        time[2] = time[1];
                        time[1] = time[0];
                        continue;
                    }
                }

                if (time[0] - time[1] < 0.25) //Lower than fourth
                {
                    if (time[0] != last && time[0] != time[1] && note._type != 3 && note._cutDirection != 8 && note._cutDirection != lastCut && AllowSpinZoom) //Spin
                    {
                        last = time[0];
                        ev = new _Events(time[0], EventType.RotationAllTrackRings, 0);
                        eventTempo.Add(ev);
                        for (int i = 0; i < 8; i++)
                        {
                            ev = new _Events(time[0] - ((time[0] - time[1]) / 8 * i), EventType.RotationAllTrackRings, 0);
                            eventTempo.Add(ev);
                        }
                    }

                    if (time[0] == time[1]) //Same timing, keep same laser speed.
                    {
                        CreateGenericLight(lastSpeed);
                    }
                    else
                    {
                        CreateGenericLight(lastSpeed = rnd.Next(FastMinSpinSpeed, FastMaxSpinSpeed));
                    }
                }
                else if (time[0] - time[1] >= 0.25 && time[0] - time[1] < 0.5) //Quarter to half
                {
                    CreateGenericLight(lastSpeed = rnd.Next(FastMinSpinSpeed, FastMaxSpinSpeed));
                }
                else if (time[0] - time[1] >= 0.5) //Half and above
                {
                    CreateGenericLight(lastSpeed = rnd.Next(SlowMinSpinSpeed, SlowMaxSpinSpeed));
                }

                lastCut = note._cutDirection; //For the spin check.

                for (int i = 3; i > 0; i--) //Keep the timing of up to three notes before.
                {
                    time[i] = time[i - 1];
                }
            }

            return eventTempo;
        }
    }
}
