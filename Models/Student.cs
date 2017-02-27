using System.ComponentModel.DataAnnotations;

namespace SimpleStudentManager.Models
{
	public class Student
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string PhotoUrl { get; set; }
	}
}
