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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class WiimoteStatusUC : StackPanel
    {
        public int ID;
        public int battery;
        public bool powersave;
        

        public WiimoteStatusUC(int id)
        {
            InitializeComponent();
            this.ID = id;
            this.lbId.Text = ""+id;
            this.setBattery(0);
            this.color.Stroke = new SolidColorBrush(IDColor.getColor(id));
        }
        /// <summary>
        /// Implementa la lógica específica de Gunmote asociada a updateStatus.
        /// </summary>
        public void updateStatus(WiimoteStatus status)
        {
            //this.lbId.Content = "" + status.ID;
            if (this.battery != status.Battery)
            {
                this.battery = status.Battery;
                this.setBattery(status.Battery);
            }
            if (this.powersave != status.InPowerSave)
            {
                this.powersave = status.InPowerSave;
                if (status.InPowerSave)
                {
                    this.lbStatus.Foreground = Brushes.White;
                    this.lbStatus.Content = "sleepmode";
                }
                else
                {
                    this.lbStatus.Foreground = new SolidColorBrush(Color.FromRgb(91,255,0));
                    this.lbStatus.Content = "connected";
                }
            }
        }
        /// <summary>
        /// Actualiza un valor interno utilizado por la lógica específica de Gunmote.
        /// </summary>
        public void setBattery(int percentage) {
            Brush light = percentage < 20 ? Brushes.Red : Brushes.White;
            Brush dark = new SolidColorBrush(Color.FromRgb(64, 64, 64));
            this.battery1.Fill = percentage > 10 ? light : dark;
            this.battery2.Fill = percentage > 20 ? light : dark;
            this.battery3.Fill = percentage > 30 ? light : dark;
            this.battery4.Fill = percentage > 40 ? light : dark;
            this.battery5.Fill = percentage > 50 ? light : dark;
            this.battery6.Fill = percentage > 70 ? light : dark;
        }
    }
}
