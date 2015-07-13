namespace Hexon.MvcTrig.Bootstrap
{
    public static class BootstrapExtensions
    {
        public static TriggerContext ModalOpen(this TriggerContext trigger, string selector = "#modal")
        {
            trigger.Add("modalOpen", selector);

            return trigger;
        }

        public static TriggerContext ModalClose(this TriggerContext trigger)
        {
            trigger.Add("modalClose", true, lowPiority: true);

            return trigger;
        }
    }
}