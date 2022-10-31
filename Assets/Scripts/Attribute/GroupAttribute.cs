namespace Attribute
{
    public abstract class GroupAttribute : System.Attribute
    {
        public string Name { get; private set; }

        public GroupAttribute(string name)
        {
            this.Name = name;
        }
    }
}
