using Lolighter.Items;
using System.Collections.Generic;
using System.Linq;

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

                if (n._type == 3) //If bomb, skip
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
                    case 0:
                        n._lineLayer = 0;
                        break;
                    case 1:
                        n._lineLayer = 2;
                        break;
                    case 2:
                        n._lineIndex = 3;
                        break;
                    case 3:
                        n._lineIndex = 0;
                        break;
                    case 4:
                        n._lineLayer = 0;
                        break;
                    case 5:
                        n._lineLayer = 0;
                        break;
                    case 6:
                        n._lineLayer = 2;
                        break;
                    case 7:
                        n._lineLayer = 2;
                        break;
                    case 8:
                        break;
                }
            }

            return noteTemp;
        }
    }
}
