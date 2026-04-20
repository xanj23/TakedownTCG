namespace TakedownTCG.cli.Views.Shared
{
    public sealed class MenuOption<TAction>
    {
        public string Label { get; }
        public TAction Action { get; }

        public MenuOption(string label, TAction action)
        {
            Label = label;
            Action = action;
        }

        public override string ToString()
        {
            return Label;
        }
    }
}
