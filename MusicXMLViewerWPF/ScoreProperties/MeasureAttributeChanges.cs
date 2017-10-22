using System.Collections.Generic;

namespace MusicXMLScore.ScoreProperties
{
    public class MeasureAttributeChanges<T> where T : IMeasureAttribute
    {
        private List<AttributeChange<T>> _attributeChanges;

        public List<AttributeChange<T>> AttributeChanges
        {
            get { return _attributeChanges; }

            set { _attributeChanges = value; }
        }

        protected MeasureAttributeChanges()
        {
            _attributeChanges = new List<AttributeChange<T>>();
        }

        /// <summary>
        /// Adds time signature change to measure staff at cursor position(measure fraction)
        /// </summary>
        /// <param name="attributeChange"></param>
        protected void Add(AttributeChange<T> attributeChange)
        {
            _attributeChanges.Add(attributeChange);
        }
    }

    public class AttributeChangesDictionary<T, TK> : Dictionary<string, T>
        where T : MeasureAttributeChanges<TK>
        where TK : IMeasureAttribute
    {
        /// <summary>
        /// Adds Generic Attribute Changes to dictionary or Appends if (measureId)Key found
        /// </summary>
        /// <param name="measureId"></param>
        /// <param name="attributeChanges"></param>
        public new void Add(string measureId, T attributeChanges)
        {
            T attributes;
            if (TryGetValue(measureId, out attributes))
            {
                attributes.AttributeChanges.AddRange(attributeChanges.AttributeChanges);
            }
            else
            {
                base.Add(measureId, attributeChanges);
            }
        }
    }
}