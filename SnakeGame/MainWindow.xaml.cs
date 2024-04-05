using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace SnakeGame
{


    public partial class MainWindow : Window
    {

        private readonly List<Point> bonusPoints = new List<Point>();

        private readonly List<Point> snakePoints = new List<Point>();

        private readonly List<Point> wallPoints = new List<Point>();

        private readonly Brush snakeColor = Brushes.Green;

        private enum Movingdirection
        {
            Upwards = 8,
            Downwards = 2,
            Toleft = 4,
            Toright = 6
        };



        private readonly Point startingPoint = new Point(400, 50);

        private Point currentPosition = new Point();

        private int direction = 0;

        private int previousDirection = 0;

        private readonly int headSize = 20;

        private int length = 100;
        private int score = 0;
        private readonly Random rnd = new Random();

        public static DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {

            InitializeComponent();

            timer.Tick += new EventHandler(timer_Tick);


            timer.Interval = TimeSpan.FromSeconds(0.002);
            timer.Start();
            
            KeyDown += new KeyEventHandler(OnButtonKeyDown);
            PaintSnake(startingPoint);
            currentPosition = startingPoint;

            for (var n = 0; n < 10; n++)
            {
                PaintBonus(n);
            }
        }


        private void PaintSnake(Point currentposition)
        {
            
            Rectangle newRect = new Rectangle
            {
                Fill = snakeColor,
                Width = headSize,
                Height = headSize
            };

            Canvas.SetTop(newRect, currentposition.Y);
            Canvas.SetLeft(newRect, currentposition.X);

            int count = PaintCanvas.Children.Count;

            PaintCanvas.Children.Add(newRect);
            snakePoints.Add(currentposition);

            if (count > length)
            {
                PaintCanvas.Children.RemoveAt(count - length + 9);
                snakePoints.RemoveAt(count - length);
            }
        }
        
        private void PaintBonus(int index)
        {
            Point bonusPoint = new Point(rnd.Next(20, 760), rnd.Next(20, 460));

            Rectangle newRect = new Rectangle
            {
                Fill = Brushes.Red,
                Width = headSize,
                Height = headSize
            };

            Canvas.SetTop(newRect, bonusPoint.Y);
            Canvas.SetLeft(newRect, bonusPoint.X);
            PaintCanvas.Children.Insert(index, newRect);
            bonusPoints.Insert(index, bonusPoint);

        }

        private void timer_Tick(object sender, EventArgs e)
        {

            switch (direction)
            {
                case (int)Movingdirection.Downwards:
                    currentPosition.Y += 1;
                    PaintSnake(currentPosition);
                    break;
                case (int)Movingdirection.Upwards:
                    currentPosition.Y -= 1;
                    PaintSnake(currentPosition);
                    break;
                case (int)Movingdirection.Toleft:
                    currentPosition.X -= 1;
                    PaintSnake(currentPosition);
                    break;
                case (int)Movingdirection.Toright:
                    currentPosition.X += 1;
                    PaintSnake(currentPosition);
                    break;


            }


            if ((currentPosition.X < 5) || (currentPosition.X > 780) ||
                (currentPosition.Y < 5) || (currentPosition.Y > 480))
                GameOver();


            int n = 0;
            foreach (Point point in bonusPoints)
            {

                if ((Math.Abs(point.X - currentPosition.X) < headSize) &&
                    (Math.Abs(point.Y - currentPosition.Y) < headSize))
                {
                    length += 10;
                    score += 10;

                    bonusPoints.RemoveAt(n);
                    PaintCanvas.Children.RemoveAt(n);
                    PaintBonus(n);
                    break;
                }
                n++;
            }

            this.Title = "Score: " + score.ToString();


            for (int q = 0; q < (snakePoints.Count - headSize * 2); q++)
            {
                Point point = new Point(snakePoints[q].X, snakePoints[q].Y);
                if ((Math.Abs(point.X - currentPosition.X) < headSize) &&
                     (Math.Abs(point.Y - currentPosition.Y) < headSize))
                {
                    GameOver();
                    break;
                }

            }
            
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    if (previousDirection != (int)Movingdirection.Upwards)
                        direction = (int)Movingdirection.Downwards;
                    break;
                case Key.Up:
                    if (previousDirection != (int)Movingdirection.Downwards)
                        direction = (int)Movingdirection.Upwards;
                    break;
                case Key.Left:
                    if (previousDirection != (int)Movingdirection.Toright)
                        direction = (int)Movingdirection.Toleft;
                    break;
                case Key.Right:
                    if (previousDirection != (int)Movingdirection.Toleft)
                        direction = (int)Movingdirection.Toright;
                    break;
                case Key.Escape:
                    ChangeGameState();
                    break;
                case Key.CapsLock:
                    RestartGame();
                    break;
                case Key.S:
                    ShowScoreboardWindow();
                    break;

            }
            previousDirection = direction;

        }



        private void GameOver()
        {
            MessageBox.Show($@"You Lose! Your score is {score}", "Game Over", MessageBoxButton.OK, MessageBoxImage.Hand);
            Entities.GetContext().Scoreboard.Add(new Scoreboard {Score = score });
            try
            {
                Entities.GetContext().SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            this.Close();
        }
       static public void ChangeGameState()
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
            }
            else
            {
                timer.Start();
            } 
                

        }

        private void RestartGame()
        {
            Entities.GetContext().Scoreboard.Add(new Scoreboard { Score = score });
            try
            {
                Entities.GetContext().SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
       
        private void ShowScoreboardWindow()
        {
            var _scoreboardWindow = new ScoreboardWindow();
            _scoreboardWindow.Show();

        }

    }

    }
