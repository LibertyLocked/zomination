using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FAZEngine;
using FAZEngine.Peds;

namespace zomination
{
    public class StatusDisplay
    {
        Player boundedPlayer;
        Vector2 position;

        Texture2D barTexture;
        Texture2D blankTexture;
        Texture2D[] weaponTextures;
        Texture2D screenBloodTexture;
        SpriteFont hudFont;

        Texture2D weaponTexToDraw;
        int clipCount, reserveCount;

        bool inCar, lastInCar;
        int lastHealth, lastMaxHealth;
        int health, maxHealth;
        int gradbar;

        public StatusDisplay(Player boundedPlayer, Vector2 position, Texture2D barTexture, Texture2D blankTexture, Texture2D[] weaponHuds, Texture2D screenBloodTexture, SpriteFont hudFont)
        {
            this.boundedPlayer = boundedPlayer;
            this.position = position;
            this.barTexture = barTexture;
            this.blankTexture = blankTexture;
            this.weaponTextures = weaponHuds;
            this.screenBloodTexture = screenBloodTexture;
            this.hudFont = hudFont;

            this.gradbar = boundedPlayer.MaxHealth;
            this.health = boundedPlayer.Health;
            this.maxHealth = boundedPlayer.MaxHealth;
        }

        public void Update(GameTime gameTime, ChasingCamera2D cam)
        {
            bool shakeit = true;
            lastHealth = health;
            lastMaxHealth = maxHealth;
            lastInCar = inCar;

            inCar = boundedPlayer.CarDriving != null;
            
            if (!inCar)
            {
                health = boundedPlayer.Health;
                maxHealth = boundedPlayer.MaxHealth;
            }
            else
            {
                health = boundedPlayer.CarDriving.Health;
                maxHealth = boundedPlayer.CarDriving.MaxHealth;
            }

            // is max health changed?
            if (maxHealth != lastMaxHealth)
            {
                shakeit = false;
            }

            // is health changed?
            if (health < lastHealth && shakeit)
            {
                cam.Shake(200, 0.05f, null, 0, 0);
            }

            // is in car stat changed?
            if (lastInCar != inCar)
            {
                gradbar = health;
            }

            // get weapon equipped and show in hud
            string weaponName = boundedPlayer.GunsOwned[boundedPlayer.GunEquipped].WeaponName;
            if (weaponName == "AK74u")
                weaponTexToDraw = weaponTextures[1];
            else if (weaponName == "M4A1")
                weaponTexToDraw = weaponTextures[2];
            else if (weaponName == "R870MCS")
                weaponTexToDraw = weaponTextures[3];
            else if (weaponName == "M60")
                weaponTexToDraw = weaponTextures[4];
            else
                weaponTexToDraw = weaponTextures[0];

            clipCount = boundedPlayer.GunsOwned[boundedPlayer.GunEquipped].ClipAmmo;
            reserveCount = boundedPlayer.GunsOwned[boundedPlayer.GunEquipped].ReserveAmmo;

            // bar grad lines
            if (gradbar > health && !inCar)
                gradbar -= 2;
            else if (gradbar > health && inCar)
                gradbar -= 5;
            if (gradbar < health)
                gradbar = health;
        }

