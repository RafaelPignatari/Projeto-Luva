using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuvaApp.Helpers
{
    public static class MetodosShared
    {
        /// <summary>
        /// Obtém os valores mais repetidos de uma mensagem.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string ValoresMaisRepetidas(string message)
        {
            string[] words = message.Split(';');
            var wordGroups = words.GroupBy(w => w);
            var mostRepeatedWordGroup = wordGroups.OrderByDescending(g => g.Count()).First();
            return mostRepeatedWordGroup.Key;
        }        
    }
}
