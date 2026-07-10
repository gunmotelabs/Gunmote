// -----------------------------------------------------------------------------
// Modificaciones Gunmote:
// Copyright (c) Gustavo A. Lara (PapaGustavoKratos / GustavoALara).
//
// Este fichero puede conservar código original de Touchmote bajo licencia GPL.
// Las modificaciones, ampliaciones y correcciones específicas de Gunmote se
// atribuyen a Gustavo A. Lara, salvo las notas de colaboración indicadas.
// -----------------------------------------------------------------------------
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;
using WindowsInput;
using WiiTUIO.Provider;
using System.Windows.Threading;
using System.Diagnostics;
using WiiTUIO.Filters;
using WiiTUIO.Properties;
using WiiTUIO.DeviceUtils;

namespace WiiTUIO.Output.Handlers
{
    /// <summary>
    /// Calcula, ajusta o envía la posición del cursor generada por Gunmote.
    /// </summary>
    internal class CursorHandler : ICursorHandler, IButtonHandler
    {
        private long id;
        private D3DCursor cursor;

        private int width;
        private int height;

        private CursorPositionHelper cursorPositionHelper;
        private OneEuroFilter testLightFilterX = new OneEuroFilter(Settings.Default.test_lightgun_oneeuro_mincutoff,
            Settings.Default.test_lightgun_oneeuro_beta, Settings.Default.test_lightgun_oneeuro_dcutoff);
        private OneEuroFilter testLightFilterY = new OneEuroFilter(Settings.Default.test_lightgun_oneeuro_mincutoff,
            Settings.Default.test_lightgun_oneeuro_beta, Settings.Default.test_lightgun_oneeuro_dcutoff);

