using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HW_21._10._23_Thread
{
    /// <summary>
    /// Interaction logic for Task1.xaml
    /// </summary>
    public partial class Task1 : Window
    {
        private List<int> primesList = new List<int>();
        private bool generating = false;
        private Thread primeThread;
        private Thread fibThread;

        public Task1()
        {
            InitializeComponent();
            TextBlockFibonachi.Loaded += TextBlockFibonachi_Loaded;
        }

        private void TextBlockFibonachi_Loaded(object sender, RoutedEventArgs e)
        {
            var scrollViewer = FindVisualChild<ScrollViewer>(TextBlockFibonachi);
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset(10);
            }
        }

        private ScrollViewer FindVisualChild<ScrollViewer>(DependencyObject parent)
    where ScrollViewer : DependencyObject
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    if (child != null && child is ScrollViewer)
                    {
                        return child as ScrollViewer;
                    }
                    else
                    {
                        var result = FindVisualChild<ScrollViewer>(child);
                        if (result != null)
                            return result;
                    }
                }
            }
            return null;
        }

        private void GeneratePrimes(int start, int? end)
        {
            generating = true;
            int current = start;

            while (generating && (end == null || current <= end))
            {
                if (IsPrime(current))
                {
                    primesList.Add(current);

                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                    {
                        TextBlockForGeneratedNums.Text += current + ", ";
                    }));
                }
                current++;
            }
        }

        private bool IsPrime(int num)
        {
            if (num < 2) return false;
            for (int i = 2; i <= Math.Sqrt(num); i++)
            {
                if (num % i == 0) return false;
            }
            return true;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            int start = int.Parse(FirstNum.Text);
            int? end = string.IsNullOrEmpty(SecondNum.Text) ? null : (int?)int.Parse(SecondNum.Text);

            primesList.Clear();
            TextBlockForGeneratedNums.Text = "";

            primeThread = new Thread(() => GeneratePrimes(start, end));
            primeThread.Start();
            
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            generating = false;
            //Выдает ошибку как устаревший метод
            //if (primeThread.ThreadState == ThreadState.Running) 
            //{ 
            //    primeThread.Abort(); 
            //}
        }

        private bool generatingFibonacci = false;
        private void FibonachiStartButton (object sender, RoutedEventArgs e)
        {
            TextBlockFibonachi.Text = "";
            generatingFibonacci = true;
            fibThread = new Thread(GenerateFibonachiNums);
            fibThread.Start();
        }

        private void GenerateFibonachiNums()
        {
            long num1 = 1;
            long num2 = 1;
            long res = 0;
            

            while (generatingFibonacci)
            {
                res = num1 + num2;
                num1 = num2;
                num2 = res;

                if (res > int.MaxValue)
                    break;

                Dispatcher.Invoke(new Action(() =>
                {
                    TextBlockFibonachi.Text += res + ", ";
                }));
            }

        }
        private void FibonacciPauseButton_Click(object sender, RoutedEventArgs e)
        {
            generatingFibonacci = false;
        }
    }
}
