namespace Hexon.MvcTrig
{
    public static class JQueryExtensions
    {
        /// <summary>
        /// Ĳ�o�@�� DOM �ƥ�
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static TriggerContext RaiseEvent(this TriggerContext trigger, string selector, string eventName, object data)
        {
            trigger.Add("event", new FireEventPack
            {
                selector = selector,
                eventName = eventName,
                data = data
            });

            return trigger;
        }

        internal class FireEventPack
        {
            public string selector { get; set; }

            public string eventName { get; set; }

            public object data { get; set; }
        }
    }
}