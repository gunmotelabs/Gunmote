// -----------------------------------------------------------------------------
// Modificaciones Gunmote:
// Copyright (c) Gustavo A. Lara (PapaGustavoKratos / GustavoALara).
//
// Este fichero puede conservar código original de Touchmote bajo licencia GPL.
// Las modificaciones, ampliaciones y correcciones específicas de Gunmote se
// atribuyen a Gustavo A. Lara, salvo las notas de colaboración indicadas.
// -----------------------------------------------------------------------------
using MahApps.Metro.Controls;
using Microsoft.Win32; // +++ AÑADIDO: Necesario para OpenFileDialog
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WiiTUIO.Properties;
using static WiiTUIO.Resources.Resources;

namespace WiiTUIO
{
    /// <summary>
    /// Interaction logic for KeymapConfigWindow.xaml
    /// </summary>
    public partial class KeymapConfigWindow : MetroWindow
    {
        public Action OnConfigChanged;

        private AdornerLayer adornerLayer;
        private KeymapOutputType selectedOutput = KeymapOutputType.ALL;
        private int selectedWiimote = 0;
        private Keymap currentKeymap;

        private SolidColorBrush defaultBrush = new SolidColorBrush(Color.FromRgb(46, 46, 46));
        private SolidColorBrush highlightBrush = new SolidColorBrush(Color.FromRgb(65, 177, 225));

        private HookApplicationViewModel hookAppVM;

