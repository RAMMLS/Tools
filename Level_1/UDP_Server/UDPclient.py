import socket

serverAddresPart = ('127.0.0.1', 20001)
bufferSize = 1024

#Создаем UDP сокет на стороне клиента
UDPClientSocket = socket.socket(family=socket.AF_INET, type=socket.SOCK_DGRAM)

while True:
	msgFromCLient = input("Отправьте текст на UDP сервер: ")
	bytesToSend = str.encode(msgFromClient)

#Отправить на сервер 
UDPClientSocket.sendto(bytesToSend, serverAddressPort)

message, address = UDPClientSocket.recvfrom(bufferSize)
msg = "\nСообщение на сервер: {} \n".format(message.decode('UTF-8'))

print(msg)
