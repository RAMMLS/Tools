#!/usr/bin/env python3

def create_reg_file(program_path, reg_filename="autostart.reg"):
    """Создает REG-файл для автозагрузки программы"""
    
    template = f"""Windows Registry Editor Version 5.00

[HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run]
"MyApplication"="{program_path.replace('/', '\\\\')}"
"""
    
    with open(reg_filename, 'w') as f:
        f.write(template)
    
    print(f"Создан файл: {reg_filename}")
    print(f"Программа: {program_path}")

# Пример использования
if __name__ == "__main__":
    program_path = input("Введите путь к программе на Windows: ")
    create_reg_file(program_path)
