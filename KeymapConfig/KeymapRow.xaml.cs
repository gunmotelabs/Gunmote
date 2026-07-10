// -----------------------------------------------------------------------------
// Modificaciones Gunmote:
// Copyright (c) Gustavo A. Lara (PapaGustavoKratos / GustavoALara).
//
// Este fichero puede conservar código original de Touchmote bajo licencia GPL.
// Las modificaciones, ampliaciones y correcciones específicas de Gunmote se
// atribuyen a Gustavo A. Lara, salvo las notas de colaboración indicadas.
// -----------------------------------------------------------------------------
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
using WiiTUIO.Provider;

namespace WiiTUIO
{
    /// <summary>
    /// Interaction logic for LayoutSelectionRow.xaml
    /// </summary>
    public partial class KeymapRow : UserControl
    {
        private Keymap keymap;

        private SolidColorBrush defaultBrush = new SolidColorBrush(Color.FromRgb(46, 46, 46));
        private SolidColorBrush highlightBrush = new SolidColorBrush(Color.FromRgb(65, 177, 225));

        public Action<Keymap> OnClick; //filename
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a KeymapRow.
        /// </summary>
        public KeymapRow(Keymap keymap, bool active, bool defaultk)
        {
            InitializeComponent();
            this.keymap = keymap;
            this.tbName.Text = keymap.getName();

            if (active)
            {
                this.border.Background = highlightBrush;
            }
            else
            {
                this.border.MouseUp += border_MouseUp;
                this.border.Cursor = Cursors.Hand;
            }
            if (defaultk)
            {
                this.tbDefault.Visibility = Visibility.Visible;
            }

        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a border_MouseUp.
        /// </summary>
        private void border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (OnClick != null)
            {
                OnClick(this.keymap);
            }
        }



    }
}
