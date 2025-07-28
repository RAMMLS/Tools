import socket
import select
from thread import *
import sys
import string

server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

if len(sys.argv) != 3:
    print("Correct usage: script, IP address, port number")
    exit()

IP_address = str(sys.argv[1])

Port = int(sys.argv[2])
server.bind((IP_ADDRESS, Port))
server.listen(100)

list_of_clients = []

key = 'abcdefghijklmnopqrstuvwxyz'

def decypt(n, ciphertext):
    result = ''

    for l in ciphertext:
        try:
            i = (key.index(l) - n)%26
            result += key[i]
        except ValueError:
            result += l
    return result

def clientthread(conn, addr):
    conn.send("Welcome to chatroom!")

    while True:
        try:
            message = conn.recv(2048)
            if message:
                offset = 4
                text = message
                decrypted = decrypt(offset, text)
                print("<" + addr[0] + "> " + decrypted)

                message_to_send = "<" + addr[0] + "> " + decrypted
                broadcast(message_to_send, conn)

            else:
                remove(conn)

        except:
            continue

def broadcast(message, connection):
    for clients in list_of_clients:
        if clients != connection:
            try:
                clients.send(message)
            except:
                clients.close()

                remove(clients)

def remove(connection):
    if connection in list_of_clients:
        list_of_clients.remove(connection)

    while True:
        conn, addr = server.accept()

        list_of_clients.append(conn)

        print(addr[0] + " connected")

        start_new_thread(clientthread, (conn, addr))

conn.close()
server.close()


