using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

namespace VideoWallProject
{
    class MaintenanceVideo
    {
        MainWindow mainWindow;
        MediaElement[] videos;
        int currentNumberOfVideos;
        DispatcherTimer timer10minutes;
        int[] ratingVideos = new int[16];
        int[] favorites = new int[16];


        /// <summary>
        /// Cette classe s'occupe de la lecture des vidéos et également de ce qu'on appelle la maintenance.
        /// Elle s'occupe de supprimer les vidéos au fur et à mesure que le temps passe pour que l'oeuvre soit éphemère.
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="mainWindow"></param>
        public MaintenanceVideo(MediaElement[] videos, MainWindow mainWindow)
        {
            this.videos = videos;
            this.mainWindow = mainWindow;
            currentNumberOfVideos = 14;
            playAllVideos();
            Random rnd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < 16; i++)
            {
                favorites[i] = rnd.Next(0,3);
                ratingVideos[i] = rnd.Next(10, 19);
            }
            ratingVideos[5] = 0;
            ratingVideos[11] = 0;
            timer10minutes = new DispatcherTimer();
            timer10minutes.Tick += new EventHandler(tickSuppression);
            timer10minutes.Interval = new TimeSpan(0, 0, 7, 0, 0);
            timer10minutes.Start();
        }

        /// <summary>
        /// Augmenter le nombre d'etoiles pour indiquer que la video a ete aimee. On augmente également son rating pour qu'elle dure plus longtemps.
        /// </summary>
        /// <param name="videoNumber"></param>
        /// <returns></returns>
        public bool increaseRating(int videoNumber)
        {
            if (favorites[videoNumber] < 3)
            {
                favorites[videoNumber]++;
                ratingVideos[videoNumber] += 20;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Lorsqu'une vidéo est ajoutée, on réinitialise ses valeurs et on la joue. Si le nombre de vidéos était déjà à son maximum, on supprime la vidéo avec le moins bon rating.
        /// </summary>
        /// <param name="videoNumber">Numero de la video</param>
        public void addVideo(int videoNumber)
        {
            favorites[videoNumber] = 0;
            ratingVideos[videoNumber] = 100;
            currentNumberOfVideos++;
            videos[videoNumber].Play();
            if (currentNumberOfVideos > 14)
            {
                tickSuppression(null, null);
            }

        }

        /// <summary>
        /// Recupere les videos favories
        /// </summary>
        /// <param name="videoNumber"></param>
        /// <returns></returns>
        public int getFavorite(int videoNumber)
        {
            return favorites[videoNumber];
        }

        /// <summary>
        /// Suppression des videos les moins aimees
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tickSuppression(object sender, EventArgs e)
        {
            if (mainWindow.getSaving() == false)
            {
                int lowestRating = 999999;
                int ratingNumber = -1;
                if (currentNumberOfVideos > 10)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        if (ratingVideos[i] != 0)
                        {
                            ratingVideos[i]++;
                            if (lowestRating > ratingVideos[i])
                            {
                                lowestRating = ratingVideos[i];
                                ratingNumber = i;
                                Console.WriteLine("Video minimale : " + ratingNumber + " avec un rating de " + lowestRating);
                            }
                        }

                    }
                    ratingVideos[ratingNumber] = 0;
                    favorites[ratingNumber] = 0;
                    stopVideo(videos[ratingNumber]);
                    mainWindow.opacityBlack(ratingNumber);
                    currentNumberOfVideos--;
                }
                else
                {
                    Console.WriteLine("Pas de suppression, limite de vidéo atteinte");
                }
            }
        }

        /// <summary>
        /// Redemarrer les videos
        /// </summary>
        /// <param name="video">Mur video</param>
        public void videoRestart(MediaElement video)
        {
            video.Stop();
            video.Play();
        }

        /// <summary>
        /// Jouer toutes les videos
        /// </summary>
        public void playAllVideos()
        {
            for (int i = 0; i < 16; i++)
            {
                videos[i].Play();
            }
        }

        /// <summary>
        /// Arreter toutes les videos
        /// </summary>
        public void stopAllVideos()
        {
            for (int i = 0; i < 16; i++)
            {
                videos[i].Stop();
            }
        }

        /// <summary>
        /// Arreter la video desiree (pendant le zoom avec croix rouge)
        /// </summary>
        /// <param name="media"></param>
        public void stopVideo(MediaElement media)
        {
            media.Stop();
        }
    }
}
