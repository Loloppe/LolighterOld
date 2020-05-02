using Lolighter.Items;
using System.Collections.Generic;
using System.Linq;
using static Lolighter.Items.Enum;

namespace Lolighter.Methods
{
    static class Sliders
    {
        static public List<_Notes> MakeSliders(List<_Notes> noteTemp, double Limiter, bool IsLimited)
        {
            List<_Notes> newNote = new List<_Notes>();
            List<_Notes> toRemove = new List<_Notes>();
            _Notes n = new _Notes(0, 0, 0, 0, 0);
            _Notes lastNote = new _Notes(0, 0, 0, 0, 0);
            bool found = false;

            for (int i = noteTemp.Count() - 1; i > -1; i--) //For each note in reverse-order
            {
                n = noteTemp[i];

                if (n._type == NoteType.Mine) //If Bomb, skip
                {
                    continue;
                }
                else if (n._cutDirection == CutDirection.Any) //If Any, skip
                {
                    continue;
                }

                found = false;

                foreach (_Notes temp in noteTemp) //For each note
                {
                    if (n._time == temp._time && n._type == temp._type && n._cutDirection == temp._cutDirection && !IsLimited)
                    {
                        //Loloppe notes
                        break;
                    }
                    else if (((n._time - temp._time < Limiter * 2 && n._time - temp._time > 0) || (temp._time - n._time < Limiter * 2 && temp._time - n._time > 0)) && temp._type == n._type)
                    {
                        found = true;
                        break;
                    }
                    else if (temp._time == n._time && temp._type == n._type && n != temp)
                    {
                        found = true;
                        break;
                    }
                    else if (n._time == temp._time && n._type != temp._type && n._cutDirection == temp._cutDirection && (n._cutDirection == CutDirection.UpLeft || n._cutDirection == CutDirection.UpRight))
                    {
                        found = true;
                        break;
                    }
                    else if (n._time == temp._time && temp._type != n._type && ((temp._lineIndex == Line.Left && n._lineIndex == Line.MiddleLeft) || (temp._lineIndex == Line.Right && n._lineIndex == Line.MiddleRight)))
                    {
                        found = true;
                        break;
                    }
                }

                if (found) //If found, then skip
                {
                    continue;
                }

                switch (n._lineLayer) //Process the note into a sliders depending on layer, lane and cut direction manually. This is pretty Pepega.
                {
                    case Layer.Bottom:
                        switch (n._lineIndex)
                        {
                            case Line.Left:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Middle, n._type, CutDirection.Any));
                                        if (n._type == NoteType.Red)
                                        {
                                            newNote.Add(new _Notes(n._time + 0.0625, Line.Left, Layer.Top, n._type, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time + 0.0625, Line.MiddleLeft, Layer.Top, n._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.Down:
                                        n._lineLayer = Layer.Top;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Middle, n._type, CutDirection.Any));
                                        newNote.Add(new _Notes(n._time + 0.0625, Line.Left, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Left:
                                        n._lineIndex = Line.MiddleLeft;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleLeft, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpLeft:
                                        n._lineIndex = 1;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Middle, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpRight:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleRight, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownLeft:
                                        n._lineIndex = Line.MiddleLeft;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleLeft, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.MiddleLeft:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time + 0.0625, Line.MiddleLeft, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleRight, Layer.Bottom, n._type, CutDirection.Any));
                                        newNote.Add(new _Notes(n._time + 0.0625, Line.Right, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpLeft:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Middle, n._type, CutDirection.Any));
                                        newNote.Add(new _Notes(n._time + 0.0625, Line.Left, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpRight:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownRight:
                                        n._lineLayer = Layer.Middle;
                                        n._lineIndex = Line.Left;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleLeft, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.MiddleRight:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time + 0.0625, Line.MiddleRight, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleLeft, Layer.Bottom, n._type, CutDirection.Any));
                                        newNote.Add(new _Notes(n._time + 0.0625, Line.Left, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpLeft:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpRight:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Middle, n._type, CutDirection.Any));
                                        newNote.Add(new _Notes(n._time + 0.0625, Line.Right, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownLeft:
                                        n._lineLayer = Layer.Middle;
                                        n._lineIndex = Line.Right;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleRight, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.Right:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Middle, n._type, CutDirection.Any));
                                        if (n._type == NoteType.Red)
                                        {
                                            newNote.Add(new _Notes(n._time + 0.0625, Line.MiddleRight, Layer.Top, n._type, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time + 0.0625, Line.Right, Layer.Top, n._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.Down:
                                        n._lineLayer = Layer.Top;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Middle, n._type, CutDirection.Any));
                                        newNote.Add(new _Notes(n._time + 0.0625, Line.Right, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleRight, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Right:
                                        n._lineIndex = Line.MiddleRight;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpLeft:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleLeft, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpRight:
                                        n._lineIndex = Line.MiddleRight;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Middle, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleRight, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownRight:
                                        n._lineIndex = Line.MiddleRight;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                }
                                break;
                        }
                        break;
                    case Layer.Middle:
                        switch (n._lineIndex)
                        {
                            case Line.Left:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpRight:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleLeft, Layer.Top, n._type, CutDirection.Any));
                                        newNote.Add(new _Notes(n._time + 0.0625, Line.MiddleRight, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownLeft:
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleLeft, Layer.Bottom, n._type, CutDirection.Any));
                                        newNote.Add(new _Notes(n._time + 0.0625, Line.MiddleRight, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.MiddleLeft:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Left:
                                        n._lineIndex = Line.Left;
                                        break;
                                    case CutDirection.Right:
                                        n._lineIndex = Line.Right;
                                        break;
                                }
                                break;
                            case Line.MiddleRight:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Left:
                                        n._lineIndex = Line.Left;
                                        break;
                                    case CutDirection.Right:
                                        n._lineIndex = Line.Right;
                                        break;
                                }
                                break;
                            case Line.Right:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpLeft:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleRight, Layer.Top, n._type, CutDirection.Any));
                                        newNote.Add(new _Notes(n._time + 0.0625, Line.MiddleLeft, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleRight, Layer.Bottom, n._type, CutDirection.Any));
                                        newNote.Add(new _Notes(n._time + 0.0625, Line.MiddleLeft, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                }
                                break;
                        }
                        break;
                    case Layer.Top:
                        switch (n._lineIndex)
                        {
                            case Line.Left:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        n._lineLayer = Layer.Bottom;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Middle, n._type, CutDirection.Any));
                                        newNote.Add(new _Notes(n._time + 0.0625, Line.Left, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Middle, n._type, CutDirection.Any));
                                        if (n._type == NoteType.Red)
                                        {
                                            newNote.Add(new _Notes(n._time + 0.0625, Line.Left, Layer.Bottom, n._type, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time + 0.0625, Line.MiddleLeft, Layer.Bottom, n._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.Left:
                                        n._lineIndex = Line.MiddleLeft;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleLeft, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpLeft:
                                        n._lineLayer = Layer.Middle;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpRight:
                                        n._lineLayer = Layer.Middle;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Middle, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleRight, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.MiddleLeft:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time + 0.0625, Line.MiddleLeft, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleRight, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpLeft:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpRight:
                                        n._lineIndex = Line.Left;
                                        n._lineLayer = Layer.Middle;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleLeft, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Middle, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.MiddleRight:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time + 0.0625, Line.MiddleRight, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleLeft, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpLeft:
                                        n._lineIndex = Line.Right;
                                        n._lineLayer = Layer.Middle;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleRight, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpRight:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Left, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Middle, n._type, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.Right:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        n._lineLayer = Layer.Bottom;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Middle, n._type, CutDirection.Any));
                                        newNote.Add(new _Notes(n._time + 0.0625, Line.Right, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Middle, n._type, CutDirection.Any));
                                        if (n._type == NoteType.Blue)
                                        {
                                            newNote.Add(new _Notes(n._time + 0.0625, Line.Right, Layer.Bottom, n._type, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time + 0.0625, Line.MiddleRight, Layer.Bottom, n._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleRight, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Right:
                                        n._lineIndex = Line.MiddleRight;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpLeft:
                                        n._lineLayer = Layer.Middle;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpRight:
                                        n._lineLayer = Layer.Middle;
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Top, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.MiddleLeft, Layer.Bottom, n._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(n._time + 0.03125, Line.Right, Layer.Middle, n._type, CutDirection.Any));
                                        break;
                                }
                                break;
                        }
                        break;
                }

