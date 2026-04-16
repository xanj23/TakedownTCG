using System;
using System.Collections.Generic;

namespace TakedownTCG.cli.Views.Shared
{
    public sealed class MenuDefinition<TAction>
        where TAction : struct, Enum
    {
        public string Name { get; }
        public IReadOnlyList<MenuOption<TAction>> Options { get; }
        public TAction? BackAction { get; }
        public TAction QuitAction { get; }

        public MenuDefinition(
            string name,
            IReadOnlyList<MenuOption<TAction>> options,
            TAction? backAction,
            TAction quitAction)
        {
            Name = name;
            Options = options;
            BackAction = backAction;
            QuitAction = quitAction;
        }
    }
}
