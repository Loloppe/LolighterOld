using Lolighter.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lolighter.Methods
{
    static class Downscaler
    {
        //Cut Direction
        static readonly List<int> up = new List<int> { 0, 4, 5 };
        static readonly List<int> down = new List<int> { 1, 6, 7 };

        static public List<_Notes> Downscale(List<_Notes> noteTemp)
        {
            List<_Notes> blueNotes = new List<_Notes>();
            List<_Notes> newBlueNotes = new List<_Notes>();
            List<_Notes> redNotes = new List<_Notes>();
            List<_Notes> newRedNotes = new List<_Notes>();
            List<_Notes> tempBomb = new List<_Notes>();
            _Notes n = new _Notes(0, 0, 0, 0, 0);
            _Notes newNote = new _Notes(0, 0, 0, 0, 0);
            bool found = false;

            for (int i = noteTemp.Count() - 1; i > -1; i--) //For each note in reverse-order
            {
                n = noteTemp[i];

                if (n._type == 3) //Skip bomb
                {
                    continue;
                }

                foreach (_Notes temp in noteTemp) //For each note
                {
                    if (temp._time == n._time && temp._type == n._type && temp != n) //Remove stacks, windows, loloppe, tower, etc.
                    {
                        noteTemp.Remove(temp);
                        break;
                    }
                    //Faster than 1/7 but slower than 0, same type and direction or one dot. Remove Slider or Dot spam.
                    else if (n._time - temp._time < 0.14286 && n._time - temp._time > 0 && ((n._cutDirection == temp._cutDirection) || (n._cutDirection == 8 || temp._cutDirection == 8)) && n._type == temp._type)
                    {
                        noteTemp.Remove(n);
                        break;
                    }
                    //Faster than 1/7 but slower than 0, same type. Arrow windowed Slider.
                    else if (n._time - temp._time <= 0.14286 && n._time - temp._time > 0 && n._type == temp._type)
                    {
                        if (up.Contains(n._cutDirection) && up.Contains(temp._cutDirection))
                        {
                            n._time = temp._time;
                            noteTemp.Remove(temp);
                            break;
                        }
                        else if (down.Contains(n._cutDirection) && down.Contains(temp._cutDirection))
                        {
                            n._time = temp._time;
                            noteTemp.Remove(temp);
                            break;
                        }
                        else if (temp._cutDirection == 2 && (n._cutDirection == 4 || n._cutDirection == 6))
                        {
                            n._time = temp._time;
                            noteTemp.Remove(temp);
                            break;
                        }
                        else if (temp._cutDirection == 3 && (n._cutDirection == 5 || n._cutDirection == 7))
                        {
                            n._time = temp._time;
                            noteTemp.Remove(temp);
                            break;
                        }
                        else if (n._cutDirection == 8 && temp._cutDirection != 8)
                        {
                            noteTemp.Remove(n);
                            break;
                        }
                    }
                }
            }

            noteTemp = noteTemp.OrderBy(o => o._time).ToList();

            //Divide notes per type to analyse the flow and make modification.
            foreach (var x in noteTemp)
            {
                if (x._type == 0)
                {
                    redNotes.Add(x);
                }
                else if (x._type == 1)
                {
                    blueNotes.Add(x);
                }
                else if(x._type == 3)
                {
                    tempBomb.Add(x);
                }
            }

            for (int z = 0; z < redNotes.Count() - 1; z++) //For each Red
            {
                if (up.Contains(redNotes[z]._cutDirection) && up.Contains(redNotes[z + 1]._cutDirection))
                {
                    redNotes[z + 1]._cutDirection = 9;
                }
                if (down.Contains(redNotes[z]._cutDirection) && down.Contains(redNotes[z + 1]._cutDirection))
                {
                    redNotes[z + 1]._cutDirection = 9;
                }
            }
            for (int z = 0; z < blueNotes.Count() - 1; z++) //For each Blue
            {
                if (up.Contains(blueNotes[z]._cutDirection) && up.Contains(blueNotes[z + 1]._cutDirection))
                {
                    blueNotes[z + 1]._cutDirection = 9;
                }
                if (down.Contains(blueNotes[z]._cutDirection) && down.Contains(blueNotes[z + 1]._cutDirection))
                {
                    blueNotes[z + 1]._cutDirection = 9;
                }
            }

            newRedNotes = DownMapper(redNotes);
            newRedNotes = Placement(newRedNotes);

            newBlueNotes = DownMapper(blueNotes);
            newBlueNotes = Placement(newBlueNotes);

            noteTemp.Clear();
            noteTemp.AddRange(newBlueNotes);
            noteTemp.AddRange(newRedNotes);
            noteTemp.AddRange(tempBomb);
            noteTemp = noteTemp.OrderBy(o => o._time).ToList();

            for (int i = noteTemp.Count() - 1; i > -1; i--) //For each note in reverse-order
            {
                n = noteTemp[i];

                if (n._type == 3) //Skip bomb
                {
                    continue;
                }
                if((n._lineIndex == 0 || n._lineIndex == 3) && n._lineLayer == 1 && n._cutDirection >= 2) //Skip middle layer and left/right lane that are not Down/Up
                {
                    continue;
                }

                found = false;

                foreach (_Notes temp in noteTemp) //For each note
                {
                    if(temp._time == n._time && temp._lineIndex == n._lineIndex && temp != n) //Same beat and lane but not same note
                    {
                        if (n._lineLayer == temp._lineLayer + 1) //Note under, skip
                        {
                            found = true;
                            continue;
                        }
                        else if (n._lineIndex == 1 || n._lineIndex == 2) //Middle lane, skip
                        {
                            found = true;
                            continue;
                        }
                        else if (((n._lineLayer == 0 && temp._lineLayer == 2) || (n._lineLayer == 2 && temp._lineLayer == 0)) && n._cutDirection != 2 && n._cutDirection != 3 && temp._cutDirection != 2 && temp._cutDirection != 3)
                        {
                            //Side note on side, skip
                            found = true;
                            continue;
                        }
                    }
                    else if(temp._time == n._time && temp._lineIndex != n._lineIndex) //Same beat but not same lane, skip
                    {
                        found = true;
                        continue;
                    }
                }

                if(found) //found = skip
                {
                    continue;
                }

                if(n._lineLayer > 0) //Lower the note to flatten
                {
                    if(n._lineIndex == 1 || n._lineIndex == 2)
                    {
                        n._lineLayer = 0;
                    }
                    else
                    {
                        n._lineLayer -= 1;
                    }
                }
            }

            //Try to fix note position here if necessary

            foreach (var x in noteTemp) //For each note
            {
                foreach (var y in noteTemp) //For each note
                {
                    //Close to eachother, red and blue, not same note.
                    if (y._time - x._time <= 0.26 && y._time - x._time >= 0 && x != y && x._type == 0 && y._type == 1)
                    {
                        //Same line and layer
                        if (y._lineLayer == x._lineLayer && y._lineIndex == x._lineIndex)
                        {
                            //Fix stacked notes
                            switch (x._lineIndex)
                            {
                                case 0:
                                    y._lineIndex = 1;
                                    break;
                                case 1:
                                    y._lineIndex = 2;
                                    break;
                                case 2:
                                    y._lineIndex = 3;
                                    break;
                                case 3:
                                    x._lineIndex = 2;
                                    break;
                            }
                        }
                        //side by side, red and blue
                        else if (y._lineIndex - x._lineIndex == 1)
                        {
                            //red going right
                            if (x._cutDirection == 3 || x._cutDirection == 5 || x._cutDirection == 7)
                            {
                                switch (y._lineIndex)
                                {
                                    case 1:
                                        y._lineIndex++;
                                        break;
                                    case 2:
                                        y._lineIndex++;
                                        break;
                                    case 3:
                                        x._lineIndex--;
                                        break;
                                }
                            }
                            //blue going left
                            else if (y._cutDirection == 2 || y._cutDirection == 4 || y._cutDirection == 6)
                            {
                                switch (x._lineIndex)
                                {
                                    case 0:
                                        y._lineIndex++;
                                        break;
                                    case 1:
                                        x._lineIndex--;
                                        break;
                                    case 2:
                                        x._lineIndex--;
                                        break;
                                }
                            }
                        }
                        //side by side, blue and red
                        else if (x._lineIndex - y._lineIndex == 1)
                        {
                            //red going right
                            if (x._cutDirection == 3 || x._cutDirection == 5 || x._cutDirection == 7)
                            {
                                switch (x._lineIndex)
                                {
                                    case 1:
                                        y._lineIndex += 2;
                                        x._lineIndex--;
                                        break;
                                    case 2:
                                        y._lineIndex++;
                                        x._lineIndex -= 2;
                                        break;
                                    case 3:
                                        y._lineIndex++;
                                        x._lineIndex -= 2;
                                        break;
                                }
                            }
                            //blue going left
                            else if (y._cutDirection == 2 || y._cutDirection == 4 || y._cutDirection == 6)
                            {
                                switch (y._lineIndex)
                                {
                                    case 0:
                                        y._lineIndex += 2;
                                        x._lineIndex--;
                                        break;
                                    case 1:
                                        y._lineIndex++;
                                        x._lineIndex -= 2;
                                        break;
                                    case 2:
                                        y._lineIndex++;
                                        x._lineIndex -= 2;
                                        break;
                                }
                            }
                        }

                        //Flatten if possible
                        if ((y._lineIndex == 1 || y._lineIndex == 2) && y._lineLayer == 1)
                        {
                            y._lineLayer = 0;
                        }
                        if ((x._lineIndex == 1 || x._lineIndex == 2) && x._lineLayer == 1)
                        {
                            x._lineLayer = 0;
                        }
                        if (y._lineLayer == 2 && y._cutDirection == 1 || y._cutDirection == 6 || y._cutDirection == 7)
                        {
                            y._lineLayer = 0;
                        }
                        if (x._lineLayer == 2 && x._cutDirection == 1 || x._cutDirection == 6 || x._cutDirection == 7)
                        {
                            x._lineLayer = 0;
                        }

                        if (x._lineIndex == y._lineIndex) //on top of eachother
                        {
                            if(x._lineIndex < 2)
                            {
                                y._lineIndex++;
                            }
                            else
                            {
                                x._lineIndex--;
                            }
                        }
                        if ((x._lineIndex == y._lineIndex - 1 || x._lineIndex == y._lineIndex - 2) && x._lineLayer == y._lineLayer) //side by side, it's preference here.
                        {
                            if (x._cutDirection == 5)
                            {
                                x._cutDirection = 0;
                            }
                            else if (x._cutDirection == 7)
                            {
                                x._cutDirection = 1;
                            }
                            else if (x._cutDirection == 4)
                            {
                                x._cutDirection = 0;
                            }
                            else if (x._cutDirection == 6)
                            {
                                x._cutDirection = 1;
                            }
                            else if(x._cutDirection == 3)
                            {
                                x._lineIndex = 1;
                                x._lineLayer = 0;
                                y._lineIndex = 3;
                                y._lineLayer = 1;
                            }
                            else if (y._cutDirection == 5)
                            {
                                y._cutDirection = 0;
                            }
                            else if (y._cutDirection == 7)
                            {
                                y._cutDirection = 1;
                            }
                            else if (y._cutDirection == 4)
                            {
                                y._cutDirection = 0;
                            }
                            else if (y._cutDirection == 6)
                            {
                                y._cutDirection = 1;
                            }
                            else if(y._cutDirection == 2)
                            {
                                y._lineIndex = 2;
                                y._lineLayer = 0;
                                x._lineIndex = 0;
                                x._lineLayer = 1;
                            }
                        }
                    }
                }


            }

            foreach (var x in noteTemp) //For each note
            {
                if(x._type == 0 && x._lineIndex == 0 && (x._cutDirection == 5 || x._cutDirection == 7)) //Red and left lane with Up-Right or Down-Right cut direction.
                {
                    if(x._cutDirection == 5) //Up-Right
                    {
                        x._cutDirection = 0; //Up
                    }
                    else if (x._cutDirection == 7) //Down-Right
                    {
                        x._cutDirection = 1; //Down
                    }
                }
                else if (x._type == 1 && x._lineIndex == 3 && (x._cutDirection == 4 || x._cutDirection == 6)) //Blue and right lane with Up-Left or Down-Left cut direction.
                {
                    if (x._cutDirection == 4) //Up-Left
                    {
                        x._cutDirection = 0; //Up
                    }
                    else if (x._cutDirection == 6) //Down-Left
                    {
                        x._cutDirection = 1; //Down
                    }
                }
            }

            List<_Notes> sorted = noteTemp.OrderBy(o => o._time).ToList();

            return sorted;
        }

        public static List<_Notes> DownMapper(List<_Notes> notes)
        {
            List<_Notes> newNotes = new List<_Notes>();
            _Notes n;
            _Notes newNote;
            bool found = false;
            int start = 0;
            double value = 0;
            double count = 0;
            bool last = false;
            int flow = 0;

            for (int i = 0; i < notes.Count(); i++) //For each note
            {
                n = notes[i];

                if (i == notes.Count() - 1) //Last note
                {
                    last = true;
                    found = false;
                    start = i;
                }
                else if (notes[i + 1]._time - n._time <= 1.1 && notes[i + 1]._time - n._time >= 0.9 && (value == 0 || value == 1)) // 1/1 rhythm
                {
                    if (!found) //Start of the rhythm
                    {
                        found = true;
                        value = 1;
                        start = i;
                    }
                }
                else if (notes[i + 1]._time - n._time <= 0.55 && notes[i + 1]._time - n._time >= 0.45 && (value == 0 || value == 0.5)) // 1/2 rhythm
                {
                    if (!found) //Start of the rhythm
                    {
                        found = true;
                        value = 0.5;
                        start = i;
                    }
                }
                else if (notes[i + 1]._time - n._time <= 0.35 && notes[i + 1]._time - n._time>= 0.30 && (value == 0 || value == 0.33333333333)) // 1/3 rhythm
                {
                    if (!found) //Start of the rhythm
                    {
                        found = true;
                        value = 0.33333333333;
                        start = i;
                    }
                }
                else if (notes[i + 1]._time - n._time <= 0.27 && notes[i + 1]._time - n._time >= 0.22 && (value == 0 || value == 0.25)) // 1/4 rhythm
                {
                    if (!found) //Start of the rhythm
                    {
                        found = true;
                        value = 0.25;
                        start = i;
                    }
                }
                else if (notes[i + 1]._time - n._time < 0.22 && notes[i + 1]._time - n._time>= 0.18 && (value == 0 || value == 0.2)) // 1/5 rhythm
                {
                    if (!found) //Start of the rhythm
                    {
                        found = true;
                        value = 0.2;
                        start = i;
                    }
                }
                else if (notes[i + 1]._time - n._time < 0.18 && notes[i + 1]._time - n._time >= 0.15 && (value == 0 || value == 0.16666666666)) // 1/6 rhythm
                {
                    if (!found) //Start of the rhythm
                    {
                        found = true;
                        value = 0.16666666666;
                        start = i;
                    }
                }
                else if (notes[i + 1]._time - n._time < 0.15 && notes[i + 1]._time - n._time >= 0.13 && (value == 0 || value == 0.14285714285)) // 1/7 rhythm
                {
                    if (!found) //Start of the rhythm
                    {
                        found = true;
                        value = 0.14285714285;
                        start = i;
                    }
                }
                else if (notes[i + 1]._time - n._time < 0.13 && notes[i + 1]._time - n._time >= 0.120 && (value == 0 || value == 0.125)) // 1/8 rhythm
                {
                    if (!found) //Start of the rhythm
                    {
                        found = true;
                        value = 0.125;
                        start = i;
                    }
                }
                else //Not the same rhythm as before
                {
                    if (found)
                    {
                        last = true; //It is the last note of the rhythm
                    }
                    found = false; //Reset the found value
                }

                if (found) //Same rhythm
                {
                    count++;
                    continue;
                }
                else if (last) //Last note of the same rhythm
                {
                    count++;

                    if (newNotes.Count() == 0)
                    {
                        for (int y = 0; y < count; y++) //All the notes to be processed
                        {
                            while (notes[flow]._cutDirection == 9 && notes.Count() == flow)
                            {
                                flow++;
                            }

                            newNotes.Add(notes[y]);
                            flow++;
                        }
                    }
                    else
                    {
                        for (int y = 0; y < Math.Round(count / 2, MidpointRounding.AwayFromZero); y++) //All the notes to be processed
                        {
                            while(notes[flow]._cutDirection == 9)
                            {
                                flow++;
                            }

                            if(start + y == notes.Count())
                            {
                                break;
                            }

                            newNote = new _Notes(notes[start]._time + ((value * 2) * (y)), notes[flow]._lineIndex, notes[flow]._lineLayer, notes[start + y]._type, notes[flow]._cutDirection);
                            flow++;
                            newNotes.Add(newNote);
                        }
                    }

                    //Reset value for the next rhythm
                    start = 0;
                    last = false;
                    value = 0;
                    count = 0;
                }

            }

            return newNotes;
        }

        public static List<_Notes> Placement(List<_Notes> notes)
        {
            if (notes.Count > 2) //Flatten and move Blue that are on the left side or Red that are on the right side.
            {
                for (int d = notes.Count() - 1; d > 0; d--) //For each note in reverse-order
                {
                    if (notes[d]._type == 1 && notes[d]._lineIndex == 0)
                    {
                        notes[d]._lineIndex = 1;
                        if(notes[d]._lineLayer == 1)
                        {
                            notes[d]._lineLayer = 0;
                        }
                    }
                    else if (notes[d]._type == 0 && notes[d]._lineIndex == 3)
                    {
                        notes[d]._lineIndex = 2;
                        if (notes[d]._lineLayer == 1)
                        {
                            notes[d]._lineLayer = 0;
                        }
                    }
                }
            }

            return notes;
        }
    }
}
