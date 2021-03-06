﻿namespace WoMFramework.Game.Model
{
    public class Equipment
    {
        public Weapon BaseWeapon { get; set; }

        private Weapon _primaryWeapon;
        public Weapon PrimaryWeapon { get { return _primaryWeapon ?? BaseWeapon; } set { _primaryWeapon = value; } }

        public Weapon SecondaryWeapon { get; set; }

        public Armor Armor { get; set; }

        public int ArmorBonus => 0 + (Armor != null ? Armor.ArmorBonus : 0);

        public int ShieldBonus => 0;
    }
}