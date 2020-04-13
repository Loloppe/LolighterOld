using Lolighter.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                if (n._type == 3) //Skip bomb
                {
                    continue;
                }
                else if (n._cutDirection == 8) //Skip dot
                {
                    continue;
                }

                switch (n._lineLayer) //Each layer, index and cut direction are handled manually.
                {
                    case 0:
                        switch (n._lineIndex)
                        {
                            case 0:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time, 1, 0, 3, 8));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 1, 0, 3, 8));
                                        break;
                                    case 2:
                                        newNote.Add(new _Notes(n._time, 0, 1, 3, 8));
                                        break;
                                    case 3:
                                        newNote.Add(new _Notes(n._time, 0, 1, 3, 8));
                                        break;
                                }
                                break;
                            case 1:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time, 0, 0, 3, 8));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 0, 0, 3, 8));
                                        break;
                                    case 5:
                                        newNote.Add(new _Notes(n._time, 0, 1, 3, 8));
                                        break;
                                    case 6:
                                        newNote.Add(new _Notes(n._time, 0, 1, 3, 8));
                                        break;
                                }
                                break;
                            case 2:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time, 3, 0, 3, 8));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 3, 0, 3, 8));
                                        break;
                                    case 4:
                                        newNote.Add(new _Notes(n._time, 3, 1, 3, 8));
                                        break;
                                    case 7:
                                        newNote.Add(new _Notes(n._time, 3, 1, 3, 8));
                                        break;
                                }
                                break;
                            case 3:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time, 2, 0, 3, 8));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 2, 0, 3, 8));
                                        break;
                                    case 2:
                                        newNote.Add(new _Notes(n._time, 3, 1, 3, 8));
                                        break;
                                    case 3:
                                        newNote.Add(new _Notes(n._time, 3, 1, 3, 8));
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
                                    case 2:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 0, 2, 3, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 0, 0, 3, 8));
                                        }
                                        break;
                                    case 3:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 0, 2, 3, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 0, 0, 3, 8));
                                        }
                                        break;
                                    case 4:
                                        newNote.Add(new _Notes(n._time, 1, 2, 3, 8));
                                        break;
                                    case 5:
                                        newNote.Add(new _Notes(n._time, 1, 0, 3, 8));
                                        break;
                                    case 6:
                                        newNote.Add(new _Notes(n._time, 1, 0, 3, 8));
                                        break;
                                    case 7:
                                        newNote.Add(new _Notes(n._time, 1, 2, 3, 8));
                                        break;
                                }
                                break;
                            case 1:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time, 0, 1, 3, 8));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 0, 1, 3, 8));
                                        break;
                                    case 2:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 1, 2, 3, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 1, 0, 3, 8));
                                        }
                                        break;
                                    case 3:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 1, 2, 3, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 1, 0, 3, 8));
                                        }
                                        break;
                                    case 4:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 0, 0, 3, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 2, 2, 3, 8));
                                        }
                                        break;
                                    case 5:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 0, 2, 3, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 2, 0, 3, 8));
                                        }
                                        break;
                                    case 6:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 0, 2, 3, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 2, 0, 3, 8));
                                        }
                                        break;
                                    case 7:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 0, 0, 3, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 2, 2, 3, 8));
                                        }
                                        break;
                                }
                                break;
                            case 2:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time, 3, 1, 3, 8));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 3, 1, 3, 8));
                                        break;
                                    case 2:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 2, 2, 3, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 2, 0, 3, 8));
                                        }
                                        break;
                                    case 3:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 2, 2, 3, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 2, 0, 3, 8));
                                        }
                                        break;
                                    case 4:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 1, 0, 3, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 3, 2, 3, 8));
                                        }
                                        break;
                                    case 5:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 1, 2, 3, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 3, 0, 3, 8));
                                        }
                                        break;
                                    case 6:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 1, 2, 3, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 3, 0, 3, 8));
                                        }
                                        break;
                                    case 7:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 1, 0, 3, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 3, 2, 3, 8));
                                        }
                                        break;
                                }
                                break;
                            case 3:
                                switch (n._cutDirection)
                                {
                                    case 2:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 3, 2, 3, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 3, 0, 3, 8));
                                        }
                                        break;
                                    case 3:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 3, 2, 3, 8));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 3, 0, 3, 8));
                                        }
                                        break;
                                    case 4:
                                        newNote.Add(new _Notes(n._time, 2, 0, 3, 8));
                                        break;
                                    case 5:
                                        newNote.Add(new _Notes(n._time, 2, 2, 3, 8));
                                        break;
                                    case 6:
                                        newNote.Add(new _Notes(n._time, 2, 2, 3, 8));
                                        break;
                                    case 7:
                                        newNote.Add(new _Notes(n._time, 2, 0, 3, 8));
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
                                        newNote.Add(new _Notes(n._time, 1, 2, 3, 8));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 1, 2, 3, 8));
                                        break;
                                    case 2:
                                        newNote.Add(new _Notes(n._time, 0, 1, 3, 8));
                                        break;
                                    case 3:
                                        newNote.Add(new _Notes(n._time, 0, 1, 3, 8));
                                        break;
                                }
                                break;
                            case 1:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time, 0, 2, 3, 8));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 0, 2, 3, 8));
                                        break;
                                }
                                break;
                            case 2:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time, 3, 2, 3, 8));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 3, 2, 3, 8));
                                        break;
                                }
                                break;
                            case 3:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time, 2, 2, 3, 8));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 2, 2, 3, 8));
                                        break;
                                    case 2:
                                        newNote.Add(new _Notes(n._time, 3, 1, 3, 8));
                                        break;
                                    case 3:
                                        newNote.Add(new _Notes(n._time, 3, 1, 3, 8));
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
                if (sorted[i]._type == 3)
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
