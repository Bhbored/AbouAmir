using AbouAmir.MVVM.ViewModels;
using AbouAmir.Views;
using CommunityToolkit.Maui.Views;

namespace AbouAmir.MVVM.Views;

public partial class InventoryView : ContentPage
{
	public InventoryView()
	{
		InitializeComponent();
		BindingContext = new IViewModel();
	}

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
	   
    }
}