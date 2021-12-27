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
            _Notes now = new _Notes(0, 0, 0, 0, 0);
            _Notes lastNote = new _Notes(0, 0, 0, 0, 0);
            bool wrong = false;

            for (int i = noteTemp.Count() - 1; i > 0; i--) // We try to find if the current note is part of a slider or can be slidified.
            {
                now = noteTemp[i];

                if (now._type == NoteType.Mine) //If Bomb, skip
                {
                    continue;
                }
                else if (now._cutDirection == CutDirection.Any) //If Any, skip
                {
                    continue;
                }

                wrong = false; //If wrong, skip

                foreach (_Notes temp in noteTemp) //For each note
                {
                    if (now == temp) continue; // Same note
                    else if (now._time == temp._time && now._type == temp._type && now._cutDirection == temp._cutDirection && !IsLimited) break; // Loloppe
                    else if (((now._time - temp._time < Limiter * 2 && now._time - temp._time > 0) || (temp._time - now._time < Limiter * 2 && temp._time - now._time > 0)) && temp._type == now._type)
                    {
                        // Already a slider, tower, stack or issue with mapping?
                        wrong = true;
                        break;
                    }
                    else if (temp._time == now._time && temp._type == now._type && now != temp)
                    {
                        // Already a slider, tower, stack or issue with mapping?
                        wrong = true;
                        break;
                    }
                    else if (now._time == temp._time && now._type != temp._type && now._cutDirection == temp._cutDirection && (now._cutDirection == CutDirection.UpLeft || now._cutDirection == CutDirection.UpRight))
                    {
                        // Diagonal double
                        wrong = true;
                        break;
                    }
                    else if (now._time == temp._time && temp._type != now._type && ((temp._lineIndex == Line.Left && now._lineIndex == Line.MiddleLeft) || (temp._lineIndex == Line.Right && now._lineIndex == Line.MiddleRight)))
                    {
                        // Collision issue
                        wrong = true;
                        break;
                    }
                }

                if (wrong) //If wrong, then skip
                {
                    continue;
                }

                switch (now._lineLayer) //Process the note into a sliders depending on layer, lane and cut direction manually. This is pretty Pepega.
                {
                    case Layer.Bottom:
                        switch (now._lineIndex)
                        {
                            case Line.Left:
                                switch (now._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Middle, now._type, CutDirection.Any));
                                        if (now._type == NoteType.Red)
                                        {
                                            newNote.Add(new _Notes(now._time + 0.0625, Line.Left, Layer.Top, now._type, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(now._time + 0.0625, Line.MiddleLeft, Layer.Top, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.Down:
                                        if(!noteTemp.Any(x => x != now && x._time == now._time && x._lineLayer == Layer.Top))
                                        {
                                            now._lineLayer = Layer.Top;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Middle, now._type, CutDirection.Any));
                                            newNote.Add(new _Notes(now._time + 0.0625, Line.Left, Layer.Bottom, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.Left:
                                        if (!noteTemp.Any(x => x != now && x._time == now._time && x._lineIndex == Line.MiddleLeft))
                                        {
                                            now._lineIndex = Line.MiddleLeft;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Bottom, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleLeft, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpLeft:
                                        if (!noteTemp.Any(x => x != now && x._time == now._time && x._lineIndex == Line.MiddleLeft))
                                        {
                                            now._lineIndex = Line.MiddleLeft;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Middle, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.UpRight:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleRight, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownLeft:
                                        if (!noteTemp.Any(x => x != now && x._time == now._time && x._lineIndex == Line.MiddleLeft))
                                        {
                                            now._lineIndex = Line.MiddleLeft;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Bottom, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleLeft, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.MiddleLeft:
                                switch (now._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(now._time + 0.0625, Line.MiddleLeft, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleRight, Layer.Bottom, now._type, CutDirection.Any));
                                        newNote.Add(new _Notes(now._time + 0.0625, Line.Right, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpLeft:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Middle, now._type, CutDirection.Any));
                                        newNote.Add(new _Notes(now._time + 0.0625, Line.Left, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpRight:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownRight:
                                        if (!noteTemp.Any(x => x != now && x._time == now._time && x._lineIndex == Line.Left && x._lineLayer == Layer.Middle))
                                        {
                                            now._lineLayer = Layer.Middle;
                                            now._lineIndex = Line.Left;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleLeft, Layer.Bottom, now._type, CutDirection.Any));
                                        }
                                        break;
                                }
                                break;
                            case Line.MiddleRight:
                                switch (now._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(now._time + 0.0625, Line.MiddleRight, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleLeft, Layer.Bottom, now._type, CutDirection.Any));
                                        newNote.Add(new _Notes(now._time + 0.0625, Line.Left, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpLeft:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpRight:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Middle, now._type, CutDirection.Any));
                                        newNote.Add(new _Notes(now._time + 0.0625, Line.Right, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownLeft:
                                        if (!noteTemp.Any(x => x != now && x._time == now._time && x._lineIndex == Line.Right && x._lineLayer == Layer.Middle))
                                        {
                                            now._lineLayer = Layer.Middle;
                                            now._lineIndex = Line.Right;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleRight, Layer.Bottom, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.Right:
                                switch (now._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Middle, now._type, CutDirection.Any));
                                        if (now._type == NoteType.Red)
                                        {
                                            newNote.Add(new _Notes(now._time + 0.0625, Line.MiddleRight, Layer.Top, now._type, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(now._time + 0.0625, Line.Right, Layer.Top, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.Down:
                                        if (!noteTemp.Any(x => x != now && x._time == now._time && x._lineLayer == Layer.Top))
                                        {
                                            now._lineLayer = Layer.Top;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Middle, now._type, CutDirection.Any));
                                            newNote.Add(new _Notes(now._time + 0.0625, Line.Right, Layer.Bottom, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleRight, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Right:
                                        if (!noteTemp.Any(x => x != now && x._time == now._time && x._lineIndex == Line.MiddleRight))
                                        {
                                            now._lineIndex = Line.MiddleRight;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Bottom, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.UpLeft:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleLeft, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpRight:
                                        if (!noteTemp.Any(x => x != now && x._time == now._time && x._lineIndex == Line.MiddleRight))
                                        {
                                            now._lineIndex = Line.MiddleRight;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Middle, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleRight, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownRight:
                                        if (!noteTemp.Any(x => x != now && x._time == now._time && x._lineIndex == Line.MiddleRight))
                                        {
                                            now._lineIndex = Line.MiddleRight;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Bottom, now._type, CutDirection.Any));
                                        }
                                        break;
                                }
                                break;
                        }
                        break;
                    case Layer.Middle:
                        switch (now._lineIndex)
                        {
                            case Line.Left:
                                switch (now._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpRight:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleLeft, Layer.Top, now._type, CutDirection.Any));
                                        newNote.Add(new _Notes(now._time + 0.0625, Line.MiddleRight, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleLeft, Layer.Bottom, now._type, CutDirection.Any));
                                        newNote.Add(new _Notes(now._time + 0.0625, Line.MiddleRight, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.Right:
                                switch (now._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpLeft:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleRight, Layer.Top, now._type, CutDirection.Any));
                                        newNote.Add(new _Notes(now._time + 0.0625, Line.MiddleLeft, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleRight, Layer.Bottom, now._type, CutDirection.Any));
                                        newNote.Add(new _Notes(now._time + 0.0625, Line.MiddleLeft, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                }
                                break;
                        }
                        break;
                    case Layer.Top:
                        switch (now._lineIndex)
                        {
                            case Line.Left:
                                switch (now._cutDirection)
                                {
                                    case CutDirection.Up:
                                        if (!noteTemp.Any(x => x != now && x._time == now._time && x._lineLayer == Layer.Bottom))
                                        {
                                            now._lineLayer = Layer.Bottom;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Middle, now._type, CutDirection.Any));
                                            newNote.Add(new _Notes(now._time + 0.0625, Line.Left, Layer.Top, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Middle, now._type, CutDirection.Any));
                                        if (now._type == NoteType.Red)
                                        {
                                            newNote.Add(new _Notes(now._time + 0.0625, Line.Left, Layer.Bottom, now._type, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(now._time + 0.0625, Line.MiddleLeft, Layer.Bottom, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.Left:
                                        if (!noteTemp.Any(x => x != now && x._time == now._time && x._lineIndex == Line.MiddleLeft))
                                        {
                                            now._lineIndex = Line.MiddleLeft;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Top, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleLeft, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpLeft:
                                        if (!noteTemp.Any(x => x != now && x._time == now._time && x._lineLayer == Layer.Middle))
                                        {
                                            now._lineLayer = Layer.Middle;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Top, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.UpRight:
                                        if (!noteTemp.Any(x => x != now && x._time == now._time && x._lineLayer == Layer.Middle))
                                        {
                                            now._lineLayer = Layer.Middle;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Top, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Middle, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleRight, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.MiddleLeft:
                                switch (now._cutDirection)
                                {
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(now._time + 0.0625, Line.MiddleLeft, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleRight, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpLeft:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpRight:
                                        if (!noteTemp.Any(x => x != now && x._time == now._time && x._lineIndex == Line.Left && x._lineLayer == Layer.Middle))
                                        {
                                            now._lineIndex = Line.Left;
                                            now._lineLayer = Layer.Middle;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleLeft, Layer.Top, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Middle, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.MiddleRight:
                                switch (now._cutDirection)
                                {
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(now._time + 0.0625, Line.MiddleRight, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleLeft, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.UpLeft:
                                        if (!noteTemp.Any(x => x != now && x._time == now._time && x._lineIndex == Line.Right && x._lineLayer == Layer.Middle))
                                        {
                                            now._lineIndex = Line.Right;
                                            now._lineLayer = Layer.Middle;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleRight, Layer.Top, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.UpRight:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Left, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Middle, now._type, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.Right:
                                switch (now._cutDirection)
                                {
                                    case CutDirection.Up:
                                        if (!noteTemp.Any(x => x != now && x._time == now._time && x._lineLayer == Layer.Bottom))
                                        {
                                            now._lineLayer = Layer.Bottom;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Middle, now._type, CutDirection.Any));
                                            newNote.Add(new _Notes(now._time + 0.0625, Line.Right, Layer.Top, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Middle, now._type, CutDirection.Any));
                                        if (now._type == NoteType.Blue)
                                        {
                                            newNote.Add(new _Notes(now._time + 0.0625, Line.Right, Layer.Bottom, now._type, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(now._time + 0.0625, Line.MiddleRight, Layer.Bottom, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleRight, Layer.Top, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.Right:
                                        if (!noteTemp.Any(x => x != now && x._time == now._time && x._lineIndex == Line.MiddleRight))
                                        {
                                            now._lineIndex = Line.MiddleRight;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Top, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.UpLeft:
                                        if (!noteTemp.Any(x => x != now && x._time == now._time && x._lineLayer == Layer.Middle))
                                        {
                                            now._lineLayer = Layer.Middle;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Top, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.UpRight:
                                        if (!noteTemp.Any(x => x != now && x._time == now._time && x._lineLayer == Layer.Middle))
                                        {
                                            now._lineLayer = Layer.Middle;
                                            newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Top, now._type, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.MiddleLeft, Layer.Bottom, now._type, CutDirection.Any));
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(now._time + 0.03125, Line.Right, Layer.Middle, now._type, CutDirection.Any));
                                        break;
                                }
                                break;
                        }
                        break;
                }

                lastNote = now;
            }

            noteTemp.RemoveAll(item => toRemove.Contains(item));
            newNote.AddRange(noteTemp);
            List<_Notes> sorted = newNote.OrderBy(o => o._time).ToList();

            for(int i = 0; i < sorted.Count(); i++)
            {
                if (sorted.Any(x => x != sorted[i] && x._lineIndex == sorted[i]._lineIndex && x._lineLayer == sorted[i]._lineLayer && x._time == sorted[i]._time))
                {
                    sorted.Remove(sorted[i]);
                    i--;
                }
            }

            return sorted;
        }
    }
}
