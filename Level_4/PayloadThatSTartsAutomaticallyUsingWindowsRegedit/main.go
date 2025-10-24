package main

import (
    "golang.org/x/sys/windows/registry"
    "os"
    "path/filepath"
)

func main() {
    // Получаем путь к исполняемому файлу
    exePath, _ := filepath.Abs(os.Args[0])
    
    // Открываем ключ реестра
    key, err := registry.OpenKey(
        registry.CURRENT_USER,
        `Software\Microsoft\Windows\CurrentVersion\Run`,
        registry.ALL_ACCESS,
    )
    if err != nil {
        panic(err)
    }
    defer key.Close()
    
    // Устанавливаем значение
    err = key.SetStringValue("MyGoApp", exePath)
    if err != nil {
        panic(err)
    }
}
