#include <stdio.h>
#include <string.h>
#include <ctype.h>

void rot13(char *text) {
    for (int i = 0; text[i] != '\0';i++) {
        if(isalpha(text[i])) {
            char base = isupper(text[i]) ? 'A': 'a';
            text[i] = ((text[i] - base +13) % 26) + base;
        }
    }
}

void caesar(char *text, int shift) {
    for (int i = 0; text[i] != '\0';i++) {
        if(isalpha(text[i])) {
            char base = isupper(text[i]) ? 'A' : 'a';
            text[i] = ((text[i] -base + shift) % 26 + 26) % 26 + base;
        }
    }
}

int main() {
    char message[100];
    int choise, shift;

    printf("Введите сообщение: ");
    fgets(message, sizeof(message), stdin);
    message[strcspn(message, "\n")] = 0;

    printf("Введите операцию: ");
    printf("1. ROT13\n");
    printf("2. Ширф Цезаря\n");
    printf("Ваши выбор: ");
    scanf("%d", &choise);
    getchar();

    switch(choise) {
        case 1:
            rot13(message);
            printf("Зашифрованное/Расшифрованное сообщение (ROT13): %s\n", message);
            break;

        case 2:
            printf("Введите сдвиг для шифра Цезаря: ");
            scanf("%d", &shift);
            getchar();
            caesar(message, shift);
            printf("Зашифрованное/Расшифрованное сообщение (Цезарь): %s\n", message);
            break;
        default:
            printf("Неверный выбор.\n");
    }

    return 0;
}
