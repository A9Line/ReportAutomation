using System.Collections.Generic;

namespace ReportAutomation
{
    // Набор правил проверки исходных данных.
    // Объединяет применяемые правила и проверяет ими каждую запись.
    public class RuleSet
    {
        private readonly List<IValidationRule> rules =
            new List<IValidationRule>();

        // Формирует стандартный набор правил проверки.
        public RuleSet()
        {
            rules.Add(new DepartmentCodeRule());
            rules.Add(new NonNegativeRule());
            rules.Add(new MinTransmittedRule());
            rules.Add(new UsefulNotExceedsRule());
            rules.Add(new BalanceRule());
        }

        // Добавляет дополнительное правило в набор.
        public void Add(IValidationRule rule)
        {
            rules.Add(rule);
        }

        // Проверяет запись всеми правилами набора.
        // Возвращает первый отрицательный результат либо успех.
        public ValidationResult CheckAll(Indicator indicator, AppConfig config)
        {
            foreach (IValidationRule rule in rules)
            {
                ValidationResult result = rule.Check(indicator, config);
                if (!result.IsValid)
                {
                    return result;
                }
            }
            return ValidationResult.Ok();
        }
    }
}
