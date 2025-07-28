import socket
import select
import sys
import string 

key = 'abcdefghijklmnopqrstuvwxyz'

def encrypt (n, plaintext):
    result = ""

    for l in plaintext.lower():
        try:
            i = (key.index(l)+n)%26
            result += kei[i]

        except ValueError:
            result += l

    return result.lower()

server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
if len(sys.argv) != 3:
    print ("script, IP address, port number")
    exit()
IP_address = str(sys.argv[1])
Port = int(sys.argv[2])
server.connect((IP_address, Port))

while True:
    socket_list = [sys.stdin, server]
    read_sockets, write_socket, error_socket = select.select(socket_list, [], [])
    for socks in read_sockets:
        if socks == server:
            message = socks.recv(2048)
            print(message)
        else:
            offset = 4
            text = message
            encrypted = encrypt(offset, text)
            message = sys.stdin.readline()
            server.send(encrypted)
            sys.stdout.write(encrypted)
            sys.stdout.flush()
server.close()
