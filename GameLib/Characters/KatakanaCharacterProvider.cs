using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLib.Characters
{
    public class KatakanaCharacterProvider : ICharacterProvider
    {

        private Dictionary<string, string> characters = new Dictionary<string, string>()
        {
            {"ア", "a"},
            {"イ", "i"},
            {"ウ", "u"},
            {"エ", "e"},
            {"オ", "o"},
            {"カ", "ka"},
            {"キ", "ki"},
            {"ク", "ku"},
            {"ケ", "ke"},
            {"コ", "ko"},
            {"サ", "sa"},
            {"シ", "shi"},
            {"ス", "su"},
            {"セ", "se"},
            {"ソ", "so"},
            {"タ", "ta"},
            {"チ", "chi"},
            {"ツ", "tsu"},
            {"テ", "te"},
            {"ト", "to"}
        };

        private string[] katakana = { "ア", "イ", "ウ", "エ", "オ", "カ", "キ", "ク", "ケ", "コ", "アカ", "サ", "シ", "ス", "セ", "ソ", "タ", "チ", "ツ", "テ", "ト" };

        public string[] Words
        {
            get { return katakana; }
        }

        public string Character(string s)
        {
            return characters[s];
        }
    }
}
