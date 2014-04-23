using System;

namespace Launcher
{
    public static class MsgBoxF
    {
        /// <summary>
        /// Display a regualr message with a title and details.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="detail"></param>
        public static void Show(string title, string detail)
        {
            using (FormMsgbox messageBox = new FormMsgbox(title, detail))
            {
                messageBox.ShowDialog();
            }
        }

        /// <summary>
        /// Display a regualr message with a title and details, TOP MOST.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="detail"></param>
        public static void Show(string title, string detail, bool topMost)
        {
            using (FormMsgbox messageBox = new FormMsgbox(title, detail))
            {
                messageBox.TopMost = topMost;
                messageBox.ShowDialog();
            }
        }

        /// <summary>
        /// Display a message with 2 buttons and URL redirecting on click.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="detail"></param>
        /// <param name="button1"></param>
        /// <param name="button2"></param>
        /// <param name="url"></param>
        public static void Show(string title, string detail, string button1, string button2, string url)
        {
            using (FormMsgbox messageBox = new FormMsgbox(title, detail, button1, button2, url))
            {
                messageBox.ShowDialog();
            }
        }

        /// <summary>
        /// Display a message with 2 buttons and URL redirecting on click, TOP MOST.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="detail"></param>
        /// <param name="button1"></param>
        /// <param name="button2"></param>
        /// <param name="url"></param>
        public static void Show(string title, string detail, string button1, string button2, string url, bool topMost)
        {
            using (FormMsgbox messageBox = new FormMsgbox(title, detail, button1, button2, url))
            {
                messageBox.TopMost = topMost;
                messageBox.ShowDialog();
            }
        }

        /// <summary>
        /// Display a message with 2 buttons and customized event.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="detail"></param>
        /// <param name="button1"></param>
        /// <param name="button2"></param>
        /// <param name="clickEvent1"></param>
        public static void Show(string title, string detail, string button1, string button2, EventHandler clickEvent1)
        {
            using (FormMsgbox messageBox = new FormMsgbox(title, detail, button1, button2, clickEvent1))
            {
                messageBox.ShowDialog();
            }
        }

        /// <summary>
        /// Display a message with 2 buttons and customized event. TOP MOST
        /// </summary>
        /// <param name="title"></param>
        /// <param name="detail"></param>
        /// <param name="button1"></param>
        /// <param name="button2"></param>
        /// <param name="clickEvent1"></param>
        public static void Show(string title, string detail, string button1, string button2, EventHandler clickEvent1, bool topMost)
        {
            using (FormMsgbox messageBox = new FormMsgbox(title, detail, button1, button2, clickEvent1))
            {
                messageBox.TopMost = topMost;
                messageBox.ShowDialog();
            }
        }
    }
}
