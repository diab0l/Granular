using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Host.Wpf.Render
{
    internal class CommandQueue
    {
        private List<Action> commands;

        public CommandQueue()
        {
            commands = new List<Action>();
        }

        public void Add(Action command)
        {
            commands.Add(command);
        }

        public void Remove(Action command)
        {
            commands.Remove(command);
        }

        public void Execute()
        {
            foreach (Action command in commands)
            {
                command();
            }

            commands.Clear();
        }
    }
}
