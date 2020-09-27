using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace UserCreateReadUpdate.VMs
{
	public static class Constants
	{
		public const string WebAPIUri = "http://localhost:64831/";
	}

	class User:ViewModelBase
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string DoB { get; set; }
		public string Email1 { get; set; }
		public string Email2 { get; set; }
		public string Email3 { get; set; }
		public string Contact1 { get; set; }
		public string Contact2 { get; set; }
		public string Contact3 { get; set; }
	}

	class EmailList: ViewModelBase
	{
		public string Email1 { get; set; }

		public string Email2 { get; set; }
		public string ShowEmail2 { get; set; } = "Collapsed";

		public string Email3 { get; set; }
		public string ShowEmail3 { get; set; } = "Collapsed";

		public void ShowNextLine()
		{
			if (ShowEmail2 != "Visible")
				ShowEmail2 = "Visible";
			else
			if (ShowEmail3 != "Visible")
				ShowEmail3 = "Visible";

			OnPropertyChanged(nameof(ShowEmail2));
			OnPropertyChanged(nameof(ShowEmail3));
		}
	}
	class ContactList:ViewModelBase
	{
		public string Contact1 { get; set; }
		public string Contact2 { get; set; }
		public string ShowContact2 { get; set; } = "Collapsed";
		public string Contact3 { get; set; }
		public string ShowContact3 { get; set; } = "Collapsed";

		public void ShowNextLine()
		{
			if (ShowContact2 != "Visible")
				ShowContact2 = "Visible";
			else
			if (ShowContact3 != "Visible")
				ShowContact3 = "Visible";

			OnPropertyChanged(nameof(ShowContact2));
			OnPropertyChanged(nameof(ShowContact3));
		}
	}
	class CreateUserVM : ViewModelBase, IDataErrorInfo
	{
		public delegate void UpdateGridEventHandler(object source, EventArgs args);
		public event UpdateGridEventHandler UpdatedUser;

		public CreateUserVM()
		{
			OneMoreEmailCommand = new RelayCommand(OneMoreEmail, CanExecuteMyMethod);
			OneMoreContactCommand = new RelayCommand(OneMoreContact, CanExecuteMyMethod);
			CreateUserCommand = new RelayCommand(CreateUser, CanExecuteMyMethod);
			UpdateUserCommand = new RelayCommand(UpdateUser, CanExecuteMyMethod);
		}
		//================Edit User====================================
		public delegate void UpdateGridDelegate();
		private async void UpdateUser(object obj)
		{
			if(!IsItemValid)
			{
				MessageBox.Show("Please clear the error(s) and try again");
				return;
			}

			User editUser = new User()
			{
				Id = this.Id,
				FirstName = this.FirstName,
				LastName = this.LastName,
				DoB = this.DateOfBirth,
				Email1 = this.EmailList.Email1,
				Email2 = this.EmailList.Email2,
				Email3 = this.EmailList.Email3,
				Contact1 = this.ContactList.Contact1,
				Contact2 = this.ContactList.Contact2,
				Contact3 = this.ContactList.Contact3
			};
			try
			{

				// Sending data to server.
				UserVM responseObj = await Edit(editUser);
				((MainWindow)Application.Current.MainWindow).UpdateGrid();

				// Verification.  
				this.FirstName = null;
				this.LastName = null;
				this.DateOfBirth = null;
				this.EmailList = null;
				this.EmailList = new EmailList();
				this.ContactList = null;
				this.ContactList = new ContactList();

				OnPropertyChanged(FirstName);
				OnPropertyChanged(LastName);
				OnPropertyChanged(DateOfBirth);
				// Display Message  
				MessageBox.Show("User successfully edited!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

			}
			catch (Exception ex)
			{
				// Display Message  
				MessageBox.Show("Something goes wrong, Please try again later.", "Fail", MessageBoxButton.OK, MessageBoxImage.Error);

				Console.Write(ex);
			}
		}

		public static async Task<UserVM> Edit(User requestedUser)
		{
			// Initialization.  
			RegInfoResponseObj responseObj = new RegInfoResponseObj();
			UserVM uservm = new UserVM();
			try
			{
				// Posting.  
				using (var client = new HttpClient())
				{
					// Setting Base address.  
					client.BaseAddress = new Uri(Constants.WebAPIUri);

					// Setting content type.  
					client.DefaultRequestHeaders.Accept.Clear();
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

					// Setting timeout.  
					client.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(1000000));

					// Initialization.  
					HttpResponseMessage response = new HttpResponseMessage();

					// HTTP PUT
					response = await client.PutAsJsonAsync("api/users", requestedUser).ConfigureAwait(false);

					// Verification  
					if (response.IsSuccessStatusCode)
					{
						// Reading Response.  
						string result = response.Content.ReadAsStringAsync().Result;
						uservm = JsonConvert.DeserializeObject<UserVM>(result);
						// Releasing.  
						response.Dispose();
					}
					else
					{
						// Reading Response.  
						string result = response.Content.ReadAsStringAsync().Result;
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return uservm;
		}
		//=====================Create User===============================

		private async void CreateUser(object obj)
		{
			if(!IsItemValid)
			{
				MessageBox.Show("Please clear errors and try again");
				return;
			}

			User newUser = new User()
			{
				FirstName = this.FirstName,
				LastName = this.LastName,
				DoB = this.DateOfBirth,
				Email1 = this.EmailList.Email1,
				Email2 = this.EmailList.Email2,
				Email3 = this.EmailList.Email3,
				Contact1 = this.ContactList.Contact1,
				Contact2 = this.ContactList.Contact2,
				Contact3 = this.ContactList.Contact3
			};
			try
			{

				// Sending data to server.
				RegInfoResponseObj responseObj = await PostRegInfo(newUser);

				// Veerification.  
				if (responseObj.code == 600)
				{
					this.FirstName = null;
					this.LastName = null;
					this.DateOfBirth = null;
					this.EmailList = null;
					this.EmailList = new EmailList();
					this.ContactList = null;
					this.ContactList = new ContactList();

					OnPropertyChanged(FirstName);
					OnPropertyChanged(LastName);
					OnPropertyChanged(DateOfBirth);
					((MainWindow)Application.Current.MainWindow).UpdateGrid();
					// Display Message  
					MessageBox.Show("User successfully added!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else if (responseObj.code == 602)
				{
					// Display Message  
					MessageBox.Show("We are unable to add user at the moment, please try again later", "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
				}

			}
			catch (Exception ex)
			{
				// Display Message  
				MessageBox.Show("Something goes wrong, Please try again later.", "Fail", MessageBoxButton.OK, MessageBoxImage.Error);

				Console.Write(ex);
			}
		}
		public static async Task<RegInfoResponseObj> PostRegInfo(User newUser)
		{
			// Initialization.  
			RegInfoResponseObj responseObj = new RegInfoResponseObj();

			try
			{
				// Posting.  
				using (var client = new HttpClient())
				{
					// Setting Base address.  
					client.BaseAddress = new Uri(Constants.WebAPIUri);

					// Setting content type.  
					client.DefaultRequestHeaders.Accept.Clear();
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

					// Setting timeout.  
					client.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(1000000));

					// Initialization.  
					HttpResponseMessage response = new HttpResponseMessage();

					// HTTP POST  
					response = await client.PostAsJsonAsync("api/users", newUser).ConfigureAwait(false);

					// Verification  
					if (response.IsSuccessStatusCode)
					{
						// Reading Response.  
						string result = response.Content.ReadAsStringAsync().Result;
						responseObj = JsonConvert.DeserializeObject<RegInfoResponseObj>(result);
						responseObj.code = 600;
						// Releasing.  
						response.Dispose();
					}
					else
					{
						// Reading Response.  
						string result = response.Content.ReadAsStringAsync().Result;
						responseObj.code = 602;
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return responseObj;
		}
		public class RegInfoResponseObj
		{
			public bool status { get; set; }
			public int code { get; set; }
			public string message { get; set; }
		}

		//=====================================================
		private bool CanExecuteMyMethod(object arg)
		{
			return true;
		}

		private void OneMoreEmail(object obj)
		{
			EmailList.ShowNextLine();
		}

		private void OneMoreContact(object obj)
		{
			ContactList.ShowNextLine();
		}
		public void UpdateProperties()
		{
			PropertyInfo[] properties = this.GetType().GetProperties();
			foreach (PropertyInfo property in properties)
			{
				OnPropertyChanged(this[property.Name]);
			}
		}

		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string DateOfBirth { get; set; }
		public EmailList EmailList { get; set; } = new EmailList();
		public ContactList ContactList { get; set; } = new ContactList();

		public ICommand OneMoreEmailCommand { get; set; }
		public ICommand OneMoreContactCommand { get; set; }
		public ICommand CreateUserCommand { get; set; }
		public ICommand UpdateUserCommand { get; set; }

		public string Error => throw new NotImplementedException();
		public string this[string columnName] 
		{ 
			get{
				string message=string.Empty;
				switch(columnName)
				{
					case nameof(FirstName):
						if (FirstName== null || FirstName.Trim().Length == 0)
							message = "First Name is required!!!";
						break;

					case nameof(LastName):
						if (LastName == null || LastName.Trim().Length == 0)
							message = "Last Name is required!!!";
						break;
				}
				return message;
			}
		}
		public bool IsItemValid
		{
			get
			{
				PropertyInfo[] properties = this.GetType().GetProperties();
				foreach (PropertyInfo property in properties)
				{
					if (!string.IsNullOrEmpty(this[property.Name]))
						return false;
				}
				return true;
			}
		}
		//private string CreateStringSeparatedByComma(List<string> list)
		//{
		//	StringBuilder oneString = new StringBuilder();
		//	foreach (var str in list)
		//	{
		//		oneString.Append(str);
		//		if (oneString.Length > 0)
		//			oneString.Append(", ");
		//	}
		//	if (oneString[oneString.Length - 2] == ',')
		//		oneString.Remove(oneString.Length - 2, 1);
		//	return oneString.ToString();
		//}
	}
}
