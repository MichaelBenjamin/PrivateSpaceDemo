namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Mood")]
    public partial class Mood
    {
        [Key]
        public int moodId { get; set; }

        [Required]
        [DisplayName("����")]
        public string MoodContent { get; set; }

        [Required]
        [Column("moodUid")]
        public int userId { get; set; }
        [DisplayName("���鷢��ʱ��")]
        public DateTime MoodTime { get; set; }

        public virtual User User { get; set; }

        public ViewModel.MoodViewModel ToViewModel()
        {
            return new ViewModel.MoodViewModel() {MoodContent=this.MoodContent,MoodId=this.moodId,MoodTime=this.MoodTime };
        }
    }
}
