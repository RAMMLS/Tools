#include <arpa/inet.h>
#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#define PORT 8888
#define MAX_MESSAGE_SIZE 1024

typedef struct {
    struct sockaddr_in serverAddr;
    int socket;
} Client;

Client client;

void* receiveMessages(void* arg)
{
    char buffer[MAX_MESSAGE_SIZE];
    ssize_t bytesRead;

    while (1) {
        bytesRead = recvfrom(client.socket, buffer,
                             sizeof(buffer), 0, NULL, NULL);
        if (bytesRead == -1) {
            perror("Receive failed");
            exit(EXIT_FAILURE);
        }

        buffer[bytesRead] = '&#92;&#48;';
        printf("Server: %s", buffer);
    }
}

int main()
{
    pthread_t receiveThread;
    char message[MAX_MESSAGE_SIZE];
    ssize_t bytesRead;

    // Create socket
    if ((client.socket = socket(AF_INET, SOCK_DGRAM, 0))
        == -1) {
        perror("Socket creation failed");
        exit(EXIT_FAILURE);
    }

    // Configure server address
    memset(&client.serverAddr, 0,
           sizeof(client.serverAddr));
    client.serverAddr.sin_family = AF_INET;
    client.serverAddr.sin_port = htons(PORT);
    if (inet_aton("127.0.0.1", &client.serverAddr.sin_addr)
        == 0) {
        perror("Invalid server address");
        exit(EXIT_FAILURE);
    }

    // Create a thread to receive messages
    pthread_create(&receiveThread, NULL, receiveMessages,
                   NULL);

    // Send and receive messages
    while (1) {
        // Get user input
        printf("Enter message: ");
        fgets(message, sizeof(message), stdin);

        // Send message to server
        sendto(client.socket, message, strlen(message), 0,
               (struct sockaddr*)&client.serverAddr,
               sizeof(client.serverAddr));
    }

    close(client.socket);

    return 0;
}
