using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using ZXing;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EtiquetasCorreios.Api.Controllers
{
    [Route("api/[controller]")]
    public class Code128Controller : Controller
    {
        // POST: api/values
        [HttpPost]
        public List<string> Get(List<string> valores)
        {
            var retorno = new List<string>();
            valores.ForEach(valor =>
            {
                BarcodeWriterPixelData writer = new BarcodeWriterPixelData() { Format = BarcodeFormat.CODE_128 };
                writer.Options.PureBarcode = true;
                var pixelData = writer.Write(valor);
                using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
                {
                    var stream = new MemoryStream();
                    var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                    try
                    {
                        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }
                    bitmap.Save(stream, ImageFormat.Png);
                    retorno.Add(Convert.ToBase64String(stream.ToArray()));
                }
            });
            return retorno;
        }
    }
}
