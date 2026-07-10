// -----------------------------------------------------------------------------
// Modificaciones Gunmote:
// Copyright (c) Gustavo A. Lara (PapaGustavoKratos / GustavoALara).
//
// Este fichero puede conservar código original de Touchmote bajo licencia GPL.
// Las modificaciones, ampliaciones y correcciones específicas de Gunmote se
// atribuyen a Gustavo A. Lara, salvo las notas de colaboración indicadas.
// -----------------------------------------------------------------------------
using HidLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;	   
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using WiimoteLib;
using WiiTUIO.ArcadeHook;
using WiiTUIO.Output.Handlers;
using WiiTUIO.Properties;
using WiiTUIO.DeviceUtils;
using WindowsInput;

namespace WiiTUIO.Provider
{
    /// <summary>
    /// Implementa la lógica específica de Gunmote asociada a WiimoteControl.
    /// </summary>
    class WiimoteControl
    {
        public DateTime LastWiimoteEventTime = DateTime.Now; //Last time recieved an update
        public DateTime LastSignificantWiimoteEventTime = DateTime.Now; //Last time when updated the cursor or button config. Used for power saving features.

        public Wiimote Wiimote;
        public WiimoteStatus Status;
        /// <summary>
        /// Used to obtain mutual exlusion over Wiimote updates.
        /// </summary>
        public Mutex WiimoteMutex = new Mutex();
        private WiiKeyMapper keyMapper;
        private OutputProvider arcadeHook;
		private volatile bool arcadeHookOutputsActive = false;										  
        private bool firstConfig = true;
        private string currentKeymap;
        private HandlerFactory handlerFactory;
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a WiimoteControl.
        /// </summary>
        public WiimoteControl(int id, Wiimote wiimote)
        {
            this.Wiimote = wiimote;
            this.Status = new WiimoteStatus();
            this.Status.ID = id;

            this.handlerFactory = new HandlerFactory();

            HidDevice hidDevice = HidDevices.GetDevice(this.Wiimote.HIDDevicePath);
            hidDevice.ReadSerialNumber(out byte[] data);
            //string serialNumber = Settings.Default.pointer_4IRMode != "none" ? System.Text.Encoding.Unicode.GetString(data, 0, 24) : null;
			string serialNumber = System.Text.Encoding.Unicode.GetString(data, 0, 24);
            this.keyMapper = new WiiKeyMapper(wiimote, id, handlerFactory, serialNumber);
            this.arcadeHook = new OutputProvider(id);

            this.keyMapper.OnButtonDown += WiiButton_Down;
            this.keyMapper.OnButtonUp += WiiButton_Up;
            this.keyMapper.OnConfigChanged += WiiKeyMap_ConfigChanged;
            this.keyMapper.OnRumble += WiiKeyMap_OnRumble;
            this.keyMapper.OnLED += WiiKeyMap_OnLED;
            this.keyMapper.OnSpeaker += WiiKeyMap_OnSpeaker;
            this.arcadeHook.OnOutput += ArcadeHook_OnOutput;
			this.arcadeHook.OnOutputConfigurationChanged += ArcadeHook_OnOutputConfigurationChanged;
            this.arcadeHook.OnIniFileCreatedWithOutputsDetected += ArcadeHook_OnIniFileCreatedWithOutputsDetected;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a WiiKeyMap_OnRumble.
        /// </summary>
        private void WiiKeyMap_OnRumble(bool rumble)
        {
			if (arcadeHookOutputsActive)
                return;	
            Console.WriteLine("Set rumble to: "+rumble);
            WiimoteMutex.WaitOne();
            this.Wiimote.SetRumble(rumble);
            WiimoteMutex.ReleaseMutex();
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a WiiKeyMap_OnLED.
        /// </summary>
        private void WiiKeyMap_OnLED(int index, bool on)
        {
			if (arcadeHookOutputsActive)
                return;	
            WiimoteMutex.WaitOne();
            switch (index)
            {
                case 1:
                    this.Wiimote.SetLEDs(on, this.Wiimote.WiimoteState.LEDState.LED2, this.Wiimote.WiimoteState.LEDState.LED3, this.Wiimote.WiimoteState.LEDState.LED4);
                    break;
                case 2:
                    this.Wiimote.SetLEDs(this.Wiimote.WiimoteState.LEDState.LED1, on, this.Wiimote.WiimoteState.LEDState.LED3, this.Wiimote.WiimoteState.LEDState.LED4);
                    break;
                case 3:
                    this.Wiimote.SetLEDs(this.Wiimote.WiimoteState.LEDState.LED1, this.Wiimote.WiimoteState.LEDState.LED2, on, this.Wiimote.WiimoteState.LEDState.LED4);
                    break;
                case 4:
                    this.Wiimote.SetLEDs(this.Wiimote.WiimoteState.LEDState.LED1, this.Wiimote.WiimoteState.LEDState.LED2, this.Wiimote.WiimoteState.LEDState.LED3, on);
                    break;
                default:
                    this.Wiimote.SetLEDs(this.Status.ID == 1, this.Status.ID == 2, this.Status.ID == 3, this.Status.ID == 4);
                    break;
            }
            WiimoteMutex.ReleaseMutex();
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a WiiKeyMap_OnSpeaker.
        /// </summary>
        private void WiiKeyMap_OnSpeaker(string filename)
        {
			if (arcadeHookOutputsActive)
                return;	
            if (filename == null)
            {
                this.Wiimote.StopPlayback();
                return;
            }

            int maxPlaybackTime = 3500; // Max playback time in milliseconds

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", filename + ".wav");

            if (AudioUtil.IsValid(filename)) // Check for valid file or convert file if necessary
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        //reader.BaseStream.Seek(44, SeekOrigin.Begin);   // Skip WAV header
                        // byte[] soundData = reader.ReadBytes((int)(fs.Length - 44));
                        byte[] soundData = ReadWavDataChunk(reader);

                        
                        //Conversión en memoria de PCM8 unsigned to signed
                        for (int i = 0; i < soundData.Length; i++)
                            soundData[i] = (byte)(soundData[i] ^ 0x80);

                        if (soundData == null || soundData.Length == 0)
                            return;

                        double seconds = maxPlaybackTime / 1000.0;
                        int sampleRate = this.Wiimote.WiimoteState.SpeakerState.SampleRate;

                        // PCM8 mono: 1 byte por sample
                        int maxBytes = (int)(sampleRate * seconds);

                        if (soundData.Length > maxBytes)
                            Array.Resize(ref soundData, maxBytes);
                                                
                        this.Wiimote.StartPlayback(soundData);

                    }
                }
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a ReadWavDataChunk.
        /// </summary>
        private static byte[] ReadWavDataChunk(BinaryReader reader)
        {
            // Asume que estamos al inicio del archivo
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            // RIFF header
            if (new string(reader.ReadChars(4)) != "RIFF") return null;
            reader.ReadInt32(); // riff size
            if (new string(reader.ReadChars(4)) != "WAVE") return null;

            // Buscar chunk "data"
            while (reader.BaseStream.Position + 8 <= reader.BaseStream.Length)
            {
                string chunkId = new string(reader.ReadChars(4));
                int chunkSize = reader.ReadInt32();

                if (chunkSize < 0) return null;
                long nextChunk = reader.BaseStream.Position + chunkSize;

                if (chunkId == "data")
                {
                    if (reader.BaseStream.Position + chunkSize > reader.BaseStream.Length) return null;
                    return reader.ReadBytes(chunkSize);
                }

                // Saltar chunk (alineación a palabra: si chunkSize es impar, suma 1)
                reader.BaseStream.Position = nextChunk + (chunkSize % 2);
            }

            return null;
        }
		/// <summary>
		/// Procesa una orden de salida de ArcadeOutputs y la traduce a una acción de Gunmote.
		/// </summary>
		private void ArcadeHook_OnIniFileCreatedWithOutputsDetected(string gameName, string filePath)
        {
            // Avoid one overlay/sound per connected Wiimote. Player 1 acts as the shared notifier.
            if (this.Status.ID != 1)
                return;

            try
            {
                SystemSounds.Asterisk.Play();
            }
            catch { }

            try
																			  
            {
                OverlayWindow.Current.ShowNotice("ArcadeOutputs creado para " + gameName + "\n" + filePath, this.Status.ID);
            }
            catch { }
        }
        /// <summary>
        /// Procesa una orden de salida de ArcadeOutputs y la traduce a una acción de Gunmote.
        /// </summary>
        private void ArcadeHook_OnOutputConfigurationChanged(bool hasConfiguredOutputs, string gameName)
        {
            arcadeHookOutputsActive = hasConfiguredOutputs;

            if (!hasConfiguredOutputs)
				 
                return;
													   
				 

            try
            {
                WiimoteMutex.WaitOne();
                this.Wiimote.SetRumble(false);
                this.Wiimote.StopPlayback();
            }
            catch { }
            finally
            {
                try { WiimoteMutex.ReleaseMutex(); } catch { }
            }
        }
        /// <summary>
        /// Procesa una orden de salida de ArcadeOutputs y la traduce a una acción de Gunmote.
        /// </summary>
        private void ArcadeHook_OnOutput(string key, string value)
        {
            int val = 0;
														 
            bool isNumeric = int.TryParse(value, out val);

            try
            {
                switch (key)
                {
                    case "Rumble":
                        if (isNumeric)
                            SetArcadeRumble(val > 0);
                        break;

                    case "LED":
                        ApplyArcadeLedCommand(value);
						 
												   
																						 
														
						 
                        break;

                    case "LEDFill":
                        if (isNumeric)
                            SetArcadeLedFill(val);
												   
																						 
														
						 
                        break;

                    case "Sound":
                        PlayArcadeSound(value);
                        break;

                    case "MameStop":
                        SetArcadeRumble(false);
                        RestorePlayerLed();
																																 
													
                        break;
                }
            }
            catch { }
        }
        /// <summary>
        /// Actualiza un valor interno utilizado por la lógica específica de Gunmote.
        /// </summary>
        private void SetArcadeRumble(bool rumble)
        {
            WiimoteMutex.WaitOne();
            try
            {
                this.Wiimote.SetRumble(rumble);
            }
            finally
            {
                WiimoteMutex.ReleaseMutex();
            }
        }
        /// <summary>
        /// Actualiza un valor interno utilizado por la lógica específica de Gunmote.
        /// </summary>
        private void SetArcadeLedFill(int val)
        {
            if (val < 0)
                val = 0;
            else if (val > 4)
                val = 4;

            WiimoteMutex.WaitOne();
            try
            {
                this.Wiimote.SetLEDs(val >= 1, val >= 2, val >= 3, val >= 4);
            }
            finally
            {
                WiimoteMutex.ReleaseMutex();
            }
        }
        /// <summary>
        /// Procesa una orden de salida de ArcadeOutputs y la traduce a una acción de Gunmote.
        /// </summary>
        private void ApplyArcadeLedCommand(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            // New format from ArcadeHook: index:state
            // Example: 1:0 turns LED1 off, 1:1 turns LED1 on.
            if (value.Contains(":"))
            {
                string[] parts = value.Split(':');
                if (parts.Length != 2)
                    return;

                if (!int.TryParse(parts[0], out int index))
                    return;

                if (!int.TryParse(parts[1], out int state))
                    return;

                SetArcadeSingleLed(index, state > 0);
                return;
            }

            // Legacy behaviour: value is only the LED index.
            if (int.TryParse(value, out int legacyLed))
            {
                WiimoteMutex.WaitOne();
                try
                {
                    this.Wiimote.SetLEDs(legacyLed == 1, legacyLed == 2, legacyLed == 3, legacyLed == 4);
                }
                finally
                {
                    WiimoteMutex.ReleaseMutex();
                }
            }
        }
        /// <summary>
        /// Actualiza un valor interno utilizado por la lógica específica de Gunmote.
        /// </summary>
        private void SetArcadeSingleLed(int index, bool on)
        {
            WiimoteMutex.WaitOne();
            try
            {
                bool led1 = this.Wiimote.WiimoteState.LEDState.LED1;
                bool led2 = this.Wiimote.WiimoteState.LEDState.LED2;
                bool led3 = this.Wiimote.WiimoteState.LEDState.LED3;
                bool led4 = this.Wiimote.WiimoteState.LEDState.LED4;

                switch (index)
                {
                    case 1: led1 = on; break;
                    case 2: led2 = on; break;
                    case 3: led3 = on; break;
                    case 4: led4 = on; break;
                    default: return;
                }

                this.Wiimote.SetLEDs(led1, led2, led3, led4);
            }
            finally
            {
                WiimoteMutex.ReleaseMutex();
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a RestorePlayerLed.
        /// </summary>
        private void RestorePlayerLed()
        {
            WiimoteMutex.WaitOne();
            try
            {
                this.Wiimote.SetLEDs(this.Status.ID == 1, this.Status.ID == 2, this.Status.ID == 3, this.Status.ID == 4);
            }
            finally
            {
                WiimoteMutex.ReleaseMutex();
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a PlayArcadeSound.
        /// </summary>
        private void PlayArcadeSound(string filename)
        {
            if (filename == null)
            {
                WiimoteMutex.WaitOne();
                try
                {
                    this.Wiimote.StopPlayback();
                }
                finally
                {
                    WiimoteMutex.ReleaseMutex();
                }
                return;
            }

            int maxPlaybackTime = 3500;
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", filename + ".wav");

            if (!AudioUtil.IsValid(filename))
                return;

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                reader.BaseStream.Seek(44, SeekOrigin.Begin);
                byte[] soundData = reader.ReadBytes((int)(fs.Length - 44));

                int maxBytes = (int)(this.Wiimote.WiimoteState.SpeakerState.SampleRate * (maxPlaybackTime / 1000.0) * 0.5);
                if (soundData.Length > maxBytes)
                    Array.Resize(ref soundData, maxBytes);

                WiimoteMutex.WaitOne();
                try
                {
                    this.Wiimote.StartPlayback(soundData);
                }
                finally
                {
                    WiimoteMutex.ReleaseMutex();
                }
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a WiiKeyMap_ConfigChanged.
        /// </summary>

        private void WiiKeyMap_ConfigChanged(WiiKeyMapConfigChangedEvent evt)
        {
            if (firstConfig)
            {
                currentKeymap = evt.Filename;
                firstConfig = false;
            }
            else if(evt.Filename != currentKeymap)
            {
                currentKeymap = evt.Filename;
                if (Settings.Default.notifications_enabled)
                    OverlayWindow.Current.ShowNotice("Perfil para el Wiimote " + this.Status.ID + " cambiado a \"" + evt.Name + "\"", this.Status.ID);
            }
        }
        /// <summary>
        /// Procesa el estado de los botones del Wiimote y lo transforma en la salida configurada en Gunmote.
        /// </summary>
        private void WiiButton_Up(WiiButtonEvent evt)
        {
        }

        private void WiiButton_Down(WiiButtonEvent evt)
        {
        }
        
        public bool handleWiimoteChanged(object sender, WiimoteChangedEventArgs e)
        {
            // Obtain mutual excluseion.
            WiimoteMutex.WaitOne();

            bool significant = false;

            try
            {
                WiimoteState ws = e.WiimoteState;
                this.Status.Battery = (ws.Battery > 0xc8 ? 0xc8 : (int)ws.Battery);

                significant = keyMapper.processWiimoteState(ws);

                if (significant)
                {
                    this.LastSignificantWiimoteEventTime = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error handling Wiimote in WiimoteControl: " + ex.Message);
                return significant;
            }
            //this.BatteryState = (pState.Battery > 0xc8 ? 0xc8 : (int)pState.Battery);
            
            // Release mutual exclusion.
            WiimoteMutex.ReleaseMutex();
            return significant;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a Teardown.
        /// </summary>
        public void Teardown()
        {
            this.keyMapper.Teardown();
        }
    }
}


        
