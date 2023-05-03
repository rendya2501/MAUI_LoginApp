using MyLoginApp.Pages.Login;
using MyLoginApp.Pages.Success;

namespace MyLoginApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(SuccessPage), typeof(SuccessPage));
    }
}