        private static KeymapConfigWindow defaultInstance;
        public static KeymapConfigWindow Instance
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new KeymapConfigWindow();
                }
                return defaultInstance;
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a BrowseHookAppEntry_Click.
        /// </summary>
        private void BrowseHookAppEntry_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null) return;

            HookApplicationViewModel.HookAppDataItem item = element.Tag as HookApplicationViewModel.HookAppDataItem;
            if (item == null) return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = KeymapConfig_SelectExecutableTitle; // "Seleccionar ejecutable";
            openFileDialog.Filter = KeymapConfig_ExecutableFilter; // "Aplicaciones ejecutables (*.exe)|*.exe|Todos los archivos (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                // Guardar la ruta completa del ejecutable
                item.SearchString = openFileDialog.FileName;

                // Guardar el cambio en la base de datos del keymap
                hookAppVM.UpdateKeymapDatabase(this.currentKeymap);
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a KeymapConfigWindow.
        /// </summary>
        public KeymapConfigWindow()
        {
            hookAppVM = new HookApplicationViewModel();

            InitializeComponent();

            this.tbKeymapTitle.Text = this.tbKeymapTitle.Tag.ToString();
            this.tbKeymapTitle.Foreground = new SolidColorBrush(Colors.Gray);
            this.tbOutputFilter.Text = this.tbOutputFilter.Tag.ToString();
            this.tbOutputFilter.Foreground = new SolidColorBrush(Colors.Gray);


            this.btnAll.IsEnabled = false;
            btnAllBorder.Background = highlightBrush;

            this.tbKeymapTitle.LostFocus += tbKeymapTitle_LostFocus;
            this.tbKeymapTitle.KeyUp += tbKeymapTitle_KeyUp;
            this.tbKeymapTitle.Foreground = new SolidColorBrush(Colors.Black);

            hookAppItemsControl.DataContext = hookAppVM;

            this.cbLayoutChooser.Checked += cbLayoutChooser_Checked;
            this.cbLayoutChooser.Unchecked += cbLayoutChooser_Unchecked;

            this.cbApplicationSearch.Checked += cbApplicationSearch_Checked;
            this.cbApplicationSearch.Unchecked += cbApplicationSearch_Unchecked;

            this.rbOnscreen.Checked += rbOnscreen_Checked;
            this.rbOffscreen.Checked += rbOffscreen_Checked;

        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a OnInitialized.
        /// </summary>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            rbOnscreen.IsChecked = true;

            this.fillOutputList(selectedOutput, null);

            List<Keymap> allKeymaps = KeymapDatabase.Current.getAllKeymaps();
            // Only need to grab default keymap filename once. Was in the loop
            string defaultKeymapFilename = KeymapDatabase.Current.getKeymapSettings().getDefaultKeymap();
            foreach (Keymap keymap in allKeymaps)
            {
                if (keymap.Filename == defaultKeymapFilename)
                {
                    this.selectKeymap(keymap);
                }
            }

            this.fillKeymapList(allKeymaps);
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a fillKeymapList.
        /// </summary>
        private void fillKeymapList(List<Keymap> allKeymaps)
        {
            this.spLayoutList.Children.Clear();
            foreach (Keymap keymap in allKeymaps)
            {
                if (keymap.Filename != KeymapDatabase.Current.getKeymapSettings().getCalibrationKeymap() &&
                        keymap.Filename != KeymapDatabase.Current.getKeymapSettings().getFinishCalibrationKeymap()) //Hide calibration keymap from config window
                {
                    bool active = this.currentKeymap.Filename == keymap.Filename;
                    bool defaultk = keymap.Filename == KeymapDatabase.Current.getKeymapSettings().getDefaultKeymap();
                    KeymapRow row = new KeymapRow(keymap, active, defaultk);
                    row.OnClick += selectKeymap;
                    this.spLayoutList.Children.Add(row);
                }
            }
        }
        /// <summary>
        /// Procesa una orden de salida de ArcadeOutputs y la traduce a una acción de Gunmote.
        /// </summary>
        private void output_OnDragStop(Adorner obj)
        {
            this.adornerLayer.Remove(obj);
        }

        private void output_OnDragStart(Adorner obj)
        {
            if (this.adornerLayer == null)
            {
                this.adornerLayer = AdornerLayer.GetAdornerLayer(this.mainPanel);
            }
            if (!this.adornerLayer.GetChildObjects().Contains(obj))
            {
                this.adornerLayer.Add(obj);
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a selectWiimoteNumber.
        /// </summary>
        private void selectWiimoteNumber(int number)
        {
            this.selectedWiimote = number;
            this.fillConnectionLists(currentKeymap, number);
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a selectKeymap.
        /// </summary>

        private void selectKeymap(Keymap keymap)
        {
            this.currentKeymap = keymap;

            this.tbKeymapTitle.Text = keymap.getName();

            hookAppItemsControl.DataContext = null;
            hookAppVM.SearchStrings.Clear();
            hookAppVM.GenerateStringsForKeymap(this.currentKeymap);
            hookAppItemsControl.DataContext = hookAppVM;

            this.cbApplicationSearch.IsChecked = KeymapDatabase.Current.getKeymapSettings().isInApplicationSearch(this.currentKeymap);
            this.cbLayoutChooser.IsChecked = KeymapDatabase.Current.getKeymapSettings().isInLayoutChooser(this.currentKeymap);

            this.tbDelete.Visibility = this.currentKeymap.Filename == KeymapDatabase.Current.getKeymapSettings().getDefaultKeymap() ? Visibility.Hidden : Visibility.Visible;

            this.fillConnectionLists(keymap, 0);

            this.fillKeymapList(KeymapDatabase.Current.getAllKeymaps());
        }
        /// <summary>
        /// Abre o prepara la conexión necesaria para comunicar Gunmote con el componente externo asociado.
        /// </summary>
        private void appendConnectionList(List<KeymapInput> list, Keymap keymap, int wiimote, bool defaultKeymap, Panel container)
        {
            foreach (KeymapInput input in list)
            {
                KeymapOutConfig config = keymap.getConfigFor(wiimote, input.Key);
                if (config != null)
                {
                    KeymapConnectionRow row = new KeymapConnectionRow(input, config, defaultKeymap);
                    row.OnConfigChanged += connectionRow_OnConfigChanged;
                    row.OnDragStart += output_OnDragStart;
                    row.OnDragStop += output_OnDragStop;
                    container.Children.Add(row);
                }
            }
        }
        /// <summary>
        /// Abre o prepara la conexión necesaria para comunicar Gunmote con el componente externo asociado.
        /// </summary>
        private void fillConnectionLists(Keymap keymap, int wiimote)
        {
            bool defaultKeymap = wiimote == 0 && keymap.Filename == KeymapDatabase.Current.getDefaultKeymap().Filename;

            this.spWiimoteConnections.Children.Clear();
            this.spNunchukConnections.Children.Clear();
            this.spClassicConnections.Children.Clear();

            // Need to force garbage collection on a switch
            GC.Collect(2, GCCollectionMode.Forced);

            this.appendConnectionList(KeymapDatabase.Current.getAvailableInputs(KeymapInputSource.IR, rbOnscreen.IsChecked ?? false), keymap, wiimote, defaultKeymap, this.spWiimoteConnections);
            this.appendConnectionList(KeymapDatabase.Current.getAvailableInputs(KeymapInputSource.WIIMOTE, rbOnscreen.IsChecked ?? false), keymap, wiimote, defaultKeymap, this.spWiimoteConnections);
            this.appendConnectionList(KeymapDatabase.Current.getAvailableInputs(KeymapInputSource.NUNCHUK, rbOnscreen.IsChecked ?? false), keymap, wiimote, defaultKeymap, this.spNunchukConnections);
            this.appendConnectionList(KeymapDatabase.Current.getAvailableInputs(KeymapInputSource.CLASSIC, rbOnscreen.IsChecked ?? false), keymap, wiimote, defaultKeymap, this.spClassicConnections);

        }
        /// <summary>
        /// Abre o prepara la conexión necesaria para comunicar Gunmote con el componente externo asociado.
        /// </summary>
        private void connectionRow_OnConfigChanged(KeymapInput input, KeymapOutConfig config)
        {
            this.currentKeymap.setConfigFor(this.selectedWiimote, input, config);
            if (OnConfigChanged != null)
            {
                OnConfigChanged();
            }
        }
        /// <summary>
        /// Procesa una orden de salida de ArcadeOutputs y la traduce a una acción de Gunmote.
        /// </summary>
        private void cbOutput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbOutput.SelectedItem != null && ((ComboBoxItem)cbOutput.SelectedItem).Content != null)
            {
                ComboBoxItem cbItem = (ComboBoxItem)cbOutput.SelectedItem;
                if (cbItem == cbiAll)
                {
                    this.selectedOutput = KeymapOutputType.ALL;
                }
                else if (cbItem == cbiKeyboard)
                {
                    this.selectedOutput = KeymapOutputType.KEYBOARD;
                }
                else if (cbItem == cbiMouse)
                {
                    this.selectedOutput = KeymapOutputType.MOUSE;
                }
                else if (cbItem == cbi360)
                {
                    this.selectedOutput = KeymapOutputType.XINPUT;
                }
                else if (cbItem == cbiWiimote)
                {
                    this.selectedOutput = KeymapOutputType.WIIMOTE;
                }
                else if (cbItem == cbiCursor)
                {
                    this.selectedOutput = KeymapOutputType.CURSOR;
                }
                else if (cbItem == cbiOther)
                {
                    this.selectedOutput = KeymapOutputType.DISABLE;
                }
                this.fillOutputList(this.selectedOutput, "");
            }
        }
        /// <summary>
        /// Procesa una orden de salida de ArcadeOutputs y la traduce a una acción de Gunmote.
        /// </summary>
        private void fillOutputList(KeymapOutputType type, string filter)
        {
            this.spOutputList.Children.Clear();
            List<KeymapOutput> allOutputs = KeymapDatabase.Current.getAvailableOutputs(type);
            allOutputs.Sort(new KeymapOutputComparer());

            foreach (KeymapOutput output in allOutputs)
            {
                if (filter == null || filter == "" || output.Name.ToLower().Contains(filter.ToLower()))
                {
                    KeymapOutputRow row = new KeymapOutputRow(output);
                    row.OnDragStart += output_OnDragStart;
                    row.OnDragStop += output_OnDragStop;
                    this.spOutputList.Children.Add(row);
                }
            }
        }
        /// <summary>
        /// Procesa una orden de salida de ArcadeOutputs y la traduce a una acción de Gunmote.
        /// </summary>
        private void tbOutputFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.tbOutputFilter.Text == this.tbOutputFilter.Tag.ToString())
            {
                this.fillOutputList(selectedOutput, null);
            }
            else
            {
                this.fillOutputList(selectedOutput, this.tbOutputFilter.Text);
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a tb_placeholder_GotFocus.
        /// </summary>
        private void tb_placeholder_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox tb = (TextBox)sender;
                if (tb.Text == tb.Tag.ToString())
                {
                    tb.Text = "";
                    tb.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a tb_placeholder_LostFocus.
        /// </summary>
        private void tb_placeholder_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox tb = (TextBox)sender;
                if (tb.Text == "")
                {
                    tb.Text = tb.Tag.ToString();
                    tb.Foreground = new SolidColorBrush(Colors.Gray);
                }
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a tbKeymapTitle_LostFocus.
        /// </summary>
        private void tbKeymapTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbKeymapTitle.Text != "" && tbKeymapTitle.Text != tbKeymapTitle.Tag.ToString())
            {
                this.currentKeymap.setName(this.tbKeymapTitle.Text);
                this.fillKeymapList(KeymapDatabase.Current.getAllKeymaps());
            }
        }

        void tbKeymapTitle_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (tbKeymapTitle.Text != "" && tbKeymapTitle.Text != tbKeymapTitle.Tag.ToString())
                {
                    this.currentKeymap.setName(this.tbKeymapTitle.Text);
                    this.fillKeymapList(KeymapDatabase.Current.getAllKeymaps());
                }
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a btnAll_Click.
        /// </summary>
        private void btnAll_Click(object sender, RoutedEventArgs e)
        {
            btnAll.IsEnabled = false;
            btnAllBorder.Background = highlightBrush;
            btn1.IsEnabled = true;
            btn1Border.Background = defaultBrush;
            btn2.IsEnabled = true;
            btn2Border.Background = defaultBrush;
            btn3.IsEnabled = true;
            btn3Border.Background = defaultBrush;
            btn4.IsEnabled = true;
            btn4Border.Background = defaultBrush;
            this.selectWiimoteNumber(0);
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a btn1_Click.
        /// </summary>
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            btnAll.IsEnabled = true;
            btnAllBorder.Background = defaultBrush;
            btn1.IsEnabled = false;
            btn1Border.Background = highlightBrush;
            btn2.IsEnabled = true;
            btn2Border.Background = defaultBrush;
            btn3.IsEnabled = true;
            btn3Border.Background = defaultBrush;
            btn4.IsEnabled = true;
            btn4Border.Background = defaultBrush;
            this.selectWiimoteNumber(1);
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a btn2_Click.
        /// </summary>
        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            btnAll.IsEnabled = true;
            btnAllBorder.Background = defaultBrush;
            btn1.IsEnabled = true;
            btn1Border.Background = defaultBrush;
            btn2.IsEnabled = false;
            btn2Border.Background = highlightBrush;
            btn3.IsEnabled = true;
            btn3Border.Background = defaultBrush;
            btn4.IsEnabled = true;
            btn4Border.Background = defaultBrush;
            this.selectWiimoteNumber(2);
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a btn3_Click.
        /// </summary>
        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            btnAll.IsEnabled = true;
            btnAllBorder.Background = defaultBrush;
            btn1.IsEnabled = true;
            btn1Border.Background = defaultBrush;
            btn2.IsEnabled = true;
            btn2Border.Background = defaultBrush;
            btn3.IsEnabled = false;
            btn3Border.Background = highlightBrush;
            btn4.IsEnabled = true;
            btn4Border.Background = defaultBrush;
            this.selectWiimoteNumber(3);
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a btn4_Click.
        /// </summary>
        private void btn4_Click(object sender, RoutedEventArgs e)
        {
            btnAll.IsEnabled = true;
            btnAllBorder.Background = defaultBrush;
            btn1.IsEnabled = true;
            btn1Border.Background = defaultBrush;
            btn2.IsEnabled = true;
            btn2Border.Background = defaultBrush;
            btn3.IsEnabled = true;
            btn3Border.Background = defaultBrush;
            btn4.IsEnabled = false;
            btn4Border.Background = highlightBrush;
            this.selectWiimoteNumber(4);
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a cbLayoutChooser_Checked.
        /// </summary>
        private void cbLayoutChooser_Checked(object sender, RoutedEventArgs e)
        {
            KeymapDatabase.Current.getKeymapSettings().addToLayoutChooser(this.currentKeymap);
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a cbLayoutChooser_Unchecked.
        /// </summary>
        private void cbLayoutChooser_Unchecked(object sender, RoutedEventArgs e)
        {
            KeymapDatabase.Current.getKeymapSettings().removeFromLayoutChooser(this.currentKeymap);
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a cbApplicationSearch_Checked.
        /// </summary>
        private void cbApplicationSearch_Checked(object sender, RoutedEventArgs e)
        {
            // Need blank string to add profile to application search list
            KeymapDatabase.Current.getKeymapSettings().addToApplicationSearch(this.currentKeymap, "");
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a cbApplicationSearch_Unchecked.
        /// </summary>
        private void cbApplicationSearch_Unchecked(object sender, RoutedEventArgs e)
        {
            // Keep events from updating UI controls for now
            hookAppItemsControl.DataContext = null;

            hookAppVM.ClearApplicationList(this.currentKeymap);
            // Update UI with updated data
            hookAppItemsControl.DataContext = hookAppVM;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a tbDelete_MouseUp.
        /// </summary>
        private void tbDelete_MouseUp(object sender, MouseButtonEventArgs e)
        {
            /*if (MessageBoxResult.Yes == MessageBox.Show("Esto borrará permanentemente el fichero " + this.currentKeymap.Filename + ", ¿estás seguro?", "Confirmación de borrado de keymap", MessageBoxButton.YesNo, MessageBoxImage.Warning))
            {
                if (KeymapDatabase.Current.deleteKeymap(this.currentKeymap))
                {
                    this.selectKeymap(KeymapDatabase.Current.getDefaultKeymap());
                }
            } */
            if (MessageBoxResult.Yes == MessageBox.Show(
            string.Format(KeymapConfig_DeleteConfirmMessage, this.currentKeymap.Filename),
            KeymapConfig_DeleteConfirmTitle,
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning))
            {
                if (KeymapDatabase.Current.deleteKeymap(this.currentKeymap))
                {
                    this.selectKeymap(KeymapDatabase.Current.getDefaultKeymap());
                }
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a tbClone_MouseUp.
        /// </summary>
        private void tbClone_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.currentKeymap == null) return;

            // No tiene sentido clonar calibración/preview si no quieres que aparezcan perfiles raros
            string calib = KeymapDatabase.Current.getKeymapSettings().getCalibrationKeymap();
            string calibPrev = KeymapDatabase.Current.getKeymapSettings().getFinishCalibrationKeymap();
            if (this.currentKeymap.Filename == calib || this.currentKeymap.Filename == calibPrev)
                return;

            string srcFilename = this.currentKeymap.Filename;
            string srcPath = Path.Combine(Settings.Default.keymaps_path, srcFilename);

            if (!File.Exists(srcPath)) return;

            string newFilename = GenerateCloneFilename(srcFilename);
            string dstPath = Path.Combine(Settings.Default.keymaps_path, newFilename);

            File.Copy(srcPath, dstPath);

            // Mantén el mismo Parent para conservar la herencia tal como estaba
            Keymap cloned = new Keymap(this.currentKeymap.Parent, newFilename);

            // Cambia el título visible del perfil (y guarda)
            cloned.setName(this.currentKeymap.getName() + " " + KeymapConfig_CopySuffix);

            // Selecciona el clon y refresca UI (selectKeymap ya vuelve a rellenar la lista)
            this.selectKeymap(cloned);
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a GenerateCloneFilename.
        /// </summary>
        private string GenerateCloneFilename(string originalFilename)
        {
            string baseName = Path.GetFileNameWithoutExtension(originalFilename);
            string ext = Path.GetExtension(originalFilename);

            // estilo: MiPerfil_copy.json, MiPerfil_copy_2.json, etc.
            string candidate = $"{baseName}_copy{ext}";
            int i = 2;

            while (File.Exists(Path.Combine(Settings.Default.keymaps_path, candidate)))
            {
                candidate = $"{baseName}_copy_{i}{ext}";
                i++;
            }

            return candidate;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a bAddKeymap_MouseUp.
        /// </summary>
        private void bAddKeymap_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Keymap newKeymap = KeymapDatabase.Current.createNewKeymap();
            selectKeymap(newKeymap);
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a OnClosing.
        /// </summary>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = System.Windows.Visibility.Collapsed;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a HookApp_TextBox_LostFocus.
        /// </summary>
        private void HookApp_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            hookAppVM.UpdateKeymapDatabase(this.currentKeymap);
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a HookApp_TextBox_KeyUp.
        /// </summary>
        private void HookApp_TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                hookAppVM.UpdateKeymapDatabase(this.currentKeymap);
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a AddHookAppEntry.
        /// </summary>
        private void AddHookAppEntry(object sender, MouseButtonEventArgs e)
        {
            hookAppVM.AddNewSearchString();
        }
        /// <summary>
        /// Desinstala o limpia controladores virtuales de Gunmote dejando solo los dispositivos requeridos.
        /// </summary>
        private void RemoveHookAppEntry(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            HookApplicationViewModel.HookAppDataItem item =
                element.Tag as HookApplicationViewModel.HookAppDataItem;

            hookAppVM.RemoveSearchString(item, this.currentKeymap);
        }
        /// <summary>
        /// Cierra la conexión o detiene el componente externo asociado de forma controlada.
        /// </summary>
        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            hookAppItemsControl.DataContext = null;
        }
        /// <summary>
        /// Actualiza los límites de pantalla cuando cambia la configuración de monitores de Windows.
        /// </summary>
        private void rbOnscreen_Checked(object sender, RoutedEventArgs e)
        {
            rbOffscreen.IsChecked = false;
            this.fillConnectionLists(currentKeymap, this.selectedWiimote);
        }
        /// <summary>
        /// Actualiza los límites de pantalla cuando cambia la configuración de monitores de Windows.
        /// </summary>
        private void rbOffscreen_Checked(object sender, RoutedEventArgs e)
        {
            rbOnscreen.IsChecked = false;
            this.fillConnectionLists(currentKeymap, this.selectedWiimote);
        }
    }
    /// <summary>
    /// Implementa la lógica específica de Gunmote asociada a HookApplicationViewModel.
    /// </summary>
    public class HookApplicationViewModel
    {
        public class HookAppDataItem : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            /// <summary>
            /// Implementa la lógica específica de Gunmote asociada a OnPropertyChanged.
            /// </summary>
            protected void OnPropertyChanged(string name)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
            private string searchString;
            public string SearchString
            {
                get => searchString;
                set
                {
                    // --- MODIFICADO: Añadida comprobación y notificación ---
                    if (searchString == value) return; // Evitar notificaciones innecesarias
                    searchString = value;
                    Placeholder = false;
                    OnPropertyChanged("SearchString"); // ¡Notificar a la UI!
                    // --- FIN DE LA MODIFICACIÓN ---
                }
            }

            private bool placeholder;
            public bool Placeholder
            {
                get => placeholder;
                set
                {
                    if (placeholder == value) return;
                    placeholder = value;

                    PlaceholderChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            public event EventHandler PlaceholderChanged;

            public SolidColorBrush BrushColor
            {
                get
                {
                    SolidColorBrush brush = null;
                    if (!placeholder)
                    {
                        brush = new SolidColorBrush(Colors.Black);
                    }
                    if (placeholder)
                    {
                        brush = new SolidColorBrush(Colors.Gray);
                    }

                    return brush;
                }
            }
            public event EventHandler BrushColorChanged;

            private int index;
            public int Index
            {
                get => index;
                set => index = value;
            }

            /// <summary>
            /// Implementa la lógica específica de Gunmote asociada a HookAppDataItem.

            /// </summary>
            public HookAppDataItem()
            {
                PlaceholderChanged += HookAppDataItem_PlaceholderChanged;
            }

            /// <summary>
            /// Implementa la lógica específica de Gunmote asociada a HookAppDataItem.

            /// </summary>
            public HookAppDataItem(string searchString, bool placeholder) : this()
            {
                this.searchString = searchString;
                this.placeholder = placeholder;
            }

            /// <summary>
            /// Implementa la lógica específica de Gunmote asociada a HookAppDataItem_PlaceholderChanged.

            /// </summary>
            private void HookAppDataItem_PlaceholderChanged(object sender, EventArgs e)
            {
                BrushColorChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        //private const string SEARCH_STRING_HELPTEXT = "Cadena de búsqueda (procesa el nombre o título de la ventana, p.ej. example.exe)";
        private static string SEARCH_STRING_HELPTEXT => KeymapConfig_SearchStringHelpText;
        private ObservableCollection<HookAppDataItem> searchStrings = new ObservableCollection<HookAppDataItem>();
        public ObservableCollection<HookAppDataItem> SearchStrings => searchStrings;
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a HookApplicationViewModel.
        /// </summary>
        public HookApplicationViewModel()
        {
            searchStrings.CollectionChanged += SearchStrings_CollectionChanged;
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a SearchStrings_CollectionChanged.
        /// </summary>
        private void SearchStrings_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                for (int i = e.OldStartingIndex; i < searchStrings.Count; i++)
                {
                    searchStrings[i].Index = i;
                }
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a GenerateStringsForKeymap.
        /// </summary>
        public void GenerateStringsForKeymap(Keymap keymap)
        {
            string searchstring = KeymapDatabase.Current.getKeymapSettings().getSearchStringFor(keymap);
            if (searchstring != null && searchstring != "")
            {
                List<string> tempList = searchstring.Split(Convert.ToChar(31)).ToList();
                int index = 0;
                foreach (string hookString in tempList)
                {
                    searchStrings.Add(
                        new HookAppDataItem(hookString, false)
                        {
                            Index = index,
                        });

                    index++;
                }
            }
            else
            {
                searchStrings.Add(
                    new HookAppDataItem(SEARCH_STRING_HELPTEXT, true)
                    {
                        Index = 0,
                    });
            }
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a AddNewSearchString.
        /// </summary>
        public void AddNewSearchString()
        {
            HookAppDataItem item = new HookAppDataItem("", false)
            {
                Index = searchStrings.Count,
            };

            searchStrings.Add(item);
        }
        /// <summary>
        /// Desinstala o limpia controladores virtuales de Gunmote dejando solo los dispositivos requeridos.
        /// </summary>
        public void RemoveSearchString(HookAppDataItem item, Keymap keymap)
        {
            int count = searchStrings.Count;
            if (count <= 1)
            {
                return;
            }

            int index = searchStrings.IndexOf(item);
            //int index = item.Index;
            searchStrings.RemoveAt(index);
            UpdateKeymapDatabase(keymap);
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a UpdateKeymapDatabase.
        /// </summary>
        public void UpdateKeymapDatabase(Keymap keymap)
        {
            StringBuilder sb = new StringBuilder();
            int count = searchStrings.Count;
            int idx = 0;
            foreach (HookAppDataItem item in searchStrings)
            {
                if (!string.IsNullOrEmpty(item.SearchString) &&
                    !item.Placeholder)
                {
                    sb.Append(item.SearchString);

                    if (idx < count - 1)
                    {
                        sb.Append(Convert.ToChar(31));
                    }
                }

                idx++;
            }

            KeymapDatabase.Current.getKeymapSettings().setSearchStringFor(keymap,
                sb.ToString());
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a AddToApplicationSearch.
        /// </summary>
        public void AddToApplicationSearch(Keymap keymap)
        {
            // Need blank string to add profile to application search list
            KeymapDatabase.Current.getKeymapSettings().addToApplicationSearch(keymap, "");
            // Possibly update with current data
            //UpdateKeymapDatabase(keymap);
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a ClearApplicationList.
        /// </summary>
        public void ClearApplicationList(Keymap keymap)
        {
            KeymapDatabase.Current.getKeymapSettings().removeFromApplicationSearch(keymap);
            searchStrings.Clear();
            searchStrings.Add(
                new HookAppDataItem(SEARCH_STRING_HELPTEXT, true)
                {
                    Index = 0,
                });
        }
    }
}