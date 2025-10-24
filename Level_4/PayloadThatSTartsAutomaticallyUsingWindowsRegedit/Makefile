build-windows:
	x86_64-w64-mingw32-gcc -o program.exe program.c -lws2_32
	@echo "Создан program.exe для Windows"

create-reg:
	@echo 'Windows Registry Editor Version 5.00' > autostart.reg
	@echo '' >> autostart.reg
	@echo '[HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run]' >> autostart.reg
	@echo '"MyProgram"="C:\\\\Program Files\\\\MyApp\\\\program.exe"' >> autostart.reg

all: build-windows create-reg
