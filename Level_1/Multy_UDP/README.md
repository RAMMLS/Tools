#CLone the repository
git clone https://github.com/RAMMLS/Tolls/Level_1/Multy_UDP

#open terminal and run
gcc client.c -o client -lpthread && gcc server.c -o server -lpthread
