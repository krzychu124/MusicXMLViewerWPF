using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXMLScore.Helpers
{
    public static class Mediator
    {
        static IDictionary<string, List<Action<object>>> pl_dict = new Dictionary<string, List<Action<object>>>();

        /// <summary>
        /// Registers Token and assigns Callback to it.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="callback"></param>
        static public void Register(string token, Action<object> callback)
        {
            if (!pl_dict.ContainsKey(token))
            {
                var list = new List<Action<object>>();
                list.Add(callback);
                pl_dict.Add(token, list);
            }
            else
            {
                bool found = false;
                foreach (var item in pl_dict[token])
                    if (item.Method.ToString() == callback.Method.ToString())
                        found = true;
                if (!found)
                    pl_dict[token].Add(callback);
            }
        }

        static public void Unregister(string token, Action<object> callback)
        {
            if (pl_dict.ContainsKey(token))
                pl_dict[token].Remove(callback);
        }
        /// <summary>
        /// Sends (object)Args to every Callback assigned to Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="args"></param>
        static public void NotifyColleagues(string token, object args)
        {
            if (pl_dict.ContainsKey(token))
                foreach (var callback in pl_dict[token])
                    callback(args);
        }
    }
}
