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
/*
 * Alex McCanna
 * Craps simulator
 * CSCD 371
 * This is a simple craps simulator, where you only get to play the Pass line.
 * Currently the betting system is set up to accomodate the Simulate 1000 runs button I added, which means that the betting system won't stop you from rolling if you go negative.
 * 
 */ 


namespace Craps
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private int point, wins, losses;
		private int bet, winnings, startingCash;
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
			if(PlayWithMoney())
			{
				winnings = 0;
				try
				{
					startingCash = int.Parse(txtStartingCash.Text);
					if(startingCash < 0)
					{
						throw new Exception();
					}
				}
				catch(Exception)
				{
					DisplayError("Cash amount");
					return;
				}
				txtStartingCash.IsReadOnly = true;
			}

			chkPlayWithMoney.IsEnabled = false;
			txtRollForPoint.Text = "Roll for point";
			btnReset.IsEnabled = true;
			btnRoll.IsEnabled = true;
			btnSim.IsEnabled = true;
			btnStart.IsEnabled = false;
		}

		private void btnReset_Click(object sender, RoutedEventArgs e)
		{
			resetBoard();
			btnReset.IsEnabled = false;
			btnRoll.IsEnabled = false;
			btnSim.IsEnabled = false;
			btnStart.IsEnabled = true;

			txtStartingCash.IsReadOnly = false;
			chkPlayWithMoney.IsEnabled = true;

			winnings = 0;
			txtAccount.Text = "0";



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
			try
			{
				bet = int.Parse(txtBet.Text);
			}
			catch (Exception)
			{
				DisplayError("Bet amount");
				return;
			}
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
			try
			{
				bet = int.Parse(txtBet.Text);
			}
			catch (Exception)
			{
				DisplayError("Bet amount");
				return;
			}
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
			return die.Next(1, 7);
		}

		private void loser()
		{
			if(PlayWithMoney())
			{
				winnings = winnings - bet;
				txtAccount.Text = winnings.ToString();
				if(winnings < 0)
				{
					txtAccount.Background = Brushes.Red;
				}
			}


			txtWinOrLoss.Text = "House wins";
			losses++;
			txtLosses.Text = losses.ToString();
			txtRollForPoint.Text = "Roll for point";
			txtPoint.Text = "";
			
		}

		private void winner()
		{
			if(PlayWithMoney())
			{
				winnings = winnings + bet;
				txtAccount.Text = winnings.ToString();
				if (winnings > 0)
				{
					txtAccount.Background = Brushes.Green;
				}
			}


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

		private bool PlayWithMoney()
		{
			return chkPlayWithMoney.IsChecked == (bool?)true;
		}

		private void DisplayError(string v)
		{
			MessageBox.Show(v + " is invalid.");
		}
		#endregion

	}
}
