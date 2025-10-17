#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <pcap.h>
#include <unistd.h>
#include <sys/socket.h>
#include <netdb.h>
#include <arpa/inet.h>

// Исправленная функция learn_IP
u_char* learn_IP() {
    struct addrinfo hints, *result;
    char hostName[1024];
    static char ip_addr[INET_ADDRSTRLEN];
    
    if (gethostname(hostName, sizeof(hostName)) == 0) {
        memset(&hints, 0, sizeof(hints));
        hints.ai_family = AF_INET;
        hints.ai_socktype = SOCK_DGRAM;
        hints.ai_flags = AI_PASSIVE;
        
        if (getaddrinfo(hostName, NULL, &hints, &result) == 0) {
            struct addrinfo* res;
            for (res = result; res != NULL; res = res->ai_next) {
                if (res->ai_family == AF_INET) {
                    struct sockaddr_in* addr = (struct sockaddr_in*)res->ai_addr;
                    inet_ntop(AF_INET, &addr->sin_addr, ip_addr, INET_ADDRSTRLEN);
                    break;
                }
            }
            freeaddrinfo(result);
        }
    }
    return (u_char*)ip_addr;
}

// Заглушка для learn_MAC (в Linux сложнее получить MAC)
u_char *learn_MAC() {
    static u_char mac_addr[6] = {0x00, 0x11, 0x22, 0x33, 0x44, 0x55};
    return mac_addr;
}

// Исправленная функция checksum
u_short checksum(u_char *buffer, int size) {
    u_long chksum = 0;
    while (size > 1) {
        chksum += *((u_short*)buffer);
        buffer += 2;
        size -= 2;
    }

    if (size) 
        chksum += *(u_char*)buffer;
    
    chksum = (chksum >> 16) + (chksum & 0xffff);
    chksum += (chksum >> 16);
    
    return (u_short)(~chksum);
}

int main() {
    char errbuf[PCAP_ERRBUF_SIZE];
    pcap_if_t *alldevs, *dev;
    int inum, i = 0;
    
    // Получаем список сетевых интерфейсов
    if (pcap_findalldevs(&alldevs, errbuf) == -1) {
        fprintf(stderr, "Error finding devices: %s\n", errbuf);
        return 1;
    }
    
    // Выводим список интерфейсов
    for (dev = alldevs; dev != NULL; dev = dev->next) {
        printf("%d. %s", ++i, dev->name);
        if (dev->description)
            printf(" (%s)\n", dev->description);
        else
            printf(" (No description available)\n");
    }
    
    if (i == 0) {
        printf("No interfaces found! Make sure you're running in privileged mode.\n");
        return 1;
    }
    
    printf("Enter the interface number (1-%d): ", i);
    scanf("%d", &inum);
    
    // Выбираем нужный интерфейс
    for (dev = alldevs, i = 0; i < inum - 1; dev = dev->next, i++);
    
    // Открываем интерфейс
    pcap_t *adhandle = pcap_open_live(dev->name, 65535, 0, 1000, errbuf);
    if (adhandle == NULL) {
        fprintf(stderr, "Unable to open adapter: %s\n", errbuf);
        pcap_freealldevs(alldevs);
        return 1;
    }
    
    printf("Sending packet...\n");
    
    // Тестовый пакет для отправки
    u_char packet[43] = {0};
    if (pcap_sendpacket(adhandle, packet, 43) != 0) {
        fprintf(stderr, "Error sending packet: %s\n", pcap_geterr(adhandle));
    } else {
        printf("Packet sent successfully!\n");
    }
    
    pcap_close(adhandle);
    pcap_freealldevs(alldevs);
    return 0;
}
