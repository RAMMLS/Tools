using System;

namespace PowerShellPayloadGenerator.Models
{
    /// <summary>
    /// Запись аудита для отслеживания операций
    /// </summary>
    public class AuditRecord
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Действие пользователя
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Время действия
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// IP адрес источника
        /// </summary>
        public string SourceIp { get; set; }

        /// <summary>
        /// Конфигурация генерации
        /// </summary>
        public PayloadConfig Config { get; set; }

        /// <summary>
        /// Результат генерации
        /// </summary>
        public GenerationResult Result { get; set; }

        /// <summary>
        /// Целевая система (если применимо)
        /// </summary>
        public string TargetSystem { get; set; }

        /// <summary>
        /// Причина использования
        /// </summary>
        public string Purpose { get; set; }

        /// <summary>
        /// Разрешение/авторизация
        /// </summary>
        public string Authorization { get; set; }

        /// <summary>
        /// Статус действия
        /// </summary>
        public AuditStatus Status { get; set; } = AuditStatus.Completed;

        /// <summary>
        /// Дополнительные заметки
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Проверка валидности записи
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Action) &&
                   !string.IsNullOrWhiteSpace(Username) &&
                   Config != null &&
                   !string.IsNullOrWhiteSpace(Purpose) &&
                   !string.IsNullOrWhiteSpace(Authorization);
        }
    }

    /// <summary>
    /// Статус аудита
    /// </summary>
    public enum AuditStatus
    {
        /// <summary>
        /// Запланировано
        /// </summary>
        Planned,

        /// <summary>
        /// В процессе
        /// </summary>
        InProgress,

        /// <summary>
        /// Завершено
        /// </summary>
        Completed,

        /// <summary>
        /// Отклонено
        /// </summary>
        Rejected,

        /// <summary>
        /// Ошибка
        /// </summary>
        Failed,

        /// <summary>
        /// Отменено
        /// </summary>
        Cancelled
    }
}
