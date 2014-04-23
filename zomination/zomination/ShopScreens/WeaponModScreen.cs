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
    class WeaponModScreen : MenuScreen
    {
        Player player;
        World world;
        BasicGun gun;

        MenuEntry lazerMenuEntry;
        MenuEntry extendedClipMenuEntry;
        MenuEntry fullAutoMenuEntry;
        MenuEntry fastReloadMenuEntry; // increase reload speed
        MenuEntry moreAmmoMenuEntry;    // increase reserve ammo size
        MenuEntry fmjMenuEntry;

        int[] lazerCosts = new int[] { 40, 80, 100, 70, 160 };
        int[] extendedClipCosts = new int[] { 90, 180, 220, 150, 320 };
        int[] fullAutoCosts = new int[] { 110, 0, 0, 300, 0 };
        int[] fastReloadCosts = new int[] { 100, 180, 240, 320, 500 };
        int[] moreAmmoCosts = new int[] { 100, 300, 320, 280, 800 };
        int[] fmjConsts = new int[] { 1300, 900, 1000, 1300, 1600 };

        public WeaponModScreen(int playerIndex, Player player, BasicGun gun, World world)
            : base("Player " + (playerIndex + 1) + ": Modding " + gun.GunType.ToString())
        {
            this.player = player;
            this.gun = gun;
            this.world = world;

            // create menu entries
            lazerMenuEntry = new MenuEntry(string.Empty);
            extendedClipMenuEntry = new MenuEntry(string.Empty);
            fullAutoMenuEntry = new MenuEntry(string.Empty);
            fastReloadMenuEntry = new MenuEntry(string.Empty);
            moreAmmoMenuEntry = new MenuEntry(string.Empty);
            fmjMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Exit Shop");

            // hook up event handlers
            lazerMenuEntry.Selected += LazerMenuEntrySelected;
            extendedClipMenuEntry.Selected += ExtendedClipMenuEntrySelected;
            fullAutoMenuEntry.Selected += FullAutoMenuEntrySelected;
            fastReloadMenuEntry.Selected += FastReloadMenuEntrySelected;
            moreAmmoMenuEntry.Selected += MoreAmmoMenuEntrySelected;
            fmjMenuEntry.Selected += FmjMenuEntrySelected;
            back.Selected += OnCancel;

            // add menu entries to screen
            MenuEntries.Add(lazerMenuEntry);
            MenuEntries.Add(extendedClipMenuEntry);
            MenuEntries.Add(fullAutoMenuEntry);
            MenuEntries.Add(fastReloadMenuEntry);
            MenuEntries.Add(moreAmmoMenuEntry);
            MenuEntries.Add(fmjMenuEntry);

            MenuEntries.Add(back);
        }

        private void SetMenuEntryText()
        {
            int i = (int)gun.GunType;
            lazerMenuEntry.Text = "Laser Sight: " + (gun.HasLazer ? "Already owned" : "Purchase for $" + lazerCosts[i]);
            extendedClipMenuEntry.Text = "Extended Clip: " + (gun.HasExtendedClip ? "Already owned" : "Purchase for $" + extendedClipCosts[i]);
            fullAutoMenuEntry.Text = "Full Auto: " + (gun.HasFullAuto ? "Already owned" : "Purchase for $" + fullAutoCosts[i]);
            fmjMenuEntry.Text = "Damage Upgrade: " + (gun.HasFmj ? "Already owned" : "Purchase for $" + fmjConsts[i]);
            fastReloadMenuEntry.Text = "Faster Reload: " + (gun.HasFastReload ? "Already owned" : "Purchase for $" + fastReloadCosts[i]);
            moreAmmoMenuEntry.Text = "Carry More Ammo: " + (gun.HasMoreAmmo ? "Already owned" : "Purchase for $" + moreAmmoCosts[i]); 
        }

        void LazerMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            int i = (int)gun.GunType;
            if (!gun.HasLazer)
            {
                if (player.Money < lazerCosts[i])
                {
                    ScreenManager.AddScreen(new MessageBoxScreen("Not enough money", false), e.PlayerIndex);
                    return;
                }
                else
                {
                    player.DeduceMoney(lazerCosts[i]);
                    gun.ModLazer();
                }
            }
            SetMenuEntryText();
        }

        void ExtendedClipMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            int i = (int)gun.GunType;
            if (!gun.HasExtendedClip)
            {
                if (player.Money < extendedClipCosts[i])
                {
                    ScreenManager.AddScreen(new MessageBoxScreen("Not enough money", false), e.PlayerIndex);
                    return;
                }
                else
                {
                    player.DeduceMoney(extendedClipCosts[i]);
                    gun.ModExtendedClip();
                }
            }
            SetMenuEntryText();
        }

        void FullAutoMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            int i = (int)gun.GunType;
            if (!gun.HasFullAuto)
            {
                if (player.Money < fullAutoCosts[i])
                {
                    ScreenManager.AddScreen(new MessageBoxScreen("Not enough money", false), e.PlayerIndex);
                    return;
                }
                else
                {
                    player.DeduceMoney(fullAutoCosts[i]);
                    gun.ModFullAuto();
                }
            }
            SetMenuEntryText();
        }

        void FmjMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            int i = (int)gun.GunType;
            if (!gun.HasFmj)
            {
                if (player.Money < fmjConsts[i])
                {
                    ScreenManager.AddScreen(new MessageBoxScreen("Not enough money", false), e.PlayerIndex);
                    return;
                }
                else
                {
                    player.DeduceMoney(fmjConsts[i]);
                    gun.ModFmj();
                }
            }
            SetMenuEntryText();
        }

        void MoreAmmoMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            int i = (int)gun.GunType;
            if (!gun.HasMoreAmmo)
            {
                if (player.Money < moreAmmoCosts[i])
                {
                    ScreenManager.AddScreen(new MessageBoxScreen("Not enough money", false), e.PlayerIndex);
                    return;
                }
                else
                {
                    player.DeduceMoney(moreAmmoCosts[i]);
                    gun.ModMoreAmmo();
                }
            }
            SetMenuEntryText();
        }

        void FastReloadMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            int i = (int)gun.GunType;
            if (!gun.HasFastReload)
            {
                if (player.Money < fastReloadCosts[i])
                {
                    ScreenManager.AddScreen(new MessageBoxScreen("Not enough money", false), e.PlayerIndex);
                    return;
                }
                else
                {
                    player.DeduceMoney(fastReloadCosts[i]);
                    gun.ModFastReload();
                }
            }
            SetMenuEntryText();
        }
    }
}
