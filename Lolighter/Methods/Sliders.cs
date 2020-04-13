using Lolighter.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                if (n._type == 3) //If bomb, skip
                {
                    continue;
                }
                else if(n._cutDirection == 8) //If dot, skip
                {
                    continue;
                }

                found = false;

                foreach (_Notes temp in noteTemp) //For each note
                {   
                    if(n._time == temp._time && n._type == temp._type && n._cutDirection == temp._cutDirection && !IsLimited)
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
                    else if (n._time == temp._time && n._type != temp._type && n._cutDirection == temp._cutDirection && (n._cutDirection == 4 || n._cutDirection == 5))
                    {
                        found = true;
                        break;
                    }
                    else if (n._time == temp._time && temp._type != n._type && ((temp._lineIndex == 0 && n._lineIndex == 1) || (temp._lineIndex == 3 && n._lineIndex == 2)))
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
                    case 0:
                        switch (n._lineIndex)
                        {
                            case 0:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 1, n._type, 8));
                                        if (n._type == 0)
                                        {
                                            newNote.Add(new _Notes(n._time + 0.0625, 0, 2, n._type, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time + 0.0625, 1, 2, n._type, 8));
                                        }
                                        break;
                                    case 1:
                                        n._lineLayer = 2;
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 1, n._type, 8));
                                        newNote.Add(new _Notes(n._time + 0.0625, 0, 0, n._type, 8));
                                        break;
                                    case 2:
                                        n._lineIndex = 1;
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 0, n._type, 8));
                                        break;
                                    case 3:
                                        newNote.Add(new _Notes(n._time + 0.03125, 1, 0, n._type, 8));
                                        break;
                                    case 4:
                                        n._lineIndex = 1;
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 1, n._type, 8));
                                        break;
                                    case 5:
                                        newNote.Add(new _Notes(n._time + 0.03125, 2, 2, n._type, 8));
                                        break;
                                    case 6:
                                        n._lineIndex = 1;
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 0, n._type, 8));
                                        break;
                                    case 7:
                                        newNote.Add(new _Notes(n._time + 0.03125, 1, 0, n._type, 8));
                                        break;
                                }
                                break;
                            case 1:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time + 0.0625, 1, 2, n._type, 8));
                                        break;
                                    case 2:
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 0, n._type, 8));
                                        break;
                                    case 3:
                                        newNote.Add(new _Notes(n._time + 0.03125, 2, 0, n._type, 8));
                                        newNote.Add(new _Notes(n._time + 0.0625, 3, 0, n._type, 8));
                                        break;
                                    case 4:
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 1, n._type, 8));
                                        newNote.Add(new _Notes(n._time + 0.0625, 0, 2, n._type, 8));
                                        break;
                                    case 5:
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 2, n._type, 8));
                                        break;
                                    case 6:
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 0, n._type, 8));
                                        break;
                                    case 7:
                                        n._lineLayer = 1;
                                        n._lineIndex = 0;
                                        newNote.Add(new _Notes(n._time + 0.03125, 1, 0, n._type, 8));
                                        break;
                                }
                                break;
                            case 2:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time + 0.0625, 2, 2, n._type, 8));
                                        break;
                                    case 2:
                                        newNote.Add(new _Notes(n._time + 0.03125, 1, 0, n._type, 8));
                                        newNote.Add(new _Notes(n._time + 0.0625, 0, 0, n._type, 8));
                                        break;
                                    case 3:
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 0, n._type, 8));
                                        break;
                                    case 4:
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 2, n._type, 8));
                                        break;
                                    case 5:
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 1, n._type, 8));
                                        newNote.Add(new _Notes(n._time + 0.0625, 3, 2, n._type, 8));
                                        break;
                                    case 6:
                                        n._lineLayer = 1;
                                        n._lineIndex = 3;
                                        newNote.Add(new _Notes(n._time + 0.03125, 2, 0, n._type, 8));
                                        break;
                                    case 7:
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 0, n._type, 8));
                                        break;
                                }
                                break;
                            case 3:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 1, n._type, 8));
                                        if (n._type == 0)
                                        {
                                            newNote.Add(new _Notes(n._time + 0.0625, 2, 2, n._type, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time + 0.0625, 3, 2, n._type, 8));
                                        }
                                        break;
                                    case 1:
                                        n._lineLayer = 2;
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 1, n._type, 8));
                                        newNote.Add(new _Notes(n._time + 0.0625, 3, 0, n._type, 8));
                                        break;
                                    case 2:
                                        newNote.Add(new _Notes(n._time + 0.03125, 2, 0, n._type, 8));
                                        break;
                                    case 3:
                                        n._lineIndex = 2;
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 0, n._type, 8));
                                        break;
                                    case 4:
                                        newNote.Add(new _Notes(n._time + 0.03125, 1, 2, n._type, 8));
                                        break;
                                    case 5:
                                        n._lineIndex = 2;
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 1, n._type, 8));
                                        break;
                                    case 6:
                                        newNote.Add(new _Notes(n._time + 0.03125, 2, 0, n._type, 8));
                                        break;
                                    case 7:
                                        n._lineIndex = 2;
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 0, n._type, 8));
                                        break;
                                }
                                break;
                        }
                        break;
                    case 1:
                        switch (n._lineIndex)
                        {
                            case 0:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 2, n._type, 8));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 0, n._type, 8));
                                        break;
                                    case 5:
                                        newNote.Add(new _Notes(n._time + 0.03125, 1, 2, n._type, 8));
                                        newNote.Add(new _Notes(n._time + 0.0625, 2, 2, n._type, 8));
                                        break;
                                    case 6:
                                        break;
                                    case 7:
                                        newNote.Add(new _Notes(n._time + 0.03125, 1, 0, n._type, 8));
                                        newNote.Add(new _Notes(n._time + 0.0625, 2, 0, n._type, 8));
                                        break;
                                }
                                break;
                            case 1:
                                switch (n._cutDirection)
                                {
                                    case 2:
                                        n._lineIndex = 0;
                                        break;
                                    case 3:
                                        n._lineIndex = 3;
                                        break;
                                }
                                break;
                            case 2:
                                switch (n._cutDirection)
                                {
                                    case 2:
                                        n._lineIndex = 0;
                                        break;
                                    case 3:
                                        n._lineIndex = 3;
                                        break;
                                }
                                break;
                            case 3:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 2, n._type, 8));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 0, n._type, 8));
                                        break;
                                    case 4:
                                        newNote.Add(new _Notes(n._time + 0.03125, 2, 2, n._type, 8));
                                        newNote.Add(new _Notes(n._time + 0.0625, 1, 2, n._type, 8));
                                        break;
                                    case 6:
                                        newNote.Add(new _Notes(n._time + 0.03125, 2, 0, n._type, 8));
                                        newNote.Add(new _Notes(n._time + 0.0625, 1, 0, n._type, 8));
                                        break;
                                }
                                break;
                        }
                        break;
                    case 2:
                        switch (n._lineIndex)
                        {
                            case 0:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        n._lineLayer = 0;
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 1, n._type, 8));
                                        newNote.Add(new _Notes(n._time + 0.0625, 0, 2, n._type, 8));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 1, n._type, 8));
                                        if (n._type == 0)
                                        {
                                            newNote.Add(new _Notes(n._time + 0.0625, 0, 0, n._type, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time + 0.0625, 1, 0, n._type, 8));
                                        }
                                        break;
                                    case 2:
                                        n._lineIndex = 1;
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 2, n._type, 8));
                                        break;
                                    case 3:
                                        newNote.Add(new _Notes(n._time + 0.03125, 1, 2, n._type, 8));
                                        break;
                                    case 4:
                                        n._lineLayer = 1;
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 2, n._type, 8));
                                        break;
                                    case 5:
                                        n._lineLayer = 1;
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 2, n._type, 8));
                                        break;
                                    case 6:
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 1, n._type, 8));
                                        break;
                                    case 7:
                                        newNote.Add(new _Notes(n._time + 0.03125, 2, 0, n._type, 8));
                                        break;
                                }
                                break;
                            case 1:
                                switch (n._cutDirection)
                                {
                                    case 1:
                                        newNote.Add(new _Notes(n._time + 0.0625, 1, 0, n._type, 8));
                                        break;
                                    case 2:
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 2, n._type, 8));
                                        break;
                                    case 3:
                                        newNote.Add(new _Notes(n._time + 0.03125, 2, 2, n._type, 8));
                                        break;
                                    case 4:
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 2, n._type, 8));
                                        break;
                                    case 5:
                                        n._lineIndex = 0;
                                        n._lineLayer = 1;
                                        newNote.Add(new _Notes(n._time + 0.03125, 1, 2, n._type, 8));
                                        break;
                                    case 6:
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 1, n._type, 8));
                                        break;
                                    case 7:
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 0, n._type, 8));
                                        break;
                                }
                                break;
                            case 2:
                                switch (n._cutDirection)
                                {
                                    case 1:
                                        newNote.Add(new _Notes(n._time + 0.0625, 2, 0, n._type, 8));
                                        break;
                                    case 2:
                                        newNote.Add(new _Notes(n._time + 0.03125, 1, 2, n._type, 8));
                                        break;
                                    case 3:
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 2, n._type, 8));
                                        break;
                                    case 4:
                                        n._lineIndex = 3;
                                        n._lineLayer = 1;
                                        newNote.Add(new _Notes(n._time + 0.03125, 2, 2, n._type, 8));
                                        break;
                                    case 5:
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 2, n._type, 8));
                                        break;
                                    case 6:
                                        newNote.Add(new _Notes(n._time + 0.03125, 0, 0, n._type, 8));
                                        break;
                                    case 7:
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 1, n._type, 8));
                                        break;
                                }
                                break;
                            case 3:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        n._lineLayer = 0;
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 1, n._type, 8));
                                        newNote.Add(new _Notes(n._time + 0.0625, 3, 2, n._type, 8));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 1, n._type, 8));
                                        if (n._type == 1)
                                        {
                                            newNote.Add(new _Notes(n._time + 0.0625, 3, 0, n._type, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time + 0.0625, 2, 0, n._type, 8));
                                        }
                                        break;
                                    case 2:
                                        newNote.Add(new _Notes(n._time + 0.03125, 2, 2, n._type, 8));
                                        break;
                                    case 3:
                                        n._lineIndex = 2;
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 2, n._type, 8));
                                        break;
                                    case 4:
                                        n._lineLayer = 1;
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 2, n._type, 8));
                                        break;
                                    case 5:
                                        n._lineLayer = 1;
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 2, n._type, 8));
                                        break;
                                    case 6:
                                        newNote.Add(new _Notes(n._time + 0.03125, 1, 0, n._type, 8));
                                        break;
                                    case 7:
                                        newNote.Add(new _Notes(n._time + 0.03125, 3, 1, n._type, 8));
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
                if (sorted[i]._cutDirection == 8) //If it's a dot
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
