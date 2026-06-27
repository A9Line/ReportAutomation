namespace ReportAutomation
{
    // Результат проверки одной записи показателей.
    // Содержит признак корректности и описание нарушения.
    public class ValidationResult
    {
        // Признак того, что запись прошла проверку.
        public bool IsValid { get; set; } = true;

        // Описание выявленного нарушения.
        public string Message { get; set; } = string.Empty;

        // Возвращает успешный результат проверки.
        public static ValidationResult Ok()
        {
            return new ValidationResult { IsValid = true };
        }

        // Возвращает результат проверки с описанием нарушения.
        public static ValidationResult Fail(string message)
        {
            return new ValidationResult { IsValid = false, Message = message };
        }
    }
}
