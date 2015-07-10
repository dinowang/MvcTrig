namespace Hexon.MvcTrig.Bootstrap
{
    public static class BootstrapExtensions
    {
        public static TriggerHelper ModelClose(this TriggerHelper trigger)
        {
            trigger.Add("modelClose", true, lowPiority: true);

            return trigger;
        }
    }
}