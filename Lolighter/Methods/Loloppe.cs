using Lolighter.Items;
using System;
using System.Collections.Generic;
using System.Linq;

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

                if (n._type == 3) //If bomb, skip
                {
                    continue;
                }
                else if (n._cutDirection == 8) //If dot, skip
                {
                    continue;
                }

                switch (n._lineLayer) //Process each note using their layer, lane and cut direction manually.
                {
                    case 0:
                        switch (n._lineIndex)
                        {
                            case 0:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time, 1, 0, n._type, n._cutDirection));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 1, 0, n._type, n._cutDirection));
                                        break;
                                    case 2:
                                        newNote.Add(new _Notes(n._time, 0, 1, n._type, n._cutDirection));
                                        break;
                                    case 3:
                                        newNote.Add(new _Notes(n._time, 0, 1, n._type, n._cutDirection));
                                        break;
                                }
                                break;
                            case 1:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time, 0, 0, n._type, n._cutDirection));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 0, 0, n._type, n._cutDirection));
                                        break;
                                    case 5:
                                        newNote.Add(new _Notes(n._time, 0, 1, n._type, n._cutDirection));
                                        break;
                                    case 6:
                                        newNote.Add(new _Notes(n._time, 0, 1, n._type, n._cutDirection));
                                        break;
                                }
                                break;
                            case 2:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time, 3, 0, n._type, n._cutDirection));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 3, 0, n._type, n._cutDirection));
                                        break;
                                    case 4:
                                        newNote.Add(new _Notes(n._time, 3, 1, n._type, n._cutDirection));
                                        break;
                                    case 7:
                                        newNote.Add(new _Notes(n._time, 3, 1, n._type, n._cutDirection));
                                        break;
                                }
                                break;
                            case 3:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time, 2, 0, n._type, n._cutDirection));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 2, 0, n._type, n._cutDirection));
                                        break;
                                    case 2:
                                        newNote.Add(new _Notes(n._time, 3, 1, n._type, n._cutDirection));
                                        break;
                                    case 3:
                                        newNote.Add(new _Notes(n._time, 3, 1, n._type, n._cutDirection));
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
                                            newNote.Add(new _Notes(n._time, 0, 2, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 0, 0, n._type, n._cutDirection));
                                        }
                                        break;
                                    case 3:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 0, 2, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 0, 0, n._type, n._cutDirection));
                                        }
                                        break;
                                    case 4:
                                        newNote.Add(new _Notes(n._time, 1, 2, n._type, n._cutDirection));
                                        break;
                                    case 5:
                                        newNote.Add(new _Notes(n._time, 1, 0, n._type, n._cutDirection));
                                        break;
                                    case 6:
                                        newNote.Add(new _Notes(n._time, 1, 0, n._type, n._cutDirection));
                                        break;
                                    case 7:
                                        newNote.Add(new _Notes(n._time, 1, 2, n._type, n._cutDirection));
                                        break;
                                }
                                break;
                            case 1:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time, 0, 1, n._type, n._cutDirection));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 0, 1, n._type, n._cutDirection));
                                        break;
                                    case 2:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 1, 2, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 1, 0, n._type, n._cutDirection));
                                        }
                                        break;
                                    case 3:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 1, 2, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 1, 0, n._type, n._cutDirection));
                                        }
                                        break;
                                    case 4:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 0, 0, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 2, 2, n._type, n._cutDirection));
                                        }
                                        break;
                                    case 5:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 0, 2, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 2, 0, n._type, n._cutDirection));
                                        }
                                        break;
                                    case 6:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 0, 2, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 2, 0, n._type, n._cutDirection));
                                        }
                                        break;
                                    case 7:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 0, 0, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 2, 2, n._type, n._cutDirection));
                                        }
                                        break;
                                }
                                break;
                            case 2:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time, 3, 1, n._type, n._cutDirection));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 3, 1, n._type, n._cutDirection));
                                        break;
                                    case 2:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 2, 2, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 2, 0, n._type, n._cutDirection));
                                        }
                                        break;
                                    case 3:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 2, 2, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 2, 0, n._type, n._cutDirection));
                                        }
                                        break;
                                    case 4:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 1, 0, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 3, 2, n._type, n._cutDirection));
                                        }
                                        break;
                                    case 5:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 1, 2, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 3, 0, n._type, n._cutDirection));
                                        }
                                        break;
                                    case 6:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 1, 2, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 3, 0, n._type, n._cutDirection));
                                        }
                                        break;
                                    case 7:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 1, 0, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 3, 2, n._type, n._cutDirection));
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
                                            newNote.Add(new _Notes(n._time, 3, 2, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 3, 0, n._type, n._cutDirection));
                                        }
                                        break;
                                    case 3:
                                        if (random.Next(1) == 0)
                                        {
                                            newNote.Add(new _Notes(n._time, 3, 2, n._type, n._cutDirection));
                                        }
                                        else
                                        {
                                            newNote.Add(new _Notes(n._time, 3, 0, n._type, n._cutDirection));
                                        }
                                        break;
                                    case 4:
                                        newNote.Add(new _Notes(n._time, 2, 0, n._type, n._cutDirection));
                                        break;
                                    case 5:
                                        newNote.Add(new _Notes(n._time, 2, 2, n._type, n._cutDirection));
                                        break;
                                    case 6:
                                        newNote.Add(new _Notes(n._time, 2, 2, n._type, n._cutDirection));
                                        break;
                                    case 7:
                                        newNote.Add(new _Notes(n._time, 2, 0, n._type, n._cutDirection));
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
                                        newNote.Add(new _Notes(n._time, 1, 2, n._type, n._cutDirection));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 1, 2, n._type, n._cutDirection));
                                        break;
                                    case 2:
                                        newNote.Add(new _Notes(n._time, 0, 1, n._type, n._cutDirection));
                                        break;
                                    case 3:
                                        newNote.Add(new _Notes(n._time, 0, 1, n._type, n._cutDirection));
                                        break;
                                }
                                break;
                            case 1:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time, 0, 2, n._type, n._cutDirection));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 0, 2, n._type, n._cutDirection));
                                        break;
                                }
                                break;
                            case 2:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time, 3, 2, n._type, n._cutDirection));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 3, 2, n._type, n._cutDirection));
                                        break;
                                }
                                break;
                            case 3:
                                switch (n._cutDirection)
                                {
                                    case 0:
                                        newNote.Add(new _Notes(n._time, 2, 2, n._type, n._cutDirection));
                                        break;
                                    case 1:
                                        newNote.Add(new _Notes(n._time, 2, 2, n._type, n._cutDirection));
                                        break;
                                    case 2:
                                        newNote.Add(new _Notes(n._time, 3, 1, n._type, n._cutDirection));
                                        break;
                                    case 3:
                                        newNote.Add(new _Notes(n._time, 3, 1, n._type, n._cutDirection));
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
