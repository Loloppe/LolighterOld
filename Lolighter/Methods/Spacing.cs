using Lolighter.Items;
using Newtonsoft.Json.Linq;
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

            // Create the new list
            List<_Notes> newNotes = new List<_Notes>();

            // Add spacing here
            if (red.Count > 0)
            {
                red = AddSpacing(red, spacing);
                newNotes.AddRange(red);
            }
            if (blue.Count > 0)
            {
                blue = AddSpacing(blue, spacing);
                newNotes.AddRange(blue);
            }
            if (bomb.Count > 0)
            {
                newNotes.AddRange(bomb);
            }

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

                // Faster or equal to 1/10, Check for CutDirection
                if (now._time - previous._time <= 0.1 && (previous._cutDirection == CutDirection.Any || now._cutDirection == CutDirection.Any))
                {
                    if (start == -1)
                    {
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
                    // Fix order
                    List<_Notes> temp = new List<_Notes>();
                    for (int j = 0; j < count; j++)
                    {
                        temp.Add(noteTemp[start + j]);
                    }
                    
                    temp = CheckOrder(temp);

                    // Replace with the fixed order
                    for (int j = 0; j < count; j++)
                    {
                        noteTemp[start + j] = temp[j];
                    }

                    if(noteTemp[start]._cutDirection != 8)
                    {
                        // For each note in the slider
                        for (int j = 0; j < count; j++)
                        {
                            // Add spacing to each
                            noteTemp[start + j]._time = noteTemp[start]._time + (spacing * j);
                        }
                    }

                    start = -1;
                }

                previous = noteTemp[i];
            }

            return noteTemp;
        }

        public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
            return list;
        }

        public static List<_Notes> CheckOrder(List<_Notes> notes)
        {
            // Analyse the sliders and fix the order
            int count = 0;
            // Find the arrow
            foreach(var note in notes)
            {
                if(note._cutDirection != 8)
                {
                    notes = Swap(notes, 0, count).ToList();
                    break;
                }

                count++;
            }

            // Here, we try to find a note close enough
            for (int i = 0; i < notes.Count() - 1; i++)
            {
                if (notes[i]._lineIndex == notes[i + 1]._lineIndex && (notes[i]._lineLayer == notes[i + 1]._lineLayer + 1 || notes[i]._lineLayer == notes[i + 1]._lineLayer - 1))
                {
                    // Do nothing
                }
                else if (notes[i]._lineLayer == notes[i + 1]._lineLayer && (notes[i]._lineIndex == notes[i + 1]._lineIndex + 1 || notes[i]._lineIndex == notes[i + 1]._lineIndex - 1))
                {
                    // Do nothing
                }
                else if (notes[i]._lineIndex == notes[i + 1]._lineIndex - 1 && notes[i]._lineLayer == notes[i + 1]._lineLayer - 1)
                {
                    // Do nothing
                }
                else if (notes[i]._lineIndex == notes[i + 1]._lineIndex + 1 && notes[i]._lineLayer == notes[i + 1]._lineLayer + 1)
                {
                    // Do nothing
                }
                else if (notes[i]._lineIndex == notes[i + 1]._lineIndex - 1 && notes[i]._lineLayer == notes[i + 1]._lineLayer + 1)
                {
                    // Do nothing
                }
                else if (notes[i]._lineIndex == notes[i + 1]._lineIndex + 1 && notes[i]._lineLayer == notes[i + 1]._lineLayer - 1)
                {
                    // Do nothing
                }
                else
                {
                    // Not linked
                    for (int j = 0; j < notes.Count() - 1; j++)
                    {
                        if (notes[i]._lineIndex == notes[j]._lineIndex && (notes[i]._lineLayer == notes[j]._lineLayer + 1 || notes[i]._lineLayer == notes[j]._lineLayer - 1))
                        {
                            notes = Swap(notes, i, j).ToList();
                        }
                        else if (notes[i]._lineLayer == notes[j]._lineLayer && (notes[i]._lineIndex == notes[j]._lineIndex + 1 || notes[i]._lineIndex == notes[j]._lineIndex - 1))
                        {
                            notes = Swap(notes, i, j).ToList();
                        }
                        else if (notes[i]._lineIndex == notes[j]._lineIndex - 1 && notes[i]._lineLayer == notes[j]._lineLayer - 1)
                        {
                            notes = Swap(notes, i, j).ToList();
                        }
                        else if (notes[i]._lineIndex == notes[j]._lineIndex + 1 && notes[i]._lineLayer == notes[j]._lineLayer + 1)
                        {
                            notes = Swap(notes, i, j).ToList();
                        }
                        else if (notes[i]._lineIndex == notes[j]._lineIndex - 1 && notes[i]._lineLayer == notes[j]._lineLayer + 1)
                        {
                            notes = Swap(notes, i, j).ToList();
                        }
                        else if (notes[i]._lineIndex == notes[j]._lineIndex + 1 && notes[i]._lineLayer == notes[j]._lineLayer - 1)
                        {
                            notes = Swap(notes, i, j).ToList();
                        }
                        // Now linked
                    }
                }
            }

            return notes;
        }
    }
}
