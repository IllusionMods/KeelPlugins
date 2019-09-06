using System;
using System.Collections.Generic;
using System.Linq;

namespace KeelPlugins
{
    internal abstract class CardHandlerCore<T> where T : CardHandlerCore<T>
    {
        private static Dictionary<Type, T> cardHandlers = new Dictionary<Type, T>();

        /// <summary>
        /// This has to be completely unique, the first Condition property to return true will be picked
        /// </summary>
        public abstract bool Condition { get; }

        public static T GetActiveCardHandler()
        {
            foreach(var handler in cardHandlers.Values)
            {
                if(handler.Condition)
                    return handler;
            }

            var mainType = typeof(T);
            var inheritingTypes = mainType.Assembly.GetTypes().Where(x => x.IsSubclassOf(mainType));

            foreach(var type in inheritingTypes)
            {
                if(!cardHandlers.TryGetValue(type, out var handler))
                {
                    handler = (T)Activator.CreateInstance(type);
                    cardHandlers.Add(type, handler);
                }

                if(handler.Condition)
                    return handler;
            }

            return null;
        }
    }
}
