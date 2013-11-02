using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLib.Characters;

namespace GameLib.Helpers
{
    internal class CharacterHelper
    {
        internal static ICharacterProvider Provider
        {
            get
            {
                return new HiraganaCharacterProvider();
            }
        }
    }
}
