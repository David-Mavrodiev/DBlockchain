using System;

namespace DBlockchain.Infrastructure.Commands.Attributes
{
    public class Command : Attribute
    {
        public Command(string name, string template)
        {
            this.name = name;
            this.template = template;
        }

        public string name { get; set; }

        public string template { get; set; }
    }
}
