using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Quest
{
    public static class Controls
    {
        public static Dictionary<KeyBind, Keys> keys = new Dictionary<KeyBind, Keys>();

        public enum KeyBind
        {
            MoveUp,
            MoveLeft,
            MoveDown,
            MoveRight,
            Dash,
            Ability1,
            Ability2,
            Ability3,
            Ability4,
            Ability5,
            Ability6,
            Ability7,
            Ability8,
            Ability9
        }

        public static void Initialize()
        {
            keys.Add(KeyBind.MoveUp, Keys.W);
            keys.Add(KeyBind.MoveLeft, Keys.A);
            keys.Add(KeyBind.MoveDown, Keys.S);
            keys.Add(KeyBind.MoveRight, Keys.D);
            keys.Add(KeyBind.Dash, Keys.LeftShift);
            keys.Add(KeyBind.Ability1, Keys.D1);
            keys.Add(KeyBind.Ability2, Keys.D2);
            keys.Add(KeyBind.Ability3, Keys.D3);
            keys.Add(KeyBind.Ability4, Keys.D4);
            keys.Add(KeyBind.Ability5, Keys.D5);
            keys.Add(KeyBind.Ability6, Keys.D6);
            keys.Add(KeyBind.Ability7, Keys.D7);
            keys.Add(KeyBind.Ability8, Keys.D8);
            keys.Add(KeyBind.Ability9, Keys.D9);
        }

        public static bool KeyPressed(KeyBind button)
        {
            return Game1.KeyPressed(keys[button]);
        }
        public static bool KeyDown(KeyBind button)
        {
            return Game1.KeyDown(keys[button]);
        }
        public static bool KeyUp(KeyBind button)
        {
            return Game1.KeyUp(keys[button]);
        }
        public static bool KeyReleased(KeyBind button)
        {
            return Game1.KeyReleased(keys[button]);
        }
    }
}
