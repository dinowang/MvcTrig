namespace Hexon.MvcTrig
{
    public static class MessageExtensions
    {
        /// <summary>
        /// 顯示通知
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static TriggerContext Notify(this TriggerContext trigger, object message, string title = null, MessageType type = MessageType.Info, int timeout = 2000)
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
        /// 顯示訊息視窗
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static TriggerContext Message(this TriggerContext trigger, object message, string title = null, MessageType type = MessageType.Info, int timeout = 2000)
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