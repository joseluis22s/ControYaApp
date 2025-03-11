//using PdfSharp;
//using PdfSharp.Maui;

//using PdfSharp.Maui.Utils;

namespace ControYaApp.Views;

public partial class PruebaPDfgGenerar : ContentPage
{
    public PruebaPDfgGenerar()
    {
        InitializeComponent();
    }

    private void GeneratePDF(object sender, EventArgs e)
    {
        //var pdfManager = new PdfManager();
        //var pdf = pdfManager.GeneratePdfFromView(Content, PageOrientation.Portrait, PageSize.A4, PdfStyle.PlatformSpecific);

        //var path = DeviceInfo.Platform == DevicePlatform.Android ? "/storage/emulated/0/Download" : Path.GetTempPath();
        //PdfSave.Save(pdf, "SinglePage.pdf", path);


        //Application.Current.MainPage.DisplayAlert(
        //    "Success",
        //    $"Your PDF generated at {path}",
        //    "OK");
    }
}