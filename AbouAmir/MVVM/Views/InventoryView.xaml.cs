using AbouAmir.MVVM.ViewModels;
using AbouAmir.Views;
using CommunityToolkit.Maui.Views;
using System.Diagnostics;

namespace AbouAmir.MVVM.Views;

public partial class InventoryView : ContentPage
{
   

    private readonly IViewModel viewModel = IViewModel.Instance;
    public InventoryView()
	{
		InitializeComponent();   
		BindingContext = new IViewModel();
	}
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Clean up any event handlers
    }


    // In InventoryView.xaml.cs, after the popup closes and a product is added:
    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var pop = new ProductPopup();
        if (pop is not null)
            await Application.Current.MainPage.ShowPopupAsync(pop);
    }

}