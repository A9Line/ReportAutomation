namespace ReportAutomation
{
    // Интерфейс правила проверки показателя.
    // Каждое правило проверяет одно условие корректности данных.
    public interface IValidationRule
    {
        // Наименование правила для вывода в журнал.
        string Name { get; }

        // Проверяет показатель и возвращает результат проверки.
        ValidationResult Check(Indicator indicator, AppConfig config);
    }
}
