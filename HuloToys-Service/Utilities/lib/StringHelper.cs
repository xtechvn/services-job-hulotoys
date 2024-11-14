using System.Text;
using System.Text.RegularExpressions;

namespace HuloToys_Service.Utilities.lib
{
    public static class StringHelper
    {
        public static bool HasSpecialCharacterExceptVietnameseCharacter(string text)
        {
            var pattern = new Regex("[^a-zA-Z0-9àáãảạăằắẳẵặâầấẩẫậèéẻẽẹêềếểễệđùúủũụưừứửữựòóỏõọôồốổỗộơờớởỡợìíỉĩịäëïîöüûñçýỳỹỵỷÀÁÃẢẠĂẰẮẲẴẶÂẦẤẨẪẬÈÉẺẼẸÊỀẾỂỄỆĐÙÚỦŨỤƯỪỨỬỮỰÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÌÍỈĨỊÄËÏÎÖÜÛÑÇÝỲỸỴỶ ]");
            return pattern.IsMatch(text);
        }
        public static string RemoveSpecialCharacterExceptVietnameseCharacter(string text)
        {
            var pattern = new Regex("[^a-zA-Z0-9àáãảạăằắẳẵặâầấẩẫậèéẻẽẹêềếểễệđùúủũụưừứửữựòóỏõọôồốổỗộơờớởỡợìíỉĩịäëïîöüûñçýỳỹỵỷÀÁÃẢẠĂẰẮẲẴẶÂẦẤẨẪẬÈÉẺẼẸÊỀẾỂỄỆĐÙÚỦŨỤƯỪỨỬỮỰÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÌÍỈĨỊÄËÏÎÖÜÛÑÇÝỲỸỴỶ ]");
            return pattern.Replace(text, "");
        }
        public static string NormalizeTextForSearch(string input)
        {
            return input
                .Normalize(NormalizationForm.FormC)
                .ToLower()
                .Trim();
        }

    }
}
