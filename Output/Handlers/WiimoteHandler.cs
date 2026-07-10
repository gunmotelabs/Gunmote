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
    /// Implementa la lógica específica de Gunmote asociada a WiimoteHandler.
    /// </summary>
    internal class WiimoteHandler : IButtonHandler, IFeedback
    {
        public Action<byte, byte> OnRumble { get; set; }
        public Action<int, bool> OnLED { get; set; }
        public Action<string> OnSpeaker { get; set; }

        private enum RumbleState
        {
            none,
            rumbleshort,
            rumblelong,
            rumblehold,
            rumblealt
        }

        private enum Leds
        {
            led1 = 1,
            led2,
            led3,
            led4
        }

        private bool rumbleState = false;
        private bool prevRumbleState = false;
        private RumbleState currRumbleState = RumbleState.none;
        private long prevRumbleTime;

        private bool isAudioLooping = false;
        private long prevAudioTime;
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a WiimoteHandler.
        /// </summary>
        public WiimoteHandler()
        {

        }

        public bool connect()
        {
            return true;
        }

        public bool disconnect()
        {
            return true;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a reset.
        /// </summary>
        public bool reset()
        {
            OnRumble?.Invoke(0, 0);
            OnLED?.Invoke(0, true);
            OnSpeaker?.Invoke(null);

            currRumbleState = RumbleState.none;
            rumbleState = false;
            prevRumbleState = false;
            isAudioLooping = false;

            return true;
        }
        /// <summary>
        /// Procesa el estado de los botones del Wiimote y lo transforma en la salida configurada en Gunmote.
        /// </summary>
        public bool setButtonDown(string key)
        {
            if (Enum.TryParse(key, true, out RumbleState state))
            {
                rumbleState = true;
                prevRumbleTime = Stopwatch.GetTimestamp();
                currRumbleState = state;

                return true;
            }
            else if (Enum.TryParse(key, true, out Leds led))
            {
                OnLED?.Invoke((int)led, true);
                return true;
            }
            else if (key.Contains("sound"))
            {
                OnSpeaker?.Invoke(key);
                return true;
            }
            else if (key == "loop")
            {
                OnSpeaker?.Invoke("loop");
                isAudioLooping = true;
                prevAudioTime = Stopwatch.GetTimestamp();
                return true;
            }

            return false;
        }
        /// <summary>
        /// Procesa el estado de los botones del Wiimote y lo transforma en la salida configurada en Gunmote.
        /// </summary>
        public bool setButtonUp(string key)
        {
            if (key.Equals("rumblehold") || key.Equals("rumblealt"))
            {
                rumbleState = false;
                currRumbleState = RumbleState.none;

                return true;
            }
            else if (Enum.TryParse(key, true, out Leds led))
            {
                OnLED?.Invoke((int)led, false);
                return true;
            }
            else if (key == "loop")
            {
                OnSpeaker?.Invoke(null);
                isAudioLooping = false;
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
            long currentTime = Stopwatch.GetTimestamp();
            double elapsedRumbleMs = (currentTime - prevRumbleTime) * (1000.0 / Stopwatch.Frequency);
            double elapsedSoundMs = (currentTime - prevAudioTime) * (1000.0 / Stopwatch.Frequency);

            switch (currRumbleState)
            {
                case RumbleState.rumbleshort when elapsedRumbleMs >= Settings.Default.wiimode_rumbleTime_short:
                case RumbleState.rumblelong when elapsedRumbleMs >= Settings.Default.wiimode_rumbleTime_long:
                    rumbleState = false;
                    currRumbleState = RumbleState.none;
                    break;
                case RumbleState.rumblealt when (rumbleState && elapsedRumbleMs >= Settings.Default.wiimode_rumbleTime_alternatingOn) || (!rumbleState && elapsedRumbleMs >= Settings.Default.wiimode_rumbleTime_alternatingOff):
                    rumbleState = !rumbleState;
                    prevRumbleTime = Stopwatch.GetTimestamp();
                    break;
                default:
                    break;
            }

            if (rumbleState != prevRumbleState)
            {
                OnRumble?.Invoke((byte)(rumbleState ? 255 : 0), 0);
                prevRumbleState = rumbleState;
            }

            if (isAudioLooping && elapsedSoundMs >= Settings.Default.wiimode_loopSoundTime)
            {
                OnSpeaker?.Invoke("loop");
                prevAudioTime = currentTime;
            }

            return true;
        }
    }
}