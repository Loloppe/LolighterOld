namespace Lolighter.Items
{
    public class Rootobject
    {
        public string _version { get; set; }
        public _Customdata _customData { get; set; } = new _Customdata();
        public _Events[] _events { get; set; }
        public _Notes[] _notes { get; set; }
        public _Obstacles[] _obstacles { get; set; }
        public object[] _waypoints { get; set; } = new object[0];
    }

    public class _Customdata
    {
        public double _time { get; set; } = 0;
        public _Bpmchanges[] _BPMChanges { get; set; } = new _Bpmchanges[0];
        public _Bookmarks[] _bookmarks { get; set; } = new _Bookmarks[0];
    }

    public class _Bookmarks
    {
        public double _time { get; set; }
        public string _name { get; set; }
    }

    public class _Bpmchanges
    {
        public double _BPM { get; set; }
        public double _time { get; set; }
        public double _beatsPerBar { get; set; }
        public double _metronomeOffset { get; set; }
    }

    public class _Events
    {
        public _Events(double v1, int v2, int v3)
        {
            _time = v1;
            _type = v2;
            _value = v3;
        }

        public double _time { get; set; }
        public int _type { get; set; }
        public int _value { get; set; }
    }

    public class _Notes
    {
        public _Notes(double v1, int v2, int v3, int type, int v4)
        {
            _time = v1;
            _lineIndex = v2;
            _lineLayer = v3;
            _type = type;
            _cutDirection = v4;
        }

        public double _time { get; set; }
        public int _lineIndex { get; set; }
        public int _lineLayer { get; set; }
        public int _type { get; set; }
        public int _cutDirection { get; set; }
    }

    public class _Obstacles
    {
        public double _time { get; set; }
        public int _lineIndex { get; set; }
        public int _type { get; set; }
        public double _duration { get; set; }
        public int _width { get; set; }
    }
}