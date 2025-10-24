#!/bin/bash

# Создание reg
cat >autostart.reg <<'EOF'
Windows Registry Editor Version 5.00 

[HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run]
"MyApp"="C:\\Path\\To\\Your\\Program.exe"
EOF

echo "REG-файл создан: autostart.reg"
