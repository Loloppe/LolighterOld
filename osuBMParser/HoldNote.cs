namespace osuBMParser
{
    public class HoldNote : HitObject
    {

        #region fields
        public int EndTime { get; set; }
        #endregion

        #region constructors
        public HoldNote() { }

        public HoldNote(Vector2 position, int time, int hitSound, int endTime, int[] addition, bool isNewCombo) : base(position, time, hitSound, addition, isNewCombo)
        {
            this.EndTime = endTime;
        }
        #endregion

    }
}