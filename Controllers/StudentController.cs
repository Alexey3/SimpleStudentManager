using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleStudentManager.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace SimpleStudentManager.Controllers
{
    public class StudentController : Controller
    {
		private Database db;
		private IHostingEnvironment environment;

		public StudentController(Database db, IHostingEnvironment environment)
		{
			this.db = db;
			this.environment = environment;
		}

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
			var model = db.Students.ToList();
            return View(model);
        }

		public IActionResult Info(int id)
		{
			var student = db.Students.Find(id);
			return View(student);
		}

		public IActionResult Add()
        {
			var student = new Student();
			student.PhotoUrl = "/images/noavatar.png";
			return View("Edit", student);
        }

		public IActionResult Edit(int id)
		{
			var student = db.Students.Find(id);
			return View(student);
		}

		public IActionResult Delete(int id)
		{
			var student = db.Students.Find(id);
			if (student != null)
			{
				db.Students.Remove(student);
				db.SaveChanges();
			}
			return RedirectToAction("List");
		}

		[HttpPost]
		public async Task<IActionResult> Save(Student student)
		{
			if (student.Id == 0)
				db.Students.Add(student);
			else
			{
				var s = db.Students.Find(student.Id);
				s.Name = student.Name;
				s.PhotoUrl = student.PhotoUrl;
			}
			await db.SaveChangesAsync();
			return Ok();
		}

		public IActionResult Error()
        {
            return View();
        }

		public IActionResult FileUpload()
		{
			return View();
		}

		[HttpPost("UploadFiles")]
		public async Task<IActionResult> Post(List<IFormFile> files)
		{
			var uploads = Path.Combine(environment.WebRootPath, "uploads");
			long size = files.Sum(f => f.Length);

			var formFile = files[0];

			if (formFile.Length > 0)
			{
				string newFileName = Path.GetFileName(Path.GetTempFileName()) + ".png";
				string filePath = Path.Combine(uploads, newFileName);
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await formFile.CopyToAsync(stream);
				}
				return Ok(new { count = files.Count, size, filePath });
			}

			return Error();
		}

		[HttpPost]
		public async Task<IActionResult> UpdateStudentPhoto(int id, string data)
		{
			var photoPath = await SaveImageAsync(data);

			var student = db.Students.Find(id);
			if (student != null)
			{
				student.PhotoUrl = Path.Combine("/uploads", Path.GetFileName(photoPath));
				await db.SaveChangesAsync();
			}
			return Ok(new { photoPath });
		}

		public async Task<string> SaveImageAsync(string data)
		{
			//"data:image/png;base64,"
			var base64 = data.Substring(22);
			byte[] bytes = Convert.FromBase64String(base64);

			var uploads = Path.Combine(environment.WebRootPath, "uploads");
			string newFileName = Path.GetFileName(Path.GetTempFileName()) + ".png";
			string filePath = Path.Combine(uploads, newFileName);
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await stream.WriteAsync(bytes, 0, bytes.Length);
			}
			return filePath;
		}

		public IActionResult SnapPhoto(int id)
		{
			ViewData["StudentId"] = id;
			return View();
		}
	}
}
