// -----------------------------------------------------------------------------
// Nuevo desarrollo y modificaciones Gunmote:
// Copyright (c) Gustavo A. Lara (PapaGustavoKratos / GustavoALara).
//
// Este fichero puede conservar código original de Touchmote bajo licencia GPL.
// Las modificaciones, ampliaciones y correcciones específicas de Gunmote se
// atribuyen a Gustavo A. Lara, salvo las notas de colaboración indicadas.
// -----------------------------------------------------------------------------
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WiiCPP;
using WiiTUIO.DeviceUtils;
using WiiTUIO.Properties;

namespace WiiTUIO.Output
{
    /// <summary>
    /// Ventana superpuesta que renderiza el cursor Direct3D de Gunmote.
    /// </summary>
    public class D3DCursorWindow
    {
        private static D3DCursorWindow defaultInstance;

        private Screen primaryScreen;

        public static D3DCursorWindow Current
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new D3DCursorWindow();
                }
                return defaultInstance;
            }
        }

        private D3DCursorWindow()
        {
            Settings.Default.PropertyChanged += SettingsChanged;
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            cursors = new List<D3DCursor>(2);
            mutex = new Mutex();

            primaryScreen = DeviceUtil.GetScreen(Settings.Default.primaryMonitor);
        }
        /// <summary>
        /// Actualiza un valor interno utilizado por la lógica específica de Gunmote.
        /// </summary>
        private void SettingsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "primaryMonitor")
            {
                primaryScreen = DeviceUtil.GetScreen(Settings.Default.primaryMonitor);
                this.updateWindowToScreen(primaryScreen);
            }
        }
        /// <summary>
        /// Actualiza los límites de pantalla cuando cambia la configuración de monitores de Windows.
        /// </summary>
        private void updateWindowToScreen(Screen screen)
        {
            Console.WriteLine("Setting cursor window position to " + screen.Bounds);
            SetD3DCursorWindowPosition(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height, true);
        }

        private Mutex mutex;
        private List<D3DCursor> cursors;
            
        [DllImport("D3DCursor.dll")]
        private static extern IntPtr StartD3DCursorWindow(IntPtr hInstance, IntPtr parent, int windowX, int windowY, int windowWidth, int windowHeight, bool topmost, float cursorScale);

        [DllImport("D3DCursor.dll")]
        private static extern void SetD3DCursorWindowPosition(int x, int y, int width, int height, bool topmost);

        [DllImport("D3DCursor.dll")]
        private static extern void SetD3DCursorPosition(int id, int x, int y);

        [DllImport("D3DCursor.dll")]
        private static extern void SetD3DCursorPressed(int id, bool pressed);

        [DllImport("D3DCursor.dll")]
        private static extern void SetD3DCursorHidden(int id, bool hidden);

        [DllImport("D3DCursor.dll")]
        private static extern void SetD3DCursorColor(int id, uint color);

        [DllImport("D3DCursor.dll")]
        private static extern void AddD3DCursor(int id, uint color);

        [DllImport("D3DCursor.dll")]
        private static extern void RemoveD3DCursor(int id);

        [DllImport("D3DCursor.dll")]
        private static extern void RenderAllD3DCursors();

        //Should be run with a dispatcher
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a Start.
        /// </summary>
        public void Start(IntPtr parent)
        {
            StartD3DCursorWindow(Process.GetCurrentProcess().Handle, parent, primaryScreen.Bounds.X, primaryScreen.Bounds.Y, primaryScreen.Bounds.Width, primaryScreen.Bounds.Height, true, (float)Settings.Default.pointer_cursorSize);
            updateWindowToScreen(primaryScreen);
        }
        /// <summary>
        /// Actualiza los límites de pantalla cuando cambia la configuración de monitores de Windows.
        /// </summary>
        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            primaryScreen = DeviceUtil.GetScreen(Settings.Default.primaryMonitor);
            this.updateWindowToScreen(primaryScreen);
        }
        /// <summary>
        /// Calcula, ajusta o envía la posición del cursor generada por Gunmote.
        /// </summary>
        public void AddCursor(D3DCursor cursor)
        {
            mutex.WaitOne();
            cursors.Add(cursor);

            AddD3DCursor(cursor.ID, (uint)((((uint)cursor.Color.R) << 16) | (((uint)cursor.Color.G) << 8) | (uint)cursor.Color.B));

            SetD3DCursorPosition(cursor.ID, cursor.X, cursor.Y);
            SetD3DCursorPressed(cursor.ID, cursor.Pressed);
            SetD3DCursorHidden(cursor.ID, cursor.Hidden);
            mutex.ReleaseMutex();
        }
        /// <summary>
        /// Desinstala o limpia controladores virtuales de Gunmote dejando solo los dispositivos requeridos.
        /// </summary>
        public void RemoveCursor(D3DCursor cursor)
        {
            mutex.WaitOne();
            cursors.Remove(cursor);

            RemoveD3DCursor(cursor.ID);
            mutex.ReleaseMutex();
        }

        bool stopRendering = false;
        int someLastFrames = 0;
        /// <summary>
        /// Calcula, ajusta o envía la posición del cursor generada por Gunmote.
        /// </summary>
        public void RefreshCursors()
        {
            bool anyCursorIsVisible = false;
            foreach(D3DCursor cursor in cursors)
            {
                SetD3DCursorPosition(cursor.ID, cursor.X, cursor.Y);
                SetD3DCursorPressed(cursor.ID, cursor.Pressed);
                SetD3DCursorHidden(cursor.ID, cursor.Hidden);

                anyCursorIsVisible |= !cursor.Hidden;
            }

            //If there are no cursors to draw, draw a couple of frames to finish the hide animation, then stop rendering.
            if (cursors.Count > 0 && anyCursorIsVisible)
            {
                stopRendering = false;
            }
            else
            {
                if (!stopRendering)
                {
                    stopRendering = true;
                    someLastFrames = 20;
                }
            }

            if (!stopRendering || someLastFrames > 0)
            {
                RenderAllD3DCursors();
                if(stopRendering)
                {
                    someLastFrames--;
                }
            }
        }

    }
}
