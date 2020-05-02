using Lolighter.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using static Lolighter.Items.Enum;

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

                if (n._type == NoteType.Mine) //Skip Bomb
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
                    else if (n._time - temp._time < 0.14286 && n._time - temp._time > 0 && ((n._cutDirection == temp._cutDirection) || (n._cutDirection == CutDirection.Any || temp._cutDirection == CutDirection.Any)) && n._type == temp._type)
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
                        else if (temp._cutDirection == CutDirection.Left && (n._cutDirection == CutDirection.UpLeft || n._cutDirection == CutDirection.DownLeft))
                        {
                            n._time = temp._time;
                            noteTemp.Remove(temp);
                            break;
                        }
                        else if (temp._cutDirection == CutDirection.Right && (n._cutDirection == CutDirection.UpRight || n._cutDirection == CutDirection.DownRight))
                        {
                            n._time = temp._time;
                            noteTemp.Remove(temp);
                            break;
                        }
                        else if (n._cutDirection == CutDirection.Any && temp._cutDirection != CutDirection.Any)
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
                if (x._type == NoteType.Red)
                {
                    redNotes.Add(x);
                }
                else if (x._type == NoteType.Blue)
                {
                    blueNotes.Add(x);
                }
                else if (x._type == NoteType.Mine)
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

                if (n._type == NoteType.Mine) //Skip Bomb
                {
                    continue;
                }
                if ((n._lineIndex == Line.Left || n._lineIndex == Line.Right) && n._lineLayer == Layer.Middle && n._cutDirection >= CutDirection.Left) //Skip middle layer and left/right lane that are not Down/Up
                {
                    continue;
                }

                found = false;

                foreach (_Notes temp in noteTemp) //For each note
                {
                    if (temp._time == n._time && temp._lineIndex == n._lineIndex && temp != n) //Same beat and lane but not same note
                    {
                        if (n._lineLayer == temp._lineLayer + 1) //Note under, skip
                        {
                            found = true;
                            continue;
                        }
                        else if (n._lineIndex == Line.MiddleLeft || n._lineIndex == Line.MiddleRight) //Middle lane, skip
                        {
                            found = true;
                            continue;
                        }
                        else if (((n._lineLayer == Layer.Bottom && temp._lineLayer == Layer.Top) || (n._lineLayer == Layer.Top && temp._lineLayer == Layer.Bottom)) && n._cutDirection != CutDirection.Left && n._cutDirection != CutDirection.Right && temp._cutDirection != CutDirection.Left && temp._cutDirection != CutDirection.Right)
                        {
                            //Side note on side, skip
                            found = true;
                            continue;
                        }
                    }
                    else if (temp._time == n._time && temp._lineIndex != n._lineIndex) //Same beat but not same lane, skip
                    {
                        found = true;
                        continue;
                    }
                }

                if (found) //found = skip
                {
                    continue;
                }

                if (n._lineLayer > 0) //Lower the note to flatten
                {
                    if (n._lineIndex == Line.MiddleLeft || n._lineIndex == Line.MiddleRight)
                    {
                        n._lineLayer = Layer.Bottom;
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
                    if (y._time - x._time <= 0.26 && y._time - x._time >= 0 && x != y && x._type == NoteType.Red && y._type == NoteType.Blue)
                    {
                        //Same line and layer
                        if (y._lineLayer == x._lineLayer && y._lineIndex == x._lineIndex)
                        {
                            //Fix stacked notes
                            switch (x._lineIndex)
                            {
                                case Line.Left:
                                    y._lineIndex = Line.MiddleLeft;
                                    break;
                                case Line.MiddleLeft:
                                    y._lineIndex = Line.MiddleRight;
                                    break;
                                case Line.MiddleRight:
                                    y._lineIndex = Line.Right;
                                    break;
                                case Line.Right:
                                    x._lineIndex = Line.MiddleRight;
                                    break;
                            }
                        }
                        //side by side, red and blue
                        else if (y._lineIndex - x._lineIndex == 1)
                        {
                            //red going right
                            if (x._cutDirection == CutDirection.Right || x._cutDirection == CutDirection.UpRight || x._cutDirection == CutDirection.DownRight)
                            {
                                switch (y._lineIndex)
                                {
                                    case Line.MiddleLeft:
                                        y._lineIndex++;
                                        break;
                                    case Line.MiddleRight:
                                        y._lineIndex++;
                                        break;
                                    case Line.Right:
                                        x._lineIndex--;
                                        break;
                                }
                            }
                            //blue going left
                            else if (y._cutDirection == CutDirection.Left || y._cutDirection == CutDirection.UpLeft || y._cutDirection == CutDirection.DownLeft)
                            {
                                switch (x._lineIndex)
                                {
                                    case Line.Left:
                                        y._lineIndex++;
                                        break;
                                    case Line.MiddleLeft:
                                        x._lineIndex--;
                                        break;
                                    case Line.MiddleRight:
                                        x._lineIndex--;
                                        break;
                                }
                            }
                        }
                        //side by side, blue and red
                        else if (x._lineIndex - y._lineIndex == 1)
                        {
                            //red going right
                            if (x._cutDirection == CutDirection.Right || x._cutDirection == CutDirection.UpRight || x._cutDirection == CutDirection.DownRight)
                            {
                                switch (x._lineIndex)
                                {
                                    case Line.MiddleLeft:
                                        y._lineIndex += 2;
                                        x._lineIndex--;
                                        break;
                                    case Line.MiddleRight:
                                        y._lineIndex++;
                                        x._lineIndex -= 2;
                                        break;
                                    case Line.Right:
                                        y._lineIndex++;
                                        x._lineIndex -= 2;
                                        break;
                                }
                            }
                            //blue going left
                            else if (y._cutDirection == CutDirection.Left || y._cutDirection == CutDirection.UpLeft || y._cutDirection == CutDirection.DownLeft)
                            {
                                switch (y._lineIndex)
                                {
                                    case Line.Left:
                                        y._lineIndex += 2;
                                        x._lineIndex--;
                                        break;
                                    case Line.MiddleLeft:
                                        y._lineIndex++;
                                        x._lineIndex -= 2;
                                        break;
                                    case Line.MiddleRight:
                                        y._lineIndex++;
                                        x._lineIndex -= 2;
                                        break;
                                }
                            }
                        }

                        //Flatten if possible
                        if ((y._lineIndex == Line.MiddleLeft || y._lineIndex == Line.MiddleRight) && y._lineLayer == Layer.Middle)
                        {
                            y._lineLayer = Layer.Bottom;
                        }
                        if ((x._lineIndex == Line.MiddleLeft || x._lineIndex == Line.MiddleRight) && x._lineLayer == Layer.Middle)
                        {
                            x._lineLayer = Layer.Bottom;
                        }
                        if (y._lineLayer == Layer.Top && y._cutDirection == CutDirection.Down || y._cutDirection == CutDirection.DownLeft || y._cutDirection == CutDirection.DownRight)
                        {
                            y._lineLayer = Layer.Bottom;
                        }
                        if (x._lineLayer == Layer.Top && x._cutDirection == CutDirection.Down || x._cutDirection == CutDirection.DownLeft || x._cutDirection == CutDirection.DownRight)
                        {
                            x._lineLayer = Layer.Bottom;
                        }

                        if (x._lineIndex == y._lineIndex) //on top of eachother
                        {
                            if (x._lineIndex < Line.MiddleRight)
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
                            if (x._cutDirection == CutDirection.UpRight)
                            {
                                x._cutDirection = CutDirection.Up;
                            }
                            else if (x._cutDirection == CutDirection.DownRight)
                            {
                                x._cutDirection = CutDirection.Down;
                            }
                            else if (x._cutDirection == CutDirection.UpLeft)
                            {
                                x._cutDirection = CutDirection.Up;
                            }
                            else if (x._cutDirection == CutDirection.DownLeft)
                            {
                                x._cutDirection = CutDirection.Down;
                            }
                            else if (x._cutDirection == CutDirection.Right)
                            {
                                x._lineIndex = Line.MiddleLeft;
                                x._lineLayer = Layer.Bottom;
                                y._lineIndex = Line.Right;
                                y._lineLayer = Layer.Middle;
                            }
                            else if (y._cutDirection == CutDirection.UpRight)
                            {
                                y._cutDirection = CutDirection.Up;
                            }
                            else if (y._cutDirection == CutDirection.DownRight)
                            {
                                y._cutDirection = CutDirection.Down;
                            }
                            else if (y._cutDirection == CutDirection.UpLeft)
                            {
                                y._cutDirection = CutDirection.Up;
                            }
                            else if (y._cutDirection == CutDirection.DownLeft)
                            {
                                y._cutDirection = CutDirection.Down;
                            }
                            else if (y._cutDirection == CutDirection.Left)
                            {
                                y._lineIndex = Line.MiddleRight;
                                y._lineLayer = Layer.Bottom;
                                x._lineIndex = Line.Left;
                                x._lineLayer = Layer.Middle;
                            }
                        }
                    }
                }


            }

            foreach (var x in noteTemp) //For each note
            {
                if (x._type == NoteType.Red && x._lineIndex == Line.Left && (x._cutDirection == CutDirection.UpRight || x._cutDirection == CutDirection.DownRight)) //Red and left lane with Up-Right or Down-Right cut direction.
                {
                    if (x._cutDirection == CutDirection.UpRight) //Up-Right
                    {
                        x._cutDirection = CutDirection.Up; //Up
                    }
                    else if (x._cutDirection == CutDirection.DownRight) //Down-Right
                    {
                        x._cutDirection = CutDirection.Down; //Down
                    }
                }
                else if (x._type == NoteType.Blue && x._lineIndex == Line.Right && (x._cutDirection == CutDirection.UpLeft || x._cutDirection == CutDirection.DownLeft)) //Blue and right lane with Up-Left or Down-Left cut direction.
                {
                    if (x._cutDirection == CutDirection.UpLeft) //Up-Left
                    {
                        x._cutDirection = CutDirection.Up; //Up
                    }
                    else if (x._cutDirection == CutDirection.DownLeft) //Down-Left
                    {
                        x._cutDirection = CutDirection.Down; //Down
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
                else if (notes[i + 1]._time - n._time <= 0.35 && notes[i + 1]._time - n._time >= 0.30 && (value == 0 || value == 0.33333333333)) // 1/3 rhythm
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
                else if (notes[i + 1]._time - n._time < 0.22 && notes[i + 1]._time - n._time >= 0.18 && (value == 0 || value == 0.2)) // 1/5 rhythm
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
                else //Not the same rhythm as before or rhythm above 1.1
                {
                    if (found)
                    {
                        last = true; //It is the last note of the rhythm
                    }
                    else
                    {
                        start = i;
                        last = true;
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

                    for (int y = 0; y < Math.Round(count / 2, MidpointRounding.AwayFromZero); y++) //All the notes to be processed
                    {
                        while (notes[flow]._cutDirection == 9)
                        {
                            flow++;
                        }

                        if (start + y == notes.Count())
                        {
                            break;
                        }

                        newNote = new _Notes(notes[start]._time + ((value * 2) * (y)), notes[flow]._lineIndex, notes[flow]._lineLayer, notes[start + y]._type, notes[flow]._cutDirection);
                        flow++;
                        newNotes.Add(newNote);
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
                    if (notes[d]._type == NoteType.Blue && notes[d]._lineIndex == Line.Left)
                    {
                        notes[d]._lineIndex = Line.MiddleLeft;
                        if (notes[d]._lineLayer == Layer.Middle)
                        {
                            notes[d]._lineLayer = Layer.Bottom;
                        }
                    }
                    else if (notes[d]._type == NoteType.Red && notes[d]._lineIndex == Line.Right)
                    {
                        notes[d]._lineIndex = Line.MiddleRight;
                        if (notes[d]._lineLayer == Layer.Middle)
                        {
                            notes[d]._lineLayer = Layer.Bottom;
                        }
                    }
                }
            }

            return notes;
        }
    }
}