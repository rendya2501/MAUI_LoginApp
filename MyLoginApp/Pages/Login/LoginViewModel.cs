using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyLoginApp.Pages.Success;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace MyLoginApp.Pages.Login;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    public string userName;

    [ObservableProperty]
    public string password;

    [ObservableProperty]
    public bool isBusy;

    [RelayCommand]
    private async void Login()
    {
        await JwtAuthentication();


        //if (IsValid(UserName, Password))
        //{
        //    // 認証が成功した場合、SuccessPageに遷移する
        //    await Shell.Current.GoToAsync(nameof(SuccessPage));
        //}
        //else
        //{
        //    // 認証が失敗した場合、エラーメッセージを表示する
        //    await Shell.Current.DisplayAlert("ログイン失敗", "ユーザー名またはパスワードが正しくありません。", "OK");
        //}

        //static bool IsValid(string username, string password)
        //{
        //    // このメソッドで、実際のユーザー名とパスワードの検証ロジックを実装してください。
        //    return username == "admin" && password == "admin";
        //}
    }

    /// <summary>
    /// ベーシック認証
    /// </summary>
    /// <returns></returns>
    private async Task BasicAuthentication()
    {
        if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("入力エラー", "ユーザー名とパスワードを入力してください。", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            using var httpClient = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes($"{UserName}:{Password}");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var response = await httpClient.PostAsync("https://localhost:7181/api/login", null);

            if (response.IsSuccessStatusCode)
            {
                await Shell.Current.GoToAsync(nameof(SuccessPage));
            }
            else
            {
                await Shell.Current.DisplayAlert("ログイン失敗", "ユーザー名またはパスワードが正しくありません。", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("エラー", "通信中にエラーが発生しました。", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// JWT認証
    /// </summary>
    /// <returns></returns>
    private async Task JwtAuthentication()
    {
        if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("入力エラー", "ユーザー名とパスワードを入力してください。", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            using var client = new HttpClient();
            var content = new StringContent(
                JsonConvert.SerializeObject(new { UserName, Password }),
                Encoding.UTF8,
                "application/json"
            );
            var response = await client.PostAsync("https://localhost:7298/api/login", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<JObject>(result)["jwt"].ToString();

                // 以降のリクエストで使用するために、トークンを保存する
                Preferences.Set("jwt", token );

                // SuccessPageに遷移
                await Shell.Current.GoToAsync(nameof(SuccessPage));
            }
            else
            {
                await Shell.Current.DisplayAlert("ログイン失敗", "ユーザー名またはパスワードが正しくありません。", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("エラー", "通信中にエラーが発生しました。", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}