using System;
using System.Collections.Generic;

namespace BattleShipCoreServer.Core
{
    public static class ExtensionMethods
    {
        public static T Find<T>(this HashSet<T> set, Predicate<T> predicate)
        {
            foreach (var item in set)
            {
                if(predicate(item))
                    return item;
            }

            return default(T);
        }
    }
}