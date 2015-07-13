namespace Hexon.MvcTrig
{
    internal class TriggerCommand
    {
        public TriggerScope Scope { get; private set; }

        public string Trigger { get; private set; }

        public object Data { get; private set; }

        public TriggerCommand(TriggerScope scope, string name, object data)
        {
            Scope = scope;
            Trigger = name;
            Data = data;
        }
    }
}