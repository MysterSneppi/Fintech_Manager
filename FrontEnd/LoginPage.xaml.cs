using System.Net.Http.Json;
using System.Text.Json;

namespace FrontEnd;

public partial class LoginPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public LoginPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UsernameEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await DisplayAlert("Error", "Please enter both username and password.", "OK");
            return;
        }

        try
        {
            var loginData = new
            {
                Username = UsernameEntry.Text,
                Password = PasswordEntry.Text
            };

            var response = await _httpClient.PostAsJsonAsync("https://localhost:53310/api/Auth/login", loginData);

            if (response.IsSuccessStatusCode)
            {              
                var result = await response.Content.ReadFromJsonAsync<JsonElement>();            
           
                if (result.TryGetProperty("token", out JsonElement tokenElement))
                {
                    string token = tokenElement.GetString();
                    await DisplayAlert("Success", "Login successful!", "OK");     
                    Application.Current.MainPage = new MainPage();
                }
                else
                {
                    await DisplayAlert("Error", "Token not found in response.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Invalid credentials or server error.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

}
