using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DirectX.Capture;
using DShowNET;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace VideoWallProject
{
    public partial class PanelPreview : Form
    {
        Filters filters = new Filters();
        Capture capture;
        MainWindow mainWindow;
        DispatcherTimer timerPreview = new DispatcherTimer();

        public PanelPreview(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        /// <summary>
        /// Preview et enregistrement des videos
        /// </summary>
        public void saveVideo()
        {
            capture = new Capture(filters.VideoInputDevices[0], null);
            Filter f = null;
            for (int i = 0; i < filters.VideoCompressors.Count; i++)
            {
                Console.WriteLine(filters.VideoCompressors[i].Name.ToString());
            }
            try
            {
                if (capture == null)
                    throw new ApplicationException("Pas de périphériques audios ou vidéos détectés.\n\n");

                if (!capture.Cued)
                {
                    f = filters.VideoCompressors[7];
                    capture.VideoCompressor = f;
                    capture.FrameSize = new System.Drawing.Size(640, 480);
                    capture.Filename = AppDomain.CurrentDomain.BaseDirectory + "Videos/savedVideo.wmv";
                    capture.PreviewWindow = panelVideo;
                }
                capture.Start();
                progressBar();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Erreur :" + ex.Message + "\n\n" + ex.ToString());
            }
        }

        /// <summary>
        /// Progress bar indiquand le temps restant pour l'enregistrement. La durée de la vidéo est déterminée par le maximum de la progressBar.
        /// </summary>
        private void progressBar()
        {
            //progressBarPreview = new System.Windows.Forms.ProgressBar();
            progressBarPreview.Minimum = 0;
            progressBarPreview.Maximum = 515;
            timerPreview.Interval = TimeSpan.FromMilliseconds(20);
            timerPreview.Tick += new EventHandler(progressTickPreview);
            timerPreview.Start();
        }

        /// <summary>
        /// Pour progress bar, la retirer quand la video de 15 secondes a ete enregistree
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void progressTickPreview(object sender, EventArgs e)
        {
            progressBarPreview.Value += 1;
            if (progressBarPreview.Value == 514)
            {
                mainWindow.gridMain.Children.Remove(mainWindow.fade);
                timerPreview.Stop();
                capture.Stop();
                this.Close();
            }
        }
    }
}