#include <windows.h>
#include <string>

bool addToRegistry(const std::string& appPath) {
    HKEY hKey;
    LONG result;
    const char* keyPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
    
    // Открываем ключ реестра
    result = RegOpenKeyExA(HKEY_CURRENT_USER, keyPath, 0, KEY_WRITE, &hKey);
    if (result != ERROR_SUCCESS) {
        return false;
    }
    
    // Устанавливаем значение
    result = RegSetValueExA(hKey, "MyApp", 0, REG_SZ, 
                           (const BYTE*)appPath.c_str(), 
                           appPath.length() + 1);
    
    RegCloseKey(hKey);
    return (result == ERROR_SUCCESS);
}

int main() {
    char path[MAX_PATH];
    GetModuleFileNameA(NULL, path, MAX_PATH);
    
    if (addToRegistry(path)) {
        MessageBoxA(NULL, "Программа добавлена в автозагрузку", "Успех", MB_OK);
    } else {
        MessageBoxA(NULL, "Ошибка добавления в автозагрузку", "Ошибка", MB_OK);
    }
    
    return 0;
}
