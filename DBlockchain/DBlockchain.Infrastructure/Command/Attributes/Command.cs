using DBlockchain.Infrastructure.Command.Enums;
using System;

namespace DBlockchain.Infrastructure.Commands.Attributes
{
    public class Command : Attribute
    {
        public Command(string name, string template, CommandType type)
        {
            this.Name = name;
            this.Template = template;
            this.Type = type;
        }

        public string Name { get; set; }

        public string Template { get; set; }

        public CommandType Type { get; set; }
    }
}
