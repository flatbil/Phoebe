using System.Collections.Generic;

namespace PhoebeBot
{
    /// <summary>
    /// Class for storing conversation state. 
    /// </summary>
    public class PhoebeState : Dictionary<string, object>
    {
            private const string NameKey = "name";
            private const string AgeKey = "age";

            public PhoebeState()
            {
                this[NameKey] = null;
                this[AgeKey] = 0;
            }

            public string Name
            {
                get { return (string)this[NameKey]; }
                set { this[NameKey] = value; }
            }

            public int Age
            {
                get { return (int)this[AgeKey]; }
                set { this[AgeKey] = value; }
            }
        
    }
}
