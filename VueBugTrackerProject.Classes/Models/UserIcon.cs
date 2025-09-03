using System.ComponentModel.DataAnnotations;

namespace VueBugTrackerProject.Classes
{
	public class UserIcon
	{
		/// <summary>
		/// Unique identifier for the class.
		/// </summary>
		[Key]
		[Required]
		public string ID { get; set; }

		/// <summary>
		/// The image for the account.
		/// </summary>
		[Required]
		public string Image { get; set; }
	
	}
}
