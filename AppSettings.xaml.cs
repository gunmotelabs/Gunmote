// -----------------------------------------------------------------------------
// Modificaciones Gunmote:
// Copyright (c) Gustavo A. Lara (PapaGustavoKratos / GustavoALara).
//
// Este fichero puede conservar código original de Touchmote bajo licencia GPL.
// Las modificaciones, ampliaciones y correcciones específicas de Gunmote se
// atribuyen a Gustavo A. Lara, salvo las notas de colaboración indicadas.
// -----------------------------------------------------------------------------
using HidLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WiiCPP;
using WiiTUIO.DeviceUtils;
using WiiTUIO.Output;
using WiiTUIO.Properties;
using WiiTUIO.Provider;

namespace WiiTUIO
{
    /// <summary>
    /// Interaction logic for AppSettings.xaml
    /// </summary>
    public partial class AppSettingsUC : UserControl, SubPanel
    {

        public event Action OnClose;

        public AppSettingsUC()
        {
            InitializeComponent();
            this.Initialize();
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a Initialize.
        /// </summary>
        public async void Initialize()
        {
            Settings.Default.PropertyChanged += Settings_PropertyChanged;

            this.reloadState();

        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a reloadState.
        /// </summary>
        private async void reloadState()
        {
            this.cbMinimizeOnStart.IsChecked = Settings.Default.minimizeOnStart;
            this.cbMinimizeToTray.IsChecked = Settings.Default.minimizeToTray;
            this.cbPairOnStart.IsChecked = Settings.Default.pairOnStart;
            this.providerSettingsContent.Children.Clear();
            this.providerSettingsContent.Children.Add(MultiWiiPointerProvider.getSettingsControl());

            this.cbWindowsStart.IsChecked = Autostart.IsAutostart();
        }
        /// <summary>
        /// Actualiza un valor interno utilizado por la lógica específica de Gunmote.
        /// </summary>
        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate()
            {
                this.reloadState();
            }), null);
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a cbWindowsStart_Checked.
        /// </summary>
        private async void cbWindowsStart_Checked(object sender, RoutedEventArgs e)
        {
            this.cbWindowsStart.IsChecked = Autostart.SetAutostart();
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a cbWindowsStart_Unchecked.
        /// </summary>
        private async void cbWindowsStart_Unchecked(object sender, RoutedEventArgs e)
        {
            this.cbWindowsStart.IsChecked = !(Autostart.UnsetAutostart());
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a btnAppSettingsBack_Click.
        /// </summary>
        private void btnAppSettingsBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.OnClose != null)
            {
                this.OnClose();
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a cbPairOnStart_Checked.
        /// </summary>
        private void cbPairOnStart_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.pairOnStart = true;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a cbPairOnStart_Unchecked.
        /// </summary>
        private void cbPairOnStart_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.pairOnStart = false;
        }
        /// <summary>
        /// Abre o prepara la conexión necesaria para comunicar Gunmote con el componente externo asociado.
        /// </summary>
        private void cbDisconnectOnExit_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.completelyDisconnect = true;
        }
        /// <summary>
        /// Abre o prepara la conexión necesaria para comunicar Gunmote con el componente externo asociado.
        /// </summary>
        private void cbDisconnectOnExit_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.completelyDisconnect = false;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a cbMinimizeToTray_Checked.
        /// </summary>
        private void cbMinimizeToTray_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.minimizeToTray = true;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a cbMinimizeToTray_Unchecked.
        /// </summary>
        private void cbMinimizeToTray_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.minimizeToTray = false;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a cbMinimizeOnStart_Checked.
        /// </summary>
        private void cbMinimizeOnStart_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.minimizeOnStart = true;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a cbMinimizeOnStart_Unchecked.
        /// </summary>
        private void cbMinimizeOnStart_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.minimizeOnStart = false;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a btnEditKeymaps_Click.
        /// </summary>
        private void btnEditKeymaps_Click(object sender, RoutedEventArgs e)
        {
            KeymapConfigWindow.Instance.Show();
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a btnAdvConfig_Click.
        /// </summary>
        private void btnAdvConfig_Click(object sender, RoutedEventArgs e)
        {
            AdvSettingsUC.Instance.Show(); // Llama a la instancia única de la ventana de configuración avanzada
        }

    }
}
