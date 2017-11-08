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

namespace Craps
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private int point, wins, losses;
		private Random die;
		public MainWindow()
		{
			InitializeComponent();
			point = 0;
			wins = 0;
			losses = 0;
			die = new Random();
		}

	#region Button clicks
		private void AppExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}
		
		private void btnStart_Click(object sender, RoutedEventArgs e)
		{
			// TODO check box stuff 
			resetBoard();
			txtRollForPoint.Text = "Roll for point";
			btnRoll.IsEnabled = true;
			btnSim.IsEnabled = true;
			btnStart.Content = "Reset Board";
		}


		private void btnRoll_Click(object sender, RoutedEventArgs e)
		{
			if (point == 0)
			{
				rollForPoint();
			}
			else
			{
				gameRoll();
			}
		}

		/*
		 * Added a button to simulate 1000 runs.
		 * Can be kind of slow, so I also put in a progress bar to show how close to completion you are.
		 * I used the progress bar update system shown at https://www.codeproject.com/articles/38555/wpf-progressbar
		 * because I couldn't get the progress bar to update.
		 */
		private void btnSim_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxButton btns = MessageBoxButton.OKCancel;
			MessageBoxResult result = MessageBox.Show("Simulate 1000 rolls of the dice?", "Run sims", btns);

			if(result == MessageBoxResult.OK)
			{
				progBar.Minimum = 0;
				progBar.Maximum = 1000;
				progBar.Value = 0;

				double value = 0;
				UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(progBar.SetValue);


				for (int i = 0; i < 1000; i++)
				{
					btnRoll_Click(this, new RoutedEventArgs());
					value += 1;
					Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { ProgressBar.ValueProperty, value });


				}
			}
		}

		private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);

		#endregion



		#region Rolls and Outcomes
		private void rollForPoint()
		{
			clearBoard();
			int d1 = getRoll();
			int d2 = getRoll();
			int total = d1 + d2;
			txtD1.Text = d1.ToString();
			txtD2.Text = d2.ToString();
			txtTotal.Text = total.ToString();
			if (total == 7 || total == 11)
			{
				winner();
			}
			else if(total == 2 || total == 3 || total == 12)
			{
				loser();
			}
			else
			{
				point = total;
				txtPoint.Text = point.ToString();
				txtRollForPoint.Text = "";
			}
		}

		private void gameRoll()
		{
			int r1 = getRoll();
			int r2 = getRoll();
			txtD1.Text = r1.ToString();
			txtD2.Text = r2.ToString();
			txtTotal.Text = (r1 + r2).ToString();

			if((r1 + r2) == 7)
			{
				loser();
				point = 0;
			}
			else if( (r1 + r2) == point)
			{
				winner();
				point = 0;
			}
		}


		private int getRoll()
		{
			return die.Next(1, 6);
		}

		private void loser()
		{
			txtWinOrLoss.Text = "House wins";
			losses++;
			txtLosses.Text = losses.ToString();
			txtRollForPoint.Text = "Roll for point";
			txtPoint.Text = "";
			
		}

		private void winner()
		{
			txtWinOrLoss.Text = "Player wins!";
			wins++;
			txtWins.Text = wins.ToString();
			txtPoint.Text = "";
			txtRollForPoint.Text = "Roll for point";
			
		}
		#endregion

		#region Board Changes
		/*
		 * Fully resets the board, including win-loss record.
		 */
		private void resetBoard()
		{
			txtWins.Text = "0";
			wins = 0;
			txtLosses.Text = "0";
			losses = 0;
			txtWinOrLoss.Text = "";
			txtPoint.Text = "";
			clearBoard();
		}

		

		/*
		 * Clears the current point and dice rolls.
		 */
		private void clearBoard()
		{
			txtWinOrLoss.Text = "";
			txtD1.Text = "";
			txtD2.Text = "";
			txtTotal.Text = "";
		}
		#endregion

		#region Misc
		private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			MessageBoxButton btns = MessageBoxButton.YesNo;
			MessageBoxResult result = MessageBox.Show("Exit program?", "Are you sure you want to quit?", btns);

			if (result == MessageBoxResult.No)
			{
				e.Cancel = true;
			}
		}
		#endregion

	}
}
