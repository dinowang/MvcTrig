namespace Hexon.MvcTrig
{
    public static class MessageExtensions
    {
        /// <summary>
        /// ��ܳq��
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static TriggerHelper Notify(this TriggerHelper trigger, object message, string title = null, MessageType type = MessageType.Info, int timeout = 2000)
        {
            trigger.Add("notify", new MessagePack
            {
                title = title,
                message = message.ToString(),
                type = type,
                timeout = timeout
            });

            return trigger;
        }

        /// <summary>
        /// ��ܰT������
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static TriggerHelper Message(this TriggerHelper trigger, object message, string title = null, MessageType type = MessageType.Info, int timeout = 2000)
        {
            trigger.Add("message", new MessagePack
            {
                title = title,
                message = message.ToString(),
                type = type,
                timeout = timeout
            });

            return trigger;
        }

    }
}