        private Point previousLightCursorPoint = new Point(0.5, 0.5);
        private long previousLightTime = 0;
        /// <summary>
        /// Calcula, ajusta o envía la posición del cursor generada por Gunmote.
        /// </summary>
        public CursorHandler(long id)
        {
            this.id = id;
            GetScreenBounds();
            cursorPositionHelper = new CursorPositionHelper();

            Settings.Default.PropertyChanged += SettingsChanged;
        }
        /// <summary>
        /// Actualiza los límites de pantalla cuando cambia la configuración de monitores de Windows.
        /// </summary>
        private void GetScreenBounds()
        {
            System.Drawing.Rectangle screenBounds = DeviceUtils.DeviceUtil.GetScreen(Settings.Default.primaryMonitor).Bounds;
            this.width = screenBounds.Width;
            this.height = screenBounds.Height;
        }
        /// <summary>
        /// Actualiza un valor interno utilizado por la lógica específica de Gunmote.
        /// </summary>
        private void SettingsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "primaryMonitor")
            {
                GetScreenBounds();
                D3DCursorWindow.Current.RefreshCursors();
            }
            else if (e.PropertyName == "Color_ID")
            {
                this.cursor.SetColor(IDColor.getColor((int)this.id));
                D3DCursorWindow.Current.RefreshCursors();
            }

        }
        /// <summary>
        /// Abre o prepara la conexión necesaria para comunicar Gunmote con el componente externo asociado.
        /// </summary>
        public bool connect()
        {
            this.cursor = new D3DCursor((int)this.id - 1, IDColor.getColor((int)this.id));
            this.cursor.SetPressed();
            this.cursor.Hide();
            D3DCursorWindow.Current.AddCursor(cursor);

            return true;
        }
        /// <summary>
        /// Abre o prepara la conexión necesaria para comunicar Gunmote con el componente externo asociado.
        /// </summary>
        public bool disconnect()
        {
            App.Current.Dispatcher.BeginInvoke(new Action(delegate ()
            {
                D3DCursorWindow.Current.RemoveCursor(this.cursor);
            }), null);

            return true;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a reset.
        /// </summary>
        public bool reset()
        {
            // Create empty smoothing filters on profile reset
            testLightFilterX = new OneEuroFilter(Settings.Default.test_lightgun_oneeuro_mincutoff,
                Settings.Default.test_lightgun_oneeuro_beta, Settings.Default.test_lightgun_oneeuro_dcutoff);
            testLightFilterY = new OneEuroFilter(Settings.Default.test_lightgun_oneeuro_mincutoff,
                Settings.Default.test_lightgun_oneeuro_beta, Settings.Default.test_lightgun_oneeuro_dcutoff);

            previousLightCursorPoint = new Point(0.5, 0.5);
            previousLightTime = Stopwatch.GetTimestamp();

            this.cursor.Hide();
            this.cursor.SetReleased();

            return true;
        }
        /// <summary>
        /// Calcula, ajusta o envía la posición del cursor generada por Gunmote.
        /// </summary>
        public bool setPosition(string key, CursorPos cursorPos)
        {
            if (key.Equals("cursor"))
            {
                if (!cursorPos.OutOfReach)
                {
                    this.cursor.Show();

                    Point smoothedPos = cursorPositionHelper.getSmoothedPosition(new Point(cursorPos.X, cursorPos.Y));

                    this.cursor.SetPosition(smoothedPos);
                    return true;
                }
                else
                {
                    this.cursor.Hide();
                }

                return true;
            }
            else if (key.Equals("lightguncursor") || key.Equals("lightguncursor-4:3") || key.Equals("lightguncursor-16:9"))
            {
                long currentTime = Stopwatch.GetTimestamp();
                long timeElapsed = currentTime - previousLightTime;
                double elapsedMs = timeElapsed * (1.0 / Stopwatch.Frequency);
                previousLightTime = currentTime;

                if (!cursorPos.OutOfReach && !cursorPos.OffScreen)
                {
                    this.cursor.Show();

                    Point smoothedPos = new Point();
                    smoothedPos.X = testLightFilterX.Filter(cursorPos.LightbarX * 1.001, 1.0 / elapsedMs);
                    smoothedPos.Y = testLightFilterY.Filter(cursorPos.LightbarY * 1.001, 1.0 / elapsedMs);

                    if (Math.Abs(smoothedPos.X) < 0.0001) smoothedPos.X = 0.0;
                    if (Math.Abs(smoothedPos.Y) < 0.0001) smoothedPos.Y = 0.0;

                    // Clamp values
                    smoothedPos.X = Math.Min(1.0, Math.Max(0.0, smoothedPos.X));
                    smoothedPos.Y = Math.Min(1.0, Math.Max(0.0, smoothedPos.Y));

                    this.cursor.SetPosition(new System.Windows.Point(smoothedPos.X * this.width, smoothedPos.Y * this.height));

                    previousLightCursorPoint = new Point(cursorPos.LightbarX, cursorPos.LightbarY);
                }
                else
                {
                    // Save last known position to smoothing buffer
                    testLightFilterX.Filter(previousLightCursorPoint.X * 1.001, 1.0 / elapsedMs);
                    testLightFilterY.Filter(previousLightCursorPoint.Y * 1.001, 1.0 / elapsedMs);

                    this.cursor.Hide();
                }

                return true;
            }

            return false;
        }
        /// <summary>
        /// Procesa el estado de los botones del Wiimote y lo transforma en la salida configurada en Gunmote.
        /// </summary>
        public bool setButtonDown(string key)
        {
            if (key.Equals("cursorpress"))
            {
                this.cursor.SetReleased();

                return true;
            }

            return false;
        }
        /// <summary>
        /// Procesa el estado de los botones del Wiimote y lo transforma en la salida configurada en Gunmote.
        /// </summary>
        public bool setButtonUp(string key)
        {
            if (key.Equals("cursorpress"))
            {
                this.cursor.SetPressed();

                return true;
            }

            return false;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a startUpdate.
        /// </summary>
        public bool startUpdate()
        {
            return true;
        }

        public bool endUpdate()
        {
            return true;
        }
    }
}
