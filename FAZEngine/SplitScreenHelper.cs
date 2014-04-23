using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace FAZEngine
{
    public class SplitScreenHelper
    {
        /// <summary>
        /// Gets viewports for all players.
        /// </summary>
        /// <param name="defaultViewport">The default viewport of the game</param>
        /// <param name="numScreens">Number of screens</param>
        /// <returns></returns>
        public static Viewport[] GetViewports(Viewport defaultViewport, int numScreens)
        {
            Viewport[] viewports = new Viewport[numScreens];
            
            // set all viewports to default first
            for (int i = 0; i < numScreens; i++)
            {
                viewports[i] = defaultViewport;
            }

            if (numScreens == 2)
            {
                viewports[0].Width = (int)(defaultViewport.Width * 0.75f);
                viewports[0].Height /= 2;

                viewports[1].X = (int)(defaultViewport.Width * 0.25f);
                viewports[1].Y = viewports[0].Height;
                viewports[1].Width = viewports[0].Width;
                viewports[1].Height /= 2;
            }

            return viewports;
        }

        public static Gamer GetGamerProfile(PlayerIndex player)
        {
            // Gamer.SignedInGamers[PlayerIndex] may not be reliable (unclear) so...  
            foreach (SignedInGamer gamer in Gamer.SignedInGamers)
                if (gamer.PlayerIndex == player) return gamer;
            return null;
        }

        /// <summary>
        /// If player isn't signed in, pop up sign in panel.
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <returns>True if player is signed in. False otherwise.</returns>
        public static bool CheckSignIn(PlayerIndex playerIndex)
        {
            Gamer gamer = GetGamerProfile(playerIndex);
            if (gamer == null)
            {
                try
                {
                    // put guide show in try catch
                    Guide.ShowSignIn(1, false);
                }
                catch { }
                return false;
            }
            else
            {
                return true;
            }
        }

        public static PlayerIndex MasterPlayerIndex
        {
            get;
            set;
        }
    }
}
