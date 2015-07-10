namespace Hexon.MvcTrig.Fancybox
{
    public static class FancyboxExtensions
    {
        /// <summary>
        /// 關閉目前視窗中的 fancybox
        /// </summary>
        /// <returns></returns>
        public static TriggerHelper FancyClose(this TriggerHelper trigger)
        {
            trigger.Add("fancyClose", true, lowPiority: true);

            return trigger;
        }

        /// <summary>
        /// 開啟一個 fancybox
        /// </summary>
        /// <param name="url"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static TriggerHelper FancyOpen(this TriggerHelper trigger, string url, int? width = null, int? height = null)
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
        /// 開啟一個 fancybox
        /// </summary>
        /// <param name="url"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static TriggerHelper FancyOpen(this TriggerHelper trigger, string url, string width = null, string height = null)
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
        /// 變動 fancybox 規格
        /// </summary>
        /// <param name="url"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static TriggerHelper FancyResize(this TriggerHelper trigger, string width = null, string height = null)
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