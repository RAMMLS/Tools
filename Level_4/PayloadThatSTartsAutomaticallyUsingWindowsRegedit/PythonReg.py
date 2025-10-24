import sys
import os
import winreg

def add_to_startup():
    """Добавляет программу в автозагрузку"""
    if getattr(sys, 'frozen', False):
        # Если программа собрана в exe
        program_path = sys.executable
    else:
        # Если запуск из скрипта
        program_path = os.path.abspath(sys.argv[0])
    
    try:
        key = winreg.OpenKey(
            winreg.HKEY_CURRENT_USER,
            r"Software\Microsoft\Windows\CurrentVersion\Run",
            0, winreg.KEY_SET_VALUE
        )
        winreg.SetValueEx(key, "MyPythonApp", 0, winreg.REG_SZ, program_path)
        winreg.CloseKey(key)
        return True
    except Exception as e:
        print(f"Ошибка: {e}")
        return False

if __name__ == "__main__":
    if add_to_startup():
        print("Успешно добавлено в автозагрузку")
