using DuAnTN.Models;

namespace DuAnTN.Areas.Admin.Data
{
    public class CategoryViewModel
    {
        public List<Category> Categories { get; set; }
        public Category CategoryToEdit { get; set; }
    }

}
