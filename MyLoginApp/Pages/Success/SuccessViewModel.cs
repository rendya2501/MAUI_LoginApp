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
            // JWT�g�[�N�����擾
            string token = Preferences.Get("jwt", string.Empty);

            // �w�b�_�[��JWT�g�[�N����ǉ�
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // API���N�G�X�g�����s
            var response = await client.GetAsync("https://localhost:7298/api/data");

            // ���������ꍇ�A�f�[�^��\��
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                await Shell.Current.DisplayAlert("�f�[�^", result, "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("�f�[�^�擾���s", "�f�[�^���擾�ł��܂���ł����B", "OK");
            }
        }

        IsBusy = false;
    }

}
