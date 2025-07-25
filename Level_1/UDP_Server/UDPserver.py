import socket 

localIP = "127.0.0.1"
localPort = 20001
bufferSize = 1024
msgFromServer = "Сообщение отправленное на сервер и присланное обратно: "

#Создание сокета
UDPServerSocket = socket.socket(family=socket.AF_INET, type=socket.SOCK_DGRAM)

#Адрес и ip в память
UDPServerSocket.bind((localIP, localPort))

print("UDP сервер запущен и ожидает")

#Прослушивание входящих значений
while(True):
	message, address = UDPServerSocket.recvfrom(bufferSize)

	print("\nСообщение от клиента: {0}\nIp адрес клиента: {1}\n".format(message.decode('UTF-8'), address))
	bytesTosend = msgFromServer + message.decode('UTF-8')

#Возврат клиенту
UDPServerSocket.sendto(str.encode(bytesTosend), address)
