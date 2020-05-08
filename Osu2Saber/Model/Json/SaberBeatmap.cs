using Osu2Saber.Model.Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osu2Saber.Model.Json
{
    public class SaberBeatmap
    {
        public string origin;
        public string _version;
        public int _beatsPerMinute;
        public int _beatsPerBar;
        public double _noteJumpSpeed;
        public int _shuffle;
        public double _shufflePeriod;
        public List<Event> _events;
        public List<Note> _notes;
        public List<Obstacle> _obstacles;
    }

    public class Event
    {
        public double _time;
        public int _type;
        public int _value;

        public Event(double time, EventType type, EventLightValue value)
        {
            _time = time; _type = (int)type; _value = (int)value;
        }

        public Event(double time, EventType type, EventRotationValue value)
        {
            _time = time; _type = (int)type; _value = (int)value;
        }

        public Event(double time, EventType type, int value)
        {
            _time = time; _type = (int)type; _value = value;
        }
    }

    public class Note
    {
        public double _time;
        public int _lineIndex;
        public int _lineLayer;
        public int _type;
        public int _cutDirection;

        public Note(Note note)
        {
            _time = note._time;
            _lineIndex = note._lineIndex;
            _lineLayer = note._lineLayer;
            _type = note._type;
            _cutDirection = note._cutDirection;
        }

        public Note(_Notes note)
        {
            _time = note._time;
            _lineIndex = note._lineIndex;
            _lineLayer = note._lineLayer;
            _type = note._type;
            _cutDirection = note._cutDirection;
        }

        public Note(double time, Line line, Layer layer, NoteType type, CutDirection cutDirection)
        {
            _time = time; _lineIndex = (int)line; _lineLayer = (int)layer; _type = (int)type;
            _cutDirection = (int)cutDirection;
        }

        public Note(double time, int line, int layer, NoteType type, CutDirection cutDirection)
        {
            _time = time; _lineIndex = line; _lineLayer = layer; _type = (int)type; _cutDirection = (int)cutDirection;
        }

        public Note(double time, int line, int layer, int type, int cutDirection)
        {
            _time = time; _lineIndex = line; _lineLayer = layer; _type = type; _cutDirection = cutDirection;
        }
    }

    public class Obstacle
    {
        public double _time;
        public int _lineIndex;
        public int _type;
        public double _duration;
        public int _width;
        
        public Obstacle(double time, Line line, ObstacleType type, double duration, int width)
        {
            _time = time; _lineIndex = (int)line; _type = (int)type; _duration = duration; _width = width;
        }
    }

    public enum Line
    {
        Left = 0,
        MiddleLeft,
        MiddleRight,
        Right,
        MaxNum
    }

    public enum Layer
    {
        Bottom = 0,
        Middle,
        Top,
        MaxNum
    }

    public enum CutDirection
    {
        Up = 0,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight,
        Any
    }

    public enum NoteType
    {
        Red = 0,
        Blue,
        Mine = 3
    }

    public enum ObstacleType
    {
        Wall = 0,
        Ceiling,
    }

    public enum EventType
    {
        LightBackTopLasers = 0,
        LightTrackRingNeons,
        LightLeftLasers,
        LightRightLasers,
        LightBottomBackSideLasers,

        RotationAllTrackRings = 8,
        RotationSmallTrackRings,

        RotatingLeftLasers = 12,
        RotatingRightLasers
    }

    public enum EventLightValue
    {
        Off = 0,
        BlueOn,
        BlueFlashStay,
        BlueFlashFade,
        RedOn = 5,
        RedFlashStay,
        RedFlashFade
    }

    public enum EventRotationValue
    {
        Stop = 0,
        Speed1,
        Speed2,
        Speed3,
        Speed4,
        Speed5,
        Speed6,
        Speed7,
        Speed8
    }
}
