using System;

namespace Attribute
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SliderAttribute : System.Attribute
    {
        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }

        public SliderAttribute(float minValue, float maxValue)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        public SliderAttribute(int minValue, int maxValue)
        {
            this.MaxValue = minValue;
            this.MaxValue = maxValue;
        }
    }
}
