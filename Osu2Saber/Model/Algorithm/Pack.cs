using Osu2Saber.Model.Json;

namespace Osu2Saber.Model.Algorithm
{
    class Pack
    {
        public _Pattern[] _pattern { get; set; }
    }

    public class _Pattern
    {
        public string name { get; set; }
        public _Notes[] _notes { get; set; }
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
}
