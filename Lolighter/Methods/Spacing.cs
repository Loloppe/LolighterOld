using Lolighter.Items;
using System.Collections.Generic;
using System.Linq;
using static Lolighter.Items.Enum;

namespace Lolighter.Methods
{
    static class Spacing
    {
        static public List<_Notes> Space(List<_Notes> noteTemp, double spacing)
        {
            // Order by time
            noteTemp = noteTemp.OrderBy(o => o._time).ToList();

            // Separate type
            List<_Notes> red = new List<_Notes>();
            List<_Notes> blue = new List<_Notes>();
            List<_Notes> bomb = new List<_Notes>();

            foreach (var note in noteTemp)
            {
                if(note._type == NoteType.Red)
                {
                    red.Add(note);
                }
                else if(note._type == NoteType.Blue)
                {
                    blue.Add(note);
                }
                else if(note._type == NoteType.Mine)
                {
                    bomb.Add(note);
                }
            }

            // Add spacing here
            red = AddSpacing(red, spacing);
            blue = AddSpacing(blue, spacing);

            // Create the new list
            List<_Notes> newNotes = new List<_Notes>();
            newNotes.AddRange(red);
            newNotes.AddRange(blue);
            newNotes.AddRange(bomb);

            // Order by time
            newNotes = newNotes.OrderBy(o => o._time).ToList();

            return newNotes;
        }

        public static List<_Notes> AddSpacing(List<_Notes> noteTemp, double spacing)
        {
            // Number of notes in the slider
            int count = 0;
            // Where the slider start
            int start = -1;
            // Notes
            _Notes now;
            _Notes previous = noteTemp[0];

            for (int i = 1; i < noteTemp.Count(); i++)
            {
                now = noteTemp[i];

                // Faster or equal to 1/10, Check for CutDirection and Position
                if (now._time - previous._time <= 0.1 && now._cutDirection == CutDirection.Any && VerifyPosition(previous, now))
                {
                    if (start == -1)
                    {
                        // We get the starting position here
                        start = i - 1;
                        count = 2;
                    }
                    else
                    {
                        // Add a note to the counter
                        count++;
                    }
                }
                // Modify the slider
                else if (start != -1)
                {
                    // For each note in the slider
                    for (int j = 0; j < count; j++)
                    {
                        // Add spacing to each
                        noteTemp[start + j]._time = noteTemp[start]._time + (spacing * j);
                    }

                    start = -1;
                }

                previous = noteTemp[i];
            }

            return noteTemp;
        }

        public static bool VerifyPosition(_Notes before, _Notes after)
        {
            if(before._lineIndex == after._lineIndex && (before._lineLayer == after._lineLayer + 1 || before._lineLayer == after._lineLayer - 1))
            {
                return true;
            }
            else if (before._lineLayer == after._lineLayer && (before._lineIndex == after._lineIndex + 1 || before._lineIndex == after._lineIndex - 1))
            {
                return true;
            }
            else if(before._lineIndex == after._lineIndex - 1 && before._lineLayer == after._lineLayer - 1)
            {
                return true;
            }
            else if (before._lineIndex == after._lineIndex + 1 && before._lineLayer == after._lineLayer + 1)
            {
                return true;
            }
            else if (before._lineIndex == after._lineIndex - 1 && before._lineLayer == after._lineLayer + 1)
            {
                return true;
            }
            else if (before._lineIndex == after._lineIndex + 1 && before._lineLayer == after._lineLayer - 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
