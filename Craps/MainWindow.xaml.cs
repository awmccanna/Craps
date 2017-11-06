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
		public MainWindow()
		{
			InitializeComponent();
		}

		private void AppExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			MessageBoxButton btns = MessageBoxButton.YesNo;
			MessageBoxResult result = MessageBox.Show("Exit program?", "Are you sure you want to quit?", btns);

			if (result == MessageBoxResult.No)
			{
				e.Cancel = true;
			}
		}

	}
}
