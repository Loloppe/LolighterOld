using Lolighter.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using static Lolighter.Items.Enum;

namespace Lolighter.Methods
{
    static class Loloppe
    {
        static public List<_Notes> LoloppeGen(List<_Notes> noteTemp)
        {
            Random random = new Random();
            List<_Notes> newNote = new List<_Notes>();
            _Notes n = new _Notes(0, 0, 0, 0, 0);

            for (int i = 0; i < noteTemp.Count(); i++) //For each note -> Big Pepega
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

                switch (n._lineLayer) //Process each note using their layer, lane and cut direction manually.
                {
                    case Layer.Bottom:
                        switch (n._lineIndex)
                        {
                            case Line.Left:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Bottom, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Bottom, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Middle, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Middle, n._type, n._cutDirection));
                                        break;
                                }
                                break;
                            case Line.MiddleLeft:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Bottom, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Bottom, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.UpRight:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Middle, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Middle, n._type, n._cutDirection));
                                        break;
                                }
                                break;
                            case Line.MiddleRight:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Bottom, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Bottom, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.UpLeft:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Middle, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Middle, n._type, n._cutDirection));
                                        break;
                                }
                                break;
                            case Line.Right:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Bottom, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Bottom, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Middle, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Middle, n._type, n._cutDirection));
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
                                    case CutDirection.Left:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Left, Layer.Top, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Left, Layer.Bottom, n._type, n._cutDirection));
                                        }
                                        break;
                                    case CutDirection.Right:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Left, Layer.Top, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Left, Layer.Bottom, n._type, n._cutDirection));
                                        }
                                        break;
                                    case CutDirection.UpLeft:
                                        newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Top, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.UpRight:
                                        newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Bottom, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Bottom, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Top, n._type, n._cutDirection));
                                        break;
                                }
                                break;
                            case Line.MiddleLeft:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Middle, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Middle, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Left:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Top, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Bottom, n._type, n._cutDirection));
                                        }
                                        break;
                                    case CutDirection.Right:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Top, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Bottom, n._type, n._cutDirection));
                                        }
                                        break;
                                    case CutDirection.UpLeft:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Left, Layer.Bottom, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Top, n._type, n._cutDirection));
                                        }
                                        break;
                                    case CutDirection.UpRight:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Left, Layer.Top, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Bottom, n._type, n._cutDirection));
                                        }
                                        break;
                                    case CutDirection.DownLeft:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Left, Layer.Top, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Bottom, n._type, n._cutDirection));
                                        }
                                        break;
                                    case CutDirection.DownRight:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Left, Layer.Bottom, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Top, n._type, n._cutDirection));
                                        }
                                        break;
                                }
                                break;
                            case Line.MiddleRight:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Middle, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Middle, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Left:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Top, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Bottom, n._type, n._cutDirection));
                                        }
                                        break;
                                    case CutDirection.Right:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Top, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Bottom, n._type, n._cutDirection));
                                        }
                                        break;
                                    case CutDirection.UpLeft:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Bottom, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Right, Layer.Top, n._type, n._cutDirection));
                                        }
                                        break;
                                    case CutDirection.UpRight:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Top, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Right, Layer.Bottom, n._type, n._cutDirection));
                                        }
                                        break;
                                    case CutDirection.DownLeft:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Top, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Right, Layer.Bottom, n._type, n._cutDirection));
                                        }
                                        break;
                                    case CutDirection.DownRight:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Bottom, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Right, Layer.Top, n._type, n._cutDirection));
                                        }
                                        break;
                                }
                                break;
                            case Line.Right:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Left:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Right, Layer.Top, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Right, Layer.Bottom, n._type, n._cutDirection));
                                        }
                                        break;
                                    case CutDirection.Right:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Right, Layer.Top, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Right, Layer.Bottom, n._type, n._cutDirection));
                                        }
                                        break;
                                    case CutDirection.UpLeft:
                                        newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Bottom, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.UpRight:
                                        newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Top, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Top, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Bottom, n._type, n._cutDirection));
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
                                        newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Top, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Top, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Middle, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Middle, n._type, n._cutDirection));
                                        break;
                                }
                                break;
                            case Line.MiddleLeft:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Top, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Top, n._type, n._cutDirection));
                                        break;
                                }
                                break;
                            case Line.MiddleRight:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Top, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Top, n._type, n._cutDirection));
                                        break;
                                }
                                break;
                            case Line.Right:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Top, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Top, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Middle, n._type, n._cutDirection));
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Middle, n._type, n._cutDirection));
                                        break;
                                }
                                break;
                        }
                        break;
                }
            }

            newNote = newNote.OrderBy(o => o._time).ToList();

            //Here we remove notes that ended up stacked because of the silly algorithm.
            for (int i = newNote.Count() - 1; i > -1; i--) //For each note in reverse-order
            {
                foreach (var note in noteTemp) //For each note
                {
                    if (newNote[i]._time - note._time < 0.125 && newNote[i]._time - note._time > 0 && newNote[i]._lineLayer == note._lineLayer && newNote[i]._lineIndex == note._lineIndex)
                    {
                        newNote.Remove(newNote[i]);
                        break;
                    }
                    else if (newNote[i]._time == note._time && newNote[i]._lineLayer == note._lineLayer && newNote[i]._lineIndex == note._lineIndex)
                    {
                        newNote.Remove(newNote[i]);
                        break;
                    }
                }
            }

            newNote.AddRange(noteTemp);
            List<_Notes> sorted = newNote.OrderBy(o => o._time).ToList();

            return sorted;
        }
    }
}
