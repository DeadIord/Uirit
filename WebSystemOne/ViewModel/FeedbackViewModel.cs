

using System.ComponentModel.DataAnnotations;

namespace WebSystemOne.ViewModel
{
    public class FeedbackViewModel
    {
        [Required(ErrorMessage = "Поле «Фамилия» обязательно.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Поле «Имя» обязательно.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Поле «Отчество» обязательно.")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Поле «Отзыв» обязательно.")]
        public string Body { get; set; }
    }

}
