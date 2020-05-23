using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest
{
    public class Item
    {
        public string name;
        public int stack, maxStack;
        public Point iconSheetPosition;
        public Point iconSize;
        
        public static Texture2D iconSheet;

        public static void LoadContent(ContentManager content)
        {
            iconSheet = content.Load<Texture2D>("Items\\IconSheet");
        }

        public virtual void DrawIcon(SpriteBatch spriteBatch, Rectangle drawRectangle)
        {
            spriteBatch.Draw(iconSheet, drawRectangle, new Rectangle(new Point(iconSize.X * iconSheetPosition.X, iconSheetPosition.Y * iconSize.Y), iconSize), Color.White);
        }

        internal void DrawIcon(DrawBatch drawBatch, Rectangle rectangle, float depth)
        {
            drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, iconSheet, rectangle, new Rectangle(new Point(iconSize.X * iconSheetPosition.X, iconSheetPosition.Y * iconSize.Y), iconSize), Color.White, depth);
        }

        public enum Type
        {
            Equipment,
            Miscellaneous
        }

        public enum ItemID
        {
            Turd
        }

        public Type type;

        public Item(Point iconSheetPosition, string name, int stack, Type type, int maxStack)
        {
            this.iconSize = new Point(32);
            this.iconSheetPosition = iconSheetPosition;
            this.name = name;
            this.type = type;
            this.stack = stack;
            this.maxStack = maxStack;
        }
        public Item(Point iconSheetPosition, string name, int stack, Type type)
        {
            this.iconSize = new Point(32);
            this.iconSheetPosition = iconSheetPosition;
            this.name = name;
            this.type = type;
            this.stack = stack;
        }
        public Item(Point iconSheetPosition, string name, Type type)
        {
            this.iconSize = new Point(32);
            this.iconSheetPosition = iconSheetPosition;
            stack = 1;
            this.name = name;
            this.type = type;
        }


        public static Item GetItem(ItemID itemID, int stack)
        {
            switch (itemID)
            {
                case ItemID.Turd:
                    return new Item(Point.Zero, "Turd", stack, Type.Miscellaneous, 3);
                default:
                    return null;
            }
        }







        public abstract class Equipment : Item
        {
            Dictionary<CharacterStats.ValueType, CharacterStats.ActiveEffect.Affector> stats = new Dictionary<CharacterStats.ValueType, CharacterStats.ActiveEffect.Affector>();
            public enum EquipmentType
            {
                Weapon
            }

            public EquipmentType equipmentType;

            public Equipment(string name, EquipmentType equipmentType) : base(Point.Zero, name, 1, Type.Equipment)
            {

            }
        }
        public abstract class Weapon : Equipment
        {
            public enum WeaponType
            {
                Sword
            }

            public WeaponType weaponType;

            public virtual float GetDamage()
            {
                return 10f;
            }

            public Weapon(string name, WeaponType weaponType) : base(name, EquipmentType.Weapon)
            {
                this.weaponType = weaponType;
            }

        }
    }
}
