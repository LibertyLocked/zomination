using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FAZEngine;
using FAZEngine.Peds;
using FAZEngine.Weapons;
using zomination.Peds;
using zomination.Weapons;

namespace zomination.ShopScreens
{
    class WeaponShopScreen : MenuScreen
    {
        Player player;
        World world;

        MenuEntry[] shopEntries;
        BasicGun[] guns;
        int[] costs = new int[] { 30 /*postol*/, 100 /*smg*/, 170 /*assult*/, 200 /*shotgun*/, 320 /*LMG*/ };

        public WeaponShopScreen(int playerIndex, Player player, World world)
            : base("Player " + (playerIndex + 1) + ": Weapon Shop")
        {
            this.player = player;
            this.world = world;
            guns = new BasicGun[5];
            shopEntries = new MenuEntry[5];

            // create menu entries
            for (int i = 0; i < shopEntries.Length; i++)
                shopEntries[i] = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Exit Shop");

            // hook up event handlers
            back.Selected += OnCancel;
            for (int i = 0; i < shopEntries.Length; i++)
            {
                shopEntries[i].Selected += ShopMenuEntrySelected;
            }

            // add menu entries to screen
            for (int i = 0; i < shopEntries.Length; i++)
                MenuEntries.Add(shopEntries[i]);
            MenuEntries.Add(back);
        }

        private void SetMenuEntryText()
        {
            // get a list of all guns owned by player
            foreach (BasicGun gun in player.GunsOwned)
            {
                if (gun.GunType == GunType.Pistol) guns[0] = gun;
                else if (gun.GunType == GunType.SMG) guns[1] = gun;
                else if (gun.GunType == GunType.AssultRifle) guns[2] = gun;
                else if (gun.GunType == GunType.Shotgun) guns[3] = gun;
                else if (gun.GunType == GunType.LMG) guns[4] = gun;
            }

            for (int i = 0; i < guns.Length; i++)
            {
                if (guns[i] == null)
                    shopEntries[i].Text = ((GunType)(i)).ToString() + ": Purchase for $" + costs[i];
                else
                {
                    if (guns[i].ReserveAmmo < guns[i].ReserveSize)
                        shopEntries[i].Text = guns[i].GunType.ToString() + ": Refill ammo for $" + (int)(costs[i] * 0.5f * guns[i].GetModValue());
                    else
                        shopEntries[i].Text = guns[i].GunType.ToString() + ": Full ammo owned";
                }
            }
        }

        private void ShopMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            int index = 0;
            for (int i = 0; i < shopEntries.Length; i++)
                if (sender == shopEntries[i]) index = i;

            // when the gun selected is not owned
            if (guns[index] == null)
            {
                if (player.Money < costs[index])
                {
                    ScreenManager.AddScreen(new MessageBoxScreen("Not enough money", false), ControllingPlayer);
                    return;
                }

                if (index == 0) player.GetGun(new M1911(player, world));
                else if (index == 1) player.GetGun(new AK74u(player, world));
                else if (index == 2) player.GetGun(new M4A1(player, world));
                else if (index == 3) player.GetGun(new R870MCS(player, world));
                else if (index == 4) player.GetGun(new M60(player, world));

                player.DeduceMoney(costs[index]);
            }
            else if (guns[index].ReserveAmmo < guns[index].ReserveSize)
            {
                if (player.Money < (int)(costs[index] * 0.5f * guns[index].GetModValue()))
                {
                    ScreenManager.AddScreen(new MessageBoxScreen("Not enough money", false), ControllingPlayer);
                    return;
                }
                else
                {
                    player.DeduceMoney((int)(costs[index] * 0.5f * guns[index].GetModValue()));
                    guns[index].AddAmmo(5000);
                    guns[index].InstaReload();
                    guns[index].AddAmmo(5000);
                }
            }

            SetMenuEntryText();
        }

    }
}
