using Osu2Saber.Model.Algorithm;
using Osu2Saber.Model.Json;
using System.Collections.Generic;

namespace Osu2Saber.Model
{
    public class Pattern
    {
        public static int patternID = -1;

        #region Stream

        public static List<Note> SelectStream(int id)
        {
            List<Note> Stream = new List<Note>();

            if(id != 999)
            {
                patternID = id;
            }
            else
            {
                do
                {
                    id = ConvertAlgorithm.RandNumber(1, 18);
                    
                } while (id == patternID);

                patternID = id;
            }

            Layer lay = 0;

            switch (patternID)
            {
                case 0: //Generic stream
                    Note note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
                case 1: //Piano roll
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
                case 2: //Z pattern
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Right, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
                case 3: //Middle-cross pattern
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
                case 4: //Inward to outward
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Right, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Right, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
                case 5: //Piano roll mixed with middle notes
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Right, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
                case 6: //W pattern
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
                case 7: //2-2
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
                case 8: //Red spin
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
                case 9: //Blue spin
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
                case 10: //Generic one-lane stream (got to randomise Line after Red Up).
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
                case 11: //Big cross-spin
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
                case 12: //Triple triple
                    int i;
                    if (ConvertAlgorithm.RandNumber(0, 2) == 0)
                    {
                        i = 0;
                    }
                    else
                    {
                        i = 1;
                    }
                    note = new Note(0, Line.Right - i, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight - i, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft - i, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.Right - i, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight - i, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft - i, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Right - i, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight - i, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
                case 13: //Outward to inward
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
                case 14: //4 - 2 - 2 - 3
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
                case 15: //3 - 1 - 2 - 3
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
                case 16: //Reversed piano roll
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Right, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Right, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
                case 17: //Inverted piano roll
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.Right, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
                case 18: //Inverted blue spin
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Right, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
                case 19: //Inverted red spin
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    break;
            }

            return Stream;
        }

        #endregion

        #region randomstream

        public static List<Note> SelectRandomStream(int id)
        {
            List<Note> random = new List<Note>();

            int cut = 1;
            int color = 1;
            int count = 0;
            int line = 0;

            for(int i = 0; i < 4; i++)
            {
                line = ConvertAlgorithm.RandNumber(0, 4);
                if(line == 0 && color == 1)
                {
                    line = ConvertAlgorithm.RandNumber(0, 4);
                }
                else if(line == 3 && color == 0)
                {
                    line = ConvertAlgorithm.RandNumber(0, 4);
                }
                Note note = new Note(0, (Line)line, Layer.Bottom, (NoteType)color, (CutDirection)cut);
                random.Add(note);
                if (cut == 1 && count == 1)
                {
                    cut = 0;
                }
                if(color == 1)
                {
                    color = 0;
                }
                else
                {
                    color = 1;
                }
                count++;
            }

            return random;
        }

        #endregion

        #region Complex

        public static List<Note> SelectComplex(int id)
        {
            List<Note> Complex = new List<Note>();

            if (id != 999)
            {
                patternID = id;
            }
            else
            {
                do
                {
                    id = ConvertAlgorithm.RandNumber(1, 11);
                } while (id == patternID);

                patternID = id;
            }

            switch (patternID)
            {
                case 1: // Wave
                    Note note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    break;
                case 2: // Spiral
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    break;
                case 3: // On top
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    break;
                case 4: // Woosh
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    break;
                case 5: //Generic middle stream
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    break;
                case 6: //Diagonal jump stream
                    CutDirection c1, c2, c3, c4;
                    if (ConvertAlgorithm.RandNumber(0, 2) == 1)
                    {
                        c1 = CutDirection.DownLeft;
                        c2 = CutDirection.DownRight;
                        c3 = CutDirection.UpRight;
                        c4 = CutDirection.UpLeft;
                    }
                    else
                    {
                        c1 = CutDirection.Down;
                        c2 = CutDirection.Down;
                        c3 = CutDirection.Up;
                        c4 = CutDirection.Up;
                    }
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, c1);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, c2);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Top, NoteType.Blue, c3);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Top, NoteType.Red, c4);
                    Complex.Add(note);
                    break;
                case 7: //Extended diagonal jump stream + middle
                    if (ConvertAlgorithm.RandNumber(0, 2) == 1)
                    {
                        c1 = CutDirection.DownLeft;
                        c2 = CutDirection.DownRight;
                        c3 = CutDirection.UpRight;
                        c4 = CutDirection.UpLeft;
                    }
                    else
                    {
                        c1 = CutDirection.Down;
                        c2 = CutDirection.Down;
                        c3 = CutDirection.Up;
                        c4 = CutDirection.Up;
                    }
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, c1);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, c2);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, c3);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, c4);
                    Complex.Add(note);
                    break;
                case 8: // Generic side stream
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    break;
                case 9: // Generic right stream
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    break;
                case 10: // Generic left stream
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    break;
            }

            return Complex;
        }

        #endregion

        public static List<Note> GetNewPattern(string name, int id)
        {
            if(name == "Random")
            {
                int i;
                i = ConvertAlgorithm.RandNumber(0, 2);
                switch(i)
                {
                    case 0:
                        name = "Stream";
                        break;
                    case 1:
                        name = "Complex";
                        break;
                }
            }

            switch(name)
            {
                case "RandomStream":
                    return new List<Note>(SelectRandomStream(id));
                case "Stream":
                    return new List<Note>(SelectStream(id));
                case "Complex":
                    return new List<Note>(SelectComplex(id));
            }

            return null;
        }

        public static int GetPatternID()
        {
            return patternID;
        }
    }
}