        public void Draw(SpriteBatch spriteBatch, Viewport viewport)
        {
            // draw a rectangle to fill the background
            spriteBatch.Draw(blankTexture, new Vector2(position.X - barTexture.Width / 2 + 7, position.Y + 7),
                new Rectangle(0, 0, 459, 30),
                Color.FromNonPremultiplied(128, 128, 128, 150), 0, Vector2.Zero,
                1f, SpriteEffects.None, 1.0f - 0.004f);

            // draw gradbar rectangle
            spriteBatch.Draw(blankTexture, new Vector2(position.X - barTexture.Width / 2 + 7, position.Y + 7),
                new Rectangle(0, 0, (int)(gradbar * 459 / maxHealth), 30),
                Color.FromNonPremultiplied(220, 20, 60, 255), 0, Vector2.Zero,
                1f, SpriteEffects.None, 1.0f - 0.003f);

            // draw real health rectangle
            Color healthRectColor;
            if (boundedPlayer.CarDriving == null)
                healthRectColor = boundedPlayer.IsInvincible ? Color.DarkGoldenrod : Color.FromNonPremultiplied(255, 192, 203, 255);
            else
                healthRectColor = Color.SteelBlue;
            spriteBatch.Draw(blankTexture, new Vector2(position.X - barTexture.Width / 2 + 7, position.Y + 7),
                new Rectangle(0, 0, (int)(health * 459 / maxHealth), 30),
                healthRectColor, 0, Vector2.Zero,
                1f, SpriteEffects.None, 1.0f - 0.002f);

            // draw bar frame
            spriteBatch.Draw(barTexture, new Vector2(position.X, position.Y),
                null,
                Color.White, 0, new Vector2(barTexture.Width / 2, 0),
                1f, SpriteEffects.None, 1.0f);

            // draw a rectangle under MONEY count
            string moneyStr = "$" + boundedPlayer.Money.ToString("d2") + @"/Kills:" + boundedPlayer.KillCount;
            Vector2 moneyStrSize = hudFont.MeasureString(moneyStr);
            Vector2 moneyStrPos = position + new Vector2(-barTexture.Width / 2 + 30, barTexture.Height);
            spriteBatch.Draw(blankTexture,
                new Rectangle((int)moneyStrPos.X - 4, (int)moneyStrPos.Y, (int)moneyStrSize.X + 8, (int)moneyStrSize.Y),
                    Color.FromNonPremultiplied(255, 255, 255, 150));
            // draw MONEY count
            spriteBatch.DrawString(hudFont, moneyStr, moneyStrPos, Color.Green);


            // DRAW WEAPONS AND AMMO COUNT
            if (weaponTexToDraw != null && boundedPlayer.Health > 0)
            {
                string ammoString = clipCount + "/" + reserveCount.ToString("d3");  // format to 3 decimal places
                Vector2 ammoStrSize = hudFont.MeasureString(ammoString);
                Vector2 ammoStringPos = new Vector2(position.X + barTexture.Width / 2 - ammoStrSize.X - 30, position.Y + barTexture.Height);
                // draw weapon texture
                spriteBatch.Draw(weaponTexToDraw, new Vector2(position.X + barTexture.Width / 2 - 60, position.Y + barTexture.Height + 10),
                    null,
                    Color.White, 0, new Vector2(weaponTexToDraw.Width / 2, weaponTexToDraw.Height / 2),
                    1f, SpriteEffects.None, 1.0f);
                // draw a nice rectangle under ammo count
                spriteBatch.Draw(blankTexture,
                    new Rectangle((int)ammoStringPos.X - 4, (int)ammoStringPos.Y, (int)ammoStrSize.X + 8, (int)ammoStrSize.Y), 
                        Color.FromNonPremultiplied(128, 128, 128, 150));
                // draw ammo count
                spriteBatch.DrawString(hudFont, ammoString, ammoStringPos, (reserveCount + clipCount) > 0 ? Color.White : Color.Red);
            }

            if (boundedPlayer.GunsOwned.Count <= 1)
            {
                // draw a rectangle under MESSAGE notification
                string messageStr = "Tap DPad Left to buy guns\nDPad Right to customize";
                Vector2 messageStrSize = hudFont.MeasureString(messageStr);
                Vector2 messageStrPos = position + new Vector2(-barTexture.Width / 2 + 24, barTexture.Height + messageStrSize.Y / 2);
                spriteBatch.Draw(blankTexture,
                    new Rectangle((int)messageStrPos.X - 4, (int)messageStrPos.Y, (int)messageStrSize.X + 8, (int)messageStrSize.Y),
                        Color.FromNonPremultiplied(255, 128, 128, 150));
                // draw MESSAGE notification
                spriteBatch.DrawString(hudFont, messageStr, messageStrPos, Color.DarkMagenta);
            }

            // draw screen blood
            if (boundedPlayer.Health < boundedPlayer.MaxHealth * 0.5f)
            {
                int bloodAlpha = (int)(140 * (1 - (float)boundedPlayer.Health / boundedPlayer.MaxHealth / 0.5f));
                spriteBatch.Draw(screenBloodTexture, new Rectangle(0, 0, viewport.Width, viewport.Height), 
                    Color.FromNonPremultiplied(255, 255, 255, bloodAlpha));
            }
        }
    }
}
