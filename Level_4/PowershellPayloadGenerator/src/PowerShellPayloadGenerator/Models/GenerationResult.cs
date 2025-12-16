using System;
using System.Collections.Generic;

namespace PowerShellPayloadGenerator.Models
{
    /// <summary>
    /// Результат генерации полезной нагрузки
    /// </summary>
    public class GenerationResult
    {
        /// <summary>
        /// Успешность генерации
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Сгенерированный код
        /// </summary>
        public string Payload { get; set; }

        /// <summary>
        /// Размер полезной нагрузки в байтах
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Уровень риска
        /// </summary>
        public RiskLevel RiskLevel { get; set; }

        /// <summary>
        /// Время генерации в миллисекундах
        /// </summary>
        public long GenerationTimeMs { get; set; }

        /// <summary>
        /// Сообщения об ошибках
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();

        /// <summary>
        /// Предупреждения
        /// </summary>
        public List<string> Warnings { get; set; } = new List<string>();

        /// <summary>
        /// Хэш SHA256 полезной нагрузки
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Временная метка создания
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Получить отформатированный результат
        /// </summary>
        public override string ToString()
        {
            return $"Success: {Success}, Size: {Size} bytes, Risk: {RiskLevel}, Time: {GenerationTimeMs}ms";
        }
    }
}