                lastNote = n;
            }

            noteTemp.RemoveAll(item => toRemove.Contains(item));
            newNote.AddRange(noteTemp);
            List<_Notes> sorted = newNote.OrderBy(o => o._time).ToList();

            //Here, we try to remove stacked notes that has been created.
            for (int i = sorted.Count() - 5; i > 4; i--) //For each note in reverse-order
            {
                if (sorted[i]._cutDirection == CutDirection.Any) //If it's a dot
                {
                    if (sorted[i]._time - sorted[i - 1]._time <= 0.25 && sorted[i]._lineLayer == sorted[i - 1]._lineLayer && sorted[i]._lineIndex == sorted[i - 1]._lineIndex)
                    {
                        sorted.Remove(sorted[i]);
                    }
                    else if (sorted[i]._time - sorted[i - 2]._time <= 0.25 && sorted[i]._lineLayer == sorted[i - 2]._lineLayer && sorted[i]._lineIndex == sorted[i - 2]._lineIndex)
                    {
                        sorted.Remove(sorted[i]);
                    }
                    else if (sorted[i]._time - sorted[i - 3]._time <= 0.25 && sorted[i]._lineLayer == sorted[i - 3]._lineLayer && sorted[i]._lineIndex == sorted[i - 3]._lineIndex)
                    {
                        sorted.Remove(sorted[i]);
                    }
                    else if (sorted[i]._time - sorted[i - 4]._time <= 0.25 && sorted[i]._lineLayer == sorted[i - 4]._lineLayer && sorted[i]._lineIndex == sorted[i - 4]._lineIndex)
                    {
                        sorted.Remove(sorted[i]);
                    }
                    else if (sorted[i + 1]._time - sorted[i]._time <= 0.25 && sorted[i]._lineLayer == sorted[i + 1]._lineLayer && sorted[i]._lineIndex == sorted[i + 1]._lineIndex)
                    {
                        sorted.Remove(sorted[i]);
                    }
                    else if (sorted[i + 2]._time - sorted[i]._time <= 0.25 && sorted[i]._lineLayer == sorted[i + 2]._lineLayer && sorted[i]._lineIndex == sorted[i + 2]._lineIndex)
                    {
                        sorted.Remove(sorted[i]);
                    }
                    else if (sorted[i + 3]._time - sorted[i]._time <= 0.25 && sorted[i]._lineLayer == sorted[i + 3]._lineLayer && sorted[i]._lineIndex == sorted[i + 3]._lineIndex)
                    {
                        sorted.Remove(sorted[i]);
                    }
                    else if (sorted[i + 4]._time - sorted[i]._time <= 0.25 && sorted[i]._lineLayer == sorted[i + 4]._lineLayer && sorted[i]._lineIndex == sorted[i + 4]._lineIndex)
                    {
                        sorted.Remove(sorted[i]);
                    }
                    else if (sorted[i]._time - sorted[i - 1]._time <= 0.25 && sorted[i - 1]._cutDirection == 8 && sorted[i]._lineIndex == sorted[i - 1]._lineIndex && sorted[i]._type != sorted[i - 1]._type)
                    {
                        sorted.Remove(sorted[i - 1]);
                    }
                }
            }

            return sorted;
        }
    }
}
