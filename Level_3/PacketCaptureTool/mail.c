#include <pcap.h>
#include <stdio.h>
#include <arpa/inet.h>
#include <time.h>
#include <string.h>

// Простая структура для IP заголовка
struct ip_header {
    u_char byte1, byte2, byte3, byte4;
};

// Простая структура для UDP заголовка
struct udp_header {
    u_short src_port;
    u_short dest_port;
};

void packet_handler(u_char *user_data, const struct pcap_pkthdr *header, const u_char *packet) {
    struct ip_header *ip_hd;
    struct udp_header *udp_hd;
    
    // Пропускаем Ethernet заголовок (14 байт)
    ip_hd = (struct ip_header*)(packet + 14);
    
    // Пропускаем IP заголовок (20 байт) чтобы добраться до UDP
    udp_hd = (struct udp_header*)(packet + 14 + 20);
    
    u_short sendport = ntohs(udp_hd->src_port);
    u_short destport = ntohs(udp_hd->dest_port);
    
    // Получаем время
    time_t now = time(NULL);
    struct tm *tm_info = localtime(&now);
    char timestr[20];
    strftime(timestr, 20, "%H:%M:%S", tm_info);
    
    printf("%s,%.6d len:%d\n", timestr, header->ts.tv_usec, header->len);
    printf("%d.%d.%d.%d.%d -> %d.%d.%d.%d.%d\n",
         ip_hd->byte1, ip_hd->byte2, ip_hd->byte3, ip_hd->byte4, sendport,
         ip_hd->byte1, ip_hd->byte2, ip_hd->byte3, ip_hd->byte4, destport);
}

int main() {
    char errbuf[PCAP_ERRBUF_SIZE];
    pcap_if_t *alldevs, *dev;
    int i = 0;
    
    // Получаем список устройств
    if (pcap_findalldevs(&alldevs, errbuf) == -1) {
        fprintf(stderr, "Error finding devices: %s\n", errbuf);
        return 1;
    }
    
    // Выводим список устройств
    for (dev = alldevs; dev != NULL; dev = dev->next) {
        printf("%d. %s", ++i, dev->name);
        if (dev->description)
            printf(" (%s)\n", dev->description);
        else
            printf(" (No description available)\n");
    }
    
    if (i == 0) {
        printf("No interfaces found!\n");
        return 1;
    }
    
    // Используем первое устройство
    dev = alldevs;
    pcap_t *handle = pcap_open_live(dev->name, BUFSIZ, 1, 1000, errbuf);
    
    if (handle == NULL) {
        fprintf(stderr, "Couldn't open device %s: %s\n", dev->name, errbuf);
        return 1;
    }
    
    printf("Capturing on %s...\n", dev->name);
    
    // Захватываем пакеты
    pcap_loop(handle, 0, packet_handler, NULL);
    
    pcap_close(handle);
    pcap_freealldevs(alldevs);
    
    return 0;
}
