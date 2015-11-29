using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Hexon.MvcTrig
{
    public class TriggerContext
    {
        #region Static

        internal static string _identifier = "D85DCDD2-2DBE-47DA-A810-73169E9BECFE_TriggerContext";

        public static bool HasTrigger
        {
            get
            {
                var trigger = HttpContext.Current.Items[_identifier] as TriggerContext;

                if (trigger == null)
                {
                    trigger = HttpContext.Current.Session[_identifier] as TriggerContext;
                }

                return trigger != null && trigger.Count > 0;
            }
        }

        public static TriggerContext Current
        {
            get
            {
                var trigger = HttpContext.Current.Items[_identifier] as TriggerContext;

                if (trigger == null)
                {
                    trigger = HttpContext.Current.Session[_identifier] as TriggerContext;

                    if (trigger != null)
                    {
                        // Probely come from previous action (302)
                        HttpContext.Current.Session.Remove(_identifier);
                        HttpContext.Current.Items[_identifier] = trigger = new TriggerContext(HttpContext.Current, trigger);
                    }
                    else
                    {
                        HttpContext.Current.Items[_identifier] = trigger = new TriggerContext(HttpContext.Current);
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

        #region Constructors

        private TriggerContext(HttpContext context)
        {
            _context = context;
        }

        private TriggerContext(HttpContext context, TriggerContext copyFrom) 
        {
            _context = context;
            _commands = copyFrom._commands;
            _lowPiorityCommands = copyFrom._lowPiorityCommands;
        }

        #endregion

        public string Tag { get; set; }

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
        public TriggerContext Parent(Action<TriggerContext> call)
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
        public TriggerContext Top(Action<TriggerContext> call)
        {
            _currentScope = TriggerScope.Top;
            call.Invoke(this);
            _currentScope = TriggerScope.Self;

            return this;
        }

        /// <summary>
        /// 重新載入本頁
        /// </summary>
        /// <returns></returns>
        public TriggerContext Reload()
        {
            Add("reload", null);

            return this;
        }

        /// <summary>
        /// 強迫採用 AJAX 機制 (HTTP header)
        /// </summary>
        /// <returns></returns>
        public TriggerContext AsAjaxTrigger()
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
            response.AddHeader("X-TrigFrom", Tag);
            var i = 0;

            foreach (var command in commands)
            {
                var pack = new
                {
                    scope = command.Scope, 
                    trigger = command.Trigger, 
                    data = command.Data
                };

                var content = JsonConvert.SerializeObject(pack, new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    Converters = new[]
                    {
                        new HttpHeaderStringEncodeConverter()
                    }
                });

                response.AddHeader("X-Trigger-" + (i++), content);
            }
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
                sb.AppendFormat("targets[{0}].getTrigger(\"{1}\").apply(targets[{0}], [null, {2}, null]);\n", target, command.Trigger, JsonConvert.SerializeObject(command.Data, Formatting.None));

            }
            sb.Append("})(jQuery);");

            var tag = new TagBuilder("script");
            tag.Attributes["type"] = "text/javascript";
            tag.Attributes["data-trig"] = Tag;
            tag.InnerHtml = sb.ToString();

            response.Write(tag);

            //response.Filter = new ContentFilter(response.Filter, x =>
            //{
            //    if (! string.IsNullOrEmpty(x) && Regex.IsMatch(x, "</body>"))
            //    {
            //        x = Regex.Replace(x, @"</body>", tag + "$0");
            //    }
            //    else
            //    {
            //        x += tag;
            //    }

            //    return x;
            //});

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