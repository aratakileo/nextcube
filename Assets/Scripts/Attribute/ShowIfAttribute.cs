using System;

namespace Attribute
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ShowIfAttribute : System.Attribute
    {
        public enum ConditionOperator
        {
            And,
            Or
        }

        public string[] Conditions { get; private set; }
        public ConditionOperator conditionOperator { get; private set; }
        public bool Reversed { get; protected set; }

        public ShowIfAttribute(string condition)
        {
            conditionOperator = ConditionOperator.And;
            Conditions = new string[1] { condition };
        }

        public ShowIfAttribute(ConditionOperator conditionOperator, params string[] conditions)
        {
            this.conditionOperator = conditionOperator;
            Conditions = conditions;
        }
    }
}
