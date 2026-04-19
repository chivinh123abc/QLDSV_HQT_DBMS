namespace QLDSV_HTC.Application.Helpers
{
    public static class DateTimeHelper
    {
        public static List<string> GetSchoolYears(int count = 5)
        {
            var years = new List<string>();
            var currentYear = DateTime.Now.Year;

            // Adjust current year if we are in the early part of the year (before September)
            // School years typically start in September.
            if (DateTime.Now.Month < 9)
            {
                currentYear--;
            }

            for (int i = 0; i < count; i++)
            {
                int startYear = currentYear - i;
                int endYear = startYear + 1;
                years.Add($"{startYear}-{endYear}");
            }

            // Optional: ensure specific years requested by the user are included if needed, 
            // but dynamic generation is generally better.
            return years;
        }

        public static string GetCurrentSchoolYear()
        {
            var currentYear = DateTime.Now.Year;
            if (DateTime.Now.Month < 9) currentYear--;
            return $"{currentYear}-{currentYear + 1}";
        }

        public static int GetCurrentSemester()
        {
            var month = DateTime.Now.Month;
            if (month >= 9 || month == 1) return 1;
            if (month >= 2 && month <= 6) return 2;
            return 3;
        }

        public static List<int> GetSemesters()
        {
            return [1, 2, 3];
        }
    }
}
