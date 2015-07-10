using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Hexon.MvcTrig
{
    public class TriggerHelper
    {
        #region Static

        public static bool HasTrigger
        {
            get
            {
                var trigger = HttpContext.Current.Items["TriggerHelper"] as TriggerHelper;

                if (trigger == null)
                {
                    trigger = HttpContext.Current.Session["TriggerHelper"] as TriggerHelper;
                }

                return trigger != null && trigger.Count > 0;
            }
        }

        public static TriggerHelper Current
        {
            get
            {
                var trigger = HttpContext.Current.Items["TriggerHelper"] as TriggerHelper;

                if (trigger == null)
                {
                    trigger = HttpContext.Current.Session["TriggerHelper"] as TriggerHelper;

                    if (trigger != null)
                    {
                        // Probely come from previous action (302)
                        HttpContext.Current.Session.Remove("TriggerHelper");
                        HttpContext.Current.Items["TriggerHelper"] = trigger = new TriggerHelper(HttpContext.Current, trigger);
                    }
                    else
                    {
                        HttpContext.Current.Items["TriggerHelper"] = trigger = new TriggerHelper(HttpContext.Current);
                    }
                }

                return trigger;
            }
        }

        #endregion

        #region Private members

        private HttpContext _context;

        private List<TriggerCommand> _commands = new List<TriggerCommand>();

        private List<TriggerCommand> _lowPiorityCommands = new List<TriggerCommand>();

        private TriggerScope _currentScope = TriggerScope.Self;

        private bool _forceAjaxTrigger = false;

        #endregion

        #region Inner classes

        enum TriggerScope
        {
            Self,
            Parent,
            Top
        }

        class TriggerCommand
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

        class MessagePack
        {
            public string title { get; set; }
            public string message { get; set; }
            public MessageType type { get; set; }
            public int timeout { get; set; }
        }


        #endregion

        #region Constructors

        private TriggerHelper(HttpContext context)
        {
            _context = context;
        }

        private TriggerHelper(HttpContext context, TriggerHelper copyFrom) 
        {
            _context = context;
            _commands = copyFrom._commands;
            _lowPiorityCommands = copyFrom._lowPiorityCommands;
        }

        #endregion

        public void Add(string name, object data, bool lowPiority = false)
        {
            var list = lowPiority 
                        ? _lowPiorityCommands 
                        : _commands;

            list.Add(new TriggerCommand(_currentScope, name, data));
        }

        /// <summary>
        /// 清除目前的所有 trigger 指令
        /// </summary>
        public void Clear()
        {
            _commands.Clear();
            _lowPiorityCommands.Clear();
        }

        public int Count
        { 
            get { return _commands.Count + _lowPiorityCommands.Count; }
        }

        /// <summary>
        /// 切換 Trigger 的操作範圍到父視窗
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        public TriggerHelper Parent(Action<TriggerHelper> call)
        {
            _currentScope = TriggerScope.Parent;
            call.Invoke(this);
            _currentScope = TriggerScope.Self;

            return this;
        }

        /// <summary>
        /// 切換 Trigger 的操作範圍到頂層視窗
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        public TriggerHelper Top(Action<TriggerHelper> call)
        {
            _currentScope = TriggerScope.Top;
            call.Invoke(this);
            _currentScope = TriggerScope.Self;

            return this;
        }


        ///// <summary>
        ///// 關閉父視窗的 fancyboxㄝ, 通常是 dialog 用來關閉自己用的
        ///// 這是一種簡化的寫法，等同於 Trigger.Parent(x => x.FancyClose())
        ///// </summary>
        ///// <returns></returns>
        //public TriggerHelper ParentFancyClose()
        //{
        //    _currentScope = TriggerScope.Parent;
        //    Add("fancyClose", true, lowPiority: true);

        //    _currentScope = TriggerScope.Self;

        //    return this;
        //}

        ///// <summary>
        ///// 關閉父視窗的 fancyboxㄝ, 通常是 dialog 用來關閉自己用的
        ///// 這是一種簡化的寫法，等同於 Trigger.Top(x => x.FancyClose())
        ///// </summary>
        ///// <returns></returns>
        //public TriggerHelper TopFancyClose()
        //{
        //    _currentScope = TriggerScope.Top;
        //    Add("fancyClose", true, lowPiority: true);

        //    _currentScope = TriggerScope.Self;

        //    return this;
        //}

        /// <summary>
        /// 變更網址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public TriggerHelper ChangeUrl(string url)
        {
            Add("changeUrl", url);

            return this;
        }

        /// <summary>
        /// 重新載入本頁
        /// </summary>
        /// <returns></returns>
        public TriggerHelper Reload()
        {
            Add("reload", null);

            return this;
        }

        /// <summary>
        /// 強迫採用 AJAX 機制 (HTTP header)
        /// </summary>
        /// <returns></returns>
        public TriggerHelper AsAjaxTrigger()
        {
            _forceAjaxTrigger = true;

            return this;
        }

        /// <summary>
        /// 將所有 trigger 命令送出
        /// </summary>
        public void Flush()
        {
            if (Count > 0)
            {
                var commands = new List<TriggerCommand>();
                commands.AddRange(_commands);
                commands.AddRange(_lowPiorityCommands);

                if (_forceAjaxTrigger || _context.Request.Headers["X-Requested-With"] != null)
                {
                    AjaxTrigger(commands);
                }
                else
                {
                    PageTrigger(commands);
                }

                Clear();
            }
        }

        /// <summary>
        /// Ajax Trigger 透過 HTTP header 傳遞，所以就算 ActionResult 是回傳 JSON 也沒問題
        /// </summary>
        /// <param name="commands"></param>
        private void AjaxTrigger(IEnumerable<TriggerCommand> commands)
        {
            var response = _context.Response;

            response.AddHeader("X-Triggers", commands.Count().ToString());
            var i = 0;

            foreach (var command in commands)
            {
                var data = CreateHttpHeaderSafeParameterContext(command);

                var pack = new
                {
                    scope = command.Scope, 
                    trigger = command.Trigger, 
                    data = data
                };
                var content = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    Converters = new[]
                    {
                        new HttpHeaderStringEncodeConverter()
                    }
                });
                //var content = _context.Server.UrlPathEncode(pack.ToJson());
                //var content = HttpUtility.UrlEncode(pack.ToJson());

                response.AddHeader("X-Trigger-" + (i++), content);
            }
        }

        private object CreateHttpHeaderSafeParameterContext(TriggerCommand command)
        {
            var data = command.Data;
            
            if (data != null)
            {
                var type = data.GetType();

                if (type.IsValueType)
                {
                }
                else if (type == typeof(string))
                {
                    data = HttpUtility.UrlPathEncode(data.ToString());
                }
                else
                {
                    var properties = type.GetProperties();
                    var values = new List<object>();
                    var ctor = type.GetConstructors().Where(x => !x.GetParameters().Any()).Any();

                    if (ctor)
                    {
                        data = Activator.CreateInstance(type);

                        foreach (var property in properties)
                        {
                            var value = property.GetValue(command.Data, null);

                            if (property.PropertyType == typeof (string) && value != null)
                            {
                                value = HttpUtility.UrlPathEncode(value.ToString());
                            }

                            property.SetValue(data, value, null);
                        }
                    }
                    else
                    {
                        foreach (var property in properties)
                        {
                            var value = property.GetValue(command.Data, null);

                            if (property.PropertyType == typeof (string) && value != null)
                            {
                                value = HttpUtility.UrlPathEncode(value.ToString());
                            }

                            values.Add(value);
                        }

                        data = Activator.CreateInstance(type, values.ToArray());
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// 非 Ajax Trigger 透過 JavaScript 發動
        /// </summary>
        /// <param name="commands"></param>
        private void PageTrigger(IEnumerable<TriggerCommand> commands)
        {
            var response = _context.Response;

            var sb = new StringBuilder();
            sb.AppendLine("(function ($) { var targets = [ window, parent || window, top || parent || window ];");
                
            foreach (var command in commands)
            {
                string target;

                switch (command.Scope)
                {
                    case TriggerScope.Parent:
                        target = "1";
                        break;
                    case TriggerScope.Top:
                        target = "2";
                        break;
                    default:
                        target = "0";
                        break;
                }
                //sb.AppendFormat("console.log(['trigger', {0}, targets[{0}].document, $(targets[{0}].document)]);\n", target);
                sb.AppendFormat("targets[{0}].getTrigger(\"{1}\").apply(targets[{0}], [null, {2}]);\n", target, command.Trigger, JsonConvert.SerializeObject(command.Data, Formatting.None));

            }
            sb.Append("})(jQuery);");

            var tag = new TagBuilder("script");
            tag.Attributes["type"] = "text/javascript";
            tag.InnerHtml = sb.ToString();

            //response.Write(tag);

            response.Filter = new ContentFilter(response.Filter, x =>
            {
                if (! string.IsNullOrEmpty(x) && Regex.IsMatch(x, "</body>"))
                {
                    x = Regex.Replace(x, @"</body>", tag + "$0");
                }
                else
                {
                    x += tag;
                }

                return x;
            });

        }
    }

    internal class ContentFilter : Stream
    {
        private Stream _shrink;
        private Func<string, string> _filter;

        public ContentFilter(Stream shrink, Func<string, string> filter)
        {
            _shrink = shrink;
            _filter = filter;
        }
  
        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return true; } }
        public override bool CanWrite { get { return true; } }
        public override void Flush() { _shrink.Flush(); }
        public override long Length { get { return 0; } }
        public override long Position { get; set; }
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _shrink.Read(buffer, offset, count);
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _shrink.Seek(offset, origin);
        }
        public override void SetLength(long value)
        {
            _shrink.SetLength(value);
        }
        public override void Close()
        {
            _shrink.Close();
        }
 
        public override void Write(byte[] buffer, int offset, int count)
        {
            // capture the data and convert to string 
            byte[] data = new byte[count];
            Buffer.BlockCopy(buffer, offset, data, 0, count);
            string s = Encoding.UTF8.GetString(buffer);
 
            // filter the string
            s = _filter(s);
 
            // write the data to stream 
            byte[] outdata = Encoding.UTF8.GetBytes(s);
            _shrink.Write(outdata, 0, outdata.GetLength(0));
        }        
    }
}