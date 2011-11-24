using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Threading;
using System.Windows.Forms;

//C pour calibration
//F pour favoris
//P pour accélerer
//O pour ralentir

namespace VideoWallProject
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Tableaux
        Uri[] imageFilename = new Uri[16];  //Tableau des sources des images des questions
        int[] flagQuestion = new int[16];   //Tableau pour selection des questions
        MediaElement[] videos = new MediaElement[16];   //Tableau des videos
        MediaElement[,] tabVideos = new MediaElement[4, 4];     //Ce tableau sert à retrouver les vidéos.
        Canvas[,] tabCanvas = new Canvas[4, 4];     //On stocke dans ce tableau tous les canvas avec lesquels on intéragit.

        MediaElement zoomedVideo;
        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer timer5seconds;
        DispatcherTimer timer1second;
        DispatcherTimer timerLimit;
        MaintenanceVideo maintenance;
        System.Windows.Forms.Panel panelVideo = new System.Windows.Forms.Panel();   //Panel servant pour la preview (enregistrement de la video)
        System.Windows.Controls.ProgressBar progressBar;

        public Canvas fade; //Ce canvas sert à cacher l'application lorsque l'on va se calibrer ou zoomer sur une vidéo.
        Canvas instructions;    //Ce canvas va afficher les règles, questions et countdown.
        Canvas canvasFavorite;  //Ce canvas va contenir l'étoile qui pourra changer si l'utilisateur vote pour la vidéo.
        Canvas canvasHit;   //Ce canvas stocke le canvas sur lequel la vidéo est actuellement zoomée.
        Canvas canvasOui;   //Ce canvas sert à afficher le bouton Oui
        Canvas canvasNon;   //Ce canvas sert à afficher le bouton Non

        int timeInstruction,timeCountdown;  //Ces int servent à suivre l'évolution du timer.
        bool savingVideo;   //Ce boolean permet de savoir si nous sommes en train de sauvegarder une vidéo pour adapter la réaction des canvas en conséquence. (Fonction hover par exemple)
        bool hasVoted;      //Ce boolean interdit l'utilisateur de voter plus d'une fois s'il ne se recalibre pas. 

        Grid buttonStop;    //Cette grille contient le canvas pour le bouton fermer la vidéo zoomée.
        Grid favorites;     //Cette grille contient les étoiles pleines ou vides sur une vidéo zoomée.
        bool zoom;

        bool clickLimit;

        /// <summary>
        /// Entree du programme
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            savingVideo = false;
            hasVoted = true;
            zoom = false;

            //Initialisation des videos dans le mur
            Video1_1.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Videos/video1.wmv");
            Video1_2.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Videos/video2.wmv");
            Video1_3.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Videos/video3.wmv");
            Video1_4.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Videos/video4.wmv");
            Video2_1.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Videos/video15.wmv");
            Video2_2.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Videos/video6.wmv");
            Video2_3.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Videos/video7.wmv");
            Video2_4.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Videos/video8.wmv");
            Video3_1.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Videos/video9.wmv");
            Video3_2.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Videos/video10.wmv");
            Video3_3.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Videos/video11.wmv");
            Video3_4.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Videos/video16.wmv");
            Video4_1.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Videos/video13.wmv");
            Video4_2.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Videos/video14.wmv");
            Video4_3.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Videos/video5.wmv");
            Video4_4.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Videos/video12.wmv");

            videos[0] = Video1_1;
            videos[1] = Video1_2;
            videos[2] = Video1_3;
            videos[3] = Video1_4;
            videos[4] = Video2_1;
            videos[5] = Video2_2;
            videos[6] = Video2_3;
            videos[7] = Video2_4;
            videos[8] = Video3_1;
            videos[9] = Video3_2;
            videos[10] = Video3_3;
            videos[11] = Video3_4;
            videos[12] = Video4_1;
            videos[13] = Video4_2;
            videos[14] = Video4_3;
            videos[15] = Video4_4;

            //Initialisation du tableau des questions avec les sources des images
            imageFilename[0] = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/question1.png");
            imageFilename[1] = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/question2.png");
            imageFilename[2] = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/question3.png");
            imageFilename[3] = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/question4.png");
            imageFilename[4] = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/question5.png");
            imageFilename[5] = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/question6.png");
            imageFilename[6] = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/question7.png");
            imageFilename[7] = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/question8.png");
            imageFilename[8] = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/question9.png");
            imageFilename[9] = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/question10.png");
            imageFilename[10] = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/question11.png");
            imageFilename[11] = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/question12.png");
            imageFilename[12] = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/question13.png");
            imageFilename[13] = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/question14.png");
            imageFilename[14] = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/question15.png");
            imageFilename[15] = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/question16.png");

            //Toutes les questions sont a 1, c'est à dire que la question ne peut être posée.
            for (int i = 0; i < 16; i++)
            {
                flagQuestion[i] = 1;
            }

            //Apparence du mur et initialisation des canvas, ils ont tous un nom différent et on leur y ajoute l'event pour zoomer dessus.
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Border canvasBorder = new Border();
                    canvasBorder.BorderThickness = new Thickness(3);
                    canvasBorder.BorderBrush = Brushes.Black;
                    tabCanvas[i, j] = new Canvas();
                    tabCanvas[i, j].Name = "c" + i + "_" + j;
                    tabCanvas[i, j].Background = new SolidColorBrush(Color.FromArgb(0,0,0,0));
                    tabCanvas[i, j].MouseLeftButtonDown += zoomVideo;
                    Grid.SetRow(canvasBorder, i);
                    Grid.SetColumn(canvasBorder, j);
                    Grid.SetRow(tabCanvas[i, j], i);
                    Grid.SetColumn(tabCanvas[i, j], j);
                    gridMain.Children.Add(canvasBorder);
                    gridMain.Children.Add(tabCanvas[i, j]);
                    tabVideos[i, j] = videos[4 * i + j];
                }
            }
            //Ces deux opacityBlack servent à l'initialisation pour n'avoir que 14 vidéos au départ, le nombre maximal que l'on a déterminé. Cela n'a aucune influence sur le fonctionnement.
            opacityBlack(5);
            opacityBlack(11);
            maintenance = new MaintenanceVideo(videos, this);
            maintenance.playAllVideos();
        }

        /// <summary>
        /// Relire la video une fois terminee
        /// </summary>
        /// <param name="sender">Video</param>
        /// <param name="e">Evenement</param>
        private void mediaEnded(object sender, RoutedEventArgs e)
        {
            MediaElement video = sender as MediaElement;
            maintenance.videoRestart(video);
        }
        
        //Sur un timer de dix secondes
        private void timerInstruction(object sender, EventArgs e)
        {
            //t5   Lancement du timer pour le countdown.
            if (timeInstruction == 0)
            {
                timeInstruction++;
                timeCountdown = 0;
                timer1second = new DispatcherTimer();
                timer1second.Tick += new EventHandler(timerCountdown);
                timer1second.Interval = new TimeSpan(0, 0, 0, 1, 0);
                timer1second.Start();
            }
            //t10   La question a été affichée pendant 7 secondes, et le countdown est passé. Lancement de l'enregistrement.
            else if (timeInstruction == 1)
            {
                gridMain.Children.Remove(instructions);
                timeInstruction++;
                PanelPreview preview = new PanelPreview(this);
                preview.TopMost = true;
                preview.Visible = true;
                preview.saveVideo();
            }
            //t25   Reprise de la lecture des vidéos et arrêt du timer.
            else if (timeInstruction == 4)
            {
                maintenance.playAllVideos();
                timer5seconds.Stop();
            }
            else
            {
                timeInstruction++;
            }
        }

        private void timerCountdown(object sender, EventArgs e)
        {
            //t7    Affichage de 1.
            if (timeCountdown == 1)
            {
                timeCountdown++;
                gridMain.Children.Remove(fade);
                gridMain.Children.Remove(instructions);
                fade.Opacity = 0.75;
                gridMain.Children.Add(fade);
                Uri source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/countdown3.png");
                BitmapImage instruction = new BitmapImage(source);
                instructions.Background = new ImageBrush(instruction);
                gridMain.Children.Add(instructions);
            }
            //t8   Affichage de 2.
            else if (timeCountdown == 2)
            {
                gridMain.Children.Remove(instructions);
                gridMain.Children.Remove(fade);
                fade.Opacity = 0.90;
                gridMain.Children.Add(fade);
                Uri source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/countdown2.png");
                BitmapImage instruction = new BitmapImage(source);
                instructions.Background = new ImageBrush(instruction);
                gridMain.Children.Add(instructions);
                timeCountdown++;
            }
            //t9   Affichage de 1.
            else if (timeCountdown == 3)
            {
                gridMain.Children.Remove(fade);
                fade.Opacity = 1;
                gridMain.Children.Add(fade);
                gridMain.Children.Remove(instructions);
                Uri source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/countdown1.png");
                BitmapImage instruction = new BitmapImage(source);
                instructions.Background = new ImageBrush(instruction);
                gridMain.Children.Add(instructions);
                timeCountdown++;
            }
            else
            {
                timeCountdown++;
            }
        }

        /// <summary>
        /// Lancement de l'application lors de la calibration
        /// </summary>
        private void calibration()
        {
            if (savingVideo == false)
            {
                savingVideo = true;
                timeInstruction = 0;
                fade = new Canvas();
                fade.Background = Brushes.Black;
                Grid.SetRowSpan(fade, 4);
                Grid.SetColumnSpan(fade, 4);
                fade.Opacity = 0.50;
                gridMain.Children.Add(fade);

                //Apparition de la page de bienvenue
                instructions = new Canvas();
                Grid.SetRowSpan(instructions, 2);
                Grid.SetColumnSpan(instructions, 2);
                Grid.SetRow(instructions, 1);
                Grid.SetColumn(instructions, 1);
                Uri source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/avertissement.png");
                BitmapImage instruction = new BitmapImage(source);
                instructions.Background = new ImageBrush(instruction);
                gridMain.Children.Add(instructions);

                //Ajout du bouton oui dans la page de bienvenue
                canvasOui = new Canvas();
                canvasOui.Background = new ImageBrush(new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/oui.png")));
                Grid.SetRow(canvasOui, 2);
                Grid.SetColumn(canvasOui, 1);
                canvasOui.Margin= new Thickness(150, 100, 20, 50);
                canvasOui.MouseLeftButtonDown += startGame;
                gridMain.Children.Add(canvasOui);

                //Ajout du bouton non dans la page de bienvenue
                canvasNon = new Canvas();
                canvasNon.Background = new ImageBrush(new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/non.png")));
                Grid.SetRow(canvasNon, 2);
                Grid.SetColumn(canvasNon, 2);
                canvasNon.Margin = new Thickness(20, 100, 150, 50);
                canvasNon.MouseLeftButtonDown += stopGame;
                gridMain.Children.Add(canvasNon);
            }
        }

        /// <summary>
        /// Demarrage du jeu si l'utilisateur clique sur oui : choix de la question par l'ordinateur, adaptation de l'interface en consequence
        /// </summary>
        /// <param name="sender">Clic oui</param>
        /// <param name="e">Evenement</param>
        private void startGame(object sender, EventArgs e)
        {
            gridMain.Children.Remove(instructions);
            gridMain.Children.Remove(fade);
            gridMain.Children.Remove(canvasOui);
            gridMain.Children.Remove(canvasNon);
            fade.Opacity = 0.60;
            gridMain.Children.Add(fade);
            //Random question
            Random rnd = new Random(DateTime.Now.Millisecond);
            int freeCell = 0;
            for (int i = 0; i < 16; i++)
            {
                if(i%4==3)
                {
                    Console.WriteLine(flagQuestion[i]);
                }
                else
                {
                    Console.Write(flagQuestion[i]);
                }
                if (flagQuestion[i] == 0)
                {
                    freeCell++;
                    tabCanvas[i / 4, i % 4].MouseEnter += colorCanvas;
                    tabCanvas[i / 4, i % 4].MouseLeave += uncolorCanvas;
                }
            }
            Console.WriteLine(freeCell);
            int question = rnd.Next(0, freeCell);
            Console.WriteLine(question);
            Uri source = null;
            for (int i = 0; i < 16; i++)
            {
                if (flagQuestion[i] == 0)
                {
                    if (question > 0)
                    {
                        question--;
                    }
                    else
                    {
                        source = imageFilename[i];
                        break;
                    }
                }
            }
            maintenance.stopAllVideos();
            Console.WriteLine(source.ToString());
            BitmapImage instruction = new BitmapImage(source);
            instructions.Background = new ImageBrush(instruction);
            gridMain.Children.Add(instructions);
            timer5seconds = new DispatcherTimer();
            timer5seconds.Tick += new EventHandler(timerInstruction);
            timer5seconds.Interval = new TimeSpan(0, 0, 0, 5, 0);
            timer5seconds.Start();
        }

        /// <summary>
        /// Arret du jeu si l'utilisateur clique sur non : retire les images et adapte l'interface en conséquence
        /// </summary>
        /// <param name="sender">Clic non</param>
        /// <param name="e">Evenement</param>
        private void stopGame(object sender, EventArgs e)
        {
            savingVideo = false;
            hasVoted = false;
            gridMain.Children.Remove(instructions);
            gridMain.Children.Remove(canvasOui);
            gridMain.Children.Remove(canvasNon);
            gridMain.Children.Remove(fade);
        }

        /// <summary>
        /// Zoom video, création de la progress bar, du "bouton" stop, des étoiles pleines et vides.
        /// </summary>
        /// <param name="sender">Video</param>
        /// <param name="e">Evenement</param>
        private void zoomVideo(object sender, MouseButtonEventArgs e)
        {
            if ((savingVideo == false)&&(clickLimit==false))
            {
                zoom = true;
                maintenance.stopAllVideos();
                fade = new Canvas();
                fade.Background = Brushes.Black;
                Grid.SetRowSpan(fade, 4);
                Grid.SetColumnSpan(fade, 4);
                fade.Opacity = 0.50;
                gridMain.Children.Add(fade);
                TimeSpan videoSize;
                canvasHit = sender as Canvas;
                String[] nameNumber = canvasHit.Name.Split('c', '_');
                MediaElement video = videoRetriever(nameNumber[1], nameNumber[2]);
                int videoNumber = ((Convert.ToInt32(nameNumber[1])) * 4) + (Convert.ToInt32(nameNumber[2]));
                Uri uriZoom = video.Source;
                zoomedVideo = new MediaElement();
                Grid.SetRowSpan(zoomedVideo, 2);
                Grid.SetColumnSpan(zoomedVideo, 2);
                Grid.SetRow(zoomedVideo, 1);
                Grid.SetColumn(zoomedVideo, 1);
                zoomedVideo.Stretch = Stretch.Fill;
                zoomedVideo.UnloadedBehavior = MediaState.Manual;
                zoomedVideo.LoadedBehavior = MediaState.Manual;
                zoomedVideo.MediaEnded += mediaEnded;
                zoomedVideo.Source = uriZoom;
                buttonStop = new Grid();
                Canvas canvasStop = new Canvas();
                canvasStop.Background = new ImageBrush(new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/cross.png")));
                canvasStop.MouseLeftButtonDown += zoomedVideoStop_Click;
                buttonStop.Children.Add(canvasStop);
                buttonStop.Height = 100;
                buttonStop.Width = 100;
                buttonStop.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                buttonStop.VerticalAlignment = VerticalAlignment.Top;
                progressBar = new System.Windows.Controls.ProgressBar();
                progressBar.Margin = new Thickness(0, 0, 0, 194);
                progressBar.Height = 30;
                progressBar.Background = Brushes.PaleTurquoise;
                progressBar.Foreground = Brushes.Blue;
                Grid.SetColumnSpan(progressBar, 2);
                Grid.SetColumn(progressBar, 1);
                Grid.SetRow(progressBar, 3);
                Grid.SetColumn(buttonStop, 2);
                Grid.SetRow(buttonStop, 1);
                favorites = new Grid();
                favorites.Width = 100;
                favorites.VerticalAlignment = VerticalAlignment.Top;
                favorites.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                Grid.SetColumn(favorites, 1);
                Grid.SetRow(favorites, 1);
                Grid.SetRowSpan(favorites, 2);
                for(int i=0;i<maintenance.getFavorite(videoNumber);i++)
                {
                    if (i < 3)
                    {
                        RowDefinition line = new RowDefinition();
                        line.Height = new GridLength(100);
                        favorites.RowDefinitions.Add(line);
                        Canvas canvas = new Canvas();
                        canvas.Background = new ImageBrush(new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/starfull.png")));
                        Grid.SetRow(canvas, i);
                        favorites.Children.Add(canvas);
                    }
                }
                if (maintenance.getFavorite(videoNumber)<3)
                {
                    if (hasVoted == false)
                    {
                        RowDefinition line = new RowDefinition();
                        line.Height = new GridLength(100);
                        favorites.RowDefinitions.Add(line);
                        canvasFavorite = new Canvas();
                        canvasFavorite.Background = new ImageBrush(new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/starempty.png")));
                        Grid.SetRow(canvasFavorite, maintenance.getFavorite(videoNumber));
                        favorites.Children.Add(canvasFavorite);
                    }
                }
                videoSize = TimeSpan.FromSeconds(15);
                gridMain.Children.Add(zoomedVideo);
                gridMain.Children.Add(buttonStop);
                gridMain.Children.Add(progressBar);
                gridMain.Children.Add(favorites);
                progressBar.Minimum = 0;
                progressBar.Maximum = videoSize.TotalSeconds;
                timer.Interval = TimeSpan.FromMilliseconds(20);
                timer.Tick += new EventHandler(progressTick);
                timer.Start();
                zoomedVideo.Position = TimeSpan.FromSeconds(15);
                zoomedVideo.Play();
                Console.WriteLine("Lecture de la vidéo zoomée : " + zoomedVideo.Source.ToString());
            }
        }

        /// <summary>
        /// Pour la progress bar lors de la lescture d'une video zoomee
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void progressTick(object sender, EventArgs e)
        {
            progressBar.Value = zoomedVideo.Position.TotalSeconds;
        }

        private void zoomedVideoStop_Click(object sender, RoutedEventArgs e)
        {
            gridMain.Children.Remove(favorites);
            gridMain.Children.Remove(fade);
            gridMain.Children.Remove(buttonStop);
            gridMain.Children.Remove(progressBar);
            gridMain.Children.Remove(zoomedVideo);
            zoomedVideo.MediaEnded -= mediaEnded;
            zoomedVideo.Stop();
            timer.Stop();
            zoom = false;
            progressBar = null;
            zoomedVideo = null;
            canvasHit = null;
            maintenance.playAllVideos();
        }

        /// <summary>
        /// Retrouver la video par le nom des canvas
        /// </summary>
        /// <param name="ligne"></param>
        /// <param name="colonne"></param>
        /// <returns></returns>
        private MediaElement videoRetriever(String ligne, String colonne)
        {
            int i = Convert.ToInt32(ligne);
            int j = Convert.ToInt32(colonne);
            Console.WriteLine(i+" "+j);
            return tabVideos[i, j];
        }

        /// <summary>
        /// Placer la video dans le mur.
        /// </summary>
        /// <param name="sender">Canvas</param>
        /// <param name="e">Clic</param>
        private void putVideoIntoTab(object sender, MouseButtonEventArgs e)
        {
            Canvas canvasTargeted = sender as Canvas;
            if (canvasTargeted.Background == Brushes.Teal)
            {
                canvasTargeted.Background = Brushes.Transparent;
                String[] nameNumber = canvasTargeted.Name.Split('c', '_');
                MediaElement video = videoRetriever(nameNumber[1], nameNumber[2]);
                canvasTargeted.MouseLeftButtonDown -= putVideoIntoTab;
                canvasTargeted.MouseLeftButtonDown += zoomVideo;
                for (int i = 0; i < 16; i++)
                {
                        tabCanvas[i / 4, i % 4].MouseEnter -= colorCanvas;
                        tabCanvas[i / 4, i % 4].MouseLeave -= uncolorCanvas;
                }
                String current=(AppDomain.CurrentDomain.BaseDirectory + "Videos/savedVideo.wmv").ToString().Replace("\\","/");
                String destination = video.Source.ToString().Substring(8);
                String poubelle = (AppDomain.CurrentDomain.BaseDirectory + "Videos/videoToDelete.wmv").ToString().Replace("\\", "/");
                video.Source = null;
                Console.WriteLine(current);
                Console.WriteLine(destination);
                Thread.Sleep(100);
                System.IO.File.Move(destination, poubelle);
                Thread.Sleep(100);
                System.IO.File.Move(current, destination);
                Thread.Sleep(100);
                video.Source = new Uri(destination);
                int ligne = Convert.ToInt32(nameNumber[1]);
                int colonne = Convert.ToInt32(nameNumber[2]);
                savingVideo = false;
                flagQuestion[ligne * 4 + colonne] = 1;
                maintenance.addVideo(ligne*4+colonne);
                Console.WriteLine(video.Source.ToString());
                video.MediaEnded -= mediaEnded;
                video.MediaEnded += mediaEnded;
                hasVoted = false;
                System.IO.File.Delete(poubelle);
                clickLimit = true;
                timerLimit = new DispatcherTimer();
                timerLimit.Tick += new EventHandler(clickLimitTicker);
                timerLimit.Interval = new TimeSpan(0, 0, 0, 2, 0);
                timerLimit.Start();
            }
        }

        void clickLimitTicker(object sender, EventArgs e)
        {
            clickLimit = false;
            timerLimit.Stop();
        }

        /// <summary>
        /// Rendre la cellule noire lorsqu'une vidéo est stoppée (l'image reste figée sinon). On va également empêcher l'utilisateur de zoomer sur une vidéo qui a arretée d'être lue.
        /// </summary>
        /// <param name="number"></param>
        public void opacityBlack(int number)
        {
            tabCanvas[(number / 4), (number % 4)].Background=Brushes.Black;
            videos[number].Stop();
            tabCanvas[(number / 4), (number % 4)].MouseLeftButtonDown -= zoomVideo;
            flagQuestion[number] = 0;
        }

        /// <summary>
        /// Colorer les canvas sur lesquelles on peut placer les videos (fonction hover)
        /// </summary>
        /// <param name="sender">Canvas</param>
        /// <param name="e">Evemenement</param>
        public void colorCanvas(object sender, EventArgs e)
        {
            if (savingVideo == true)
            {
                Canvas canvasTargeted = sender as Canvas;
                canvasTargeted.Background = Brushes.Teal;
                canvasTargeted.MouseLeftButtonDown += putVideoIntoTab;
            }
        }

        /// <summary>
        /// Décolorer le canvas lorsque l'on bouge le curseur en dehors de la cellule où il est contenu.
        /// </summary>
        /// <param name="sender">Canvas</param>
        /// <param name="e"></param>
        public void uncolorCanvas(object sender, EventArgs e)
        {
            if (savingVideo == true)
            {
                Canvas canvasTargeted = sender as Canvas;
                canvasTargeted.Background = Brushes.Black;
            }
        }

        /// <summary>
        /// Actions qui peuvent etre realisees dans l'interface grace aux touches claviers (voter pour une video, accelerer....)
        /// </summary>
        /// <param name="keyEvent">Touche clavier</param>
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs keyEvent)
        {
            if (canvasHit != null)
            {
                String[] nameNumber = canvasHit.Name.Split('c', '_');
                MediaElement video = videoRetriever(nameNumber[1], nameNumber[2]);
                int videoNumber = (Convert.ToInt32(nameNumber[1]) * 4) + (Convert.ToInt32(nameNumber[2]));
                if (keyEvent.Key == Key.F)
                {
                    if (hasVoted == false)
                    {
                        hasVoted = true;
                        if (maintenance.increaseRating(videoNumber) == true)
                        {
                            canvasFavorite.Background = new ImageBrush(new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/starfull.png")));
                        }
                    }
                }
                //Accelerer
                if (keyEvent.Key == Key.P)
                {
                    if(zoomedVideo.SpeedRatio==0.50)
                    {
                        zoomedVideo.SpeedRatio = 0.75;
                    }
                    if (zoomedVideo.SpeedRatio == 0.75)
                    {
                        zoomedVideo.SpeedRatio = 1.0;
                    }
                    if (zoomedVideo.SpeedRatio == 1.0)
                    {
                        zoomedVideo.SpeedRatio = 1.50;
                    }
                    if (zoomedVideo.SpeedRatio == 1.50)
                    {
                        zoomedVideo.SpeedRatio = 2.0;
                    }
                }
                //Ralentir
                if (keyEvent.Key == Key.O)
                {
                    if (zoomedVideo.SpeedRatio == 0.75)
                    {
                        zoomedVideo.SpeedRatio = 0.50;
                    }
                    if (zoomedVideo.SpeedRatio == 1.0)
                    {
                        zoomedVideo.SpeedRatio = 0.75;
                    }
                    if (zoomedVideo.SpeedRatio == 1.50)
                    {
                        zoomedVideo.SpeedRatio = 1.0;
                    }
                    if (zoomedVideo.SpeedRatio == 2.0)
                    {
                        zoomedVideo.SpeedRatio = 1.50;
                    }
                }
            }
            //Calibration
            if ((savingVideo == false)&&(zoom==false))
            {
                if (keyEvent.Key == Key.C)
                {
                    calibration();
                }
            }
        }

        public bool getSaving()
        {
            return savingVideo;
        }
    }
}