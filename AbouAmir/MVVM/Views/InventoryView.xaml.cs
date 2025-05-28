using AbouAmir.MVVM.ViewModels;

namespace AbouAmir.MVVM.Views;

public partial class InventoryView : ContentPage
{
	public InventoryView()
	{
		InitializeComponent();
		BindingContext = new IViewModel();
	}
}