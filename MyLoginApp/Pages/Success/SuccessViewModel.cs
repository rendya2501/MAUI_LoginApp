using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http.Headers;

namespace MyLoginApp.Pages.Success;

public partial class SuccessViewModel : ObservableObject
{
    [ObservableProperty]
    public bool isBusy;

    [RelayCommand]
    private static async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task ExecuteAPI()
    {
        IsBusy = true;

        using (var client = new HttpClient())
        {
            // JWTトークンを取得
            string token = Preferences.Get("jwt", string.Empty);

            // ヘッダーにJWTトークンを追加
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // APIリクエストを実行
            var response = await client.GetAsync("https://localhost:7298/api/data");

            // 成功した場合、データを表示
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                await Shell.Current.DisplayAlert("データ", result, "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("データ取得失敗", "データを取得できませんでした。", "OK");
            }
        }

        IsBusy = false;
    }

}
