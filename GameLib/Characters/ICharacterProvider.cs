using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLib.Characters
{
    public interface ICharacterProvider
    {
        string[] Words { get; }
        string Character(string s);
    }
}
