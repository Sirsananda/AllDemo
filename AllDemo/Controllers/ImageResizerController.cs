using AllDemo.Configuration;
using AllDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NuGet.Packaging.Signing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace AllDemo.Controllers
{
    public class ImageResizerController : Controller
    {
        private readonly ImageSettings _imageSettings;
        private readonly IWebHostEnvironment _environment;
        public ImageResizerController(IOptions<ImageSettings> imageSetting,IWebHostEnvironment environment)
        {
                _imageSettings=imageSetting.Value;
                _environment=environment;
        }
        [HttpGet]
        public IActionResult ImageResize()
        {
            return View(new ImageResizerViewModel
            {
                _AfterResizeImage = new AfterResizeImage()
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImageResize(ImageResizerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            if (model.Image.Length > _imageSettings.MaxfileSize || model.ImageByte.Length >_imageSettings.MaxfileSize) //here check the file size
                return BadRequest($"File too large. Max{_imageSettings.MaxfileSize/ 1024 * 1024} MB allowed.");
            //here check the file extensions
            var extensionImage = Path.GetExtension(model.Image.FileName).ToString();
            var extensionImageByte = Path.GetExtension(model.ImageByte.FileName).ToString();
            if (!_imageSettings.AllowedExtensions.Contains(extensionImage) &&
                !_imageSettings.AllowedExtensions.Contains(extensionImageByte))
                return BadRequest($"invalid file type. Allowed file type are {_imageSettings.AllowedExtensions}");

            #region new method first resize then image and then save to the database

            //resize the first image
            //using (var image1 = SixLabors.ImageSharp.Image.Load(model.Image.OpenReadStream()))
            //{
            //    image1.Mutate(item => item.Resize(new ResizeOptions()
            //    {
            //        Size = new Size(100, 100),// fixed size width=100px, height=100px
            //        Mode = ResizeMode.Crop // maintain aspect ratio
            //    }));
            //}

            ////resize the second image
            //using (var image2 = SixLabors.ImageSharp.Image.Load(model.ImageByte.OpenReadStream()))
            //{
            //    image2.Mutate(item => item.Resize(new ResizeOptions()
            //    {
            //        Size = new Size(100, 100), // fixed size width=100px, height=100px
            //        Mode = ResizeMode.Crop // maintain aspect ratio
            //    }));
            //}

            ////simple save to the local folder
            //var uploadsFolder = Path.Combine(_environment.WebRootPath, "Image-Resizer");
            //if (!Directory.Exists(uploadsFolder))
            //    Directory.CreateDirectory(uploadsFolder);
            //var filePath = Path.Combine(uploadsFolder, Path.GetFileName(model.Image.FileName));
            //using (var stream = new FileStream(filePath, FileMode.Create))
            //{
            //    model.Image.CopyTo(stream);
            //}

            ////convert FileAsString to Base64
            //string base64String = string.Empty;
            //if (model.Image != null && model.Image.Length > 0)
            //{
            //    using var ms1 = new MemoryStream();
            //    model.Image.CopyTo(ms1);
            //    base64String = Convert.ToBase64String(ms1.ToArray());
            //}

            ////convert file to byte[]
            //byte[]? fileBytes = null;
            //if (model.ImageByte != null && model.ImageByte.Length > 0)
            //{
            //    using var ms1 = new MemoryStream();
            //    model.ImageByte.CopyTo(ms1);
            //    fileBytes = ms1.ToArray();
            //}

            #endregion

            #region get the original width and height  original image

            using (var stream = model.Image.OpenReadStream())
            using (var image = SixLabors.ImageSharp.Image.Load(stream))
            {
                int width = image.Width;
                int height = image.Height;
            }

            #endregion

            #region new method

            using var inputstream1 = model.Image.OpenReadStream();
            using var image1 = Image.Load(inputstream1);
            image1.Mutate(item=>item.Resize(new ResizeOptions()
            {
                Size=new Size(100,100),
                Mode = ResizeMode.Crop
            }));

            using var inputstream2 = model.ImageByte.OpenReadStream();
            using  var image2=Image.Load(inputstream2);
            image2.Mutate(item=>item.Resize(new ResizeOptions()
            {
                Size=new Size(100,100),
                Mode = ResizeMode.Crop
            }));

            //save resize image1 to local folder(wwwroot/Image-Resizer)
            string upload_folder = Path.Combine(_environment.WebRootPath, "Image-Resizer");
            if (!Directory.Exists(upload_folder))
                Directory.CreateDirectory(upload_folder);

            string originalFileName = Path.GetFileName(model.Image.FileName);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(originalFileName);
            string extension = Path.GetExtension(originalFileName);

            // final file name
            string fileName = originalFileName;
            string filepath1 = Path.Combine(upload_folder, fileName);

            // If file already exists → generate unique name
            int counter = 1;
            while (System.IO.File.Exists(filepath1))
            {
                fileName = $"{fileNameWithoutExt}_{counter}{extension}";
                filepath1 = Path.Combine(upload_folder, fileName);
                counter++;
            }
           // string filepath1 = Path.Combine(upload_folder, Path.GetFileName(model.Image.FileName));
           
            using (var fs = new FileStream(filepath1, FileMode.Create))
            {
                await image1.SaveAsPngAsync(fs);  // or SaveAsJpegAsync
            }

            #region delete the image from the local folder if the record are delete from the database

            // delete file from local folder
            //if (!string.IsNullOrEmpty(record.FilePath))
            //{
            //    string fullPath = Path.Combine(_environment.WebRootPath, record.FilePath);
            //    if (System.IO.File.Exists(fullPath))
            //    {
            //        System.IO.File.Delete(fullPath);
            //    }
            //}

            #endregion
            //convert image1 to base64
            using var ms1= new MemoryStream();
            await image1.SaveAsPngAsync(ms1);
            string base64image1 = Convert.ToBase64String(ms1.ToArray());

            //convert image2 to byte[]
            using var ms2=new MemoryStream();
            await image2.SaveAsPngAsync(ms2);
            byte[] bytesImage2 = ms2.ToArray();

            model._AfterResizeImage = new AfterResizeImage
            {
                LocalDirectoryImage = filepath1,
                ImageStringBase64 = base64image1,
                ImageByte = bytesImage2
            };
            
            //
            #endregion
            return View(model);
        }
    }
}
