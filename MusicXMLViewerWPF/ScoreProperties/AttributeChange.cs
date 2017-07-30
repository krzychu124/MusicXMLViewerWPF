namespace MusicXMLScore.ScoreProperties
{
    public class AttributeChange<T> where T : IMeasureAttribute
    {
        public string StaffNumber { get; set; }
        public int TimeFraction { get; set; }
        public T AttributeEntity { get; set; }

        protected AttributeChange(string staffNumber, int timeFraction, T attributeEntity)
        {
            StaffNumber = staffNumber;
            TimeFraction = timeFraction;
            AttributeEntity = attributeEntity;
        }
    }
}