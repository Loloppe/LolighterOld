using Lolighter.Items;
using System.Collections.Generic;
using System.Linq;
using static Lolighter.Items.Enum;
using static Lolighter.Items.Utils;

namespace Lolighter.Methods
{
    static class Light
    {
        static public List<_Events> CreateLight(List<_Notes> noteTempo, double ColorOffset, double ColorSwap, bool AllowBackStrobe, bool AllowNeonStrobe, bool AllowSideStrobe, bool AllowFade, bool AllowSpinZoom, bool NerfStrobes)
        {
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
            int currentSpeed = 3;
            double lastSpeed = 0;
            double nextDouble = 0;
            bool firstSlider = false;
            _Notes nextSlider = new _Notes(0, 0, 0, 0, 0);
            List<int> sliderLight = new List<int>(){ 0, 1, 4 };
            int sliderIndex = 0;
            double sliderNoteCount = 0;
            bool wasSlider = false;
            List<int> pattern = new List<int>(Enumerable.Range(0, 5));
            int patternIndex = 0;
            int patternCount = 0;
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
                            //eventTempo.Add(new _Events((time[0] - (time[0] - time[1]) / 2), light[i], 0));
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
                        eventTempo.Add(new _Events(time[0], EventType.LightLeftLasers, color));
                        eventTempo.Add(new _Events(time[0], EventType.RotatingLeftLasers, speed));
                        break;
                    case 3:
                        eventTempo.Add(new _Events(time[0], EventType.LightRightLasers, color));
                        eventTempo.Add(new _Events(time[0], EventType.RotatingRightLasers, speed));
                        break;
                }

                lastLight = light[0];
            }

            ResetTimer();

            foreach (_Notes note in noteTempo) //Process specific light (Side/Neon) using time.
            {
                double now = note._time;
                time[0] = now;

                //Here we process Spin and Zoom
                if (now == firstNote && time[1] == 0.0D && AllowSpinZoom) //If we are processing the first note, add spin + zoom to it.
                {
                    eventTempo.Add(new _Events(now, EventType.RotationAllTrackRings, 0));
                    eventTempo.Add(new _Events(now, EventType.RotationSmallTrackRings, 0));
                }
                else if (now >= ColorOffset + ColorSwap + offset && now > firstNote && AllowSpinZoom) //If we are reaching the next threshold of the timer
                {
                    //Add a spin at timer.
                    eventTempo.Add(new _Events(offset, EventType.RotationAllTrackRings, 0));
                    if (count == 0) //Only add zoom every 2 spin.
                    {
                        eventTempo.Add(new _Events(offset, EventType.RotationSmallTrackRings, 0));
                        count = 1;
                    }
                    else
                    {
                        count--;
                    }
                }
                //If there's a quarter between two double parallel notes and timer didn't pass the check.
                else if (time[1] - time[2] == 0.25 && time[3] == time[2] && time[1] == now && timer < offset && AllowSpinZoom)
                {
                    eventTempo.Add(new _Events(now, EventType.RotationAllTrackRings, 0));
                }

                TimerDuration();

                if ((now == time[1] || (now - time[1] <= 0.02 && time[1] != time[2])) && (time[1] != 0.0D && now != last)) //If not same note, same beat, apply once.
                {
                    if (!NerfStrobes) //Off event
                    {
                        if(now - last >= 0.5)
                        {
                            if (AllowBackStrobe) //Back Top Laser
                            {
                                eventTempo.Add(new _Events(now + 0.25, EventType.LightBackTopLasers, 0));
                            }
                            if (AllowNeonStrobe) //Neon Light
                            {
                                eventTempo.Add(new _Events(now + 0.25, EventType.LightTrackRingNeons, 0));
                            }
                            if (AllowSideStrobe) //Side Light
                            {
                                eventTempo.Add(new _Events(now + 0.25, EventType.LightBottomBackSideLasers, 0));
                            }
                        }
                        else
                        {
                            if (AllowBackStrobe) //Back Top Laser
                            {
                                eventTempo.Add(new _Events(now - (now - last) / 2, EventType.LightBackTopLasers, 0));
                            }
                            if (AllowNeonStrobe) //Neon Light
                            {
                                eventTempo.Add(new _Events(now - (now - last) / 2, EventType.LightTrackRingNeons, 0));
                            }
                            if (AllowSideStrobe) //Side Light
                            {
                                eventTempo.Add(new _Events(now - (now - last) / 2, EventType.LightBottomBackSideLasers, 0));
                            }
                        }
                    }

                    eventTempo.Add(new _Events(now, EventType.LightBackTopLasers, color)); //Back Top Laser
                    eventTempo.Add(new _Events(now, EventType.LightBottomBackSideLasers, color)); //Side Light
                    eventTempo.Add(new _Events(now, EventType.LightTrackRingNeons, color)); //Track Ring Neons

                    last = now;
                }

                for (int i = 3; i > 0; i--) //Keep the timing of up to three notes before.
                {
                    time[i] = time[i - 1];
                }
            }

