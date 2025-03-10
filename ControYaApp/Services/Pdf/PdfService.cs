using PdfSharp;
using PdfSharp.Maui;
using PdfSharp.Maui.Utils;

namespace ControYaApp.Services.Pdf
{

    public class PdfService
    {
        public string GeneratePdf(View view, string codProduccion, int orden, string codMaterial)
        {
            try
            {
                var fileName = $"{codProduccion}_{orden}_{codMaterial}_{DateTime.Now:d-M-yyyy}";

                var pdfManager = new PdfManager();

                var pdf = pdfManager.GeneratePdfFromView(view, PageOrientation.Portrait, PageSize.A4, PdfStyle.PlatformSpecific);
#if ANDROID
                string pathAndroid = Android.App.Application.Context.GetExternalFilesDir(null).AbsoluteFile.Path;

                PdfSave.Save(pdf, $"xd.pdf", pathAndroid);

                return pathAndroid + fileName;
#else

                var path = Path.GetTempPath();

                PdfSave.Save(pdf, $"ffff.pdf", path);


                return path + "ffff.pdf";

#endif
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeletePdf(string filePath)
        {
            var file = new FileInfo(filePath);
            file.Delete();
        }
    }
}
