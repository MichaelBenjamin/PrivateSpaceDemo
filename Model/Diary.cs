namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Diary")]
    public partial class Diary
    {
        public int diaryId { get; set; }

        [Required]
        [StringLength(20)]
        [DisplayName("�ռǱ���")]
        public string diaryTitle { get; set; }

        [Column(TypeName = "ntext")]
        [Required]
        [DisplayName("�ռ�����")]
        public string diaryContent { get; set; }


        [DisplayName("�ռ�ʱ��")]
        public DateTime diaryTime { get; set; }

        [Required]
        [Column("diaryUid")]
        public int userId { get; set; }

        public virtual User User { get; set; }

        public ViewModel.DiaryViewModel ToViewModel()
        {
            return new ViewModel.DiaryViewModel() {DiaryId=this.diaryId,DiaryContent=this.diaryContent,DiaryTitle=this.diaryTitle,DiaryTime=this.diaryTime };
        }
    }
}
