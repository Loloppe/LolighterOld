using Lolighter.Items;
using System.Collections.Generic;
using System.Linq;
using static Lolighter.Items.Enum;

namespace Lolighter.Methods
{
    static class Inverted
    {
        static public List<_Notes> MakeInverted(List<_Notes> noteTemp, double Limiter, bool IsLimited)
        {
            _Notes n;
            bool found;

            for (int i = noteTemp.Count() - 1; i > -1; i--) //For each note in reverse-order
            {
                n = noteTemp[i];

                if (n._type == NoteType.Mine) //If Bomb, skip
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
                    if (((n._time - temp._time < Limiter && n._time - temp._time > 0) || (temp._time - n._time < Limiter && temp._time - n._time > 0)) && temp._type == n._type)
                    {
                        found = true;
                        break;
                    }
                    else if (temp._time == n._time && temp._type == n._type && n != temp)
                    {
                        found = true;
                        break;
                    }
                    else if (temp._time == n._time && temp._type != n._type && n != temp && (temp._lineIndex == n._lineIndex || temp._lineLayer == n._lineLayer))
                    {
                        found = true;
                        break;
                    }
                }

                if (found) //If found, then skip
                {
                    continue;
                }

                switch (n._cutDirection) //Based on the cut direction, change the layer of the note.
                {
                    case CutDirection.Up:
                        n._lineLayer = Layer.Bottom;
                        break;
                    case CutDirection.Down:
                        n._lineLayer = Layer.Top;
                        break;
                    case CutDirection.Left:
                        n._lineIndex = Line.Right;
                        break;
                    case CutDirection.Right:
                        n._lineIndex = Line.Left;
                        break;
                    case CutDirection.UpLeft:
                        n._lineLayer = Layer.Bottom;
                        break;
                    case CutDirection.UpRight:
                        n._lineLayer = Layer.Bottom;
                        break;
                    case CutDirection.DownLeft:
                        n._lineLayer = Layer.Top;
                        break;
                    case CutDirection.DownRight:
                        n._lineLayer = Layer.Top;
                        break;
                    case CutDirection.Any:
                        break;
                }
            }

            return noteTemp;
        }
    }
}
