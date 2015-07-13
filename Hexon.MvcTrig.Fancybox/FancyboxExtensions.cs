namespace Hexon.MvcTrig.Fancybox
{
    public static class FancyboxExtensions
    {
        /// <summary>
        /// �����ثe�������� fancybox
        /// </summary>
        /// <returns></returns>
        public static TriggerContext FancyClose(this TriggerContext trigger)
        {
            trigger.Add("fancyClose", true, lowPiority: true);

            return trigger;
        }

        /// <summary>
        /// �}�Ҥ@�� fancybox
        /// </summary>
        /// <param name="url"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static TriggerContext FancyOpen(this TriggerContext trigger, string url, int? width = null, int? height = null)
        {
            trigger.Add("fancyOpen", new FancyboxPack
            {
                url = url,
                width = width.ToString(),
                height = height.ToString()
            },
            lowPiority: true);

            return trigger;
        }

        /// <summary>
        /// �}�Ҥ@�� fancybox
        /// </summary>
        /// <param name="url"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static TriggerContext FancyOpen(this TriggerContext trigger, string url, string width = null, string height = null)
        {
            trigger.Add("fancyOpen", new FancyboxPack
            {
                url = url,
                width = width,
                height = height
            },
            lowPiority: true);

            return trigger;
        }

        /// <summary>
        /// �ܰ� fancybox �W��
        /// </summary>
        /// <param name="url"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static TriggerContext FancyResize(this TriggerContext trigger, string width = null, string height = null)
        {
            trigger.Add("fancyResize", new FancyboxPack
            {
                width = width,
                height = height
            },
            lowPiority: false);

            return trigger;
        }

    }
}