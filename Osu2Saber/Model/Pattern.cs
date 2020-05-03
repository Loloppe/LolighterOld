using Osu2Saber.Model.Algorithm;
using Osu2Saber.Model.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osu2Saber.Model
{
    public class Pattern
    {
        public static int patternID = -1;

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
                    id = ConvertAlgorithm.RandNumber(1, 17);
                    
                } while (id == patternID || id == 10);

                patternID = id;
            }

            Layer lay = 0;

            //if(ConvertAlgorithm.RandNumber(0, 2) == 0)
            //{
            //    lay = (Layer)2;
            //}

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
                    if (ConvertAlgorithm.RandNumber(0, 2) == 0)
                    {
                        lay = Layer.Bottom;
                    }
                    else
                    {
                        lay = Layer.Middle;
                    }
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Right, lay, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, lay, NoteType.Red, CutDirection.Up);
                    Stream.Add(note);
                    if (ConvertAlgorithm.RandNumber(0, 2) == 0)
                    {
                        lay = Layer.Bottom;
                    }
                    else
                    {
                        lay = Layer.Middle;
                    }
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
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Stream.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Up);
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
            }

            return Stream;
        }

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

        public static List<Note> SelectJump(int id)
        {
            List<Note> Jump = new List<Note>();

            if (id != 999)
            {
                patternID = id;
            }
            else
            {
                do
                {
                    id = ConvertAlgorithm.RandNumber(1, 10);
                } while (id == patternID);

                patternID = id;
            }

            switch (patternID)
            {
                case 1: //Generic middle jump stream
                    Note note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Jump.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Jump.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Jump.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Red, CutDirection.Up);
                    Jump.Add(note);
                    break;
                case 2: //Extended diagonal jump stream
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Jump.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.DownRight);
                    Jump.Add(note);
                    note = new Note(0, Line.Right, Layer.Top, NoteType.Blue, CutDirection.UpRight);
                    Jump.Add(note);
                    note = new Note(0, Line.Left, Layer.Top, NoteType.Red, CutDirection.UpLeft);
                    Jump.Add(note);
                    break;
                case 3: //Diagonal jump stream
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Jump.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Jump.Add(note);
                    note = new Note(0, Line.Right, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Jump.Add(note);
                    note = new Note(0, Line.Left, Layer.Top, NoteType.Red, CutDirection.Up);
                    Jump.Add(note);
                    break;
                case 4: //Extended diagonal jump stream + middle
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Jump.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.DownRight);
                    Jump.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.UpRight);
                    Jump.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.UpLeft);
                    Jump.Add(note);
                    break;
                case 5: //Extended diagonal jump stream + middle
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Jump.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.DownRight);
                    Jump.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.UpRight);
                    Jump.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.UpLeft);
                    Jump.Add(note);
                    break;
                case 6: //Diagonal side into up
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.DownRight);
                    Jump.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.DownLeft);
                    Jump.Add(note);
                    note = new Note(0, Line.Right, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Jump.Add(note);
                    note = new Note(0, Line.Left, Layer.Top, NoteType.Red, CutDirection.Up);
                    Jump.Add(note);
                    break;
                case 7: //Side into up
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Jump.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Jump.Add(note);
                    note = new Note(0, Line.Right, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Jump.Add(note);
                    note = new Note(0, Line.Left, Layer.Top, NoteType.Red, CutDirection.Up);
                    Jump.Add(note);
                    break;
                case 8: //Both going right
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Jump.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.DownRight);
                    Jump.Add(note);
                    note = new Note(0, Line.Right, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Jump.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.Up);
                    Jump.Add(note);
                    break;
                case 9: //Both going left
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Jump.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Jump.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Jump.Add(note);
                    note = new Note(0, Line.Left, Layer.Top, NoteType.Red, CutDirection.Up);
                    Jump.Add(note);
                    break;
            }

            return Jump;
        }

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
                    id = ConvertAlgorithm.RandNumber(0, 17);
                } while (id == patternID || id == 11 || id == 4);

                patternID = id;
            }

            switch (patternID)
            {
                case 0: //Complexification by fraies
                    Note note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Red, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Top, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Top, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Top, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Red, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    break;
                case 1: //Aimer with chelly (EGOIST) by Saut
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
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
                case 2: //FIRST - AKI AKANE by Saut
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    break;
                case 3: //FIRST - AKI AKANE by Saut
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Red, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Top, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Top, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    break;
                case 4: //FIRST - AKI AKANE by Saut
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    break;
                case 5: //FIRST - AKI AKANE by Saut
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
                    //Not sure if more = better
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    break;
                case 6: //FIRST - AKI AKANE by Saut
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
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
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Top, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Top, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Red, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Top, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    break;
                case 7: //FIRST - AKI AKANE by Saut
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    break;
                case 8: //Kodoku no Kakurenbo - Himeringo by Saut
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
                case 9: //Kodoku no Kakurenbo - Himeringo by Saut
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    break;
                case 10: //Kodoku no Kakurenbo - Himeringo by Saut
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Blue, CutDirection.UpLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    break;
                case 11: //Kodoku no Kakurenbo - Himeringo by Saut
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Middle, NoteType.Blue, CutDirection.UpLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Middle, NoteType.Blue, CutDirection.UpLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Blue, CutDirection.UpLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    break;
                case 12: //If You Can't Hang - Sleeping With Sirens by Saut
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    break;
                case 13: //If You Can't Hang - Sleeping With Sirens by Saut
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
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
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Blue, CutDirection.UpLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    break;
                case 14: //If You Can't Hang - Sleeping With Sirens by Saut
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    break;
                case 15: //Your voice so... feat. Such - PSYQUI by Saut
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.UpRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Red, CutDirection.UpRight);
                    Complex.Add(note);
                    break;
                case 16: //CANDYYYLAND (Pa's Lam System Remix) - tofubeats by Saut
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Down);
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
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Top, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Blue, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.UpLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Top, NoteType.Blue, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Middle, NoteType.Red, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.UpLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Middle, NoteType.Red, CutDirection.UpLeft);
                    Complex.Add(note);
                    break;
            }

            /* Backup list
             *      Aimer with chelly (EGOIST) by Saut/slightly modified. Break at last up red.
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Right, Layer.Bottom, NoteType.Blue, CutDirection.DownRight);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Red, CutDirection.Down);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.Left, Layer.Bottom, NoteType.Blue, CutDirection.DownLeft);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleLeft, Layer.Bottom, NoteType.Red, CutDirection.Up);
                    Complex.Add(note);
                    note = new Note(0, Line.MiddleRight, Layer.Bottom, NoteType.Blue, CutDirection.Up);
                    Complex.Add(note);
                    break;
             */

            return Complex;
        }

        public static List<Note> GetNewPattern(string name, int id)
        {
            if(name == "Random")
            {
                int i;
                i = ConvertAlgorithm.RandNumber(0, 3);
                if(i == 0)
                {
                    name = "Stream";
                }
                else if(i == 1)
                {
                    name = "Complex";
                }
                else if(i == 2)
                {
                    name = "Jump";
                }
            }

            if(name == "RandomStream")
            {
                return new List<Note>(SelectRandomStream(id));
            }

            if (name == "Stream")
            {
                return new List<Note>(SelectStream(id));
            }
            else if(name == "Complex")
            {
                return new List<Note>(SelectComplex(id));
            }
            else if (name == "Jump")
            {
                return new List<Note>(SelectJump(id));
            }

            return null;
        }

        public static int GetPatternID()
        {
            return patternID;
        }
    }
}