            if (NerfStrobes)
            {
                double lastTimeTop = 100;
                double lastTimeNeon = 100;
                double lastTimeSide = 100;

                foreach(var x in eventTempo)
                {
                    if(x._type == EventType.LightBackTopLasers)
                    {
                        if (x._time - lastTimeTop <= 1)
                        {
                            x._value = Swap(x._value);
                        }
                        lastTimeTop = x._time;
                    }
                    else if(x._type == EventType.LightTrackRingNeons)
                    {
                        if (x._time - lastTimeNeon <= 1)
                        {
                            x._value = Swap(x._value);
                        }
                        lastTimeNeon = x._time;
                    }
                    else if (x._type == EventType.LightBottomBackSideLasers)
                    {
                        if (x._time - lastTimeSide <= 1)
                        {
                            x._value = Swap(x._value);
                        }
                        lastTimeSide = x._time;
                    }
                }
            }

            ResetTimer();

            foreach (_Notes note in noteTempo) //Process all note using time.
            {
                time[0] = note._time;

                TimerDuration();

                if(wasSlider)
                {
                    if(sliderNoteCount != 0)
                    {
                        sliderNoteCount--;
                        lastCut = note._cutDirection; //For the spin check.

                        for (int i = 3; i > 0; i--) //Keep the timing of up to three notes before.
                        {
                            time[i] = time[i - 1];
                        }
                        continue;
                    }
                    else
                    {
                        wasSlider = false;
                    }
                }

                if (time[2] == 0.0D) //No third note processed yet.
                {
                    if (time[1] == 0.0D) //No second note processed yet.
                    {
                        time[1] = time[0]; //Skip first note.
                        continue;
                    }
                    else //The second note is processed a very specific way.
                    {
                        if(!firstSlider)
                        {
                            eventTempo.Add(new _Events(time[0], EventType.LightRightLasers, color));
                            eventTempo.Add(new _Events(0, EventType.RotatingRightLasers, 1));
                            eventTempo.Add(new _Events(time[1], EventType.LightLeftLasers, color));
                            eventTempo.Add(new _Events(0, EventType.RotatingLeftLasers, 1));
                        }
                        time[2] = time[1];
                        time[1] = time[0];
                        continue;
                    }
                }
                
                if(firstSlider)
                {
                    firstSlider = false;
                    continue;
                }

                // Find the next double
                if(time[0] >= nextDouble)
                {
                    for (int i = noteTempo.FindIndex(n => n == note); i < noteTempo.Count() - 1; i++)
                    {
                        if (noteTempo[i]._time == noteTempo[i - 1]._time)
                        {
                            nextDouble = noteTempo[i]._time;
                            break;
                        }
                    }
                }

                // Find the next slider (1/8 minimum)
                if (time[0] >= nextSlider._time)
                {
                    sliderNoteCount = 0;

                    for (int i = noteTempo.FindIndex(n => n == note); i < noteTempo.Count() - 1; i++)
                    {
                        // Between 1/8 and 0, same cut direction or dots
                        if (noteTempo[i]._time - noteTempo[i - 1]._time <= 0.125 && noteTempo[i]._time - noteTempo[i - 1]._time > 0 && (noteTempo[i]._cutDirection == noteTempo[i - 1]._cutDirection || noteTempo[i]._cutDirection == 8))
                        {
                            // Search for the last note of the slider
                            if(sliderNoteCount == 0)
                            {
                                // This is the first note of the slider
                                nextSlider = noteTempo[i - 1];
                            }
                            sliderNoteCount++;
                        }
                        else if(sliderNoteCount != 0)
                        {
                            break;
                        }
                    }
                }

                // Slider time
                if(nextSlider == note)
                {
                    // Take a light between neon, side or backlight and strobes it via On/Flash
                    if (sliderIndex == -1)
                    {
                        int old = sliderLight[sliderIndex + 1];

                        do
                        {
                            sliderLight.Shuffle();
                        } while (sliderLight[2] == old);

                        sliderIndex = 2;
                    }

                    // Place light
                    if (AllowFade)
                    {
                        eventTempo.Add(new _Events(time[0], sliderLight[sliderIndex], color - 2));
                        eventTempo.Add(new _Events(time[0] + 0.125, sliderLight[sliderIndex], color - 1));
                        eventTempo.Add(new _Events(time[0] + 0.25, sliderLight[sliderIndex], color - 2));
                        eventTempo.Add(new _Events(time[0] + 0.375, sliderLight[sliderIndex], color - 1));
                    }
                    else
                    {
                        eventTempo.Add(new _Events(time[0], sliderLight[sliderIndex], color));
                        eventTempo.Add(new _Events(time[0] + 0.125, sliderLight[sliderIndex], color + 1));
                        eventTempo.Add(new _Events(time[0] + 0.25, sliderLight[sliderIndex], color));
                        eventTempo.Add(new _Events(time[0] + 0.375, sliderLight[sliderIndex], color + 1));
                    }
                    eventTempo.Add(new _Events(time[0] + 0.5, sliderLight[sliderIndex], 0));

                    sliderIndex--;

                    // Spin goes brrr
                    if(AllowSpinZoom)
                    {
                        eventTempo.Add(new _Events(time[0], EventType.RotationAllTrackRings, 0));
                        for (int i = 0; i < 8; i++)
                        {
                            eventTempo.Add(new _Events(time[0] + 0.5 - (0.5 / 8 * i), EventType.RotationAllTrackRings, 0));
                        }
                    }

                    wasSlider = true;
                }
                // Not a double
                else if (time[0] != nextDouble)
                {
                    if(time[0] - time[1] >= lastSpeed + 0.02 || time[0] - time[1] <= lastSpeed - 0.02 || patternCount == 20) // New speed or 20 notes of the same pattern
                    {
                        int old = 0;
                        // New pattern
                        if(patternIndex != 0)
                        {
                            old = pattern[patternIndex - 1];
                        }
                        else
                        {
                            old = pattern[4];
                        }

                        do
                        {
                            pattern.Shuffle();
                        } while (pattern[0] == old);
                        patternIndex = 0;
                        patternCount = 0;
                    }

                    // Place the next light
                    eventTempo.Add(new _Events(time[0], pattern[patternIndex], color));

                    if(time[0] - time[1] < 0.25)
                    {
                        currentSpeed = 7;
                    }
                    else if(time[0] - time[1] >= 0.25 && time[0] - time[1] < 0.5)
                    {
                        currentSpeed = 5;
                    }
                    else if(time[0] - time[1] >= 0.5 && time[0] - time[1] < 1)
                    {
                        currentSpeed = 3;
                    }
                    else
                    {
                        currentSpeed = 1;
                    }

                    if (pattern[patternIndex] == 2)
                    {
                        eventTempo.Add(new _Events(time[0], EventType.RotatingLeftLasers, currentSpeed));
                    }
                    else if(pattern[patternIndex] == 3)
                    {
                        eventTempo.Add(new _Events(time[0], EventType.RotatingRightLasers, currentSpeed));
                    }

                    // Place off event
                    if(noteTempo[noteTempo.Count() - 1] != note)
                    {
                        if (noteTempo[noteTempo.FindIndex(n => n == note) + 1]._time == nextDouble)
                        {
                            if(noteTempo[noteTempo.FindIndex(n => n == note) + 1]._time - time[0] <= 2)
                            {
                                double value = (noteTempo[noteTempo.FindIndex(n => n == note) + 1]._time - noteTempo[noteTempo.FindIndex(n => n == note)]._time) / 2;
                                eventTempo.Add(new _Events(noteTempo[noteTempo.FindIndex(n => n == note)]._time + value, pattern[patternIndex], 0));
                            }
                        }
                        else
                        {
                            eventTempo.Add(new _Events(noteTempo[noteTempo.FindIndex(n => n == note) + 1]._time, pattern[patternIndex], 0));
                        }
                    }

                    // Pattern have 5 notes in total (5 lights available)
                    if (patternIndex < 4)
                    {
                        patternIndex++;
                    }
                    else
                    {
                        patternIndex = 0;
                    }

                    patternCount++;
                    lastSpeed = time[0] - time[1];
                }
                else if (time[0] - time[1] < 0.25) //Lower than fourth
                {
                    if (time[0] != last && time[0] != time[1] && note._type != 3 && note._cutDirection != 8 && note._cutDirection != lastCut && AllowSpinZoom && !NerfStrobes) //Spin
                    {
                        last = time[0];
                        eventTempo.Add(new _Events(time[0], EventType.RotationAllTrackRings, 0));
                        for (int i = 0; i < 8; i++)
                        {
                            eventTempo.Add(new _Events(time[0] - ((time[0] - time[1]) / 8 * i), EventType.RotationAllTrackRings, 0));
                        }
                    }

                    if(time[0] == time[1])
                    {
                        CreateGenericLight(currentSpeed);
                    }
                    else
                    {
                        CreateGenericLight(currentSpeed = 7);
                    }
                }
                else if (time[0] - time[1] >= 0.25 && time[0] - time[1] < 0.5) //Quarter to half
                {
                    CreateGenericLight(currentSpeed = 5);
                }
                else if (time[0] - time[1] >= 0.5 && time[0] - time[1] < 1) //Half to 1
                {

                    CreateGenericLight(currentSpeed = 3);
                }
                else if (time[0] - time[1] >= 1) //1 and above
                {
                    CreateGenericLight(currentSpeed = 1);
                }

                lastCut = note._cutDirection; //For the spin check.

                for (int i = 3; i > 0; i--) //Keep the timing of up to three notes before.
                {
                    time[i] = time[i - 1];
                }
            }

            // Sort lights
            eventTempo = eventTempo.OrderBy(o => o._time).ToList();

            // Remove fused
            for (int i = 1; i < eventTempo.Count() - 1; i++)
            {
                // Very close to eachother
                if (eventTempo.Any(e => e._time == eventTempo[i]._time && e._type == eventTempo[i]._type && e != eventTempo[i]))
                {
                    // Off event
                    if (eventTempo[i]._value == 0 || eventTempo[i]._value == 4)
                    {
                        eventTempo.Remove(eventTempo[i]);
                        i--;
                    }
                }
            }

            return eventTempo;
        }
    }
}
