using DBlockchain.Infrastructure.Command.Enums;
using System;

namespace DBlockchain.Infrastructure.Commands.Attributes
{
    public class Command : Attribute
    {
        public Command(string name, string template, CommandType type, string description = "none")
        {
            this.Name = name;
            this.Template = template;
            this.Type = type;
            this.Description = description;
        }

        public string Name { get; set; }

        public string Template { get; set; }

        public string Description { get; set; }

        public CommandType Type { get; set; }
    }
}
