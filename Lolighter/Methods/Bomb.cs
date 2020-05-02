using Lolighter.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using static Lolighter.Items.Enum;

namespace Lolighter.Methods
{
    static class Bomb
    {
        public static List<_Notes> CreateBomb(List<_Notes> noteTemp)
        {
            Random random = new Random();
            List<_Notes> newNote = new List<_Notes>();
            _Notes n = new _Notes(0, 0, 0, 0, 0);

            for (int i = 0; i < noteTemp.Count(); i++) //For each note
            {
                n = noteTemp[i];

                if (n._type == NoteType.Mine) //Skip Bomb
                {
                    continue;
                }
                else if (n._cutDirection == CutDirection.Any) //Skip Any
                {
                    continue;
                }

                switch (n._lineLayer) //Each layer, index and cut direction are handled manually.
                {
                    case Layer.Bottom:
                        switch (n._lineIndex)
                        {
                            case Line.Left:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Middle, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Middle, NoteType.Mine, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.MiddleLeft:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.UpRight:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Middle, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Middle, NoteType.Mine, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.MiddleRight:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.UpLeft:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Middle, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Middle, NoteType.Mine, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.Right:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Middle, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Middle, NoteType.Mine, CutDirection.Any));
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
                                            newNote.Add(new _Notes(n._time, Line.Left, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Left, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.Right:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Left, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Left, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.UpLeft:
                                        newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.UpRight:
                                        newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.MiddleLeft:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Middle, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Middle, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Left:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.Right:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.UpLeft:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Left, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.UpRight:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Left, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.DownLeft:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Left, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.DownRight:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Left, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        }
                                        break;
                                }
                                break;
                            case Line.MiddleRight:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Middle, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Middle, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Left:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.Right:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.UpLeft:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Right, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.UpRight:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Right, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.DownLeft:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Right, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.DownRight:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Right, Layer.Top, NoteType.Mine, CutDirection.Any));
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
                                            newNote.Add(new _Notes(n._time, Line.Right, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Right, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.Right:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Right, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, Line.Right, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        }
                                        break;
                                    case CutDirection.UpLeft:
                                        newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Bottom, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.UpRight:
                                        newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.DownLeft:
                                        newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.DownRight:
                                        newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Bottom, NoteType.Mine, CutDirection.Any));
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
                                        newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.MiddleLeft, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Middle, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Middle, NoteType.Mine, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.MiddleLeft:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.Left, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.MiddleRight:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        break;
                                }
                                break;
                            case Line.Right:
                                switch (n._cutDirection)
                                {
                                    case CutDirection.Up:
                                        newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Down:
                                        newNote.Add(new _Notes(n._time, Line.MiddleRight, Layer.Top, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Left:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Middle, NoteType.Mine, CutDirection.Any));
                                        break;
                                    case CutDirection.Right:
                                        newNote.Add(new _Notes(n._time, Line.Right, Layer.Middle, NoteType.Mine, CutDirection.Any));
                                        break;
                                }
                                break;
                        }
                        break;
                }
            }

            newNote.AddRange(noteTemp);
            List<_Notes> sorted = newNote.OrderBy(o => o._time).ToList();

            for (int i = sorted.Count() - 5; i > 4; i--) //Dumb method to remove bomb that conflict with a note. (Hitbox issue)
            {
                if (sorted[i]._type == NoteType.Mine)
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
                }
            }

            return sorted;
        }
    }
}
