using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserCreateReadUpdate.VMs;
using static UserCreateReadUpdate.VMs.CreateUserVM;

namespace UserCreateReadUpdate
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			BindUserList();
			CreateUser.DataContext = new CreateUserVM();
		}

		ObservableCollection<UserVM> Users;
		private void BindUserList(string str = "")
		{
			HttpClient client = new HttpClient();
			//Constants.WebAPIUri+"api/users/0"
			client.BaseAddress = new Uri(Constants.WebAPIUri);
			//client.DefaultRequestHeaders.Add("appkey", "myapp_key");
			client.DefaultRequestHeaders.Accept.Add(
			   new MediaTypeWithQualityHeaderValue("application/json"));
			HttpResponseMessage response = new HttpResponseMessage();
			if (str == null || str.Length == 0)
				response = client.GetAsync("api/users").Result;
			else
			{
				var builder = new UriBuilder(Constants.WebAPIUri+"api/users");
				var query = HttpUtility.ParseQueryString(builder.Query);
				query["searchtext"] = str;
				builder.Query = query.ToString();
				var completeUri = builder.ToString();
				response = client.GetAsync(completeUri).Result;
			}

			if (response.IsSuccessStatusCode)
			{
				Users = response.Content.ReadAsAsync<ObservableCollection<UserVM>>().Result;
				grdUsers.DataContext = Users;
				grdUsers.ItemsSource = null;
				grdUsers.ItemsSource = Users;
			}
			else
			{
				MessageBox.Show("Error Code: " + response.StatusCode + ", Message: " + response.ReasonPhrase);
			}

		}
		CreateUserVM UpdateUserData;
		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.Source is TabControl)
			{
				if (CreateUser.IsSelected)
				{
					if (UpdateUser.DataContext != null)
						UpdateUser.DataContext = null;

					if (CreateUser.DataContext != null)
						return;
					CreateUser.DataContext = new CreateUserVM();
				}
				else if (UpdateUser.IsSelected)
				{
					if (CreateUser.DataContext != null)
						CreateUser.DataContext = null;
					if (UpdateUser.DataContext != null)
						return;
					UpdateUserData = new CreateUserVM();
					UpdateUser.DataContext = UpdateUserData;
				}
			}
		}

		private void GrdUsers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			UserVM selectedUser = (UserVM)grdUsers.SelectedItem;
			UpdateUser.IsSelected = true;
			UpdateUserData.Id = selectedUser.Id;
			UpdateUserData.FirstName = selectedUser.FirstName;
			UpdateUserData.LastName = selectedUser.LastName;
			UpdateUserData.DateOfBirth = selectedUser.DoB;
			UpdateUserData.EmailList = new EmailList();
			UpdateUserData.EmailList.Email1 = selectedUser.Email1;
			UpdateUserData.EmailList.Email2 = selectedUser.Email2;
			UpdateUserData.EmailList.Email3 = selectedUser.Email3;
			UpdateUserData.ContactList = new ContactList();
			UpdateUserData.ContactList.Contact1 = selectedUser.Contact1;
			UpdateUserData.ContactList.Contact2 = selectedUser.Contact2;
			UpdateUserData.ContactList.Contact3 = selectedUser.Contact3;
			UpdateUserData.UpdateProperties();

			UpdateGridDelegate updateGridDelegate = UpdateGrid;
		}
		public void UpdateGrid()
		{
			BindUserList();
		}

		private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			string text = ((TextBox)sender).Text;
			BindUserList(text);
			//ObservableCollection<UserVM> users = Users.Where(u => u.FirstName.Contains(""+text) || u.LastName.Contains(text));
		}
	}
}
