namespace ReportAutomation
{
    // Результат формирования отчёта.
    // Возвращается управляющим классом в главное окно.
    public class ReportResult
    {
        // Признак успешного формирования отчёта.
        public bool Success { get; set; }

        // Путь к сформированному отчётному документу.
        public string OutputFilePath { get; set; } = string.Empty;

        // Текст журнала операций для вывода оператору.
        public string Log { get; set; } = string.Empty;

        // Сообщение об ошибке при неуспешном формировании.
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
