namespace PowerShellPayloadGenerator.Models
{
    /// <summary>
    /// Типы полезных нагрузок
    /// </summary>
    public enum PayloadType
    {
        /// <summary>
        /// Обратная оболочка
        /// </summary>
        ReverseShell,

        /// <summary>
        /// Загрузка файла
        /// </summary>
        Download,

        /// <summary>
        /// Выполнение команды
        /// </summary>
        Command,

        /// <summary>
        /// Обнаружение информации
        /// </summary>
        Discovery,

        /// <summary>
        /// Перечисление привилегий
        /// </summary>
        PrivilegeEnumeration,

        /// <summary>
        /// Проверка уязвимостей
        /// </summary>
        VulnerabilityCheck,

        /// <summary>
        /// Сканирование портов
        /// </summary>
        PortScan,

        /// <summary>
        /// Сбор логов
        /// </summary>
        LogCollection,

        /// <summary>
        /// Проверка безопасности
        /// </summary>
        SecurityAudit
    }

    /// <summary>
    /// Уровень опасности нагрузки
    /// </summary>
    public enum RiskLevel
    {
        /// <summary>
        /// Информационный
        /// </summary>
        Info,

        /// <summary>
        /// Низкий риск
        /// </summary>
        Low,

        /// <summary>
        /// Средний риск
        /// </summary>
        Medium,

        /// <summary>
        /// Высокий риск
        /// </summary>
        High,

        /// <summary>
        /// Критический риск
        /// </summary>
        Critical
    }
}
