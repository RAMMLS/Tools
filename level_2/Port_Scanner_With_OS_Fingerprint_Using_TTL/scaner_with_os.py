import sys
import threading
from queue import Queue
from scapy.all import *
import socket
import logging
from scapy.layers.inet import IP, ICMO
from concurrent.futures import ThreadPoolExecutor, as_completed

#Подтверждение сообщений об ошибке от scapy
logging.getLogger("scapy.runtime").setlevel(logging.ERROR)

#Функция сканирования портов
def port_scan(ip, port, open_ports):
	try:
		sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
		#Установка таймаута 0.2
		sock.settimeout(0.2)
		#Согласование подключения по IP и порту
		sock.connect((ip, port))
		#Если соединение успешно, добавить номер порта в очередь
		open_ports.put(port)
		#Закрытие соединения
		sock.close()
	exept:
		#Игнорирование
		pass

#Анализ ОС на TTL
def analyze_os(ip, port):
	try:
		#Отправляем ICMP пакет, ждем ответа
		ans = sri(dst=ip)/IMCP(id=RandShort()), timeout = 1, retry = 2, vebrose = 0)

		if ans:
			#Получение TTL от пакета ожиданий
			ttl = ans[IP].ttl
			if ttl <= 64:
				os_guess = "Linux or Unix"
			elif ttl == 100:
				os_guess = "Windonw2000"
			elif ttl == 107:
				os_guess = "win NT"
			elif ttl == 127:
				os_guess = "win9x"
			elif ttl == 252:
				os_guess = "Solaris"
			elif ttl == 128:
				os_guess = "Windows"
			else:
				os_guess = "Unix"

			#Выводим номер порта, Количество TTL и предпологаемую систему
			print(f"{ip}, порт открыт: {port}, TTL: {ttl}, ОС: {os_guess}")

	except Exception as e:
		pass #Игнорирование исключений

def port_scan_all(ip):
	open_ports = Queue()

	with ThreadPoolExecutor(max_workers=50) as executor:

	futures = [executor.submit(port_scan, ip, port, open_ports) for port in range (1, 65536)]

	for future in as_completed(futures):
		pass

	open_port_list = []
	while not open_ports.empty():
		open_port_list.append(open_ports.get())

	return open_port_list

def main():
	if len(sys.argv) == 2:
		ip_target = sys.argv[1]

		open_ports = port_scan_all(ip_target)

		for port in open_ports:
			analyze_os(ip_target, port)

	else:
		print("Правильное использование: script, IP адрес цели")
		sys.exit(0)

#Запуск скрипта
if __name__ == "__main__"
	main()
