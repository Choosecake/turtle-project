using System;
using System.Linq;
using UnityEngine;

namespace UI
{
    public static class ActionExtensions
    {
        public static void AddListeners(ref Action action, params Action[] listeners)
        {
            action = listeners.Aggregate(action, (current, listener) => current + listener);
        }

        public static void ExcludeListeners(ref Action action, params Action[] listeners)
        {
            action = listeners.Aggregate(action, (current, listener) => current - listener);
        }
        
        public static void SetListeners(ref Action action, bool add, params Action[] listeners)
        {
            if (add) AddListeners(ref action, listeners);
            else ExcludeListeners(ref action, listeners);
        }
    }
}