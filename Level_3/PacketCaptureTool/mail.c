#include <stdio.h>
#include <pcap.h>

char pckfilter[] = "ipdst host 192.168.1.1";
struct bpf_program fcode;
if (d -> addresses != NULL) 
  netmask = ((struct sockaddr_in *) (d -> addresses -> netmask)) ->
else netmask = 0xffffff;
pcap_compile(adhandle, &fcode, pckfilter, 1, netmask);
pcap_setfilter(adhandle, &fcode);

void packet_handler(u_char *param, const struct pcap_pkthdr *pkt_header, const u_char *pkd_data);

time_t local_tv_sec = header -> ts.tv_sec;
char strtime[16];
localtime_s(&local_tv_sec, &ltime);
strftime(strtime, sizeof strtime, "%H:%M:%S", &ltime);

ip_header *ip_hg;
ip_hd = (ip_header*)(pkd_data + 14);
if (!Count_ip_check_sum(ip_hd)) {
  return;
}

u_int ip_len = (ip_hd->ver_ihl & 0xf) * 4;
udp_header *udp_hd;
udp_hd = (udp_header *)((u_char*)ip_hd + ip_len);
u_char* pshd = new u_char[12];	// псевдозаголовок
pshd[0] = ip_hd->src_ip.byte1; pshd[1] = ip_hd->src_ip.byte2; 
pshd[2] = ip_hd->src_ip.byte3; pshd[3] = ip_hd->src_ip.byte4;
pshd[4] = ip_hd->dest_ip.byte1; pshd[5] = ip_hd->dest_ip.byte2; 
pshd[6] = ip_hd->dest_ip.byte3; pshd[7] = ip_hd->dest_ip.byte4;
pshd[8] = 0x00; pshd[9] = 0x11; pshd[10] = 0x00; pshd[11] = 0x09;
if (!Count_udp_check_sum(udp_hd, pshd))
	return;

u_short sendport = ntohs(udp_hd->src_port);
u_short destport = ntohs(udp_hd->dest_port);
printf("%s,%.6d len:%d\n", timestr, header->ts.tv_usec, header->len);
printf("%d.%d.%d.%d.%d -> %d.%d.%d.%d.%d\n",
     ip_hd->src_ip.byte1, ip_hd->src_ip.byte2, ip_hd->src_ip.byte3, ip_hd->src_ip.byte4, sendport,
     ip_hd->dest_ip.byte1, ip_hd->dest_ip.byte2, ip_hd->dest_ip.byte3, ip_hd->dest_ip.byte4, destport);



